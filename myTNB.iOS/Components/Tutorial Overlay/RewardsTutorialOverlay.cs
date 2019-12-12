using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class RewardsTutorialOverlay : BaseComponent
    {
        RewardsViewController _controller;
        UIView _parentView, _containerView, _footerView;
        UIPageControl _pageControl;
        int _currentPageIndex = 1;
        int _totalViews;
        UITextView _swipeText;
        public Action OnDismissAction;

        public RewardsTutorialOverlay(UIView parent, RewardsViewController controller)
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

            _totalViews = 3;

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

            CreateFooterView();
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
                case 3:
                    nextView.AddSubview(GetThirdView());
                    break;
                case 2:
                    UIView nView = _controller.CategoryMenuIsVisible() ? GetSecondView() : GetThirdView();
                    nextView.AddSubview(nView);
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
                    UIView nView = _controller.CategoryMenuIsVisible() ? GetSecondView() : GetThirdView();
                    nextView.AddSubview(nView);
                    break;
                case 3:
                    nextView.AddSubview(GetThirdView());
                    break;
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
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

        private UIView GetFirstView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;

            UIView topView = new UIView(new CGRect(0, 0, width, _controller.GetFirstRewardYPos()))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 159F), width, height - GetYLocationFromFrame(topView.Frame, 159F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView boxView = new UIView(new CGRect(BaseMarginWidth16, topView.Frame.GetMaxY() - GetScaledHeight(1F), width - (BaseMarginWidth16 * 2), GetScaledHeight(161F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(17F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(17F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(27.5F), 0, GetScaledWidth(1F), GetScaledHeight(30.5F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() + GetScaledWidth(0.5F) - GetScaledWidth(4F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(29F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(RewardsConstants.I18N_TutorialRewardTitle)
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(RewardsConstants.I18N_TutorialRewardDesc)
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

            UITextView desc = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextAlignment = UITextAlignment.Left
            };
            desc.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetWidth(desc, cGSize.Width);
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);

            bottomView.AddSubviews(new UIView { title, desc });
            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });

            return parentView;
        }

        private UIView GetSecondView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, _controller.GetNavigationMaxYPos()))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 47F), width, height - GetYLocationFromFrame(topView.Frame, 47F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), bottomLine.Frame.GetMaxY(), GetScaledWidth(1F), GetScaledHeight(30.5F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(verticalLine);

            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() + GetScaledWidth(0.5F) - GetScaledWidth(4F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(29F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(RewardsConstants.I18N_TutorialCategoryTitle)
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(RewardsConstants.I18N_TutorialCategoryDesc)
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

            UITextView desc = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextAlignment = UITextAlignment.Left
            };
            desc.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetWidth(desc, cGSize.Width);
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);

            bottomView.AddSubviews(new UIView { title, desc });
            parentView.AddSubviews(new UIView { topView, bottomView });
            return parentView;
        }

        private UIView GetThirdView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;

            UIView topView = new UIView(new CGRect(0, 0, width, DeviceHelper.GetStatusBarHeight()))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView bottomView = new UIView(new CGRect(0, _controller.GetNavigationMaxYPos(), width, height - _controller.GetNavigationMaxYPos()))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            nfloat boxSize = _controller.GetNavigationMaxYPos() - DeviceHelper.GetStatusBarHeight();
            //nfloat iconCenterPos = _controller.GetSavedRewardFrame().GetMaxY() - (_controller.GetSavedRewardFrame().Width / 2);
            //nfloat iconCenterPos = width - boxSize - GetScaledWidth(5F);
            nfloat boxXPos = width - boxSize - GetScaledWidth(5F);
            UIView boxView = new UIView(new CGRect(boxXPos, topView.Frame.GetMaxY() - GetScaledHeight(1F), boxSize, boxSize + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);

            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), boxView.Frame.GetMinX() + GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(9F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            UIView verticalLine = new UIView(new CGRect(boxView.Frame.GetMaxX() - (boxSize / 2) - GetScaledWidth(0.5F), 0, GetScaledWidth(1F), GetScaledHeight(31.5F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(verticalLine);

            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() + GetScaledWidth(0.5F) - GetScaledWidth(4F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);

            nfloat textXPos = GetScaledWidth(24F);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textPadding = width - circle.Frame.GetMinX() + GetScaledWidth(12F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Right,
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
                    Alignment = UITextAlignment.Right,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView desc = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            desc.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(270F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);
            bottomView.AddSubviews(new UIView { title, desc });

            nfloat btnGotItXPos = width - GetScaledWidth(142F) - (width - desc.Frame.GetMaxX());
            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(btnGotItXPos, GetYLocationFromFrame(desc.Frame, 16F), GetScaledWidth(142F), GetScaledHeight(48F)),
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
            bottomView.AddSubview(btnGotIt);

            parentView.AddSubview(topView);
            parentView.AddSubview(bottomView);
            parentView.AddSubviews(new UIView { leftView, rightView, boxView });

            return parentView;
        }
        private void CreateFooterView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat yPos = _parentView.Frame.Height - GetScaledHeight(88F) - GetBottomPadding;
            _footerView = new UIView(new CGRect(0, yPos, width, GetScaledHeight(88F)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubview(_footerView);

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
                UserInteractionEnabled = false,
                Transform = CGAffineTransform.MakeScale(1.25F, 1.25F)
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
