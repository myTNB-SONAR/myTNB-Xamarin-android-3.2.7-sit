using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Components
{
    public class RefreshScreenComponent
    {
        private readonly UIView _parentView;
        DashboardHomeViewController _dashboardHomeViewController;
        UIView _viewContainer;
        UIImageView _iconView;
        UITextView _txtDescription;
        public UIButton _btnRefresh;
        public Action OnButtonTap;

        string _descriptionMessage;
        bool _isBCRMDown;
        string _buttonText;
        bool _isBtnHidden;

        public RefreshScreenComponent(DashboardHomeViewController dashboardHomeViewController, UIView parentView)
        {
            _dashboardHomeViewController = dashboardHomeViewController;
            _parentView = parentView;
        }

        public void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat iconYPos = 64f;
            nfloat lineTextHeight = 24f;

            _viewContainer = new UIView(new CGRect(0, 0, width, 300f))
            {
                BackgroundColor = UIColor.Clear
            };

            nfloat iconWidth = ScaleUtility.GetScaledWidth(138f);
            _iconView = new UIImageView(new CGRect(ScaleUtility.GetXLocationToCenterObject(iconWidth, _viewContainer), iconYPos, iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_RefreshIcon)
            };

            var descMsg = _descriptionMessage ?? _dashboardHomeViewController.GetI18NValue(DashboardHomeConstants.I18N_RefreshMsg);
            var btnText = _buttonText ?? _dashboardHomeViewController.GetI18NValue(DashboardHomeConstants.I18N_RefreshBtnTxt);

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans16_300,
                ForegroundColor = MyTNBColor.BrownGreyThree,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans16_300,
                ForegroundColor = MyTNBColor.BrownGreyThree,
                UnderlineStyle = NSUnderlineStyle.Single,
                BackgroundColor = UIColor.Clear
            };

            nfloat descPadding = ScaleUtility.GetScaledWidth(32f);
            nfloat buttonPadding = ScaleUtility.GetScaledWidth(16f);
            nfloat labelWidth = (float)(_viewContainer.Frame.Width - (descPadding * 2));
            nfloat buttonWidth = (float)(_viewContainer.Frame.Width - (buttonPadding * 2));
            nfloat buttonHeight = ScaleUtility.GetScaledHeight(48f);

            _txtDescription = new UITextView(new CGRect(descPadding, _iconView.Frame.GetMaxY() + ScaleUtility.BaseMarginWidth16, labelWidth, 90f))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                Selectable = false
            };

            NSError htmlError = null;
            try
            {
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(descMsg, ref htmlError, MyTNBFont.FONTNAME_300, 12f);
                if (htmlBody != null)
                {
                    NSMutableAttributedString mutableDowntime = new NSMutableAttributedString(htmlBody);
                    if (mutableDowntime != null)
                    {
                        mutableDowntime.AddAttributes(msgAttributes, new NSRange(0, htmlBody.Length));
                        _txtDescription.AttributedText = mutableDowntime;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }

            _txtDescription.WeakLinkTextAttributes = linkAttributes.Dictionary;
            if (_isBCRMDown)
            {
                _txtDescription.UserInteractionEnabled = true;
                _txtDescription.AddGestureRecognizer(
                    new UITapGestureRecognizer(() =>
                    {
                        string str = descMsg;
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str.Contains("faqid"))
                            {
                                var startStr = str.Substring(str.IndexOf('{'));
                                if (!string.IsNullOrEmpty(startStr))
                                {
                                    string faqId = startStr?.Split('"')[0];
                                    if (!string.IsNullOrEmpty(faqId))
                                    {
                                        ViewHelper.GoToFAQScreenWithId(faqId);
                                    }
                                }
                            }
                            else if (str.Contains("http") || str.Contains("https"))
                            {
                                var startStr = str.Substring(str.IndexOf('"') + 1);
                                if (!string.IsNullOrEmpty(startStr))
                                {
                                    string url = startStr?.Split('"')[0];
                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        ViewHelper.OpenBrowserWithUrl(url);
                                    }
                                }
                            }
                        }
                    }));
            }

            CGSize cGSize = _txtDescription.SizeThatFits(new CGSize(labelWidth, 1000f));
            _txtDescription.Frame = new CGRect(descPadding, _iconView.Frame.GetMaxY() + ScaleUtility.BaseMarginWidth16, labelWidth, cGSize.Height);

            _btnRefresh = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(buttonPadding, _txtDescription.Frame.GetMaxY() + buttonPadding, buttonWidth, buttonHeight),
                Hidden = _isBtnHidden,
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = MyTNBFont.MuseoSans16_500
            };

            _btnRefresh.Layer.CornerRadius = 4;
            _btnRefresh.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnRefresh.Layer.BorderWidth = 1;
            _btnRefresh.SetTitle(btnText, UIControlState.Normal);
            _btnRefresh.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnRefresh.TouchUpInside += (sender, e) =>
            {
                OnButtonTap?.Invoke();
            };

            _viewContainer.AddSubview(_iconView);
            _viewContainer.AddSubview(_txtDescription);
            _viewContainer.AddSubview(_btnRefresh);
            AdjustContainerHeight();
        }

        public UIView GetView()
        {
            return _viewContainer;
        }

        public void SetDescription(string desc)
        {
            _descriptionMessage = desc;
        }

        public void SetIsBCRMDown(bool flag)
        {
            _isBCRMDown = flag;
        }

        public void SetButtonText(string text)
        {
            _buttonText = text;
        }

        public void SetRefreshButtonHidden(bool flag)
        {
            _isBtnHidden = flag;
        }

        private void AdjustContainerHeight()
        {
            CGRect frame = _viewContainer.Frame;
            frame.Height = _isBtnHidden ? _txtDescription.Frame.GetMaxY() + ScaleUtility.BaseMarginWidth16 : _btnRefresh.Frame.GetMaxY() + ScaleUtility.BaseMarginWidth16;
            _viewContainer.Frame = frame;
        }

        public nfloat GetViewHeight()
        {
            return _viewContainer.Frame.Height;
        }
    }
}
