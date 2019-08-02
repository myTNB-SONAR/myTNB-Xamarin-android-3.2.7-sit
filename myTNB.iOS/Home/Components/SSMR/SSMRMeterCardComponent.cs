using System;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRMeterCardComponent
    {
        private readonly UIView _parentView;
        UIView _containerView, _prevReadingView, _viewBoxContainer;
        UIImageView _iconView;
        UILabel _errorLabel;
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
        public string _meterReadingValue = string.Empty;

        public SSMRMeterCardComponent(UIView parentView, nfloat yLocation)
        {
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
            _iconView = new UIImageView(new CGRect(_containerView.Frame.Width - padding - iconWidth, _iconYposOriginal, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle("kWh-Icon")
            };

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

        public void SetIconImage(string imageStr)
        {
            if (!string.IsNullOrEmpty(imageStr))
            {
                _iconView.Image = UIImage.FromBundle(imageStr);
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
                TextAlignment = UITextAlignment.Center
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
                        if (textField.Tag == boxMaxCount)
                        {
                            UpdateReadingValueText(textStr.Substring(1, 1));
                        }
                        textField.Text = textStr.Substring(1, 1);
                        PopulateTextFields();
                        textField.BecomeFirstResponder();
                    }
                }
                else
                {
                    UpdateReadingValueText(textField.Text);
                }
                UpdateMeterReadingValue();
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                {
                    textField.Enabled = false;
                }
                RepopulateTextFields();
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
            Debug.WriteLine("_meterReadingValue==== " + _meterReadingValue);
        }

        private void RepopulateTextFields()
        {
            ResetTextFields();
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

        private void ResetTextFields()
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
            string[] readingList = prevReading.Split(".");
            if (readingList.Length > 1)
            {
                string combinedString = readingList[0] + readingList[1];
                PopulatePreviousReading(combinedString);
            }
            else
            {
                PopulatePreviousReading(readingList[0]);
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

        }

        public void ShowErrorLabel(bool isError, string message)
        {
            if (isError)
            {
                _errorLabel.Hidden = false;
                _errorLabel.Text = message ?? string.Empty;
                CGRect iconFrame = _iconView.Frame;
                iconFrame.Y = _errorLabel.Frame.GetMaxY() + 8f;
                _iconView.Frame = iconFrame;

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

                CGRect containerFrame = _containerView.Frame;
                containerFrame.Height = _containerHeightOriginal;
                _containerView.Frame = containerFrame;
            }
            AddCardShadow(ref _containerView);
        }

        public string GetMeterReadingValue()
        {
            return _meterReadingValue;
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
