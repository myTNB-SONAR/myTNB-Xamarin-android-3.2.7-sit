using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class BillTutorialOverlay : BaseComponent
    {
        BillViewController _controller;
        UIView _parentView, _containerView, _footerView;
        UIPageControl _pageControl;
        int _currentPageIndex = 1;
        int _totalViews;
        UITextView _swipeText;
        public Action OnDismissAction;
        public nfloat NavigationHeight, HeaderViewHeight;

        public BillTutorialOverlay(UIView parent, BillViewController controller)
        {
            _parentView = parent;
            _controller = controller;
        }

        private void CreateView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };

            UISwipeGestureRecognizer rightSwipe = new UISwipeGestureRecognizer(OnSwipeRightAction);
            rightSwipe.Direction = UISwipeGestureRecognizerDirection.Right;
            _containerView.AddGestureRecognizer(rightSwipe);

            UISwipeGestureRecognizer leftSwipe = new UISwipeGestureRecognizer(OnSwipeLeftAction);
            leftSwipe.Direction = UISwipeGestureRecognizerDirection.Left;
            _containerView.AddGestureRecognizer(leftSwipe);

            UITapGestureRecognizer doubleTap = new UITapGestureRecognizer(OnDoubleTapAction);
            doubleTap.NumberOfTapsRequired = 2;
            _containerView.AddGestureRecognizer(doubleTap);

            UIView firstView = new UIView(_parentView.Bounds);
            firstView.AddSubview(GetViewForAccountSelection());
            firstView.Tag = 1;
            _totalViews++;

            _containerView.AddSubviews(new UIView { firstView });

            CreateFooterView();
        }

        public UIView GetView()
        {
            CreateView();
            return _containerView;
        }

        private void OnDoubleTapAction()
        {
            OnDismissAction?.Invoke();
        }

        private void OnSwipeRightAction()
        {

        }

        private void OnSwipeLeftAction()
        {

        }

        private UIView GetViewForAccountSelection()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, NavigationHeight))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, HeaderViewHeight), width, height - GetYLocationFromFrame(topView.Frame, HeaderViewHeight)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            parentView.AddSubviews(new UIView { topView, bottomView });
            return parentView;
        }

        public static void FadeAnimation(UIView view, bool isVisible, double duration = 0.3)
        {
            nfloat minAlpha = 0F;
            nfloat maxAlpha = 1F;
            view.Alpha = isVisible ? minAlpha : maxAlpha;
            view.Transform = CGAffineTransform.MakeIdentity();
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                view.Alpha = isVisible ? maxAlpha : minAlpha;
            }, null);
        }

        private void CreateFooterView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat yPos = _parentView.Frame.Height - GetScaledHeight(88F);
            _footerView = new UIView(new CGRect(0, yPos, width, GetScaledHeight(88F)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubview(_footerView);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("Swipe to see more,<br>double tap to dismiss."
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(12F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Center,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            nfloat textWidth = width - (GetScaledWidth(32F) * 2);
            _swipeText = new UITextView(new CGRect(GetScaledWidth(32F), 0, textWidth, GetScaledHeight(32F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = _swipeText.SizeThatFits(new CGSize(textWidth, GetScaledHeight(32F)));
            ViewHelper.AdjustFrameSetHeight(_swipeText, cGSize.Height);
            _footerView.AddSubview(_swipeText);

            AddPageControl(_swipeText.Frame.GetMaxY());
            UpdatePageControl(_pageControl, _currentPageIndex, _totalViews);
        }

        private void AddPageControl(nfloat yPos)
        {
            _pageControl = new UIPageControl(new CGRect(0, yPos, _footerView.Frame.Width, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = UIColor.White,
                PageIndicatorTintColor = UIColor.FromWhiteAlpha(1, 0.5F),
                CurrentPageIndicatorTintColor = UIColor.White,
                UserInteractionEnabled = false
            };
            _footerView.AddSubview(_pageControl);
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages)
        {
            if (pageControl != null)
            {
                pageControl.CurrentPage = current;
                pageControl.Pages = pages;
                pageControl.UpdateCurrentPageDisplay();
            }
        }
    }
}
