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
        UIView _viewContainer;
        UIImageView _iconView;
        UILabel _lblDescription;
        public UIButton _btnRefresh;
        public Action OnButtonTap;

        string _descriptionMessage;
        string _buttonText;
        bool _isBtnHidden;

        public RefreshScreenComponent(UIView parentView)
        {
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

            float iconWidth = DeviceHelper.GetScaledWidth(138f);
            _iconView = new UIImageView(new CGRect(DeviceHelper.GetCenterXWithObjWidth(iconWidth, _viewContainer), iconYPos, iconWidth, DeviceHelper.GetScaledHeight(138.0f)))
            {
                Image = UIImage.FromBundle("Refresh-Icon")
            };

            var descMsg = _descriptionMessage ?? string.Empty;
            var btnText = _buttonText ?? "Error_RefreshBtnTitle".Translate();

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center,
                MinimumLineHeight = lineTextHeight,
                MaximumLineHeight = lineTextHeight
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans16_300,
                ForegroundColor = MyTNBColor.BrownGreyThree,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            var attributedText = new NSMutableAttributedString(descMsg);
            attributedText.AddAttributes(msgAttributes, new NSRange(0, descMsg.Length));

            _lblDescription = new UILabel()
            {
                AttributedText = attributedText,
                Lines = 0
            };

            nfloat descPadding = 32f;
            nfloat buttonPadding = 16f;
            nfloat labelWidth = (float)(_viewContainer.Frame.Width - (descPadding * 2));
            nfloat buttonWidth = (float)(_viewContainer.Frame.Width - (buttonPadding * 2));
            nfloat buttonHeight = 48f;
            CGSize cGSize = _lblDescription.SizeThatFits(new CGSize(labelWidth, 1000f));
            _lblDescription.Frame = new CGRect(descPadding, _iconView.Frame.GetMaxY() + 16f, labelWidth, cGSize.Height);

            _btnRefresh = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(buttonPadding, _lblDescription.Frame.GetMaxY() + buttonPadding, buttonWidth, buttonHeight),
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
            _viewContainer.AddSubview(_lblDescription);
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
            frame.Height = _isBtnHidden ? _lblDescription.Frame.GetMaxY() + 16f : _btnRefresh.Frame.GetMaxY() + 16f;
            _viewContainer.Frame = frame;
        }

        public nfloat GetViewHeight()
        {
            return _viewContainer.Frame.Height;
        }
    }
}
