using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Components
{
    public class RefreshScreenComponent : BaseComponent
    {
        private readonly UIView _parentView;
        CustomUIView _viewContainer;
        UIImageView _iconView;
        UITextView _txtDescription;
        public UIButton _btnRefresh;
        public Action OnButtonTap;

        string _descriptionMessage;
        bool _isBCRMDown;
        string _buttonText;
        bool _isBtnHidden;
        nfloat _iconYPos;

        public RefreshScreenComponent(UIView parentView, nfloat iconYPos)
        {
            _parentView = parentView;
            _iconYPos = iconYPos;
        }

        public void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;

            _viewContainer = new CustomUIView(new CGRect(0, 0, width, 300f))
            {
                BackgroundColor = UIColor.Clear
            };

            nfloat iconWidth = _isBCRMDown ? GetScaledWidth(80f) : GetScaledWidth(85f);
            nfloat iconHeight = _isBCRMDown ? GetScaledHeight(78f) : GetScaledHeight(80f);
            _iconView = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth, _viewContainer), _iconYPos, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(_isBCRMDown ? Constants.IMG_BCRMDownIcon : Constants.IMG_RefreshIcon)
            };

            var descMsg = !string.IsNullOrEmpty(_descriptionMessage) && !string.IsNullOrWhiteSpace(_descriptionMessage) ? _descriptionMessage : GetCommonI18NValue(Constants.Common_RefreshMessage);
            var btnText = (!string.IsNullOrEmpty(_buttonText)) && !string.IsNullOrWhiteSpace(_buttonText) ? _buttonText : GetCommonI18NValue(Constants.Common_RefreshBtnText);

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_14_300,
                ForegroundColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_14_300,
                ForegroundColor = UIColor.White,
                UnderlineStyle = NSUnderlineStyle.Single,
                BackgroundColor = UIColor.Clear
            };

            nfloat descPadding = GetScaledWidth(32f);
            nfloat buttonPadding = GetScaledWidth(16f);
            nfloat labelWidth = (float)(_viewContainer.Frame.Width - (descPadding * 2));
            nfloat buttonWidth = (float)(_viewContainer.Frame.Width - (buttonPadding * 2));
            nfloat buttonHeight = GetScaledHeight(48f);

            _txtDescription = new UITextView(new CGRect(descPadding, _iconView.Frame.GetMaxY() + GetScaledHeight(18F), labelWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                Selectable = false
            };

            NSError htmlError = null;
            try
            {
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(descMsg, ref htmlError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));
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

            CGSize cGSize = _txtDescription.SizeThatFits(new CGSize(labelWidth, 1000F));
            ViewHelper.AdjustFrameHeight(_txtDescription, cGSize.Height);

            _btnRefresh = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(buttonPadding, _txtDescription.Frame.GetMaxY() + GetScaledHeight(12F), buttonWidth, buttonHeight),
                Hidden = _isBtnHidden,
                BackgroundColor = UIColor.White,
                Font = TNBFont.MuseoSans_16_500
            };

            _btnRefresh.Layer.CornerRadius = 4;
            _btnRefresh.SetTitle(btnText, UIControlState.Normal);
            _btnRefresh.SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            _btnRefresh.TouchUpInside += (sender, e) =>
            {
                OnButtonTap?.Invoke();
            };

            _viewContainer.AddSubview(_iconView);
            _viewContainer.AddSubview(_txtDescription);
            _viewContainer.AddSubview(_btnRefresh);
            AdjustContainerHeight();
        }

        public CustomUIView GetView()
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
            ViewHelper.AdjustFrameSetHeight(_viewContainer, _isBtnHidden ? _txtDescription.Frame.GetMaxY() + GetScaledHeight(24F) : _btnRefresh.Frame.GetMaxY() + GetScaledHeight(24F));
        }

        public nfloat GetViewHeight()
        {
            return _viewContainer.Frame.Height;
        }
    }
}
