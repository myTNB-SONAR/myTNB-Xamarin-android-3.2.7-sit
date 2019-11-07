using System;
using CoreGraphics;
using Foundation;
using myTNB.Home.Bill;
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
        public Func<string, string> GetI18NValue;
        public Action OnDismissAction, ScrollTableToTheTop, ScrollToHistorySection;
        public nfloat NavigationHeight, HeaderViewHeight, TabBarHeight, ViewCTAMinY;
        public UIView ViewCTA;
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
            if (_currentPageIndex == 1)
                return;
            UIView currentView = _containerView.ViewWithTag(_currentPageIndex);
            FadeAnimation(currentView, false, 0.3);

            TableAutoScroll(true);

            UIView nextView = _containerView.ViewWithTag(_currentPageIndex - 1);
            foreach (UIView subView in nextView)
            {
                if (subView != null)
                {
                    subView.RemoveFromSuperview();
                }
            }

            if (IsREAccount)
            {
                if (_currentPageIndex - 1 == 1)
                {
                    UIView sView = GetViewForAccountSelection();
                    nextView.AddSubview(sView);
                }
            }
            else
            {
                switch (_currentPageIndex - 1)
                {
                    case 1:
                        UIView fView = GetViewForAccountSelection();
                        nextView.AddSubview(fView);
                        break;
                    case 2:
                        UIView sView = GetPayView();
                        nextView.AddSubview(sView);
                        break;
                    case 3:
                        UIView tView = GetViewDetailsView();
                        nextView.AddSubview(tView);
                        break;
                }
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

            TableAutoScroll(false);

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
                    UIView sView = GetHistoryView(_controller._previousScrollOffset);
                    nextView.AddSubview(sView);
                }
            }
            else
            {
                switch (_currentPageIndex + 1)
                {
                    case 2:
                        UIView sView = GetPayView();
                        nextView.AddSubview(sView);
                        break;
                    case 3:
                        UIView tView = GetViewDetailsView();
                        nextView.AddSubview(tView);
                        break;
                    case 4:
                        UIView fView = GetHistoryView(_controller._previousScrollOffset);
                        nextView.AddSubview(fView);
                        break;
                }
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
        }

        private void TableAutoScroll(bool isSwipeRight)
        {
            if (isSwipeRight)
            {
                if (IsREAccount)
                {
                    if (_currentPageIndex - 1 == 1)
                    {
                        ScrollTableToTheTop?.Invoke();
                    }
                }
                else
                {
                    if (_currentPageIndex - 1 == 1 || _currentPageIndex - 1 == 2 || _currentPageIndex - 1 == 3)
                    {
                        ScrollTableToTheTop?.Invoke();
                    }
                }
            }
            else
            {
                if (IsREAccount)
                {
                    if (_currentPageIndex + 1 == 2)
                    {
                        ScrollToHistorySection?.Invoke();
                    }
                }
                else
                {
                    if (_currentPageIndex + 1 == 2 || _currentPageIndex + 1 == 3)
                    {
                        ScrollTableToTheTop?.Invoke();
                    }
                    else if (_currentPageIndex + 1 == 4)
                    {
                        ScrollToHistorySection?.Invoke();
                    }
                }
            }
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
            nfloat bottomViewYPos = topView.Frame.GetMaxY() + _controller.GetDateAmountMaxY;
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
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
                Text = GetI18NValue(BillConstants.I18N_TutorialBillTitle)
            };
            string desc;
            if (IsREAccount)
            {
                desc = GetI18NValue(BillConstants.I18N_TutorialBillREAcctDesc);
            }
            else
            {
                desc = GetI18NValue(BillConstants.I18N_TutorialBillNormalAcctDesc);
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

        private UIView GetPayView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, NavigationHeight + ViewCTAMinY - GetScaledHeight(4)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView verticalLine = new UIView(new CGRect(width - GetScaledWidth(50.5F), topView.Frame.GetMaxY() - GetScaledHeight(69F), GetScaledWidth(1F), GetScaledHeight(69F)))
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
                Text = GetI18NValue(BillConstants.I18N_TutorialPayTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(BillConstants.I18N_TutorialPayDesc)
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

            UITextView desc = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(20F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(40F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);
            topView.AddSubviews(new UIView { title, desc });
            nfloat boxViewXPos = width / 2 - GetScaledWidth(2F);
            nfloat boxViewWidth = width / 2 - GetScaledWidth(10F);
            UIView boxView = new UIView(new CGRect(boxViewXPos, topView.Frame.GetMaxY() - GetScaledHeight(1F), boxViewWidth, ViewCTA.Frame.Height + GetScaledHeight(8F) + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            nfloat bottomViewYPos = boxView.Frame.GetMaxY() - GetScaledHeight(1F);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), width / 2 - GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(13F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });
            _swipeText.Hidden = false;
            return parentView;
        }

        private UIView GetViewDetailsView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, NavigationHeight + ViewCTAMinY - GetScaledHeight(4)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(34.5F), topView.Frame.GetMaxY() - GetScaledHeight(88F), GetScaledWidth(1F), GetScaledHeight(88F)))
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
            nfloat textPadding = GetScaledWidth(46F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(BillConstants.I18N_TutorialViewDetailsTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(BillConstants.I18N_TutorialViewDetailsDesc)
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
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(60F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);
            topView.AddSubviews(new UIView { title, description });
            nfloat boxViewXPos = GetScaledWidth(12F);
            nfloat boxViewWidth = width / 2 - GetScaledWidth(10F);
            UIView boxView = new UIView(new CGRect(boxViewXPos, topView.Frame.GetMaxY() - GetScaledHeight(1F), boxViewWidth, ViewCTA.Frame.Height + GetScaledHeight(8F) + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            nfloat bottomViewYPos = boxView.Frame.GetMaxY() - GetScaledHeight(1F);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(12F) + GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), width / 2 - GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });
            _swipeText.Hidden = false;
            return parentView;
        }

        private UIView GetHistoryView(nfloat tableViewContentOffsetY)
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            CGRect historyRect = _controller._historyTableView.RectForRowAtIndexPath(NSIndexPath.Create(0, 0));
            UIView topView = new UIView(new CGRect(0, 0, width, historyRect.Y + NavigationHeight - tableViewContentOffsetY))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), topView.Frame.GetMaxY() - GetScaledHeight(195F), GetScaledWidth(1F), GetScaledHeight(195F)))
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
            nfloat textPadding = GetScaledWidth(20F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(BillConstants.I18N_TutorialHistoryTitle)
            };
            string desc;
            if (IsREAccount)
            {
                desc = GetI18NValue(BillConstants.I18N_TutorialHistoryREAcctDesc);
            }
            else
            {
                desc = GetI18NValue(BillConstants.I18N_TutorialHistoryNormalAcctDesc);
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

            UITextView description = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(70F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);
            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(textXPos, GetYLocationFromFrame(description.Frame, 16F), GetScaledWidth(142F), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = UIColor.White,
                UserInteractionEnabled = true
            };
            btnGotIt.SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            btnGotIt.SetTitle(GetI18NValue(BillConstants.I18N_GotIt), UIControlState.Normal);
            btnGotIt.Layer.CornerRadius = GetScaledHeight(4F);
            btnGotIt.Layer.BorderColor = UIColor.White.CGColor;
            btnGotIt.TouchUpInside += (sender, e) =>
            {
                OnDismissAction?.Invoke();
            };
            topView.AddSubviews(new UIView { title, description, btnGotIt });
            topView.AddSubview(btnGotIt);
            UIView bottomView = new UIView(new CGRect(0, height - TabBarHeight, width, TabBarHeight))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            parentView.AddSubview(topView);
            parentView.AddSubview(bottomView);
            _swipeText.Hidden = true;
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
            nfloat yPos = _parentView.Frame.Height - GetScaledHeight(80F) - (DeviceHelper.IsIphoneXUpResolution() ? 20F : 0F);
            _footerView = new UIView(new CGRect(0, yPos, width, GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubview(_footerView);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(LanguageUtility.GetCommonI18NValue(Constants.Common_SwipeText)
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
