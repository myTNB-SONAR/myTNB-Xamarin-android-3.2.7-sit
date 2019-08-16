using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class EnergyTipsComponent
    {
        UIView _parentView, _containerView;
        UIScrollView _scrollView;
        UILabel _title, _desc;
        UIImageView _iconView;

        nfloat scrollViewRatio = 100.0f / 288.0f;

        nfloat margin = ScaleUtility.GetScaledWidth(8f);
        nfloat containerHeight = ScaleUtility.GetScaledHeight(110f);
        int currentPageIndex;
        int dummyArrayCount = 3;

        public EnergyTipsComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, containerHeight))
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
            for (int i = 0; i < dummyArrayCount; i++)
            {
                UIView viewContainer = new UIView(_scrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                CGRect frame = viewContainer.Frame;
                frame.X = (i * width) + margin;
                frame.Height = frame.Height;
                frame.Y = (_scrollView.Frame.Height - frame.Height) / 2;
                frame.Width = width - (margin * 1);
                viewContainer.Frame = frame;
                viewContainer.Layer.CornerRadius = ScaleUtility.GetScaledHeight(4f);
                AddCardShadow(ref viewContainer);
                _scrollView.AddSubview(viewContainer);
            }
            _scrollView.ContentSize = new CGSize(_scrollView.Frame.Width * dummyArrayCount, _scrollView.Frame.Height);
        }

        public UIView GetUI()
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

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
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
