using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class SSMRMeterCardComponent
    {
        SSMRReadMeterViewController _controller;
        SMRMROValidateRegisterDetailsInfoModel _model;
        private readonly UIView _parentView;
        UIView _containerView, _prevReadingView, _viewBoxContainer, _iconView;
        UILabel _errorLabel, _iconLabel;
        nfloat containerRatio = 112.0f / 288.0f;
        nfloat viewBoxContainerRatio = 40.0f / 256.0f;
        float imgHeight = 20.0f;
        float imgWidth = 52.0f;
        nfloat padding = 16f;
        nfloat halfPadding = 8f;
        int boxMaxCount = 9;
        nfloat _yLocation;
        nfloat _iconYposOriginal;
        nfloat _containerHeightOriginal;
        string _previousMeterReadingValue = string.Empty;
        public string _meterReadingValue = string.Empty;

        public SSMRMeterCardComponent(SSMRReadMeterViewController controller, UIView parentView, nfloat yLocation)
        {
            _controller = controller;
            _parentView = parentView;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            _containerHeightOriginal = _parentView.Frame.Width * containerRatio;
            _containerView = new UIView(new CGRect(padding, _yLocation, _parentView.Frame.Width - (padding * 2), _containerHeightOriginal))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = 5.0f;

            nfloat viewBoxContainerHeight = _containerView.Frame.Width * viewBoxContainerRatio;
            nfloat viewBoxContainerWidth = _containerView.Frame.Width - (padding * 2);
            _viewBoxContainer = new UIView(new CGRect(padding, DeviceHelper.GetCenterYWithObjHeight((float)viewBoxContainerHeight, _containerView), viewBoxContainerWidth, viewBoxContainerHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            for (int i = 0; i < boxMaxCount; i++)
            {
                _viewBoxContainer.AddSubview(CreateViewBox(_viewBoxContainer, i));
            }
            _viewBoxContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                BoxContainerOnTap();
            }));
            _containerView.AddSubview(_viewBoxContainer);

            _errorLabel = new UILabel(new CGRect(padding, _viewBoxContainer.Frame.GetMaxY() + 4f, viewBoxContainerWidth, 14f))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.Tomato,
                TextAlignment = UITextAlignment.Right,
                Hidden = true
            };
            _containerView.AddSubview(_errorLabel);

            nfloat prevReadingHeight = _viewBoxContainer.Frame.GetMinY() - (halfPadding * 2);
            _prevReadingView = new UIView(new CGRect(padding, halfPadding, _viewBoxContainer.Frame.Width, prevReadingHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _containerView.AddSubview(_prevReadingView);

            nfloat iconHeight = DeviceHelper.GetScaledHeight(imgHeight);
            nfloat iconWidth = DeviceHelper.GetScaledWidth(imgWidth);
            _iconYposOriginal = _viewBoxContainer.Frame.GetMaxY() + (_containerHeightOriginal - _viewBoxContainer.Frame.GetMaxY()) / 2 - (iconHeight / 2);
            _iconView = new UIView(new CGRect(_containerView.Frame.Width - padding - iconWidth, _iconYposOriginal, iconWidth, iconHeight))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            _iconView.Layer.CornerRadius = 5f;

            _iconLabel = new UILabel(new CGRect(0, 0, _iconView.Frame.Width, _iconView.Frame.Height))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };

            _iconView.AddSubview(_iconLabel);
            _containerView.AddSubview(_iconView);

            AddCardShadow(ref _containerView);
            _meterReadingValue = string.Empty;
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
            return _containerView;
        }

        public void SetModel(SMRMROValidateRegisterDetailsInfoModel model)
        {
            if (model != null)
            {
                _model = model;
            }
        }

        public void SetIconText(SMRMROValidateRegisterDetailsInfoModel model)
        {
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.RegisterNumber) && !string.IsNullOrWhiteSpace(model.RegisterNumber))
                {
                    string stringLabel = string.Empty;
                    switch (model.RegisterNumberType)
                    {
                        case RegisterNumberEnum.kWh:
                            stringLabel = "kWh";
                            break;
                        case RegisterNumberEnum.kVARh:
                            stringLabel = "kVARh";
                            break;
                        case RegisterNumberEnum.kW:
                            stringLabel = "kW";
                            break;
                    }
                    _iconLabel.Text = stringLabel;
                }
            }
        }

        private void BoxContainerOnTap()
        {
            UIView[] subViews = _viewBoxContainer.Subviews;
            foreach (UIView view in subViews)
            {
                if (view.Tag == 1)
                {
                    UIView[] subSubViews = view.Subviews;
                    if (subSubViews.Length > 0)
                    {
                        UITextField txtField = subSubViews[0] as UITextField;
                        txtField.Enabled = true;
                        txtField.BecomeFirstResponder();
                    }
                    break;
                }
            }
        }

        public UIView CreateViewBox(UIView containerView, int index)
        {
            var width = (containerView.Frame.Width / boxMaxCount) - 1;
            var height = containerView.Frame.Height;
            var xPos = (containerView.Frame.Width / boxMaxCount * index) + 1;
            UIView viewBox = new UIView(new CGRect(xPos, 0, width, height))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkFour,
                Tag = boxMaxCount - index
            };
            UITextField txtFieldDigit = new UITextField(new CGRect(0, 0, width, height))
            {
                Enabled = false,
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_500,
                Tag = index + 1,
                KeyboardType = UIKeyboardType.NumberPad,
                AutocorrectionType = UITextAutocorrectionType.No,
                AutocapitalizationType = UITextAutocapitalizationType.None,
                SpellCheckingType = UITextSpellCheckingType.No,
                ReturnKeyType = UIReturnKeyType.Done,
                TextAlignment = UITextAlignment.Center,
                TintColor = MyTNBColor.PowerBlue
            };
            CreateDoneButton(txtFieldDigit);
            SetTextFieldEvents(txtFieldDigit);
            viewBox.AddSubview(txtFieldDigit);
            if (index == 7)
            {
                UILabel dotLabel = new UILabel(new CGRect(width / 2, 0, width, height))
                {
                    Font = MyTNBFont.MuseoSans16_500,
                    TextColor = MyTNBColor.TunaGrey(),
                    TextAlignment = UITextAlignment.Center,
                    Text = "."
                };
                viewBox.AddSubview(dotLabel);
            }
            return viewBox;
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            textField.EditingChanged += (sender, e) =>
            {
                string textStr = textField.Text;

                if (textField.Text.Length > 1)
                {
                    int nextTag = (int)textField.Tag - 1;
                    if (nextTag > 0)
                    {
                        if (_meterReadingValue.Length < boxMaxCount)
                        {
                            if (textField.Tag == boxMaxCount)
                            {
                                UpdateReadingValueText(textStr.Substring(1, 1));
                            }
                            textField.Text = textStr.Substring(1, 1);
                            PopulateTextFields();
                            textField.BecomeFirstResponder();
                        }
                        else
                        {
                            textField.Text = textStr.Substring(0, 1);
                        }
                    }
                }
                else if (textField.Text.Length == 1)
                {
                    UpdateReadingValueText(textField.Text);
                }
                else
                {
                    int len = _meterReadingValue.Length - 1;
                    int indx = (int)(len - (boxMaxCount - textField.Tag));
                    RemoveCharInString(indx);
                    RepopulateTextFields();
                    ValidateTextField();
                    textField.BecomeFirstResponder();
                }
                if (_meterReadingValue.Length <= boxMaxCount)
                {
                    UpdateMeterReadingValue();
                }
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                {
                    textField.Enabled = false;
                }
                RepopulateTextFields();
                ValidateTextField();
            };
        }

        private void UpdateReadingValueText(string digit)
        {
            if (string.IsNullOrEmpty(digit) || string.IsNullOrWhiteSpace(digit))
            {
                var len = _meterReadingValue.Length;
                _meterReadingValue = _meterReadingValue.Substring(0, len - 1);
            }
            else
            {
                _meterReadingValue += digit;
            }
        }

        private void RemoveCharInString(int index)
        {
            if (index > -1 && index < _meterReadingValue.Length)
            {
                _meterReadingValue = _meterReadingValue.Remove(index, 1);
            }
        }

        private void UpdateMeterReadingValue()
        {
            string strValue = string.Empty;
            UIView[] subViews = _viewBoxContainer.Subviews;
            foreach (UIView view in subViews)
            {
                UIView[] subSubViews = view.Subviews;
                if (subSubViews.Length > 0)
                {
                    UITextField txtField = subSubViews[0] as UITextField;
                    if (!string.IsNullOrEmpty(txtField.Text) && !string.IsNullOrWhiteSpace(txtField.Text))
                    {
                        strValue += txtField.Text;
                    }
                }
            }
            _meterReadingValue = strValue;
        }

        private void RepopulateTextFields()
        {
            UpdateUIForNormalState();
            var len = _meterReadingValue.Length;
            for (int i = 1; i <= _meterReadingValue.Length; i++)
            {
                UIView[] subViews = _viewBoxContainer.Subviews;
                foreach (UIView view in subViews)
                {
                    UIView[] subSubViews = view.Subviews;
                    if (subSubViews.Length > 0)
                    {
                        UITextField txtField = subSubViews[0] as UITextField;
                        if (txtField != null)
                        {
                            if (view.Tag == i)
                            {
                                txtField.Enabled = true;
                                txtField.Text = _meterReadingValue.Substring(len - i, 1);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateTextFields()
        {
            var readingValueLength = _meterReadingValue.Length - 1;
            for (int i = 1; i <= readingValueLength; i++)
            {
                UIView[] subViews = _viewBoxContainer.Subviews;
                foreach (UIView view in subViews)
                {
                    if (view.Tag == (i + 1))
                    {
                        UIView[] subSubViews = view.Subviews;
                        if (subSubViews.Length > 0)
                        {
                            UITextField txtField = subSubViews[0] as UITextField;
                            txtField.Enabled = true;
                            txtField.Text = _meterReadingValue.Substring(readingValueLength - i, 1);
                        }
                        break;
                    }
                }
            }
        }

        private void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                textField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            textField.InputAccessoryView = toolbar;
        }

        public UIView CreateViewBoxForPrevReading(UIView containerView, int index, char digit)
        {
            var width = (containerView.Frame.Width / boxMaxCount) - 1;
            var height = containerView.Frame.Height;
            var xPos = (containerView.Frame.Width / boxMaxCount * index) + 1;
            UIView viewBox = new UIView(new CGRect(xPos, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                Tag = index
            };

            UILabel digitLabel = new UILabel(new CGRect(0, 0, width, height))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Center,
                Text = digit.ToString()
            };

            if (index == 8)
            {
                UILabel dotLabel = new UILabel(new CGRect(0, 0, width, height))
                {
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.Grey,
                    TextAlignment = UITextAlignment.Left,
                    Text = "."
                };
                viewBox.AddSubview(dotLabel);
            }

            viewBox.AddSubview(digitLabel);

            return viewBox;
        }

        public void SetPreviousReading(string prevReading)
        {
            if (!string.IsNullOrEmpty(prevReading) && !string.IsNullOrWhiteSpace(prevReading))
            {
                _previousMeterReadingValue = prevReading;
                string[] readingList = prevReading.Split(".");
                if (readingList.Length > 1)
                {
                    string combinedString = readingList[0] + readingList[1];
                    PopulatePreviousReading(combinedString);
                }
                else
                {
                    PopulatePreviousReading(readingList[0] + "0");
                }
            }
        }

        private void PopulatePreviousReading(string text)
        {
            int startIndx = boxMaxCount - text.Length;
            for (int i = 0; i < text.Length; i++)
            {
                _prevReadingView.AddSubview(CreateViewBoxForPrevReading(_prevReadingView, i + startIndx, text[i]));
            }
        }

        private void ValidateTextField()
        {
            int len = _meterReadingValue.Length;
            if (len > 0)
            {
                string currentReadingWithDecimal = _meterReadingValue.Insert(len - 1, ".");
                if (double.TryParse(_previousMeterReadingValue, out double previousValue))
                {
                    if (double.TryParse(currentReadingWithDecimal, out double currentValue))
                    {
                        if (previousValue > currentValue)
                        {
                            UpdateUI(true, "This value is less than your previous reading.", currentValue);
                        }
                        else
                        {
                            UpdateUI(false, string.Empty, currentValue);
                        }
                    }
                }
            }
        }

        public void UpdateUI(bool isError, string message, double currentReading)
        {
            if (isError)
            {
                _errorLabel.Hidden = false;
                _errorLabel.Text = message ?? string.Empty;
                CGRect iconFrame = _iconView.Frame;
                iconFrame.Y = _errorLabel.Frame.GetMaxY() + 8f;
                _iconView.Frame = iconFrame;
                _iconView.BackgroundColor = MyTNBColor.Tomato;

                CGRect containerFrame = _containerView.Frame;
                containerFrame.Height = _iconView.Frame.GetMaxY() + 8f;
                _containerView.Frame = containerFrame;
            }
            else
            {
                _errorLabel.Hidden = true;
                _errorLabel.Text = string.Empty;
                CGRect iconFrame = _iconView.Frame;
                iconFrame.Y = _iconYposOriginal;
                _iconView.Frame = iconFrame;
                _iconView.BackgroundColor = MyTNBColor.FreshGreen;

                CGRect containerFrame = _containerView.Frame;
                containerFrame.Height = _containerHeightOriginal;
                _containerView.Frame = containerFrame;
            }
            AddCardShadow(ref _containerView);
            UpdateTextFieldTextColor(isError);
            _controller.SetIsValidManualReadingFlags(_model, isError);
            if (!isError)
            {
                string currentReadingValue = currentReading.ToString("0.00", CultureInfo.InvariantCulture);
                _controller.SetCurrentReadingValue(_model, currentReadingValue);
            }
        }

        private void UpdateUIForNormalState()
        {
            UIView[] subViews = _viewBoxContainer.Subviews;
            foreach (UIView view in subViews)
            {
                UIView[] subSubViews = view.Subviews;
                if (subSubViews.Length > 0)
                {
                    UITextField txtField = subSubViews[0] as UITextField;
                    if (txtField != null)
                    {
                        txtField.Enabled = false;
                        txtField.Text = string.Empty;
                        txtField.TextColor = MyTNBColor.TunaGrey();
                    }
                    if (view.Tag == 2)
                    {
                        UILabel label = subSubViews[1] as UILabel;
                        if (label != null)
                        {
                            label.TextColor = MyTNBColor.TunaGrey();
                        }
                    }
                }
            }

            _errorLabel.Hidden = true;
            _errorLabel.Text = string.Empty;
            CGRect iconFrame = _iconView.Frame;
            iconFrame.Y = _iconYposOriginal;
            _iconView.Frame = iconFrame;
            _iconView.BackgroundColor = MyTNBColor.WaterBlue;

            CGRect containerFrame = _containerView.Frame;
            containerFrame.Height = _containerHeightOriginal;
            _containerView.Frame = containerFrame;

            AddCardShadow(ref _containerView);
        }

        private void UpdateTextFieldTextColor(bool isError)
        {
            UIView[] subViews = _viewBoxContainer.Subviews;
            foreach (UIView view in subViews)
            {
                UIView[] subSubViews = view.Subviews;
                if (subSubViews.Length > 0)
                {
                    UITextField txtField = subSubViews[0] as UITextField;
                    if (txtField != null)
                    {
                        txtField.TextColor = isError ? MyTNBColor.Tomato : MyTNBColor.FreshGreen;
                    }
                    if (view.Tag == 2)
                    {
                        UILabel label = subSubViews[1] as UILabel;
                        if (label != null)
                        {
                            label.TextColor = isError ? MyTNBColor.Tomato : MyTNBColor.FreshGreen;
                        }
                    }
                }
            }
        }

        public void UpdateMeterReadingValueFromOCR(string readingStr)
        {
            if (!string.IsNullOrEmpty(readingStr) && !string.IsNullOrWhiteSpace(readingStr))
            {
                string readingString = string.Empty;
                string[] readingList = readingStr.Split(".");
                if (readingList.Length > 1)
                {
                    readingString = readingList[0] + readingList[1];
                }
                else
                {
                    readingString = readingList[0] + "0";
                }

                var readingValueLength = readingString.Length - 1;
                for (int i = 0; i < readingString.Length; i++)
                {
                    UIView[] subViews = _viewBoxContainer.Subviews;
                    foreach (UIView view in subViews)
                    {
                        if (view.Tag == (i + 1))
                        {
                            UIView[] subSubViews = view.Subviews;
                            if (subSubViews.Length > 0)
                            {
                                UITextField txtField = subSubViews[0] as UITextField;
                                txtField.Enabled = true;
                                txtField.Text = readingString.Substring(readingValueLength - i, 1);
                            }
                            break;
                        }
                    }
                }
                _meterReadingValue = readingString;
                ValidateTextField();
            }
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
