﻿using System;
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

         public void AddCards(List<HelpModel> helpList)
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            bool hasData = helpList.Count > 0;

            if (hasData)
            {
                AddContentData(helpList);
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
                UIView viewParent = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewShimmerUI = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = MyTNBColor.PowderBlue };
                viewShimmerContent.AddSubview(viewShimmerUI);

                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();

                _scrollView.Add(viewParent);
                xLoc += cardWidth + 12.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddContentData(List<HelpModel> helpList)
        {
            nfloat xLoc = 16f;
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
                    Font = MyTNBFont.MuseoSans10_500,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = helpList[i]?.Title ?? string.Empty
                };
                helpCardView.AddSubviews(new UIView[] { imgView, lblHelp });
                _scrollView.Add(helpCardView);
                xLoc += cardWidth + 12.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, 40);
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
            view.Layer.MasksToBounds = true;
            view.Layer.ShadowColor = MyTNBColor.SilverChalice.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 4.0F;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
