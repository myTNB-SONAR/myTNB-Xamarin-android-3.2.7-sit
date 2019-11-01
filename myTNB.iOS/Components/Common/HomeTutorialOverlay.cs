using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class HomeTutorialOverlay : BaseComponent
    {
        UIView _parentView, _containerView, _footerView;
        UIPageControl _pageControl;
        int _currentPageIndex = 1;
        int _totalViews = 3;
        public Action OnDismissAction;

        public HomeTutorialOverlay(UIView parent)
        {
            _parentView = parent;
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

            UIView firstView = GetFirstView();
            firstView.Tag = 1;
            UIView secondView = GetSecondView();
            secondView.Tag = 2;
            secondView.Alpha = 0F;
            UIView thirdView = GetThirdView();
            thirdView.Tag = 3;
            thirdView.Alpha = 0F;
            _containerView.AddSubviews(new UIView { firstView, secondView, thirdView });

            CreateFooterView();
        }

        public UIView GetView()
        {
            CreateView();
            return _containerView;
        }

        private void OnDoubleTapAction()
        {
            Debug.WriteLine("OnDoubleTapAction");
            OnDismissAction?.Invoke();
        }

        private void OnSwipeRightAction()
        {
            Debug.WriteLine("OnSwipeRightAction");
            if (_currentPageIndex == 1)
                return;
            UIView currentView = _containerView.ViewWithTag(_currentPageIndex);
            FadeAnimation(currentView, false, 0.3);
            UIView nextView = _containerView.ViewWithTag(_currentPageIndex - 1);
            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex--;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
        }

        private void OnSwipeLeftAction()
        {
            Debug.WriteLine("OnSwipeLeftAction");
            if (_currentPageIndex >= _totalViews)
                return;
            UIView currentView = _containerView.ViewWithTag(_currentPageIndex);
            FadeAnimation(currentView, false, 0.3);
            UIView nextView = _containerView.ViewWithTag(_currentPageIndex + 1);
            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
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
            UIView topView = new UIView(new CGRect(0, 0, width, GetScaledHeight(98F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);

            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 118F), width, height - GetYLocationFromFrame(topView.Frame, 118F)))
            {
                BackgroundColor = MyTNBColor.Black75
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

            UIView circle = new UIView(new CGRect(GetScaledWidth(32F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);

            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(29F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, GetYLocationFromFrame(bottomLine.Frame, 23F), textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = "Your Accounts at a glance."
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("Add an electricity account to myTNB and you’ll have access to your usage and all services offered."
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
                UserInteractionEnabled = false
            };
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);

            bottomView.AddSubviews(new UIView { title, desc });
            parentView.AddSubviews(new UIView { topView, bottomView });

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
            UIView topView = new UIView(new CGRect(0, 0, width, GetScaledHeight(246F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), topView.Frame.GetMaxY() - GetScaledHeight(1F) - GetScaledHeight(121.7F), GetScaledWidth(1F), GetScaledHeight(121.7F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(GetScaledWidth(32F), verticalLine.Frame.GetMinY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubview(circle);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textPadding = GetScaledWidth(38F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = "Quick actions."
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("Get all of the services myTNB has to offer. New features are highlighted so you don’t miss out on anything!"
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
                UserInteractionEnabled = false
            };
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(90F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);
            topView.AddSubviews(new UIView { title, desc });
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 83F), width, height - GetYLocationFromFrame(topView.Frame, 83F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };

            UIView boxView = new UIView(new CGRect(BaseMarginWidth16, topView.Frame.GetMaxY() - GetScaledHeight(1F), width - (BaseMarginWidth16 * 2), GetScaledHeight(85F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);

            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(17F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };

            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(17F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };

            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });
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
            UIView topView = new UIView(new CGRect(0, 0, width, GetScaledHeight(343F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), topView.Frame.GetMaxY() - GetScaledHeight(173.5F), GetScaledWidth(1F), GetScaledHeight(173.5F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(GetScaledWidth(32F), verticalLine.Frame.GetMinY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubview(circle);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textPadding = GetScaledWidth(42F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = "Need help?"
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("We’ve highlighted some of the most commonly asked questions for you to browse through."
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
                UserInteractionEnabled = false
            };
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(70F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);
            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(textXPos, GetYLocationFromFrame(desc.Frame, 16F), GetScaledWidth(142F), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = UIColor.White
            };
            btnGotIt.SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            btnGotIt.SetTitle("Got it!", UIControlState.Normal);
            btnGotIt.Layer.CornerRadius = GetScaledHeight(4F);
            btnGotIt.Layer.BorderColor = UIColor.White.CGColor;
            btnGotIt.TouchUpInside += (sender, e) =>
            {
                OnDismissAction?.Invoke();
            };
            topView.AddSubviews(new UIView { title, desc, btnGotIt });
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 95F), width, height - GetYLocationFromFrame(topView.Frame, 95F)))
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

        private void CreateFooterView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat yPos = _parentView.Frame.Height - GetScaledHeight(88F);
            _footerView = new UIView(new CGRect(0, yPos, width, GetScaledHeight(88F)))
            {
                BackgroundColor = MyTNBColor.Black75
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
            UITextView swipeText = new UITextView(new CGRect(GetScaledWidth(32F), 0, textWidth, GetScaledHeight(32F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = swipeText.SizeThatFits(new CGSize(textWidth, GetScaledHeight(32F)));
            ViewHelper.AdjustFrameSetHeight(swipeText, cGSize.Height);
            _footerView.AddSubview(swipeText);

            AddPageControl(swipeText.Frame.GetMaxY());
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
