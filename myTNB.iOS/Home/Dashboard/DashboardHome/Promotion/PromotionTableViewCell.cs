using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class PromotionTableViewCell : UITableViewCell
    {
        private UIScrollView _scrollView;
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat cardWidth;
        private nfloat cardHeight;
        private int _imgIndex;
        public UILabel _titleLabel;
        public PromotionTableViewCell(IntPtr handle) : base(handle)
        {
            cardWidth = cellWidth * 0.64F;
            cardHeight = cardWidth * 0.98F;
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
            _scrollView = new UIScrollView(new CGRect(0, 0, view.Frame.Width, cardHeight))
            {
                ScrollEnabled = true,
                ShowsHorizontalScrollIndicator = false
            };
            view.AddSubview(_scrollView);
            AddSubview(view);
            BackgroundColor = UIColor.Clear;
            SelectionStyle = UITableViewCellSelectionStyle.None;

            ClipsToBounds = true;
        }

        public void AddCards()
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            bool hasData = false;

            if (hasData)
            {
                AddContentData();
            }
            else
            {
                AddShimmer();
            }
        }

        private void AddShimmer()
        {
            nfloat xLoc = 16f;
            for (int i = 0; i < 3; i++)
            {
                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewParent = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
                AddCardShadow(ref viewParent);
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewImg = new UIView(new CGRect(0, 0, cardWidth, cardHeight * 0.48F)) { BackgroundColor = MyTNBColor.PowderBlue };
                UIView viewTitle = new UIView(new CGRect(16, viewImg.Frame.GetMaxY() + 16, cardWidth * 0.62F, 16))
                { BackgroundColor = MyTNBColor.PowderBlue };
                UIView viewContent1 = new UIView(new CGRect(16, viewTitle.Frame.GetMaxY() + 4, viewTitle.Frame.Width * 0.8F, 14))
                { BackgroundColor = MyTNBColor.PowderBlue };
                UIView viewContent2 = new UIView(new CGRect(16, viewContent1.Frame.GetMaxY() + 4, viewTitle.Frame.Width * 0.5F, 14))
                { BackgroundColor = MyTNBColor.PowderBlue };
                UIView viewContent3 = new UIView(new CGRect(16, viewContent2.Frame.GetMaxY() + 4, cardWidth - 32, 14))
                { BackgroundColor = MyTNBColor.PowderBlue };

                viewShimmerContent.AddSubviews(new UIView[] { viewImg, viewTitle, viewContent1, viewContent2, viewContent3 });

                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();

                _scrollView.Add(viewParent);
                xLoc += cardWidth + 8.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddContentData()
        {
            nfloat xLoc = 16f;

            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5.0F;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 0.5F;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
