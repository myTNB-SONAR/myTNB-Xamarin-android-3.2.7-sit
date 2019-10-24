using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class WalkthroughComponent : BaseComponent
    {
        private readonly UIView _parentView;
        private UIView _containerView;
        private UIView _footerButtonView, _toolTipFooterView;
        private UIScrollView _scrollView;
        private int _currentPageIndex;
        private UIPageControl _pageControl;
        private UILabel _skipLabel;
        public Func<string, string> GetI18NValue;
        public Action DismissAction;
        private nfloat ViewHeight;
        public List<OnboardingItemModel> OnboardingModel;

        public WalkthroughComponent(UIView parent, nfloat viewHeight)
        {
            _parentView = parent;
            ViewHeight = viewHeight;
            ViewHeight += DeviceHelper.GetStatusBarHeight();
        }

        public UIView GetWalkthroughView()
        {
            CreateWalkthrough();
            return _containerView;
        }

        private void CreateWalkthrough()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            UIView bottomView;
            if (OnboardingModel.Count == 1)
            {
                SetFooterButtonView();
                bottomView = _footerButtonView;
            }
            else
            {
                SetFooterView();
                bottomView = _toolTipFooterView;
            }
            SetScrollView(bottomView);
            SetSubViews();
        }

        private void SetFooterView()
        {
            nfloat width = _containerView.Frame.Width;
            _toolTipFooterView = new UIView(new CGRect(0, ViewHeight - GetScaledHeight(32F), width, GetScaledHeight(32F)))
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true,
                UserInteractionEnabled = true
            };
            AddSkipButton();
            AddPageControl();
            UpdatePageControl(_pageControl, _currentPageIndex, OnboardingModel.Count);
            _containerView.AddSubview(_toolTipFooterView);
        }

        private void SetFooterButtonView()
        {
            nfloat width = _containerView.Frame.Width;
            _footerButtonView = new UIView(new CGRect(0, ViewHeight - GetScaledHeight(64F), width, GetScaledHeight(64F)))
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true
            };
            CustomUIButtonV2 btnStart = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMarginWidth16, 0, width - GetScaledWidth(32F), GetScaledHeight(48F)),
                Enabled = true,
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = TNBFont.MuseoSans_16_500
            };
            btnStart.SetTitle(GetI18NValue(OnboardingConstants.I18N_LetsStart), UIControlState.Normal);
            btnStart.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissAction?.Invoke();
            }));
            _footerButtonView.AddSubview(btnStart);
            _containerView.AddSubview(_footerButtonView);
        }

        private void SetSubViews()
        {
            for (int i = 0; i < OnboardingModel.Count; i++)
            {
                UIView viewContainer = new UIView(_scrollView.Bounds);
                nfloat width = viewContainer.Frame.Width;
                viewContainer.BackgroundColor = UIColor.Clear;

                nfloat imageHeight = GetScaledHeight(306F);
                UIImageView imageView = new UIImageView(new CGRect(0, 0, width, imageHeight))
                {
                    Image = UIImage.FromBundle(OnboardingModel[i].Image)
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(0, GetYLocationFromFrame(imageView.Frame, 22F), width, GetScaledHeight(24F)))
                {
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.WaterBlue,
                    TextAlignment = UITextAlignment.Center,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = GetI18NValue(OnboardingModel[i].Title)
                };
                viewContainer.AddSubview(title);

                NSError htmlBodyError = null;
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(OnboardingModel[i].Description)
                    , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(12F));
                NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
                mutableHTMLBody.AddAttributes(new UIStringAttributes
                {
                    ForegroundColor = MyTNBColor.WarmGrey,
                    ParagraphStyle = new NSMutableParagraphStyle
                    {
                        LineSpacing = 3.0f
                    }
                }, new NSRange(0, htmlBody.Length));

                UITextView description = new UITextView(new CGRect(GetScaledWidth(24F)
                    , title.Frame.GetMaxY() + GetScaledHeight(12f), width - (GetScaledWidth(24F) * 2), 0))
                {
                    Editable = false,
                    ScrollEnabled = true,
                    AttributedText = mutableHTMLBody,
                    TextAlignment = UITextAlignment.Center,
                    UserInteractionEnabled = false,
                    ContentInset = new UIEdgeInsets(0, -5, 0, -5)
                };
                description.ScrollIndicatorInsets = UIEdgeInsets.Zero;
                CGSize size = description.SizeThatFits(new CGSize(description.Frame.Width, GetScaledHeight(86F)));
                description.Frame = new CGRect(description.Frame.Location, new CGSize(description.Frame.Width, size.Height));
                viewContainer.AddSubview(description);

                if (OnboardingModel.Count > 1 && i == OnboardingModel.Count - 1)
                {
                    CustomUIButtonV2 btnStart = new CustomUIButtonV2()
                    {
                        Frame = new CGRect(BaseMarginWidth16, viewContainer.Frame.Height - GetScaledHeight(48F) - GetScaledHeight(16F), width - GetScaledWidth(32F), GetScaledHeight(48F)),
                        Enabled = true,
                        BackgroundColor = MyTNBColor.FreshGreen,
                        Font = TNBFont.MuseoSans_16_500
                    };
                    btnStart.SetTitle(GetI18NValue(OnboardingConstants.I18N_LetsStart), UIControlState.Normal);
                    btnStart.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        DismissAction?.Invoke();
                    }));
                    viewContainer.AddSubview(btnStart);
                }

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);

                _scrollView.AddSubview(viewContainer);
            }
            _scrollView.ContentSize = new CGSize(_scrollView.Frame.Width * OnboardingModel.Count, _scrollView.Frame.Height);
        }

        private void SetScrollView(UIView bottomView)
        {
            nfloat width = _containerView.Frame.Width;
            nfloat height = ViewHeight - bottomView.Frame.Height;
            _scrollView = new UIScrollView(new CGRect(0, 0, width, height))
            {
                Delegate = new WalkthroughDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.Clear,
                Hidden = false,
                Bounces = false
            };
            _containerView.AddSubview(_scrollView);
        }

        private void AddSkipButton()
        {
            nfloat width = _toolTipFooterView.Frame.Width / 4;
            nfloat xPos = _toolTipFooterView.Frame.Width - GetScaledWidth(24F) - width;
            _skipLabel = new UILabel(new CGRect(xPos, 0, width, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.BrownGrey,
                TextAlignment = UITextAlignment.Right,
                UserInteractionEnabled = true,
                Text = GetI18NValue(OnboardingConstants.I18N_Skip)
            };
            _skipLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissAction?.Invoke();
            }));
            _toolTipFooterView.AddSubview(_skipLabel);
        }

        private void AddPageControl()
        {
            if (_pageControl != null)
            {
                _pageControl.RemoveFromSuperview();
            }
            _pageControl = new UIPageControl(new CGRect(0, 0, _toolTipFooterView.Frame.Width, Constants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false
            };
            _toolTipFooterView.AddSubview(_pageControl);
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages)
        {
            pageControl.CurrentPage = current;
            pageControl.Pages = pages;
            pageControl.UpdateCurrentPageDisplay();
        }

        private void ScrollViewHasPaginated()
        {
            if (OnboardingModel.Count > 1)
            {
                UpdatePageControl(_pageControl, _currentPageIndex, OnboardingModel.Count);
                _skipLabel.Hidden = !(_currentPageIndex < OnboardingModel.Count - 1);
            }
        }

        private class WalkthroughDelegate : UIScrollViewDelegate
        {
            WalkthroughComponent _controller;
            public WalkthroughDelegate(WalkthroughComponent controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._scrollView.ContentOffset.X / _controller._scrollView.Frame.Width);
                if (newPageIndex == _controller._currentPageIndex)
                    return;

                _controller._currentPageIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
    }
}
