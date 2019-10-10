using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class EnergyTipsComponent
    {
        private CustomUIView _containerView;
        private UIView _parentView;
        private UIScrollView _scrollView;

        private List<TipsPresentationModel> _tipsList;

        private nfloat scrollViewRatio = 100.0f / 288.0f;
        private nfloat margin = ScaleUtility.GetScaledWidth(8f);
        private nfloat paddingX = ScaleUtility.GetScaledWidth(16f);
        private nfloat paddingY = ScaleUtility.GetScaledHeight(16f);
        private nfloat containerHeight = ScaleUtility.GetScaledHeight(100f);
        private int currentPageIndex;

        public EnergyTipsComponent(UIView parentView, List<TipsPresentationModel> tipsList)
        {
            _parentView = parentView;
            _tipsList = tipsList;
        }

        private void CreateComponent()
        {
            _containerView = new CustomUIView(new CGRect(0, 0, _parentView.Frame.Width, containerHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            CreateScrollView();
            SetScrollViewSubViews();
        }

        private void CreateScrollView()
        {
            nfloat scrollViewHeight = _containerView.Frame.Width * scrollViewRatio;
            _scrollView = new UIScrollView(new CGRect(0, 0, _containerView.Frame.Width, scrollViewHeight))
            {
                Delegate = new ScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = false,
                BackgroundColor = UIColor.Clear,
                Hidden = false
            };
            AdjustFrame(_scrollView, margin, 0, -margin * 3, 0);
            _containerView.AddSubview(_scrollView);
        }

        private void SetScrollViewSubViews()
        {
            nfloat width = _scrollView.Frame.Width;
            for (int i = 0; i < _tipsList.Count; i++)
            {
                UILabel title, desc;
                UIImageView bgView, iconView;

                CustomUIView viewContainer = new CustomUIView(_scrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                ViewHelper.AdjustFrameSetX(viewContainer, (i * width) + margin);
                ViewHelper.AdjustFrameSetY(viewContainer, (_scrollView.Frame.Height - viewContainer.Frame.Height) / 2);
                ViewHelper.AdjustFrameSetWidth(viewContainer, width - (margin * 1));

                viewContainer.Layer.CornerRadius = ScaleUtility.GetScaledHeight(4f);

                nfloat viewWidth = viewContainer.Frame.Width;
                nfloat viewHeight = viewContainer.Frame.Height;

                bgView = new UIImageView(new CGRect(0, 0, viewWidth, viewHeight))
                {
                    Image = UIImage.FromBundle(Constants.IMG_SavingTipsBg)
                };
                viewContainer.AddSubview(bgView);

                title = new UILabel(new CGRect(paddingX, ScaleUtility.GetScaledHeight(12f), viewWidth - (paddingX * 2), ScaleUtility.GetScaledHeight(16f)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = MyTNBColor.GreyishBrown,
                    TextAlignment = UITextAlignment.Left,
                    Text = _tipsList[i].Title ?? string.Empty
                };
                viewContainer.AddSubview(title);

                nfloat descViewPosX = ScaleUtility.GetScaledWidth(12f);
                nfloat descViewPosY = title.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(8f);
                nfloat descViewHeight = viewHeight - descViewPosY - paddingY;
                UIView descView = new UIView(new CGRect(descViewPosX, descViewPosY, viewWidth - (descViewPosX + paddingY), descViewHeight))
                {
                    BackgroundColor = UIColor.Clear,
                };
                viewContainer.AddSubview(descView);

                UIImage displayImage = UIImage.FromBundle(string.Empty);
                if (_tipsList[i].NSDataImage != null)
                {
                    displayImage = UIImage.LoadFromData(_tipsList[i].NSDataImage);
                }

                nfloat iconWidth = ScaleUtility.GetScaledWidth(36f);
                iconView = new UIImageView(new CGRect(0, ScaleUtility.GetYLocationToCenterObject(iconWidth, descView), iconWidth, iconWidth))
                {
                    Image = displayImage
                };
                descView.AddSubview(iconView);

                nfloat descPosX = iconView.Frame.GetMaxX() + ScaleUtility.GetScaledWidth(8f);
                nfloat descWidth = descView.Frame.Width - descPosX - paddingX;
                desc = new UILabel(new CGRect(descPosX, 0, descWidth, descView.Frame.Height))
                {
                    BackgroundColor = UIColor.Clear,
                    Font = TNBFont.MuseoSans_12_300,
                    TextColor = MyTNBColor.GreyishBrown,
                    Lines = 0,
                    TextAlignment = UITextAlignment.Left,
                    Text = _tipsList[i].Description
                };
                descView.AddSubview(desc);
                AddTipsShadow(ref viewContainer);
                _scrollView.AddSubview(viewContainer);
            }
            _scrollView.ContentSize = new CGSize(_scrollView.Frame.Width * _tipsList.Count, _scrollView.Frame.Height);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private CGRect AdjustFrame(CGRect f, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            f.X += x;
            f.Y += y;
            f.Width += w;
            f.Height += h;

            return f;
        }

        private void AdjustFrame(UIView view, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            view.Frame = AdjustFrame(view.Frame, x, y, w, h);
        }

        private void AddTipsShadow(ref CustomUIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = .32f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private class ScrollViewDelegate : UIScrollViewDelegate
        {
            EnergyTipsComponent _controller;
            public ScrollViewDelegate(EnergyTipsComponent controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._scrollView.ContentOffset.X / _controller._scrollView.Frame.Width);
                if (newPageIndex == _controller.currentPageIndex)
                    return;
                _controller.currentPageIndex = newPageIndex;
            }
        }
    }
}
