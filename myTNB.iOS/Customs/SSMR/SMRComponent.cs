using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SMRComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        UIImageView _iconView;
        UILabel _labelViewHistory;
        UILabel _description;
        UIButton _smrButton;
        nfloat _yLocation = 0f;
        nfloat _descWidth = 0f;
        nfloat _padding = 16f;

        public SMRComponent(UIView parentView, nfloat yLocation)
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
                Image = UIImage.FromBundle("SMR-Medium-Icon")
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

            _smrButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(_padding, _description.Frame.GetMaxY() + _padding, _containerView.Frame.Width - (_padding * 2), buttonHeight)
            };
            _smrButton.Layer.CornerRadius = 4;
            _smrButton.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _smrButton.Layer.BorderWidth = 1;
            _smrButton.SetTitle("Dashboard_ViewReadingHistory".Translate(), UIControlState.Normal);
            _smrButton.Font = MyTNBFont.MuseoSans16_500;
            _smrButton.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);

            _containerView.AddSubviews(new UIView { _iconView, _description, _smrButton });
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

        private void AdjustViewFrames()
        {
            CGRect btnFrame = _smrButton.Frame;
            btnFrame.Y = _description.Frame.GetMaxY() + _padding;
            _smrButton.Frame = btnFrame;

            CGRect parentFrame = _containerView.Frame;
            parentFrame.Height = _smrButton.Frame.GetMaxY() + _padding;
            _containerView.Frame = parentFrame;
        }

        public void SetFrameByPrecedingView(float yLocation)
        {
            var newFrame = _containerView.Frame;
            newFrame.Y = yLocation + 18f;
            _containerView.Frame = newFrame;
        }
    }
}
