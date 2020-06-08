using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
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
        private UISegmentedControl _toggleBar;
        private nfloat ViewHeight;

        public List<OnboardingItemModel> OnboardingModel;
        public Action DismissAction;
        public Action<int> ChangeLanguageAction { set; private get; }

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
                Font = TNBFont.MuseoSans_16_500,
                Tag = 2004
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
                UIView viewContainer = new UIView(_scrollView.Bounds)
                {
                    Tag = 1000 + i
                };
                viewContainer.UserInteractionEnabled = true;
                nfloat width = viewContainer.Frame.Width;
                viewContainer.BackgroundColor = UIColor.Clear;

                nfloat imageHeight = GetScaledHeight(306F);
                UIImageView imageView = new UIImageView(new CGRect(0, 0, width, imageHeight))
                {
                    Image = UIImage.FromBundle(OnboardingModel[i].Image)
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(GetScaledWidth(16)
                    , GetYLocationFromFrame(imageView.Frame, 22F), width - GetScaledWidth(32), GetScaledHeight(48F)))
                {
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.WaterBlue,
                    TextAlignment = UITextAlignment.Center,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    Text = GetI18NValue(OnboardingModel[i].Title),
                    Tag = 2002
                };
                nfloat newTitleHeight = title.GetLabelHeight(GetScaledHeight(48));
                title.Frame = new CGRect(title.Frame.Location, new CGSize(title.Frame.Width, newTitleHeight));
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
                    Tag = 2003,
                    TextContainerInset = UIEdgeInsets.Zero,
                    ContentInset = UIEdgeInsets.Zero
                };
                description.ScrollIndicatorInsets = UIEdgeInsets.Zero;
                CGSize size = description.SizeThatFits(new CGSize(description.Frame.Width, GetScaledHeight(86F)));
                description.Frame = new CGRect(description.Frame.Location, new CGSize(description.Frame.Width, size.Height));
                viewContainer.AddSubview(description);

                AddToggleBar(OnboardingModel[i].IsLanguageEntry, viewContainer, description.Frame);

                if (OnboardingModel.Count > 1 && i == OnboardingModel.Count - 1)
                {
                    CustomUIButtonV2 btnStart = new CustomUIButtonV2()
                    {
                        Frame = new CGRect(BaseMarginWidth16, viewContainer.Frame.Height - GetScaledHeight(48F) - GetScaledHeight(16F), width - GetScaledWidth(32F), GetScaledHeight(48F)),
                        Enabled = true,
                        BackgroundColor = MyTNBColor.FreshGreen,
                        Font = TNBFont.MuseoSans_16_500,
                        Tag = 2004
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
            CGSize size = _skipLabel.SizeThatFits(new CGSize(_skipLabel.Frame.Width, GetScaledHeight(16F)));
            ViewHelper.AdjustFrameSetHeight(_skipLabel, size.Height);
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
            _pageControl = new UIPageControl(new CGRect(0, GetScaledHeight(4F), _toolTipFooterView.Frame.Width, GetScaledHeight(8F)))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false,
                Transform = CGAffineTransform.MakeScale(1.25F, 1.25F)
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

        #region Language
        private void AddToggleBar(bool isLanguageEntry, UIView viewContainer, CGRect refFrame)
        {
            if (isLanguageEntry)
            {
                UIView toggleView = new UIView(new CGRect(0, GetYLocationFromFrame(refFrame, 18)
                    , viewContainer.Frame.Width, GetScaledHeight(26)))
                {
                    BackgroundColor = UIColor.Clear,
                    UserInteractionEnabled = true
                };
                nfloat toggleWidth = GetScaledWidth(122);
                nfloat toggleHeight = GetScaledHeight(26);

                _toggleBar = new UISegmentedControl(new CGRect(GetXLocationToCenterObject(toggleWidth, viewContainer)
                   , 0, toggleWidth, toggleHeight));
                _toggleBar.InsertSegment(LanguageUtility.GetLanguageCodeForDisplayByIndex(0), 0, false);
                _toggleBar.InsertSegment(LanguageUtility.GetLanguageCodeForDisplayByIndex(1), 1, false);
                _toggleBar.TintColor = MyTNBColor.WaterBlue;
                _toggleBar.SetTitleTextAttributes(new UITextAttributes
                {
                    Font = TNBFont.MuseoSans_12_300,
                    TextColor = MyTNBColor.WaterBlue
                }, UIControlState.Normal);

                _toggleBar.SetTitleTextAttributes(new UITextAttributes
                {
                    Font = TNBFont.MuseoSans_12_300,
                    TextColor = UIColor.White,
                }, UIControlState.Selected);

                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    _toggleBar.SelectedSegmentTintColor = MyTNBColor.WaterBlue;
                }

                _toggleBar.Layer.CornerRadius = toggleHeight / 2;
                _toggleBar.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
                _toggleBar.Layer.BorderWidth = GetScaledHeight(1);
                _toggleBar.Layer.MasksToBounds = true;
                _toggleBar.ValueChanged += (sender, e) =>
                {
                    Debug.WriteLine("selected index: " + ((UISegmentedControl)sender).SelectedSegment);
                    AlertHandler.DisplayCustomAlert(GetFormattedString(Constants.Common_ChangeLanguageTitle)
                     , GetFormattedString(Constants.Common_ChangeLanguageMessage)
                     , new Dictionary<string, Action> {
                            { GetFormattedString(Constants.Common_ChangeLanguageNo)
                            , ()=>{
                                _toggleBar.SelectedSegment = LanguageUtility.CurrentLanguageIndex;
                            } }
                            ,{ GetFormattedString(Constants.Common_ChangeLanguageYes)
                            , ()=>{
                                if (ChangeLanguageAction!=null)
                                {
                                    ChangeLanguageAction.Invoke((int)((UISegmentedControl)sender).SelectedSegment);
                                }
                            } } }
                     , UITextAlignment.Center
                     , UITextAlignment.Center);
                };
                _toggleBar.SelectedSegment = LanguageUtility.CurrentLanguageIndex;
                _toggleBar.Enabled = true;
                toggleView.AddSubview(_toggleBar);
                viewContainer.AddSubview(toggleView);
            }
        }

        private string GetFormattedString(string preffix)
        {
            return GetCommonI18NValue(string.Format("{0}_{1}", preffix, TNBGlobal.APP_LANGUAGE));
        }

        /*
         *  Tags
         *  1001 - parent view
         *  1002 - title label
         *  1003 - description text view
         *  1004 - UI button Get started
         */
        public void RefreshContent()
        {
            if (_scrollView == null) { return; }
            int dataCount = OnboardingModel.Count;
            for (int i = 0; i < _scrollView.Subviews.Length; i++)
            {
                int viewIndex = 1000 + i;
                UIView view = _scrollView.Subviews[i];
                if (view != null)
                {
                    UILabel lblTitle = view.ViewWithTag(2002) as UILabel;
                    if (lblTitle != null)
                    {
                        lblTitle.Text = GetI18NValue(OnboardingModel[i].Title);
                        nfloat newTitleHeight = lblTitle.GetLabelHeight(GetScaledHeight(48));
                        lblTitle.Frame = new CGRect(lblTitle.Frame.Location, new CGSize(lblTitle.Frame.Width, newTitleHeight));
                    }

                    UITextView txtViewDescription = view.ViewWithTag(2003) as UITextView;
                    if (txtViewDescription != null)
                    {
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
                        txtViewDescription.AttributedText = mutableHTMLBody;
                        txtViewDescription.TextAlignment = UITextAlignment.Center;
                        txtViewDescription.ScrollIndicatorInsets = UIEdgeInsets.Zero;

                        nfloat yLocation = txtViewDescription.Frame.Y;
                        if (lblTitle != null)
                        {
                            yLocation = lblTitle.Frame.GetMaxY() + GetScaledHeight(12);
                        }

                        CGSize size = txtViewDescription.SizeThatFits(new CGSize(txtViewDescription.Frame.Width, GetScaledHeight(100F)));
                        txtViewDescription.Frame = new CGRect(new CGPoint(txtViewDescription.Frame.X, yLocation)
                            , new CGSize(txtViewDescription.Frame.Width, size.Height));
                    }

                    CustomUIButtonV2 ctaButton = view.ViewWithTag(2004) as CustomUIButtonV2;
                    if (ctaButton != null)
                    {
                        ctaButton.SetTitle(GetI18NValue(OnboardingConstants.I18N_LetsStart), UIControlState.Normal);
                    }
                }
            }
            if (_skipLabel != null)
            {
                _skipLabel.Text = GetI18NValue(OnboardingConstants.I18N_Skip);
            }
            if (_footerButtonView != null)
            {
                CustomUIButtonV2 ctaButton = _footerButtonView.ViewWithTag(2004) as CustomUIButtonV2;
                if (ctaButton != null)
                {
                    ctaButton.SetTitle(GetI18NValue(OnboardingConstants.I18N_LetsStart), UIControlState.Normal);
                }
            }
        }
        #endregion
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

        public int SelectedToggle
        {
            set
            {
                if (_toggleBar != null && value > -1 && value < _toggleBar.NumberOfSegments)
                {
                    _toggleBar.SelectedSegment = value;
                }
            }
        }
    }
}