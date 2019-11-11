using System;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryHeaderComponent
    {
        private readonly UIView _parentView;
        private UIView _containerView;
        private UILabel _labelTitle, _lblAction, _lblAccountName;
        private UITextView _txtDesc;
        private CustomUIButtonV2 _btnSubmit;
        private CustomUIView _viewDropDownContainer;
        public Action OnButtonTap;

        string _buttonText;
        bool _isBtnHidden;

        private nfloat _padding = ScaleUtility.BaseMarginWidth16;
        private nfloat _navBarHeight;
        private nfloat _bgImgHeight;
        private nfloat _imgWidth = ScaleUtility.GetScaledWidth(24);

        public SSMRReadingHistoryHeaderComponent(UIView parentView, nfloat navBarHeight)
        {
            _parentView = parentView;
            _navBarHeight = navBarHeight;
            _bgImgHeight = parentView.Frame.Width * 0.7F;
        }

        private void CreateComponent()
        {
            nfloat baseYLoc = _bgImgHeight - _navBarHeight - DeviceHelper.GetStatusBarHeight();
            nfloat baseWidth = _parentView.Frame.Width - (_padding * 2);

            _containerView = new UIView(new CGRect(0, baseYLoc, _parentView.Frame.Width, 300f))
            {
                BackgroundColor = UIColor.White
            };

            _labelTitle = new UILabel(new CGRect(_padding, ScaleUtility.GetScaledHeight(24)
                , baseWidth, ScaleUtility.GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            #region Dropdown
            _viewDropDownContainer = new CustomUIView(new CGRect(_padding, _labelTitle.Frame.GetMaxY() + _padding, baseWidth
               , ScaleUtility.GetScaledHeight(51)));
            UILabel lblDropDownTitle = new UILabel(new CGRect(0, 0, baseWidth, ScaleUtility.GetScaledHeight(12)))
            {
                Font = TNBFont.MuseoSans_9_300,
                TextColor = MyTNBColor.BrownGrey,
                TextAlignment = UITextAlignment.Left,
                Text = LanguageUtility.GetCommonI18NValue("account").ToUpper()
            };

            UIImageView imgDropDown = new UIImageView(new CGRect(baseWidth - _imgWidth - ScaleUtility.GetScaledWidth(6)
                , lblDropDownTitle.Frame.GetMaxY(), _imgWidth, _imgWidth))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Dropdow)
            };

            _lblAccountName = new UILabel(new CGRect(0, lblDropDownTitle.Frame.GetMaxY()
               , baseWidth - imgDropDown.Frame.Width - ScaleUtility.GetScaledWidth(6), ScaleUtility.GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = LanguageUtility.GetCommonI18NValue("selectAccount")
            };

            UIView _viewLineTerminate = new UIView(new CGRect(0, ScaleUtility.GetYLocationFromFrame(_lblAccountName.Frame, 1)
                , baseWidth, ScaleUtility.GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkTwo
            };

            _viewDropDownContainer.AddSubviews(new UIView[] { lblDropDownTitle, _lblAccountName, imgDropDown, _viewLineTerminate });
            #endregion
            _lblAction = new UILabel(new CGRect(_padding, ScaleUtility.GetYLocationFromFrame(_viewDropDownContainer.Frame, 16)
               , baseWidth, ScaleUtility.GetScaledHeight(20)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrownTwo,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            _txtDesc = new UITextView(new CGRect(_padding, ScaleUtility.GetYLocationFromFrame(_lblAction.Frame, 8)
                , baseWidth, ScaleUtility.GetScaledHeight(60)))
            {
                Editable = false,
                ScrollEnabled = true,
                UserInteractionEnabled = false,
                ContentInset = new UIEdgeInsets(0, -5, 0, -5)
            };
            _txtDesc.TextContainerInset = UIEdgeInsets.Zero;

            _btnSubmit = new CustomUIButtonV2
            {
                Frame = new CGRect(_padding, _txtDesc.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16)
                    , _parentView.Frame.Width - (_padding * 2), ScaleUtility.GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            { OnButtonTap?.Invoke(); }));
            _containerView.AddSubviews(new UIView { _labelTitle, _lblAction, _txtDesc });
            _containerView.AddSubview(_viewDropDownContainer);
            _containerView.AddSubview(_btnSubmit);
            AdjustViewFrames();
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

        public void SetSubmitButtonHidden(MeterReadingHistoryModel model, bool forceDisplay = false, string title = "")
        {
            if (forceDisplay)
            {
                _isBtnHidden = false;
                _btnSubmit.SetTitle(title, UIControlState.Normal);
                _btnSubmit.Hidden = _isBtnHidden;
            }
            else
            {
                if (model != null)
                {
                    var ctaChar = model?.DashboardCTAType?.ToLower() ?? string.Empty;
                    if (ctaChar == DashboardHomeConstants.CTA_ShowReadingHistory)
                    {
                        _isBtnHidden = true;
                    }
                    else if (ctaChar == DashboardHomeConstants.CTA_ShowSubmitReading)
                    {
                        _isBtnHidden = model.IsCurrentPeriodSubmitted;
                        _isBtnHidden = model.IsDashboardCTADisabled;
                    }
                    _buttonText = model?.DashboardCTAText ?? string.Empty;
                }
                _btnSubmit.SetTitle(_buttonText, UIControlState.Normal);
                _btnSubmit.Hidden = _isBtnHidden;
            }
            AdjustViewFrames();
        }

        public void SetTitle(string text)
        {
            _labelTitle.Text = text ?? string.Empty;
            CGSize labelNewSize = CustomUILabel.GetLabelSize(_labelTitle, _parentView.Frame.Width - (_padding * 2), 1000f);
            CGRect frame = _labelTitle.Frame;
            frame.Height = labelNewSize.Height;
            _labelTitle.Frame = frame;
            AdjustViewFrames();
        }

        public string ActionTitle
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                _lblAction.Text = value;
            }
        }

        public string AccountName
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                _lblAccountName.Text = value;
            }
        }

        public Action DropdownAction
        {
            set
            {
                _viewDropDownContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (value != null) { value.Invoke(); }
                }));
            }
        }

        public void SetDescription(string text)
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(text ?? string.Empty, ref htmlBodyError
                , MyTNBFont.FONTNAME_300, (float)ScaleUtility.GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.GreyishBrownTwo
            }, new NSRange(0, htmlBody.Length));
            _txtDesc.AttributedText = mutableHTMLBody;
            CGSize labelNewSize = _txtDesc.SizeThatFits(new CGSize(_parentView.Frame.Width - (_padding * 2), 1000f));
            CGRect frame = _txtDesc.Frame;
            frame.Height = labelNewSize.Height;
            _txtDesc.Frame = frame;
            AdjustViewFrames();
        }

        public void SetNoSSMRHeader()
        {
            CGRect frame = _containerView.Frame;
            frame.Height = ScaleUtility.GetYLocationFromFrame(_viewDropDownContainer.Frame, 16);
            _containerView.Frame = frame;
        }

        private void AdjustViewFrames()
        {
            bool hasAction = !string.IsNullOrEmpty(_lblAction.Text);
            if (hasAction)
            {
                CGRect frame = _txtDesc.Frame;
                frame.Y = ScaleUtility.GetYLocationFromFrame(_lblAction.Frame, 8);
                _txtDesc.Frame = frame;
            }
            else
            {
                CGRect frame = _txtDesc.Frame;
                frame.Y = ScaleUtility.GetYLocationFromFrame(_viewDropDownContainer.Frame, 16);
                _txtDesc.Frame = frame;
            }

            if (!_isBtnHidden)
            {
                CGRect frame = _btnSubmit.Frame;
                frame.Y = ScaleUtility.GetYLocationFromFrame(_txtDesc.Frame, 16);
                _btnSubmit.Frame = frame;
            }
            CGRect containerFrame = _containerView.Frame;
            containerFrame.Height = _isBtnHidden ? _txtDesc.Frame.GetMaxY() + _padding : _btnSubmit.Frame.GetMaxY() + _padding;
            _containerView.Frame = containerFrame;
        }
    }
}