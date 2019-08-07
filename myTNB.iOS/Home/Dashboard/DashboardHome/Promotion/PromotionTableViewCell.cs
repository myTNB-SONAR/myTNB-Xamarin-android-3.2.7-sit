using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
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
            UIView view = new UIView(new CGRect(0, _titleLabel.Frame.GetMaxY(), cellWidth, cardHeight + 24.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            _scrollView = new UIScrollView(new CGRect(0, 0, view.Frame.Width, cardHeight + 16))
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

        public void AddCards(List<PromotionsModelV2> promotions)
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            if (promotions != null && promotions.Count > 0)
            {
                AddContentData(promotions);
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
                UIView viewParent = new UIView(new CGRect(xLoc, 8, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
                AddCardShadow(ref viewParent);
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewImg = new UIView(new CGRect(0, 0, cardWidth, cardHeight * 0.50F)) { BackgroundColor = MyTNBColor.PowderBlue };
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

        private void AddContentData(List<PromotionsModelV2> promotions)
        {
            nfloat xLoc = 16f;
            for (int i = 0; i < promotions.Count; i++)
            {
                PromotionsModelV2 promotion = promotions[i];
                UIView viewParent = new UIView(new CGRect(xLoc, 8, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
                AddCardShadow(ref viewParent);
                UIImageView imgView = new UIImageView(new CGRect(0, 0, viewParent.Frame.Width, cardHeight * 0.50F))
                {
                    //Image = UIImage.FromBundle(promotion.LandscapeImage)
                };

                UILabel lblTitle = new UILabel(new CGRect(16, imgView.Frame.GetMaxY() + 16, viewParent.Frame.Width - 32, 16))
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = MyTNBColor.WaterBlue,
                    Font = MyTNBFont.MuseoSans12_500,
                    Text = promotion.Title
                };

                UILabel lblDescription = new UILabel(new CGRect(16, lblTitle.Frame.GetMaxY() + 8, viewParent.Frame.Width - 32, 48))
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = MyTNBColor.GreyishBrown,
                    Font = MyTNBFont.MuseoSans12_300,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = promotion.Text
                };

                viewParent.AddSubviews(new UIView[] { imgView, lblTitle, lblDescription });
                _scrollView.Add(viewParent);
                xLoc += cardWidth + 8.0F;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5.0F;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 0.5F;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 4;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private List<PromotionsModelV2> GetStaticPromotionList()
        {
            return new List<PromotionsModelV2>
            {
                new PromotionsModelV2
                {
                    Title="TNB Energy Night Run",
                    Text = "Join some excited 3,500 runners to raise awareness towards energy conservation.",
                    LandscapeImage = "Stub-Promotion-1"
                },
                new PromotionsModelV2
                {
                    Title="Maevi - Celcom Bonanza 2019",
                    Text = "Get 15% discount off all MAEVI devices and extra 30% discount off MAEVI Gateway.",
                    LandscapeImage = "Stub-Promotion-2"
                }
            };
        }
    }
}
