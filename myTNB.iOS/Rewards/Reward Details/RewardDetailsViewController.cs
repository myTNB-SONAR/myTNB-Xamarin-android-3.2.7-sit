using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardDetailsViewController : CustomUIViewController
    {
        public RewardsModel RewardModel;
        public string RedeemedDate;
        public bool IsFromSavedRewards;
        private UIView _footerView;
        private UIScrollView _scrollView;
        private Timer _useTimer;
        private UIView _timerContainerView;
        private UILabel _timerLabel;
        private int _currentSeconds;
        private UIView _tutorialContainer;
        private UIBarButtonItem _btnShareReward;
        private DateTime _exitTime;
        private int _exitSeconds;
        private bool _hotspotIsOn, _countdownHasStarted;

        public override void ViewDidLoad()
        {
            PageName = IsFromSavedRewards ? RewardsConstants.PageName_SavedRewards : RewardsConstants.PageName_RewardDetails;
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            NotifCenterUtility.AddObserver(UIApplication.DidEnterBackgroundNotification, OnEnterBackground);
            NotifCenterUtility.AddObserver(UIApplication.WillChangeStatusBarFrameNotification, OnChangeStatusBarFrame);
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            SetNavigationBar();
            AddFooterView();
            SetScrollView();
            PrepareDetailView();

            if (RewardModel != null)
            {
                if (RewardModel.IsUsed)
                {
                    ViewHelper.AdjustFrameSetHeight(_scrollView, ViewHeight - GetScaledHeight(116F));
                    UpdateView();
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!RewardModel.IsRead)
            {
                RewardModel.IsRead = true;
                InvokeInBackground(async () =>
                {
                    await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Read, true);
                    RewardModel.IsRead = true;
                    RewardsServices.UpdateRewardItem(RewardModel);
                });
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (RewardModel != null && !RewardModel.IsUsed)
            {
                _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
                CheckTutorialOverlay();
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        private void OnEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is RewardDetailsViewController)
                {
                    OnChangeStatusBarFrame(null);
                    if (!_countdownHasStarted) { return; }
                    _countdownHasStarted = false;
                    var timeNow = DateTime.Now;
                    var diffInSeconds = (timeNow - _exitTime).TotalSeconds;
                    var remainingSeconds = _exitSeconds - diffInSeconds;
                    if (remainingSeconds > 0)
                    {
                        ResumeTimer((int)Math.Ceiling(remainingSeconds));
                    }
                    else
                    {
                        if (_useTimer != null)
                        {
                            _useTimer.Enabled = false;
                        }
                        ViewHelper.AdjustFrameSetHeight(_scrollView, ViewHeight - GetScaledHeight(116F));
                        if (_timerContainerView != null)
                        {
                            _timerContainerView.RemoveFromSuperview();
                            _timerContainerView = null;
                        }
                        UpdateView();
                        if (_btnShareReward != null)
                        {
                            _btnShareReward.Enabled = true;
                        }
                    }
                }
            }
        }

        private void OnEnterBackground(NSNotification notification)
        {
            Debug.WriteLine("OnEnterBackground");
            if (_useTimer != null)
            {
                _useTimer.Enabled = false;
            }
            _exitSeconds = _currentSeconds;
            _exitTime = DateTime.Now;
        }

        private void OnChangeStatusBarFrame(NSNotification notification)
        {
            if (DeviceHelper.IsIphoneXUpResolution())
                return;
            Debug.WriteLine("OnChangeStatusBarFrame...");
            SetFrames();
            _hotspotIsOn = DeviceHelper.GetStatusBarHeight() > 20;
            if (_tutorialContainer != null)
            {
                _tutorialContainer.RemoveFromSuperview();
            }
            CheckTutorialOverlay();
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(RewardsConstants.I18N_Title);
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            _btnShareReward = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_ShareIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (NetworkUtility.isReachable)
                {
                    ActivityIndicator.Show();
                    BaseService baseService = new BaseService();
                    APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                    string linkUrl = baseService.GetDomain(env) + "/rewards/redirect.aspx/rid=" + RewardModel.ID;

                    var deeplinkUrl = string.Empty;
                    var components = RewardsServices.GenerateLongURL(linkUrl);
                    components.GetShortenUrl((shortUrl, warnings, error) =>
                    {
                        if (error == null)
                        {
                            deeplinkUrl = shortUrl.AbsoluteString;
                        }
                        else
                        {
                            deeplinkUrl = linkUrl;
                        }
                        ShareAction(deeplinkUrl);
                        ActivityIndicator.Hide();
                    });
                }
                else
                {
                    AlertHandler.DisplayNoDataAlert(this);
                }
            });
            NavigationItem.RightBarButtonItem = _btnShareReward;
        }

        private void ShareAction(string deeplinkUrl)
        {
            NSObject item = NSObject.FromObject(deeplinkUrl);
            NSObject[] activityItems = { item };
            UIActivity[] applicationActivities = null;
            UIActivityViewController activityController = new UIActivityViewController(activityItems, applicationActivities);
            UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
            activityController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(activityController, true, null);
        }

        private void SetScrollView()
        {
            if (_scrollView != null)
            {
                _scrollView.RemoveFromSuperview();
                _scrollView = null;
            }
            _scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = true
            };
            View.AddSubview(_scrollView);
        }

        private void PrepareDetailView()
        {
            nfloat imgHeight = GetScaledHeight(180F);

            UIView imageContainer = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
            {
                BackgroundColor = UIColor.Clear,
            };

            UIImageView imageView = new UIImageView(imageContainer.Bounds)
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Tag = RewardsConstants.Tag_DetailRewardImage
            };

            if (RewardModel.Image.IsValid())
            {
                try
                {
                    UIView imgLoadingView = new UIView(imageContainer.Bounds)
                    {
                        BackgroundColor = UIColor.Clear
                    };
                    _scrollView.AddSubview(imgLoadingView);

                    UIView viewImage = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                    {
                        BackgroundColor = MyTNBColor.PaleGreyThree
                    };

                    CustomShimmerView shimmeringView = new CustomShimmerView();
                    UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                    { BackgroundColor = UIColor.Clear };
                    UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                    { BackgroundColor = UIColor.Clear };
                    viewShimmerParent.AddSubview(shimmeringView);
                    shimmeringView.ContentView = viewShimmerContent;
                    shimmeringView.Shimmering = true;
                    shimmeringView.SetValues();

                    viewShimmerContent.AddSubview(viewImage);
                    imgLoadingView.AddSubview(viewShimmerParent);
                    NSUrl url = new NSUrl(RewardModel.Image);
                    NSUrlSession session = NSUrlSession
                        .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                    NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                    {
                        if (error == null && response != null && data != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                using (var image = UIImage.LoadFromData(data))
                                {
                                    imageView.Image = image;
                                }
                                if (RewardModel.IsUsed) { imageView.Image = RewardsServices.ConvertToGrayScale(imageView.Image); }
                                imgLoadingView.RemoveFromSuperview();
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                if (RewardModel.IsUsed) { imageView.Image = RewardsServices.ConvertToGrayScale(imageView.Image); }
                                imgLoadingView.RemoveFromSuperview();
                            });
                        }
                    });
                    dataTask.Resume();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Image load Error: " + e.Message);
                    InvokeOnMainThread(() =>
                    {
                        imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                        if (RewardModel.IsUsed) { imageView.Image = RewardsServices.ConvertToGrayScale(imageView.Image); }
                    });
                }
            }
            else
            {
                InvokeOnMainThread(() =>
                {
                    imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                    if (RewardModel.IsUsed) { imageView.Image = RewardsServices.ConvertToGrayScale(imageView.Image); }
                });
            }

            nfloat viewWidth = ViewWidth - (BaseMarginWidth16 * 2);

            UITextView titleTextView = CreateHTMLContent(RewardModel.Title, true);
            titleTextView.Tag = RewardsConstants.Tag_DetailRewardTitle;
            CGSize titleTextViewSize = titleTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(titleTextView, titleTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(titleTextView, GetScaledWidth(16F));
            ViewHelper.AdjustFrameSetY(titleTextView, GetYLocationFromFrame(imageView.Frame, 20F));
            ViewHelper.AdjustFrameSetWidth(titleTextView, viewWidth);

            nfloat iconWidth = GetScaledWidth(24F);
            nfloat iconHeight = GetScaledHeight(24F);

            UIView rewardPeriodView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(titleTextView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Tag = RewardsConstants.Tag_DetailRewardView
            };

            UIImageView rpIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardPeriodIcon),
                Tag = RewardsConstants.Tag_DetailRewardPeriodImage
            };

            UILabel rpTitle = new UILabel(new CGRect(GetXLocationFromFrame(rpIcon.Frame, 4F), 0, viewWidth - (rpIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_RewardPeriod),
                Tag = RewardsConstants.Tag_DetailRewardTitle
            };

            UITextView rpTextView = CreateHTMLContent(RewardModel.PeriodLabel);
            CGSize rpTextViewSize = rpTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(rpTextView, rpTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(rpTextView, 0);
            ViewHelper.AdjustFrameSetY(rpTextView, GetYLocationFromFrame(rpTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(rpTextView, viewWidth);

            UIView rpLine = new UIView(new CGRect(0, GetYLocationFromFrame(rpTextView.Frame, 16F), viewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkSeven
            };

            rewardPeriodView.AddSubviews(new UIView { rpIcon, rpTitle, rpTextView, rpLine });

            ViewHelper.AdjustFrameSetHeight(rewardPeriodView, rpLine.Frame.GetMaxY());

            UIView locationView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(rewardPeriodView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Tag = RewardsConstants.Tag_DetailRewardView
            };

            UIImageView locationIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardLocationIcon),
                Tag = RewardsConstants.Tag_DetailLocationImage
            };

            UILabel locationTitle = new UILabel(new CGRect(GetXLocationFromFrame(locationIcon.Frame, 4F), 0, viewWidth - (locationIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_Location),
                Tag = RewardsConstants.Tag_DetailRewardTitle
            };

            UITextView locationTextView = CreateHTMLContent(RewardModel.LocationLabel);
            CGSize locationTextViewSize = locationTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(locationTextView, locationTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(locationTextView, 0);
            ViewHelper.AdjustFrameSetY(locationTextView, GetYLocationFromFrame(locationTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(locationTextView, viewWidth);

            UIView locationLine = new UIView(new CGRect(0, GetYLocationFromFrame(locationTextView.Frame, 16F), viewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkSeven
            };

            locationView.AddSubviews(new UIView { locationIcon, locationTitle, locationLine });
            locationView.AddSubview(locationTextView);

            ViewHelper.AdjustFrameSetHeight(locationView, locationLine.Frame.GetMaxY());

            UIView tandCView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(locationView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Tag = RewardsConstants.Tag_DetailRewardView
            };

            UIImageView tandCIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardTCIcon),
                Tag = RewardsConstants.Tag_DetailTCImage
            };

            UILabel tandCTitle = new UILabel(new CGRect(GetXLocationFromFrame(tandCIcon.Frame, 4F), 0, viewWidth - (tandCIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_TNC),
                Tag = RewardsConstants.Tag_DetailRewardTitle
            };

            UITextView tandCTextView = CreateHTMLContent(RewardModel.TandCLabel);
            CGSize tandCTextViewSize = tandCTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(tandCTextView, tandCTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(tandCTextView, 0);
            ViewHelper.AdjustFrameSetY(tandCTextView, GetYLocationFromFrame(tandCTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(tandCTextView, viewWidth);

            tandCView.AddSubviews(new UIView { tandCIcon, tandCTitle });
            tandCView.AddSubview(tandCTextView);

            ViewHelper.AdjustFrameSetHeight(tandCView, tandCTextView.Frame.GetMaxY());
            _scrollView.AddSubview(imageContainer);
            _scrollView.AddSubview(imageView);
            _scrollView.AddSubview(titleTextView);
            _scrollView.AddSubview(rewardPeriodView);
            _scrollView.AddSubview(locationView);
            _scrollView.AddSubview(tandCView);
            UpdateScrollViewContentSize(tandCView);
        }

        private UITextView CreateHTMLContent(string textValue, bool isTitle = false)
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(textValue
                , ref htmlBodyError, isTitle ? TNBFont.FONTNAME_500 : TNBFont.FONTNAME_300
                , isTitle ? (float)GetScaledHeight(16F) : (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = isTitle ? MyTNBColor.WaterBlue : MyTNBColor.CharcoalGrey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_14_500,
                UnderlineColor = UIColor.Clear,
                UnderlineStyle = NSUnderlineStyle.None
            };

            UITextView textView = new UITextView()
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextContainerInset = UIEdgeInsets.Zero
            };

            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                if (url != null)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                    BrowserViewController viewController =
                        storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                    if (viewController != null)
                    {
                        viewController.NavigationTitle = GetI18NValue(RewardsConstants.I18N_Title);
                        viewController.URL = url.AbsoluteString;
                        viewController.IsDelegateNeeded = false;
                        UINavigationController navController = new UINavigationController(viewController)
                        {
                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        };
                        PresentViewController(navController, true, null);
                    }
                }
            });
            textView.Delegate = new TextViewDelegate(action)
            {
                InteractWithURL = false
            };

            return textView;
        }

        private void UpdateScrollViewContentSize(UIView lastView)
        {
            _scrollView.ContentSize = new CGSize(ViewWidth, lastView.Frame.GetMaxY());
        }

        private void AddFooterView(bool isUsedReward = false)
        {
            bool isRedeemDateEmpty = false;
            if (_footerView != null)
            {
                _footerView.RemoveFromSuperview();
                _footerView = null;
            }
            nfloat height = isUsedReward ? GetScaledHeight(116F) : GetScaledHeight(80F);
            nfloat width = ViewWidth;
            _footerView = new UIView(new CGRect(0, ViewHeight - height, width, height + GetBottomPadding))
            {
                BackgroundColor = UIColor.White
            };
            _footerView.Layer.MasksToBounds = false;
            _footerView.Layer.ShadowColor = MyTNBColor.SilverChalice10.CGColor;
            _footerView.Layer.ShadowOpacity = 1F;
            _footerView.Layer.ShadowOffset = new CGSize(0, -1);
            _footerView.Layer.ShadowRadius = 8;
            _footerView.Layer.ShadowPath = UIBezierPath.FromRect(_footerView.Bounds).CGPath;
            View.AddSubview(_footerView);

            if (isUsedReward)
            {
                string dateValue = RedeemedDate.IsValid() ? GetI18NValue(RewardsConstants.I18N_RewardUsedPrefix) + RedeemedDate : string.Empty;
                UILabel usedLabel = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), width - (BaseMarginWidth16 * 2), GetScaledHeight(20F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Font = TNBFont.MuseoSans_14_300I,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Text = dateValue
                };

                isRedeemDateEmpty = !dateValue.IsValid();
                nfloat rewardButtonYPos = dateValue.IsValid() ? GetYLocationFromFrame(usedLabel.Frame, 16F) : GetScaledHeight(16F);
                UIView rewardUsedBtn = new UIView(new CGRect(BaseMarginWidth16, rewardButtonYPos, width - (BaseMarginWidth16 * 2), GetScaledHeight(48F)))
                {
                    BackgroundColor = UIColor.White
                };
                rewardUsedBtn.Layer.CornerRadius = 4f;
                rewardUsedBtn.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
                rewardUsedBtn.Layer.BorderWidth = 1f;

                UILabel rewardUsedLbl = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationToCenterObject(GetScaledHeight(24F), rewardUsedBtn), rewardUsedBtn.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(24F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.SilverChalice,
                    TextAlignment = UITextAlignment.Center,
                    Text = GetI18NValue(RewardsConstants.I18N_RewardUsed)
                };

                rewardUsedBtn.AddSubview(rewardUsedLbl);
                _footerView.AddSubviews(new UIView { usedLabel, rewardUsedBtn });

                if (isRedeemDateEmpty)
                {
                    nfloat newHeight = GetScaledHeight(80F);
                    ViewHelper.AdjustFrameSetHeight(_footerView, newHeight + GetBottomPadding);
                    ViewHelper.AdjustFrameSetY(_footerView, ViewHeight - newHeight);
                    ViewHelper.AdjustFrameSetHeight(_scrollView, ViewHeight - newHeight);
                }
            }
            else
            {
                CustomUIView saveBtnContainer = new CustomUIView(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), (width / 2) - GetScaledWidth(18F), GetScaledHeight(48F)))
                {
                    BackgroundColor = UIColor.White
                };
                saveBtnContainer.Layer.CornerRadius = 4f;
                saveBtnContainer.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                saveBtnContainer.Layer.BorderWidth = 1f;

                saveBtnContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    RewardModel.IsSaved = !RewardModel.IsSaved;
                    InvokeInBackground(async () =>
                    {
                        await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Favourite, RewardModel.IsSaved);
                    });
                    InvokeOnMainThread(() =>
                    {
                        UpdateSaveButton(saveBtnContainer);
                    });
                }));

                _footerView.AddSubview(saveBtnContainer);

                UIView saveBtnView = new UIView(new CGRect(0, 0, 0, GetScaledHeight(24F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 3000
                };
                saveBtnContainer.AddSubview(saveBtnView);

                nfloat imgWidth = GetScaledWidth(18F);
                nfloat imgHeight = GetScaledHeight(15F);
                UIImageView imgView = new UIImageView(new CGRect(0, 0, imgWidth, imgHeight))
                {
                    Image = UIImage.FromBundle(RewardsConstants.Img_HeartUnsavedGreenIcon),
                    Tag = 3001
                };
                saveBtnView.AddSubview(imgView);

                nfloat saveLblWidth = saveBtnContainer.Frame.Width - imgView.Frame.Width - GetScaledWidth(2F);
                UILabel saveLbl = new UILabel(new CGRect(imgView.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(12F), saveLblWidth, GetScaledHeight(24F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.FreshGreen,
                    Lines = 0,
                    TextAlignment = UITextAlignment.Left,
                    Tag = 3002
                };
                saveBtnView.AddSubview(saveLbl);
                UpdateSaveButton(saveBtnContainer);
                UIButton btnUseNow = new UIButton(UIButtonType.Custom)
                {
                    Frame = new CGRect(saveBtnContainer.Frame.GetMaxX() + GetScaledWidth(4F), GetScaledHeight(16F), (width / 2) - GetScaledWidth(18F), GetScaledHeight(48F))
                };
                btnUseNow.Layer.CornerRadius = GetScaledHeight(4F);
                btnUseNow.Layer.BackgroundColor = MyTNBColor.FreshGreen.CGColor;
                btnUseNow.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                btnUseNow.Layer.BorderWidth = GetScaledHeight(1F);
                btnUseNow.SetTitle(GetI18NValue(RewardsConstants.I18N_UseNow), UIControlState.Normal);
                btnUseNow.Font = TNBFont.MuseoSans_16_500;
                btnUseNow.TouchUpInside += (sender, e) =>
                {
                    var title = GetI18NValue(RewardsConstants.I18N_UseNowPopupTitle);
                    if (RewardModel.RewardUseTitle.IsValid())
                    {
                        title = RewardModel.RewardUseTitle;
                    }
                    var desc = GetI18NValue(RewardsConstants.I18N_UseNowPopupMessage);
                    if (RewardModel.RewardUseDescription.IsValid())
                    {
                        desc = RewardModel.RewardUseDescription;
                    }
                    DisplayCustomAlert(title, desc, new Dictionary<string, Action> {
                    { GetI18NValue(RewardsConstants.I18N_UseLater), null }
                    , { GetI18NValue(RewardsConstants.I18N_Confirm), OnUseNowAction } }
                    , UIImage.FromBundle(RewardsConstants.Img_UseRewardBanner));
                };
                _footerView.AddSubview(btnUseNow);
            }
        }

        private void UpdateSaveButton(CustomUIView viewRef)
        {
            UIView saveBtnView = viewRef.ViewWithTag(3000) as UIView;
            UIImageView imgView = viewRef.ViewWithTag(3001) as UIImageView;
            UILabel saveLbl = viewRef.ViewWithTag(3002) as UILabel;

            imgView.Image = UIImage.FromBundle(RewardModel.IsSaved ? RewardsConstants.Img_HeartSavedGreenIcon : RewardsConstants.Img_HeartUnsavedGreenIcon);
            saveLbl.Text = GetI18NValue(RewardModel.IsSaved ? RewardsConstants.I18N_Unsave : RewardsConstants.I18N_Save);

            nfloat saveLblWidth = viewRef.Frame.Width - imgView.Frame.Width - GetScaledWidth(2F);
            CGSize cGSizeLbl = saveLbl.SizeThatFits(new CGSize(saveLblWidth, GetScaledHeight(24F)));

            ViewHelper.AdjustFrameSetHeight(saveLbl, cGSizeLbl.Height);
            ViewHelper.AdjustFrameSetWidth(saveLbl, cGSizeLbl.Width);
            ViewHelper.AdjustFrameSetY(saveLbl, GetYLocationToCenterObject(saveLbl.Frame.Height, saveBtnView));
            ViewHelper.AdjustFrameSetY(imgView, GetYLocationToCenterObject(imgView.Frame.Height, saveBtnView));
            ViewHelper.AdjustFrameSetWidth(saveBtnView, saveLbl.Frame.GetMaxX());
            ViewHelper.AdjustFrameSetY(saveBtnView, GetYLocationToCenterObject(saveBtnView.Frame.Height, viewRef));
            ViewHelper.AdjustFrameSetX(saveBtnView, GetXLocationToCenterObject(saveBtnView.Frame.Width, viewRef));
        }

        #region TUTORIAL OVERLAY
        public void CheckTutorialOverlay()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(RewardsConstants.Pref_RewardsDetailTutorialOverlay);

            if (tutorialOverlayHasShown) { return; }

            if (RewardModel != null)
            {
                InvokeOnMainThread(() =>
                {
                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is RewardDetailsViewController)
                        {
                            ShowTutorialOverlay();
                        }
                        else
                        {
                            if (_tutorialContainer != null)
                            {
                                _tutorialContainer.RemoveFromSuperview();
                            }
                        }
                    }
                });
            }
        }

        private void ShowTutorialOverlay()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            if (_tutorialContainer != null && _tutorialContainer.IsDescendantOfView(currentWindow)) { return; }

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;

            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 1001
            };

            RewardsDetailTutorialOverlay tutorialView = new RewardsDetailTutorialOverlay(_tutorialContainer, this, _hotspotIsOn)
            {
                OnDismissAction = HideTutorialOverlay,
                GetI18NValue = GetI18NValue
            };
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is RewardDetailsViewController && _tutorialContainer != null && !_tutorialContainer.IsDescendantOfView(currentWindow))
                {
                    foreach (UIView view in currentWindow.Subviews)
                    {
                        if (view.Tag == 1001)
                        {
                            view.RemoveFromSuperview();
                            break;
                        }
                    }

                    _tutorialContainer.AddSubview(tutorialView.GetView());
                    currentWindow.AddSubview(_tutorialContainer);
                }
                else
                {
                    if (_tutorialContainer != null)
                    {
                        _tutorialContainer.RemoveFromSuperview();
                    }
                }
            }
        }

        private void HideTutorialOverlay()
        {
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);

                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, RewardsConstants.Pref_RewardsDetailTutorialOverlay);
                sharedPreference.Synchronize();
            }
        }

        public nfloat GetFooterButtonXPos()
        {
            try
            {
                return _footerView.Frame.Y + NavigationController.NavigationBar.Frame.GetMaxY() + GetScaledHeight(16F);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in services: " + e.Message);
                return 0;
            }
        }
        #endregion

        #region Action Methods
        private void OnUseNowAction()
        {
            _countdownHasStarted = true;
            ActivityIndicator.Show();
            InvokeInBackground(async () =>
            {
                UpdateRewardsResponseModel response = await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Redeemed, true);
                InvokeOnMainThread(() =>
                {
                    if (response != null && response.d != null &&
                        response.d.didSucceed)
                    {
                        ActivityIndicator.Hide();
                        OnUseNowDone();
                        InvokeInBackground(async () =>
                        {
                            await RewardsServices.GetUserRewards();
                            DateTime? rDate = RewardsCache.GetRedeemedDate(RewardModel.ID);
                            string rDateStr = string.Empty;
                            if (rDate != null)
                            {
                                try
                                {
                                    DateTime? rDateValue = rDate.Value.ToLocalTime();
                                    rDateStr = rDateValue.Value.ToString(RewardsConstants.Format_Date, DateHelper.DateCultureInfo);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine("Error in ParseDate: " + e.Message);
                                }
                            }
                            RedeemedDate = rDateStr;
                        });
                    }
                    else
                    {
                        ActivityIndicator.Hide();
                        AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_DefaultErrorTitle),
                            LanguageUtility.GetCommonI18NValue(Constants.Common_RedeemRewardFailMsg),
                            new Dictionary<string, Action> {
                        {LanguageUtility.GetCommonI18NValue(Constants.Common_IllDoItLater), null },
                        {LanguageUtility.GetCommonI18NValue(Constants.Common_TryAgain), () => OnUseNowAction()}});
                    }
                });
            });
        }

        private void OnUseNowDone()
        {
            _btnShareReward.Enabled = false;
            nfloat height = GetScaledHeight(122F);
            _timerContainerView = new UIView(new CGRect(0, ViewHeight - height, ViewWidth, height + GetBottomPadding))
            {
                BackgroundColor = UIColor.White
            };
            _timerContainerView.Layer.MasksToBounds = false;
            _timerContainerView.Layer.ShadowColor = MyTNBColor.BrownGrey60.CGColor;
            _timerContainerView.Layer.ShadowOpacity = 1F;
            _timerContainerView.Layer.ShadowOffset = new CGSize(0, -4);
            _timerContainerView.Layer.ShadowRadius = 8;
            _timerContainerView.Layer.ShadowPath = UIBezierPath.FromRect(_timerContainerView.Bounds).CGPath;

            _timerLabel = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(36F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_36_300,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center
            };

            UILabel timerDesc = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_timerLabel.Frame, 12F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(32F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                Text = GetI18NValue(RewardsConstants.I18N_RedeemRewardNote)
            };
            _timerContainerView.AddSubviews(new UIView { _timerLabel, timerDesc });
            View.AddSubview(_timerContainerView);

            ViewHelper.AdjustFrameSetHeight(_scrollView, _timerContainerView.Frame.GetMinY());

            StartTimer();
        }

        private void StartTimer()
        {
            _currentSeconds = RewardModel.RewardUseWithinTime * 60;
            TimeSpan time = TimeSpan.FromSeconds(_currentSeconds);
            _timerLabel.Text = time.ToString(@"mm\:ss");
            _useTimer = new Timer
            {
                Interval = 1000F,
                AutoReset = true,
                Enabled = true
            };
            _useTimer.Elapsed += TimerElapsed;
        }

        private void ResumeTimer(int remainingSeconds)
        {
            _currentSeconds = remainingSeconds;
            TimeSpan time = TimeSpan.FromSeconds(_currentSeconds);
            _timerLabel.Text = time.ToString(@"mm\:ss");
            _useTimer = new Timer
            {
                Interval = 1000F,
                AutoReset = true,
                Enabled = true
            };
            _useTimer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _currentSeconds--;
            InvokeOnMainThread(() =>
            {
                if (_currentSeconds > 0)
                {
                    TimeSpan time = TimeSpan.FromSeconds(_currentSeconds);
                    if (_timerLabel != null)
                    {
                        _timerLabel.Text = time.ToString(@"mm\:ss");
                        CGSize size = _timerLabel.SizeThatFits(new CGSize(ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(36F)));
                        ViewHelper.AdjustFrameSetWidth(_timerLabel, size.Width);
                        ViewHelper.AdjustFrameSetX(_timerLabel, GetXLocationToCenterObject(size.Width, _timerContainerView));
                    }
                }
                else
                {
                    _useTimer.Enabled = false;
                    ViewHelper.AdjustFrameSetHeight(_scrollView, ViewHeight - GetScaledHeight(116F));
                    if (_timerContainerView != null)
                    {
                        _timerContainerView.RemoveFromSuperview();
                        _timerContainerView = null;
                    }
                    UpdateView();
                    _btnShareReward.Enabled = true;
                }
            });
        }

        private void UpdateView()
        {
            foreach (var view in _scrollView.Subviews)
            {
                if (view != null)
                {
                    UIImageView rewardImageView = view.ViewWithTag(RewardsConstants.Tag_DetailRewardImage) as UIImageView;
                    if (rewardImageView != null)
                    {
                        rewardImageView.Image = RewardsServices.ConvertToGrayScale(rewardImageView.Image);
                    }
                    UITextView text = view.ViewWithTag(RewardsConstants.Tag_DetailRewardTitle) as UITextView;
                    if (text != null)
                    {
                        text.TextColor = MyTNBColor.GreyishBrown;
                    }
                    UIView subView = view.ViewWithTag(RewardsConstants.Tag_DetailRewardView) as UIView;
                    if (subView != null)
                    {
                        foreach (var innerView in subView.Subviews)
                        {
                            UIView innerSubView = innerView as UIView;
                            if (innerSubView != null)
                            {
                                foreach (var childView in innerSubView.Subviews)
                                {
                                    if (childView.ViewWithTag(RewardsConstants.Tag_DetailRewardTitle) is UILabel lbl)
                                    {
                                        lbl.TextColor = MyTNBColor.GreyishBrown;
                                    }
                                    if (childView.ViewWithTag(RewardsConstants.Tag_DetailRewardPeriodImage) is UIImageView imgViewPeriod)
                                    {
                                        imgViewPeriod.Image = UIImage.FromBundle(RewardsConstants.Img_RewardPeriodIconUsed);
                                    }
                                    if (childView.ViewWithTag(RewardsConstants.Tag_DetailLocationImage) is UIImageView imgViewLocation)
                                    {
                                        imgViewLocation.Image = UIImage.FromBundle(RewardsConstants.Img_RewardLocationIconUsed);
                                    }
                                    if (childView.ViewWithTag(RewardsConstants.Tag_DetailTCImage) is UIImageView imgViewTC)
                                    {
                                        imgViewTC.Image = UIImage.FromBundle(RewardsConstants.Img_RewardTCIconUsed);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AddFooterView(true);

            nfloat usedWidth = GetScaledWidth(52F);
            nfloat usedHeight = GetScaledHeight(24F);
            UIView usedView = new UIView(new CGRect(_scrollView.Frame.Width - usedWidth - GetScaledWidth(12F), GetScaledHeight(16F), usedWidth, usedHeight))
            {
                BackgroundColor = MyTNBColor.GreyishBrown
            };
            usedView.Layer.CornerRadius = GetScaledHeight(5F);

            UILabel usedLbl = new UILabel(new CGRect(0, GetYLocationToCenterObject(GetScaledHeight(16F), usedView), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(RewardsConstants.I18N_Used)
            };

            CGSize lblSize = usedLbl.SizeThatFits(new CGSize(_scrollView.Frame.Width - (BaseMarginWidth16 * 2), usedLbl.Frame.Height));
            ViewHelper.AdjustFrameSetWidth(usedView, lblSize.Width + (GetScaledWidth(12F) * 2));
            ViewHelper.AdjustFrameSetWidth(usedLbl, lblSize.Width);
            ViewHelper.AdjustFrameSetX(usedLbl, GetXLocationToCenterObject(lblSize.Width, usedView));
            ViewHelper.AdjustFrameSetX(usedView, _scrollView.Frame.Width - usedView.Frame.Width - GetScaledWidth(12F));
            usedView.AddSubview(usedLbl);
            _scrollView.AddSubview(usedView);
        }
        #endregion
    }
}
