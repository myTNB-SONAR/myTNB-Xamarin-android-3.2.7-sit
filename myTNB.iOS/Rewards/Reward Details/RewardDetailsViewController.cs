using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;
using CoreGraphics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardDetailsViewController : CustomUIViewController
    {
        public RewardsModel RewardModel;
        public string RedeemedDate;
        private UIView _footerView;
        private UIScrollView _scrollView;
        private Timer _useTimer;
        private UIView _timerContainerView;
        private UILabel _timerLabel;
        private int _currentSeconds;
        private UIView _lastView;

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_RewardDetails;
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

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
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

            if (!RewardModel.IsUsed)
            {
                UIBarButtonItem btnShareReward = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_ShareIcon), UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    BaseService baseService = new BaseService();
                    APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                    string deeplinkUrl = baseService.GetDomain(env) + "/rewards/redirect.aspx?rid=" + RewardModel.ID;
                    NSObject item = NSObject.FromObject(deeplinkUrl);
                    NSObject[] activityItems = { item };
                    UIActivity[] applicationActivities = null;
                    UIActivityViewController activityController = new UIActivityViewController(activityItems, applicationActivities);
                    UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                    activityController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(activityController, true, null);
                });
                NavigationItem.RightBarButtonItem = btnShareReward;
            }
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
            UIView imageContainer = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(180F)))
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
                    ActivityIndicatorComponent activityIndicator = new ActivityIndicatorComponent(imageContainer);
                    activityIndicator.Show();
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
                                activityIndicator.Hide();
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                activityIndicator.Hide();
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
                    });
                }
            }
            else
            {
                InvokeOnMainThread(() =>
                {
                    imageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
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
                Tag = RewardsConstants.Tag_DetailRewardImage
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
                Tag = RewardsConstants.Tag_DetailRewardImage
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

            locationView.AddSubviews(new UIView { locationIcon, locationTitle, locationTextView, locationLine });

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
                Tag = RewardsConstants.Tag_DetailRewardImage
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
            tandCTextView.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                BrowserViewController viewController =
                    storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                if (viewController != null)
                {
                    viewController.NavigationTitle = "Rewards";
                    viewController.URL = url.AbsoluteString;
                    viewController.IsDelegateNeeded = false;
                    UINavigationController navController = new UINavigationController(viewController)
                    {
                        ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                    };
                    PresentViewController(navController, true, null);
                }
            }));
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
            _lastView = tandCView;
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
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = MyTNBColor.WaterBlue
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

            return textView;
        }

        private void UpdateScrollViewContentSize(UIView lastView)
        {
            _scrollView.ContentSize = new CGSize(ViewWidth, lastView.Frame.GetMaxY());
        }

        private void AddFooterView(bool isUsedReward = false)
        {
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
                string dateValue = RedeemedDate.IsValid() ? "Reward used " + RedeemedDate : string.Empty;
                UILabel usedLabel = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), width - (BaseMarginWidth16 * 2), GetScaledHeight(20F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Font = TNBFont.MuseoSans_14_300I,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Text = dateValue
                };

                UIView rewardUsedBtn = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(usedLabel.Frame, 16F), width - (BaseMarginWidth16 * 2), GetScaledHeight(48F)))
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
                    Text = "Reward Used"
                };

                rewardUsedBtn.AddSubview(rewardUsedLbl);
                _footerView.AddSubviews(new UIView { usedLabel, rewardUsedBtn });
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
                    Debug.WriteLine("btnUseNow on tap");
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

        #region Action Methods
        private void OnUseNowAction()
        {
            ActivityIndicator.Show();
            InvokeInBackground(async () =>
            {
                UpdateRewardsResponseModel response = await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Redeemed, true);
                InvokeOnMainThread(() =>
                {
                    ActivityIndicator.Hide();
                    if (response != null && response.d != null &&
                        response.d.didSucceed)
                    {
                        OnUseNowDone();
                    }
                    else
                    {
                        AlertHandler.DisplayServiceError(this, response?.d?.DisplayMessage);
                    }
                });
            });
        }

        private void OnUseNowDone()
        {
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
                Text = "Reward redeemed. Please show the merchant this screen before the timer runs out."
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
                                    UILabel lbl = childView.ViewWithTag(RewardsConstants.Tag_DetailRewardTitle) as UILabel;
                                    if (lbl != null)
                                    {
                                        lbl.TextColor = MyTNBColor.GreyishBrown;
                                    }
                                    UIImageView imageView = childView.ViewWithTag(RewardsConstants.Tag_DetailRewardImage) as UIImageView;
                                    if (imageView != null)
                                    {
                                        imageView.Image = RewardsServices.ConvertToGrayScale(imageView.Image);
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
                Text = "Used"
            };

            CGSize lblSize = usedLbl.SizeThatFits(new CGSize(_scrollView.Frame.Width - (BaseMarginWidth16 * 2), usedLbl.Frame.Height));
            ViewHelper.AdjustFrameSetWidth(usedView, lblSize.Width + (GetScaledWidth(12F) * 2));
            ViewHelper.AdjustFrameSetWidth(usedLbl, lblSize.Width);
            ViewHelper.AdjustFrameSetX(usedLbl, GetXLocationToCenterObject(lblSize.Width, usedView));
            usedView.AddSubview(usedLbl);
            _scrollView.AddSubview(usedView);
        }
        #endregion
    }
}
