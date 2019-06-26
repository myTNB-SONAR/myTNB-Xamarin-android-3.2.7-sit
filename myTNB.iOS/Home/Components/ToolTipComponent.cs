using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class ToolTipComponent
    {
        readonly UIView _parentView;
        UIView _toolTipView;
        UILabel _lblToolTip;
        public ToolTipComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        public void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width / 2;
            _toolTipView = new UIView(new CGRect(width, 0, width, 16));
            _lblToolTip = new UILabel(new CGRect(0, 0, _toolTipView.Frame.Width, _toolTipView.Frame.Height))
            {
                TextAlignment = UITextAlignment.Right,
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.SunGlow
            };
            _toolTipView.AddSubview(_lblToolTip);
        }

        public void SetEvent(UITapGestureRecognizer tapEvent)
        {
            _toolTipView.AddGestureRecognizer(tapEvent);
        }

        public void SetTextColor(UIColor textColor)
        {
            _lblToolTip.TextColor = textColor;
        }

        public void SetContent(string key)
        {
            _lblToolTip.Text = key.Translate();
        }

        public void SetTextAlignment(UITextAlignment textAlignment)
        {
            _lblToolTip.TextAlignment = textAlignment;
        }

        public void SetTopMargin(nfloat topMargin)
        {
            _toolTipView.Frame = new CGRect(_toolTipView.Frame.X, topMargin
                , _toolTipView.Frame.Width, _toolTipView.Frame.Height);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _toolTipView;
        }
    }
}