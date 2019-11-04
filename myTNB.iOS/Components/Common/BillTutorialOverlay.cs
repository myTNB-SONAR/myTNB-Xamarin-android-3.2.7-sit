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
        public nfloat NavigationHeight, DateAmountMaxY, HeaderViewHeight;
        public bool IsREAccount;

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

            nfloat totalPages = IsREAccount ? 2 : 4;

            for (int i = 1; i <= totalPages; i++)
            {
                UIView view = new UIView(_parentView.Bounds);
                if (i == 1)
                {
                    view.AddSubview(GetViewForAccountSelection());
                }
                else
                {
                    view.Alpha = 0F;
                }
                view.Tag = i;
                _totalViews++;
                _containerView.AddSubview(view);
            }
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
            if (_currentPageIndex >= _totalViews)
                return;

            UIView currentView = _containerView.ViewWithTag(_currentPageIndex);
            FadeAnimation(currentView, false, 0.3);

            UIView nextView = _containerView.ViewWithTag(_currentPageIndex + 1);
            foreach (UIView subView in nextView)
            {
                if (subView != null)
                {
                    subView.RemoveFromSuperview();
                }
            }

            if (IsREAccount)
            {
                if (_currentPageIndex + 1 == 2)
                {
                    UIView sView = GetHistoryView();
                    nextView.AddSubview(sView);
                }
            }
            else
            {
                switch (_currentPageIndex + 1)
                {
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:

                        break;
                }
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
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
            nfloat bottomViewYPos = topView.Frame.GetMaxY() + DateAmountMaxY;
            UIView bottomView = new UIView(new CGRect(0, topView.Frame.GetMaxY() + DateAmountMaxY, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), bottomLine.Frame.GetMaxY(), GetScaledWidth(1F), GetScaledHeight(32.5F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(verticalLine);

            UIView circle = new UIView(new CGRect(GetScaledWidth(32F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(40F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = "Your bill overview."
            };
            string desc = string.Empty;
            if (IsREAccount)
            {
                desc = "Tap “ ” to switch between different accounts. You’ll see how much you have earned or if you’ve been paid extra.";
            }
            else
            {
                desc = "Tap “ ” to switch between different accounts. You’ll see how much is due, if you’ve cleared your bill or if you’ve paid extra.";
            }
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(desc
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView description = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(100F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);

            bottomView.AddSubviews(new UIView { title, description });
            parentView.AddSubviews(new UIView { topView, bottomView });
            if (_swipeText != null)
            {
                _swipeText.Hidden = false;
            }
            parentView.AddSubviews(new UIView { topView, bottomView });
            return parentView;
        }

        private UIView GetHistoryView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, NavigationHeight + HeaderViewHeight))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            parentView.AddSubviews(new UIView { topView });
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
