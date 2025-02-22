﻿using System;
using System.Drawing;
using CoreGraphics;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRMeterCardComponent : BaseComponent
    {
        private SSMRReadMeterViewController _controller;
        private SMRMROValidateRegisterDetailsInfoModel _model;
        private readonly UIView _parentView;
        private UIView _containerView, _prevReadingView, _viewBoxContainer, _iconView;
        private UILabel _errorLabel, _iconLabel, _prevReadingLabel;
        private nfloat containerRatio = 148.0f / 288.0f;
        private nfloat viewBoxContainerRatio = 40.0f / 256.0f;
        private nfloat prevReadingContainerRatio = 20.0f / 256.0f;
        private float imgHeight = 20.0f;
        private float imgWidth = 52.0f;
        private nfloat _paddingX = ScaleUtility.GetScaledWidth(16f);
        private nfloat _paddingY = ScaleUtility.GetScaledHeight(16f);
        private int boxMaxCount = 8;
        private nfloat _yLocation;
        private nfloat _iconYposOriginal;
        private nfloat _containerHeightOriginal;
        string _previousMeterReadingValue = string.Empty;

        public string _meterReadingValue = string.Empty;
        public bool IsActive;

        public SSMRMeterCardComponent(SSMRReadMeterViewController controller, UIView parentView, nfloat yLocation)
        {
            _controller = controller;
            _parentView = parentView;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            _containerHeightOriginal = _parentView.Frame.Width * containerRatio;
            _containerView = new UIView(new CGRect(_paddingX, _yLocation, _parentView.Frame.Width - (_paddingX * 2), _containerHeightOriginal))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = ScaleUtility.GetScaledHeight(5f);

            nfloat viewBoxContainerHeight = _containerView.Frame.Width * viewBoxContainerRatio;
            nfloat viewBoxContainerWidth = _containerView.Frame.Width - (_paddingX * 2);
            nfloat prevReadingContainerHeight = _containerView.Frame.Width * prevReadingContainerRatio;

            _prevReadingLabel = new UILabel(new CGRect(_paddingX, _paddingY, viewBoxContainerWidth, ScaleUtility.GetScaledHeight(16f)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.BrownGreyThree,
                TextAlignment = UITextAlignment.Right,
                Text = _controller.GetI18NValue(SSMRConstants.I18N_PreviousReading)
            };
            _containerView.AddSubview(_prevReadingLabel);

            _prevReadingView = new UIView(new CGRect(_paddingX, _prevReadingLabel.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(4f), viewBoxContainerWidth, prevReadingContainerHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _containerView.AddSubview(_prevReadingView);

            _viewBoxContainer = new UIView(new CGRect(_paddingX, _prevReadingView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(8f), viewBoxContainerWidth, viewBoxContainerHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            _viewBoxContainer.Layer.CornerRadius = ScaleUtility.GetScaledHeight(4f);
            for (int i = 0; i < boxMaxCount; i++)
            {
                _viewBoxContainer.AddSubview(CreateViewBox(_viewBoxContainer, i));
            }
            _viewBoxContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                BoxContainerOnTap();
            }));
            _containerView.AddSubview(_viewBoxContainer);

            _errorLabel = new UILabel(new CGRect(_paddingX, _viewBoxContainer.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(4f), viewBoxContainerWidth, ScaleUtility.GetScaledHeight(14f)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.Tomato,
                TextAlignment = UITextAlignment.Right,
                Hidden = true
            };
            _containerView.AddSubview(_errorLabel);

            nfloat iconHeight = DeviceHelper.GetScaledHeight(imgHeight);
            nfloat iconWidth = DeviceHelper.GetScaledWidth(imgWidth);
            _iconYposOriginal = _viewBoxContainer.Frame.GetMaxY() + (_containerHeightOriginal - _viewBoxContainer.Frame.GetMaxY()) / 2 - (iconHeight / 2);
            _iconView = new UIView(new CGRect(_containerView.Frame.Width - _paddingX - iconWidth, _iconYposOriginal, iconWidth, iconHeight))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            _iconView.Layer.CornerRadius = ScaleUtility.GetScaledHeight(5f);

            _iconLabel = new UILabel(new CGRect(0, 0, _iconView.Frame.Width, _iconView.Frame.Height))
            {
                Font = TNBFont.MuseoSans_14_500,
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
                if (!string.IsNullOrEmpty(model.ReadingUnit) && !string.IsNullOrWhiteSpace(model.ReadingUnit))
                {
                    _iconLabel.Text = string.IsNullOrEmpty(model.ReadingUnitDisplayTitle)
                            ? model.ReadingUnit : model.ReadingUnitDisplayTitle;
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
                Tag = boxMaxCount - index,
                ClipsToBounds = false
            };
            UITextField txtFieldDigit = new UITextField(new CGRect(0, 0, width, height))
            {
                Enabled = false,
                TextColor = MyTNBColor.GreyishBrownTwo,
                Font = TNBFont.MuseoSans_14_500,
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
            return viewBox;
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            textField.ShouldBeginEditing = (sender) =>
            {
                IsActive = true;
                return true;
            };
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
                IsActive = true;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                {
                    textField.Enabled = false;
                }
                RepopulateTextFields();
                ValidateTextField();
                IsActive = false;
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
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.BrownGreyThree,
                TextAlignment = UITextAlignment.Center,
                Text = digit.ToString()
            };

            viewBox.AddSubview(digitLabel);

            return viewBox;
        }

        public void SetPreviousReading(string prevReading)
        {
            if (!string.IsNullOrEmpty(prevReading) && !string.IsNullOrWhiteSpace(prevReading))
            {
                _previousMeterReadingValue = prevReading;
                PopulatePreviousReading(_previousMeterReadingValue);
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

        public void ValidateTextField()
        {
            if (!string.IsNullOrEmpty(_meterReadingValue) && !string.IsNullOrWhiteSpace(_meterReadingValue))
            {
                double.TryParse(_meterReadingValue, out double currentValue);
                _controller.SetCurrentReadingValue(_model, currentValue.ToString());
            }
            else
            {
                _controller.SetCurrentReadingValue(_model, string.Empty);
            }

            _controller.SetIsValidManualReadingFlags(_model, false);
        }

        public void UpdateUI(bool isError, string message, string currentReading)
        {
            if (isError)
            {
                _errorLabel.Hidden = false;
                _errorLabel.Text = message ?? string.Empty;
                _iconView.BackgroundColor = MyTNBColor.Tomato;
                ViewHelper.AdjustFrameSetY(_iconView, _errorLabel.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(8f));
            }
            else
            {
                _errorLabel.Hidden = true;
                _errorLabel.Text = string.Empty;
                _iconView.BackgroundColor = MyTNBColor.AlgaeGreen;
                ViewHelper.AdjustFrameSetY(_iconView, _iconYposOriginal);
            }
            AddCardShadow(ref _containerView);
            UpdateTextFieldTextColor(isError);
            _controller.SetIsValidManualReadingFlags(_model, isError);
            if (!isError)
            {
                _controller.SetCurrentReadingValue(_model, currentReading);
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
                        txtField.TextColor = MyTNBColor.GreyishBrownTwo;
                    }
                }
            }

            _errorLabel.Hidden = true;
            _errorLabel.Text = string.Empty;
            ViewHelper.AdjustFrameSetY(_iconView, _iconYposOriginal);
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
                        txtField.TextColor = isError ? MyTNBColor.Tomato : MyTNBColor.AlgaeGreen;
                    }
                }
            }
        }

        public void UpdateMeterReadingValueFromOCR(string readingStr)
        {
            if (!string.IsNullOrEmpty(readingStr) && !string.IsNullOrWhiteSpace(readingStr))
            {
                if (readingStr.Length > 0)
                {
                    var readingValueLength = readingStr.Length - 1;
                    for (int i = 0; i < readingStr.Length; i++)
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
                                    txtField.Text = readingStr.Substring(readingValueLength - i, 1);
                                }
                                break;
                            }
                        }
                    }
                }
                _meterReadingValue = readingStr;
                ValidateTextField();
            }
        }

        public void UpdateMeterReadingValueFromSubmission(string readingStr)
        {
            if (!string.IsNullOrEmpty(readingStr) && !string.IsNullOrWhiteSpace(readingStr))
            {
                if (readingStr.Length > 0)
                {
                    var readingValueLength = readingStr.Length - 1;
                    for (int i = 0; i < readingStr.Length; i++)
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
                                    txtField.Text = readingStr.Substring(readingValueLength - i, 1);
                                }
                                break;
                            }
                        }
                    }
                }
                _meterReadingValue = readingStr;
            }
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = 0.5f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}