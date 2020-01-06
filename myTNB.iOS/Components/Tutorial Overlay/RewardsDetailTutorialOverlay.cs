using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class RewardsDetailTutorialOverlay : BaseComponent
    {
        RewardDetailsViewController _controller;
        UIView _parentView, _containerView, _headerView;
        UIPageControl _pageControl;
        int _currentPageIndex = 1;
        int _totalViews;
        UITextView _swipeText;
        public Action OnDismissAction;

        public RewardsDetailTutorialOverlay(UIView parent, RewardDetailsViewController controller)
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

            UISwipeGestureRecognizer rightSwipe = new UISwipeGestureRecognizer(OnSwipeRightAction)
            {
                Direction = UISwipeGestureRecognizerDirection.Right
            };
            _containerView.AddGestureRecognizer(rightSwipe);

            UISwipeGestureRecognizer leftSwipe = new UISwipeGestureRecognizer(OnSwipeLeftAction)
            {
                Direction = UISwipeGestureRecognizerDirection.Left
            };
            _containerView.AddGestureRecognizer(leftSwipe);

            UITapGestureRecognizer doubleTap = new UITapGestureRecognizer(OnDoubleTapAction)
            {
                NumberOfTapsRequired = 2
            };
            _containerView.AddGestureRecognizer(doubleTap);

            _totalViews = 2;

            for (int i = 1; i <= _totalViews; i++)
            {
                UIView view = new UIView(_parentView.Bounds);
                if (i == 1)
                {
                    view.AddSubview(GetFirstView());
                }
                else
                {
                    view.Alpha = 0F;
                }
                view.Tag = i;
                _containerView.AddSubview(view);
            }

            CreateHeaderView();
        }

        public UIView GetView()
        {
            CreateView();
            return _containerView;
        }

        private void OnSwipeRightAction()
        {
            if (_currentPageIndex == 1)
                return;

            UIView currentView = _containerView.ViewWithTag(_currentPageIndex);
            FadeAnimation(currentView, false, 0.3);

            UIView nextView = _containerView.ViewWithTag(_currentPageIndex - 1);
            foreach (UIView subView in nextView)
            {
                if (subView != null)
                {
                    subView.RemoveFromSuperview();
                }
            }

            switch (_currentPageIndex - 1)
            {
                case 2:
                    nextView.AddSubview(GetSecondView());
                    break;
                case 1:
                    UIView fView = GetFirstView();
                    nextView.AddSubview(fView);
                    break;
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex--;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
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

            switch (_currentPageIndex + 1)
            {
                case 2:
                    nextView.AddSubview(GetSecondView());
                    break;
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
        }

        private UIView GetFirstView()
        {
            nfloat padding = GetScaledHeight(4F);
            nfloat buttonHeight = GetScaledHeight(48F);
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;

            UIView topView = new UIView(new CGRect(0, 0, width, _controller.GetFooterButtonXPos() - padding))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(49.5F), topView.Frame.GetMaxY() - GetScaledHeight(85F), GetScaledWidth(1F), GetScaledHeight(85F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() - GetScaledWidth(4F) + GetScaledWidth(.5F), verticalLine.Frame.GetMinY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubview(circle);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(38F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(RewardsConstants.I18N_TutorialSaveTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(RewardsConstants.I18N_TutorialSaveDesc)
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

            UITextView description = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(40F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            description.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(60F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);

            topView.AddSubviews(new UIView { title, description });

            nfloat bottomViewYPos = topView.Frame.GetMaxY() + buttonHeight + (padding * 2);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            nfloat boxViewWidth = width / 2 - GetScaledWidth(10F);
            UIView boxView = new UIView(new CGRect(BaseMarginWidth16 - GetScaledWidth(4F), topView.Frame.GetMaxY() - GetScaledHeight(1F), boxViewWidth, buttonHeight + (padding * 2) + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(17F) - GetScaledWidth(4F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), width / 2 - GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });
            if (_swipeText != null)
            {
                _swipeText.Hidden = false;
            }

            return parentView;
        }

        private UIView GetSecondView()
        {
            nfloat padding = GetScaledHeight(4F);
            nfloat buttonHeight = GetScaledHeight(48F);
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;

            UIView topView = new UIView(new CGRect(0, 0, width, _controller.GetFooterButtonXPos() - padding))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView verticalLine = new UIView(new CGRect(width - GetScaledWidth(49.5F), topView.Frame.GetMaxY() - GetScaledHeight(169F), GetScaledWidth(1F), GetScaledHeight(169F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() - GetScaledWidth(4F) + GetScaledWidth(.5F), verticalLine.Frame.GetMinY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubview(circle);
            nfloat textXPos = GetScaledWidth(20F);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textPadding = width - circle.Frame.GetMinX() + GetScaledWidth(12F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Right,
                Text = GetI18NValue(RewardsConstants.I18N_TutorialUseNowTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(RewardsConstants.I18N_TutorialUseNowDesc)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Right,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView description = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(40F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            description.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(60F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);

            topView.AddSubviews(new UIView { title, description });

            nfloat btnGotItXPos = width - GetScaledWidth(142F) - (width - description.Frame.GetMaxX());
            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(btnGotItXPos, GetYLocationFromFrame(description.Frame, 16F), GetScaledWidth(142F), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = UIColor.White,
                UserInteractionEnabled = true
            };
            btnGotIt.SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            btnGotIt.SetTitle(GetCommonI18NValue(Constants.Common_GotIt), UIControlState.Normal);
            btnGotIt.Layer.CornerRadius = GetScaledHeight(4F);
            btnGotIt.Layer.BorderColor = UIColor.White.CGColor;
            btnGotIt.TouchUpInside += (sender, e) =>
            {
                OnDismissAction?.Invoke();
            };
            topView.AddSubview(btnGotIt);

            nfloat bottomViewYPos = topView.Frame.GetMaxY() + buttonHeight + (padding * 2);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            nfloat boxViewXPos = width / 2 - GetScaledWidth(2F);
            nfloat boxViewWidth = width / 2 - GetScaledWidth(10F);
            UIView boxView = new UIView(new CGRect(boxViewXPos, topView.Frame.GetMaxY() - GetScaledHeight(1F), boxViewWidth, buttonHeight + (padding * 2) + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), width / 2 - GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), width / 2 - GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            parentView.AddSubview(topView);
            parentView.AddSubviews(new UIView { bottomView, leftView, rightView, boxView });
            if (_swipeText != null)
            {
                _swipeText.Hidden = true;
            }

            return parentView;
        }

        private void OnDoubleTapAction()
        {
            OnDismissAction?.Invoke();
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

        private void CreateHeaderView()
        {
            nfloat width = _parentView.Frame.Width;
            _headerView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(12F), width, GetScaledHeight(88F)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubview(_headerView);

            AddPageControl(0);
            UpdatePageControl(_pageControl, _currentPageIndex, _totalViews);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetCommonI18NValue(Constants.Common_SwipeText)
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
            _swipeText = new UITextView(new CGRect(GetScaledWidth(32F), GetScaledHeight(24F), textWidth, GetScaledHeight(32F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = _swipeText.SizeThatFits(new CGSize(textWidth, GetScaledHeight(32F)));
            ViewHelper.AdjustFrameSetHeight(_swipeText, cGSize.Height);
            _headerView.AddSubview(_swipeText);
        }

        private void AddPageControl(nfloat yPos)
        {
            _pageControl = new UIPageControl(new CGRect(0, yPos, _headerView.Frame.Width, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = UIColor.White,
                PageIndicatorTintColor = UIColor.FromWhiteAlpha(1, 0.5F),
                CurrentPageIndicatorTintColor = UIColor.White,
                UserInteractionEnabled = false,
                Transform = CGAffineTransform.MakeScale(1.25F, 1.25F)
            };
            _headerView.AddSubview(_pageControl);
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
