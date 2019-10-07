using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class HelpTableViewCell : UITableViewCell
    {
        private UIScrollView _scrollView;
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat cardWidth;
        private nfloat cardHeight;
        private int _imgIndex;
        public UILabel _titleLabel;
        public HelpTableViewCell(IntPtr handle) : base(handle)
        {
            cardWidth = ScaleUtility.GetScaledWidth(92);
            cardHeight = cardWidth;
            _titleLabel = new UILabel(new CGRect(ScaleUtility.BaseMarginWidth16, ScaleUtility.GetScaledHeight(2f), cellWidth - 32, 20f))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue
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
            if (view != null)
            {
                view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(List<HelpModel> helpList, bool isLoading)
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            if (isLoading || helpList == null)
            {
                AddShimmer();
            }
            else
            {
                AddContentData(helpList);
            }
        }

        private void AddShimmer()
        {
            nfloat xLoc = ScaleUtility.BaseMarginWidth16;
            for (int i = 0; i < 3; i++)
            {
                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewParent = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewShimmerUI = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = MyTNBColor.PaleGrey };
                viewShimmerContent.AddSubview(viewShimmerUI);

                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();

                _scrollView.Add(viewParent);
                xLoc += cardWidth + ScaleUtility.BaseMarginWidth8;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddContentData(List<HelpModel> helpList)
        {
            nfloat xLoc = ScaleUtility.BaseMarginWidth16;
            _imgIndex = -1;
            for (int i = 0; i < helpList.Count; i++)
            {
                string helpKey = helpList[i].TargetItem;
                UIView helpCardView = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { ClipsToBounds = true };
                helpCardView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    ViewHelper.GoToFAQScreenWithId(helpKey);
                }));
                UIImageView imgView = new UIImageView(new CGRect(0, 0, cardWidth, cardHeight))
                {
                    Image = UIImage.FromBundle(string.Format("Help-Background-{0}", GetBackgroundImage(i)))
                };
                AddCardShadow(ref helpCardView);
                UILabel lblHelp = new UILabel(new CGRect(8, 8, helpCardView.Frame.Width - 16, helpCardView.Frame.Height - 16))
                {
                    TextColor = UIColor.White,
                    Font = TNBFont.MuseoSans_12_500,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = helpList[i]?.Title ?? string.Empty
                };
                helpCardView.AddSubviews(new UIView[] { imgView, lblHelp });
                _scrollView.Add(helpCardView);
                xLoc += cardWidth + ScaleUtility.BaseMarginWidth8;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private int GetBackgroundImage(int index)
        {
            _imgIndex = (index / 7) % 1 == 0 ? _imgIndex + 1 : 0;
            _imgIndex = _imgIndex > 6 || _imgIndex < 0 ? 0 : _imgIndex;
            return _imgIndex;
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 4.0F;
            view.ClipsToBounds = true;
        }
    }
}