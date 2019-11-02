using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class HomeTutorialOverlay : BaseComponent
    {
        DashboardHomeViewController _controller;
        UIView _parentView, _containerView, _footerView;
        UIPageControl _pageControl;
        int _currentPageIndex = 1;
        int _totalViews;
        UITextView _swipeText;
        public Action OnDismissAction;
        public Action ScrollTableToTheTop;
        public Action ScrollTableToTheBottom;
        public HomeTutorialEnum TutorialType;

        public enum HomeTutorialEnum
        {
            None = 0,
            NOACCOUNT,
            MORETHANTHREEACCOUNTS,
            LESSTHANFOURACCOUNTS,
        }

        public HomeTutorialOverlay(UIView parent, DashboardHomeViewController controller)
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
            firstView.AddSubview(GetFirstView());
            firstView.Tag = 1;
            _totalViews++;

            UIView secondView = new UIView(_parentView.Bounds);
            secondView.Tag = 2;
            secondView.Alpha = 0F;
            _totalViews++;

            UIView thirdView = new UIView(_parentView.Bounds);
            thirdView.Tag = 3;
            thirdView.Alpha = 0F;
            _totalViews++;
            _containerView.AddSubviews(new UIView { firstView, secondView, thirdView });

            if (TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS)
            {
                UIView fourthView = new UIView(_parentView.Bounds);
                fourthView.Tag = 4;
                fourthView.Alpha = 0F;
                _totalViews++;
                _containerView.AddSubview(fourthView);
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

            switch (_currentPageIndex - 1)
            {
                case 3:
                    UIView tView = TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS ? GetSecondView(_controller._previousScrollOffset) : GetThirdView(_controller._previousScrollOffset);
                    nextView.AddSubview(tView);
                    break;
                case 2:
                    UIView sView = TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS ? GetSecondViewForMorethan3Accounts() : GetSecondView(_controller._previousScrollOffset);
                    nextView.AddSubview(sView);
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

            TableAutoScroll(false);

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
                    UIView sView = TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS ? GetSecondViewForMorethan3Accounts() : GetSecondView(_controller._previousScrollOffset);
                    nextView.AddSubview(sView);
                    break;
                case 3:
                    UIView tView = TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS ? GetSecondView(_controller._previousScrollOffset) : GetThirdView(_controller._previousScrollOffset);
                    nextView.AddSubview(tView);
                    break;
                case 4:
                    UIView fView = GetThirdView(_controller._previousScrollOffset);
                    nextView.AddSubview(fView);
                    break;
            }

            FadeAnimation(nextView, true, 0.3);
            _currentPageIndex++;
            UpdatePageControl(_pageControl, _currentPageIndex - 1, _totalViews);
        }

        private void TableAutoScroll(bool isSwipeRight)
        {
            if (isSwipeRight)
            {
                if (_currentPageIndex - 1 == 1)
                {
                    ScrollTableToTheTop?.Invoke();
                }

                if (TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS)
                {
                    if (_currentPageIndex - 1 == 2)
                    {
                        ScrollTableToTheTop?.Invoke();
                    }
                    else if (_currentPageIndex - 1 == 3 || _currentPageIndex - 1 == 4)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                }
                else
                {
                    if (_currentPageIndex - 1 == 2)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                    if (_currentPageIndex - 1 == 3)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                }
            }
            else
            {
                if (_currentPageIndex + 1 == 1)
                {
                    ScrollTableToTheTop?.Invoke();
                }

                if (TutorialType == HomeTutorialEnum.MORETHANTHREEACCOUNTS)
                {
                    if (_currentPageIndex + 1 == 2)
                    {
                        ScrollTableToTheTop?.Invoke();
                    }
                    else if (_currentPageIndex + 1 == 3 || _currentPageIndex + 1 == 4)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                }
                else
                {
                    if (_currentPageIndex + 1 == 2)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                    if (_currentPageIndex + 1 == 3)
                    {
                        ScrollTableToTheBottom?.Invoke();
                    }
                }
            }
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
            nfloat searchViewYPos = DeviceHelper.GetStatusBarHeight() + _controller._homeTableView.TableHeaderView.Frame.Height + GetScaledHeight(12F);
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, searchViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);

            nfloat bottomViewYPos = 0;
            string descText = string.Empty;
            switch (TutorialType)
            {
                case HomeTutorialEnum.NOACCOUNT:
                    bottomViewYPos = 118F;
                    descText = "Add an electricity account to myTNB and you’ll have access to your usage and all services offered.";
                    break;
                case HomeTutorialEnum.LESSTHANFOURACCOUNTS:
                    var activeAcctList = DataManager.DataManager.SharedInstance.ActiveAccountList;
                    bottomViewYPos = 61F * activeAcctList.Count + 49F;
                    descText = activeAcctList.Count > 1 ? "View a summary of all your linked electricity accounts here. Tap “Add” to link an account to myTNB." : "View a summary of all your linked electricity accounts here.";
                    break;
                case HomeTutorialEnum.MORETHANTHREEACCOUNTS:
                    bottomViewYPos = 61F * 3 + 93F;
                    descText = "View a summary of all your linked electricity accounts here.";
                    break;
            }
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, bottomViewYPos), width, height - GetYLocationFromFrame(topView.Frame, bottomViewYPos)))
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
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(descText
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
            if (_swipeText != null)
            {
                _swipeText.Hidden = false;
            }
            return parentView;
        }

        private UIView GetSecondViewForMorethan3Accounts()
        {
            nfloat searchViewYPos = DeviceHelper.GetStatusBarHeight() + _controller._homeTableView.TableHeaderView.Frame.Height + GetScaledHeight(12F);
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, searchViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView boxView = new UIView(new CGRect(GetScaledWidth(169F), topView.Frame.GetMaxY() - GetScaledHeight(1F), width - (GetScaledWidth(169F) + GetScaledWidth(8F)), GetScaledHeight(45f) + GetScaledHeight(2F)))
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
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(275.5F), 0, GetScaledWidth(1F), GetScaledHeight(32.3F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() + GetScaledWidth(1F) - GetScaledWidth(4F), verticalLine.Frame.GetMaxY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            bottomView.AddSubview(circle);
            nfloat textXPos = GetScaledWidth(24F);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textPadding = GetScaledWidth(60F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Right,
                Text = "Quick account access."
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("Tap <b>“Add”</b> to link an account to  myTNB. Use <b>“Search”</b> to look for a specific one! Just type in the nickname or account number."
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
            CGSize cGSize = desc.SizeThatFits(new CGSize(textWidth, GetScaledHeight(270F)));
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);
            bottomView.AddSubviews(new UIView { title, desc });
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(169F) + GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(9F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            parentView.AddSubviews(new UIView { topView, bottomView, leftView, rightView, boxView });
            _swipeText.Hidden = false;
            return parentView;
        }

        private UIView GetSecondView(nfloat tableViewContentOffsetY)
        {
            CGRect servicesCellRect = _controller._homeTableView.RectForRowAtIndexPath(NSIndexPath.Create(0, DashboardHomeConstants.CellIndex_Services));
            nfloat quickActionsYPos = DeviceHelper.GetStatusBarHeight() + servicesCellRect.Y;

            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, quickActionsYPos - tableViewContentOffsetY))
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
            _swipeText.Hidden = false;
            return parentView;
        }

        private UIView GetThirdView(nfloat tableViewContentOffsetY)
        {
            CGRect helpCellRect = _controller._homeTableView.RectForRowAtIndexPath(NSIndexPath.Create(0, DashboardHomeConstants.CellIndex_Help));
            nfloat needHelpYPos = DeviceHelper.GetStatusBarHeight() + helpCellRect.Y - GetScaledHeight(5F);

            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, needHelpYPos - tableViewContentOffsetY))
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
            UIView bottomView = new UIView(new CGRect(0, GetYLocationFromFrame(topView.Frame, 100F), width, height - GetYLocationFromFrame(topView.Frame, 100F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            parentView.AddSubviews(new UIView { topView, bottomView });
            _swipeText.Hidden = bottomView.Frame.Y >= _footerView.Frame.Y;
            return parentView;
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
