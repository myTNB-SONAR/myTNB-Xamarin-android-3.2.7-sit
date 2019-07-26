using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryHeaderComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        UIImageView _icon;
        UILabel _labelTitle, _labelDesc;
        public SSMRReadingHistoryHeaderComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, 300f));
            _icon = new UIImageView(new CGRect(0, 0, _parentView.Frame.Width, 159f))
            {
                Image = UIImage.FromBundle("SMR-Open-Submitted-BG"),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            _labelTitle = new UILabel(new CGRect(0, _icon.Frame.GetMaxY() + 16f, _parentView.Frame.Width, 24f))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _labelDesc = new UILabel(new CGRect(0, _labelTitle.Frame.GetMaxY() + 8f, _parentView.Frame.Width, 60f))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.GreyishBrownTwo,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _containerView.AddSubviews(new UIView { _icon, _labelTitle, _labelDesc });
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

        public void SetTitle(string text)
        {
            _labelTitle.Text = text ?? string.Empty;
        }

        public void SetDescription(string text)
        {
            _labelDesc.Text = text ?? string.Empty;
            CGSize labelNewSize = _labelDesc.SizeThatFits(new CGSize(_parentView.Frame.Width, 1000f));
            CGRect frame = _labelDesc.Frame;
            frame.Height = labelNewSize.Height;
            _labelDesc.Frame = frame;

            AdjustViewFrames();
        }

        private void AdjustViewFrames()
        {
            CGRect containerFrame = _containerView.Frame;
            containerFrame.Height = _labelDesc.Frame.GetMaxY() + 16f;
            _containerView.Frame = containerFrame;
        }
    }
}
