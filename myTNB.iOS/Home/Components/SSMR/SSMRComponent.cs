using System;
using System.Diagnostics;
using CoreGraphics;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        UIImageView _iconView;
        public UILabel _labelViewHistory;
        UILabel _description;
        public UIButton _smrButton;
        nfloat _yLocation = 0f;
        nfloat _descWidth = 0f;
        nfloat _padding = 16f;

        public SSMRComponent(UIView parentView, nfloat yLocation)
        {
            _parentView = parentView;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            nfloat iconPadding = 12f;
            nfloat imageWidth = 32f;
            nfloat imageHeight = 32f;
            nfloat buttonHeight = 40f;
            nfloat yPadding = 18f;
            nfloat width = _parentView.Frame.Width;

            _containerView = new UIView(new CGRect(_padding, _yLocation + yPadding, width - (_padding * 2), 116f))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = 4.0f;

            _iconView = new UIImageView(new CGRect(iconPadding, _padding, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_SMRMediumIcon)
            };
            _descWidth = _containerView.Frame.Width - (_iconView.Frame.GetMaxX() + (iconPadding * 2));
            _description = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + iconPadding, _padding, _descWidth, 48))
            {
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _labelViewHistory = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + iconPadding, _description.Frame.GetMaxY() + 8f, _descWidth, 20f))
            {
                UserInteractionEnabled = true,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.WaterBlue,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Hidden = true
            };
            _smrButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(_padding, _description.Frame.GetMaxY() + _padding, _containerView.Frame.Width - (_padding * 2), buttonHeight)
            };
            _smrButton.Layer.CornerRadius = 4;
            _smrButton.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _smrButton.Layer.BorderWidth = 1;
            _smrButton.Font = MyTNBFont.MuseoSans16_500;
            _smrButton.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _smrButton.Enabled = true;

            _containerView.AddSubviews(new UIView { _iconView, _description });
            _containerView.AddSubview(_labelViewHistory);
            _containerView.AddSubview(_smrButton);
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

        public void SetDescription(string text)
        {
            _description.Text = text;
            CGSize descNewSize = _description.SizeThatFits(new CGSize(_descWidth, 1000f));
            CGRect frame = _description.Frame;
            frame.Height = descNewSize.Height;
            _description.Frame = frame;

            AdjustViewFrames();
        }

        private void AdjustViewFrames(bool linkIsVisible = false)
        {
            CGRect btnFrame = _smrButton.Frame;
            btnFrame.Y = linkIsVisible ? _labelViewHistory.Frame.GetMaxY() + _padding : _description.Frame.GetMaxY() + _padding;
            _smrButton.Frame = btnFrame;

            CGRect containerFrame = _containerView.Frame;
            containerFrame.Height = _smrButton.Frame.GetMaxY() + _padding;
            _containerView.Frame = containerFrame;
        }

        public void SetFrameByPrecedingView(float yLocation)
        {
            var newFrame = _containerView.Frame;
            newFrame.Y = yLocation + 18f;
            _containerView.Frame = newFrame;
        }

        public void SetSRMButtonEnable(bool isEnable)
        {
            _smrButton.Enabled = isEnable;
            _smrButton.SetTitleColor(isEnable ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice, UIControlState.Normal);
            _smrButton.Layer.BorderColor = isEnable ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        public void SetButtonText(string text)
        {
            _smrButton.SetTitle(text, UIControlState.Normal);
        }

        public void ShowHistoryLink(bool showLink, string text)
        {
            _labelViewHistory.Hidden = !showLink;
            if (showLink)
            {
                _labelViewHistory.Text = text;
                CGSize labelNewSize = _labelViewHistory.SizeThatFits(new CGSize(_descWidth, 1000f));
                CGRect frame = _labelViewHistory.Frame;
                frame.Y = _description.Frame.GetMaxY() + 8f;
                frame.Height = labelNewSize.Height;
                _labelViewHistory.Frame = frame;
            }
            AdjustViewFrames(showLink);
        }
    }
}
