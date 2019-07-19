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
        private nfloat cardWidth;
        private nfloat cardHeight;
        public UILabel _titleLabel;
        public HelpTableViewCell(IntPtr handle) : base(handle)
        {
            cardWidth = cellWidth * 0.30F;
            cardHeight = cardWidth;
            _titleLabel = new UILabel(new CGRect(16f, 16f, cellWidth - 32, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.PowerBlue
            };
            AddSubview(_titleLabel);
            UIView view = new UIView(new CGRect(0, _titleLabel.Frame.GetMaxY() + 8f, cellWidth, cardHeight + 24.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            _scrollView = new UIScrollView(new CGRect(0, 0, view.Frame.Width, cardHeight));
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

        public void AddCards(bool hasData = false)
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            if (hasData)
            {
                AddContentData();
            }
            else
            {
                AddShimmer();
                InvokeInBackground(() =>
                {
                    System.Threading.Thread.Sleep(8000);
                    InvokeOnMainThread(() =>
                    {
                        AddCards(true);
                    });
                });
            }
        }

        private void AddShimmer()
        {
            nfloat xLoc = 16f;
            for (int i = 0; i < 3; i++)
            {
                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewParent = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewShimmerUI = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = MyTNBColor.PowderBlue };
                viewShimmerContent.AddSubview(viewShimmerUI);

                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;

                _scrollView.Add(viewParent);
                xLoc += cardWidth + 12.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }



        private void AddContentData()
        {
            List<string> helpList = new List<string>() { "How to read meter?", "How to reset password?", "How to apply for AutoPay ?" };
            nfloat xLoc = 16f;
            for (int i = 0; i < helpList.Count; i++)
            {
                UIView helpCardView = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight))
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
                xLoc += cardWidth + 12.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, 40);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = true;
            view.Layer.ShadowColor = MyTNBColor.SilverChalice.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 4.0F;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}