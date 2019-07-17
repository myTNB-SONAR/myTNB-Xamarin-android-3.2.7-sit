using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class HelpTableViewCell : UITableViewCell
    {
        private UIScrollView _scrollView;
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        public HelpTableViewCell(IntPtr handle) : base(handle)
        {
            UIView view = new UIView(new CGRect(0, 0, cellWidth, 64.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            _scrollView = new UIScrollView(new CGRect(16, 0, view.Frame.Width - 16, 40.0F));
            _scrollView.ScrollEnabled = true;
            _scrollView.ShowsHorizontalScrollIndicator = false;
            view.AddSubview(_scrollView);
            AddSubview(view);
            BackgroundColor = UIColor.Clear;
            view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards()
        {
            List<string> helpList = new List<string>() { "How to read meter?", "How to reset password?", "How to apply for AutoPay ?" };
            nfloat xLoc = 0;
            nfloat cardWidth = cellWidth * 0.312F;
            for (int i = 0; i < helpList.Count; i++)
            {
                UIView helpCardView = new UIView(new CGRect(xLoc, 0, cardWidth, 40))
                {
                    BackgroundColor = UIColor.Blue
                };
                helpCardView.Layer.CornerRadius = 4.0F;
                AddCardShadow(ref helpCardView);
                UILabel lblHelp = new UILabel(new CGRect(8, 8, helpCardView.Frame.Width - 16, helpCardView.Frame.Height - 16))
                {
                    TextColor = UIColor.White,
                    Font = MyTNBFont.MuseoSans10_500,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = helpList[i]
                };
                helpCardView.AddSubview(lblHelp);
                _scrollView.Add(helpCardView);
                xLoc += cardWidth + 8.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, 40);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.SilverChalice.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 0.5F;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}