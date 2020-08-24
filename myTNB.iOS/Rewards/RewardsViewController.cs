using CoreGraphics;
using Force.DeepCloner;
using Foundation;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;

namespace myTNB
{
    public partial class RewardsViewController : CustomUIViewController
    {
        internal UIScrollView _loadingScrollView, _topBarScrollView, _rewardsScrollView;
        internal UIView _skeletonLoadingView, _emptyRewardView, _refreshScreenView;
        private List<RewardsModel> _categoryList;
        private List<RewardsModel> _rewardsList;
        private int _selectedCategoryIndex;
        private bool _isViewDidLoad;
        private UIView _tutorialContainer;
        private bool _hotspotIsOn; 

        public RewardsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_Rewards;
            base.ViewDidLoad();
            _isViewDidLoad = true;
            NotifCenterUtility.AddObserver((NSString)"OnReceivedRewardsNotification", OnReceivedRewards);
            NotifCenterUtility.AddObserver(UIApplication.WillChangeStatusBarFrameNotification, OnChangeStatusBarFrame);
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            View.BackgroundColor = MyTNBColor.VeryLightPinkEight;
            InitiateView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading && !AppLaunchMasterCache.IsRewardsDisabled)
            {
                CheckForRewardUpdates();
            }
            else
            {
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                SetSkeletonLoading();
            }
            _isViewDidLoad = false;
        }

        private void OnEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is RewardsViewController)
                {
                    OnChangeStatusBarFrame(null);
                }
            }
        }

        private void OnChangeStatusBarFrame(NSNotification notification)
        {
            if (DeviceHelper.IsIphoneXUpResolution())
                return;

            SetFrames();
            _hotspotIsOn = DeviceHelper.GetStatusBarHeight() > 20;
            if (_tutorialContainer != null)
            {
                _tutorialContainer.RemoveFromSuperview();
            }
            CheckTutorialOverlay();
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> Rewards LanguageDidChange");
            base.LanguageDidChange(notification);
            Title = GetI18NValue(RewardsConstants.I18N_Title);
        }

        private void OnReceivedRewards(NSNotification notification)
        {
            Debug.WriteLine("OnReceivedRewardsNotification");
            if (RewardsCache.RewardIsAvailable)
            {
                ProcessRewards();
            }
            else
            {
                SetRefreshScreen();
            }
        }

        private void CheckForRewardUpdates()
        {
            if (NetworkUtility.isReachable)
            {
                InvokeInBackground(async () =>
                {
                    bool hasUpdate = await RewardsServices.RewardListHasUpdates() || RewardsCache.RefreshReward;
                    InvokeOnMainThread(() =>
                    {
                        if (hasUpdate)
                        {
                            RewardsCache.RefreshReward = false;
                            DataManager.DataManager.SharedInstance.IsRewardsLoading = true;
                            ResetViews();
                            NavigationController.NavigationBar.Hidden = false;
                            SetSkeletonLoading();
                            InvokeInBackground(async () =>
                            {
                                await RewardsServices.GetLatestRewards();
                                if (RewardsCache.RewardIsAvailable)
                                {
                                    await RewardsServices.GetUserRewards();
                                    InvokeOnMainThread(() =>
                                    {
                                        DataManager.DataManager.SharedInstance.IsRewardsLoading = false;
                                        if (RewardsCache.RewardIsAvailable)
                                        {
                                            ProcessRewards();
                                        }
                                        else
                                        {
                                            SetRefreshScreen();
                                        }
                                    });
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        DataManager.DataManager.SharedInstance.IsRewardsLoading = false;
                                        SetRefreshScreen();
                                    });
                                }
                            });
                        }
                        else
                        {
                            if (RewardsCache.RewardIsAvailable)
                            {
                                bool needsUpdate = RewardsServices.FilterExpiredRewards() || RewardsServices.FilterUsedRewards();
                                if (needsUpdate)
                                {
                                    ValidateRewards();
                                }
                                else
                                {
                                    if (!_isViewDidLoad)
                                    {
                                        NotifCenterUtility.PostNotificationName("RewardsFetchUpdate", new NSObject());
                                        RefreshRewardsList();
                                    }
                                }
                            }
                            else
                            {
                                SetRefreshScreen();
                            }
                        }
                    });
                });
            }
            else
            {
                AlertHandler.DisplayNoDataAlert(this);
            }
        }

        private void SetRefreshScreen()
        {
            ResetViews();
            NavigationController.NavigationBar.Hidden = true;

            _refreshScreenView = new UIView(new CGRect(0, 0, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView refreshIcon = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(RewardsConstants.IMG_Refresh),
                BackgroundColor = UIColor.Clear
            };

            _refreshScreenView.AddSubview(refreshIcon);

            UILabel title = new UILabel(new CGRect(BaseMarginWidth16, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(24F)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(RewardsConstants.I18N_Title),
                TextAlignment = UITextAlignment.Center
            };
            _refreshScreenView.AddSubview(title);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetCommonI18NValue(Constants.Common_RefreshMessage)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(16F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.Grey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            nfloat descwidth = ViewWidth - (BaseMarginWidth16 * 2);
            UITextView desc = new UITextView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(refreshIcon.Frame, 16F), descwidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextAlignment = UITextAlignment.Center

            };
            desc.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = desc.SizeThatFits(new CGSize(descwidth, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetWidth(desc, cGSize.Width);
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);

            _refreshScreenView.AddSubview(desc);

            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, GetYLocationFromFrame(desc.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                PageName = PageName,
                EventName = RewardsConstants.EVENT_Refresh,
                Hidden = false
            };
            btnRefresh.SetTitle(GetCommonI18NValue(Constants.Common_RefreshNow), UIControlState.Normal);
            btnRefresh.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                RefreshButtonOnTap();
            }));
            _refreshScreenView.AddSubview(btnRefresh);
            View.AddSubview(_refreshScreenView);
        }

        private void RefreshButtonOnTap()
        {
            if (AppLaunchMasterCache.IsRewardsDisabled) { return; }

            if (NetworkUtility.isReachable)
            {
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                SetSkeletonLoading();

                InvokeInBackground(async () =>
                {
                    DataManager.DataManager.SharedInstance.IsRewardsLoading = true;
                    await SitecoreServices.Instance.LoadRewards();
                    if (RewardsCache.RewardIsAvailable)
                    {
                        await RewardsServices.GetUserRewards();
                        InvokeOnMainThread(() =>
                        {
                            if (RewardsCache.RewardIsAvailable)
                            {
                                ProcessRewards();
                            }
                            else
                            {
                                SetRefreshScreen();
                            }
                        });
                        DataManager.DataManager.SharedInstance.IsRewardsLoading = false;
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            SetRefreshScreen();
                        });
                        DataManager.DataManager.SharedInstance.IsRewardsLoading = false;
                    }
                });
            }
            else
            {
                AlertHandler.DisplayNoDataAlert(this);
            }
        }

        private void ResetViews()
        {
            if (_skeletonLoadingView != null)
            {
                _skeletonLoadingView.RemoveFromSuperview();
                _skeletonLoadingView = null;
            }
            if (_topBarScrollView != null)
            {
                _topBarScrollView.RemoveFromSuperview();
                _topBarScrollView = null;
            }
            if (_rewardsScrollView != null)
            {
                _rewardsScrollView.RemoveFromSuperview();
                _rewardsScrollView = null;
            }
            if (_emptyRewardView != null)
            {
                _emptyRewardView.RemoveFromSuperview();
                _emptyRewardView = null;
            }
            if (_refreshScreenView != null)
            {
                _refreshScreenView.RemoveFromSuperview();
                _refreshScreenView = null;
            }
        }

        private void InitiateView()
        {
            SetNavigationBar();
            SetSkeletonLoading();
            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading)
            {
                if (RewardsCache.RewardIsAvailable)
                {
                    ProcessRewards();
                }
                else
                {
                    SetRefreshScreen();
                }
            }
        }

        private void ProcessRewards()
        {
            RewardsServices.FilterExpiredRewards();
            RewardsServices.FilterUsedRewards();
            ValidateRewards();
        }

        private void ValidateRewards()
        {
            InvokeOnMainThread(async () =>
            {
                NotifCenterUtility.PostNotificationName("RewardsFetchUpdate", new NSObject());
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                RewardsEntity rewardsEntity = new RewardsEntity();
                _rewardsList = rewardsEntity.GetAllItems();
                if (_rewardsList != null && _rewardsList.Count > 0)
                {
                    SetRightBarItem(false);
                    _categoryList = new List<RewardsModel>();
                    _categoryList = _rewardsList.GroupBy(x => x.CategoryID).Select(x => x.First()).ToList();

                    if (_categoryList.Count > 1)
                    {
                        RewardsModel viewAllModel = new RewardsModel()
                        {
                            CategoryID = "1001",
                            CategoryName = GetI18NValue(RewardsConstants.I18N_ViewAll)
                        };
                        _categoryList.Insert(0, viewAllModel);
                    }
                    _selectedCategoryIndex = 0;
                    AddRewardsScrollView();
                    if (_categoryList.Count > 1)
                    {
                        CreateCategoryTopBar(0);
                    }
                    _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
                    CheckTutorialOverlay();
                }
                else
                {
                    SetRightBarItem(true);
                    SetEmptyRewardView();
                }
            });
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(RewardsConstants.I18N_Title);
        }

        private void SetRightBarItem(bool isEmptyReward = false)
        {
            if (!isEmptyReward)
            {
                UIBarButtonItem btnSavedRewards = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_HeartIcon), UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    if (RewardsCache.RewardIsAvailable && !DataManager.DataManager.SharedInstance.IsRewardsLoading && _rewardsList != null)
                    {
                        SavedRewardsViewController savedRewardsView = new SavedRewardsViewController
                        {
                            SavedRewardsList = _rewardsList.FindAll(x => x.IsSaved)
                        };
                        UINavigationController navController = new UINavigationController(savedRewardsView);
                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(navController, true, null);
                    }
                });
                NavigationItem.RightBarButtonItem = btnSavedRewards;
            }
            else
            {
                NavigationItem.RightBarButtonItem = null;
            }
        }

        private void SetSkeletonLoading()
        {
            ResetViews();
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            _skeletonLoadingView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height - addtl, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(_skeletonLoadingView);

            UIScrollView scrollViewCategories = new UIScrollView(new CGRect(0, 0, ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            scrollViewCategories.Layer.CornerRadius = GetScaledHeight(5F);
            scrollViewCategories.Layer.MasksToBounds = false;
            scrollViewCategories.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            scrollViewCategories.Layer.ShadowOpacity = 0.5F;
            scrollViewCategories.Layer.ShadowOffset = new CGSize(-4, 8);
            scrollViewCategories.Layer.ShadowRadius = 8;
            scrollViewCategories.Layer.ShadowPath = UIBezierPath.FromRect(scrollViewCategories.Bounds).CGPath;
            scrollViewCategories.ShowsHorizontalScrollIndicator = false;
            _skeletonLoadingView.AddSubview(scrollViewCategories);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, _skeletonLoadingView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, _skeletonLoadingView.Frame.Height))
            { BackgroundColor = UIColor.Clear };

            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);
            for (int i = 0; i < 5; i++)
            {
                UIView categoryView = new UIView(scrollViewCategories.Bounds);
                categoryView.BackgroundColor = UIColor.White;

                UIView catItemView = new UIView(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), GetScaledWidth(52F), labelHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };
                catItemView.Layer.CornerRadius = GetScaledHeight(2F);
                ViewHelper.AdjustFrameSetWidth(categoryView, catItemView.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);
                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(catItemView);
                scrollViewCategories.AddSubview(categoryView);

                viewShimmerContent.AddSubview(categoryView);
            }
            scrollViewCategories.AddSubview(viewShimmerParent);

            UIView lineView = new UIView(new CGRect(padding, scrollViewCategories.Frame.Height - GetScaledHeight(2F), GetScaledWidth(52F), GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            lineView.Layer.CornerRadius = GetScaledHeight(1F);
            scrollViewCategories.AddSubview(lineView);

            nfloat topPadding = GetScaledWidth(17F);
            nfloat yPos = scrollViewCategories.Frame.GetMaxY() + topPadding;
            for (int i = 0; i < 5; i++)
            {
                UIView itemView = new UIView(new CGRect(BaseMarginWidth16, yPos, ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(160F)))
                {
                    BackgroundColor = UIColor.White,
                    ClipsToBounds = true
                };
                itemView.Layer.CornerRadius = GetScaledHeight(5F);
                itemView.Layer.MasksToBounds = false;
                itemView.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
                itemView.Layer.ShadowOpacity = 0.5F;
                itemView.Layer.ShadowOffset = new CGSize(-4, 8);
                itemView.Layer.ShadowRadius = 8;
                itemView.Layer.ShadowPath = UIBezierPath.FromRect(itemView.Bounds).CGPath;

                UIView rewardImgView = new UIImageView(new CGRect(0, 0, itemView.Frame.Width, GetScaledHeight(112F)))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree,
                    ClipsToBounds = false
                };
                itemView.AddSubview(rewardImgView);

                UIView titleView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(rewardImgView.Frame, 16F), itemView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };
                titleView.Layer.CornerRadius = GetScaledHeight(2F);
                itemView.AddSubview(titleView);

                yPos = itemView.Frame.GetMaxY() + topPadding;
                _skeletonLoadingView.AddSubview(itemView);

                viewShimmerContent.AddSubview(itemView);
            }

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();
        }

        private void SetEmptyRewardView()
        {
            ResetViews();
            _emptyRewardView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(_emptyRewardView);
            nfloat iconWidth = GetScaledWidth(102F);
            nfloat iconHeight = GetScaledHeight(94F);
            UIImageView emptyIcon = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth), DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + GetScaledHeight(88F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(RewardsConstants.Img_EmptyRewardIcon)
            };
            UITextView emptyDesc = new UITextView(new CGRect(GetScaledWidth(32F), GetYLocationFromFrame(emptyIcon.Frame, 24F), ViewWidth - (GetScaledWidth(32F) * 2), GetScaledHeight(70F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(RewardsConstants.I18N_NoRewards)
            };
            emptyDesc.TextContainer.LineFragmentPadding = 0F;

            _emptyRewardView.AddSubviews(new UIView { emptyIcon, emptyDesc });
        }

        #region REWARDS SCROLL VIEW
        private void AddRewardsScrollView()
        {
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            var categoryHeight = _categoryList != null && _categoryList.Count > 1 ? GetScaledHeight(44F) : 0;
            _rewardsScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + categoryHeight - addtl
                , ViewWidth, ViewHeight - categoryHeight + addtl))
            {
                Delegate = new ScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.Clear,
                Hidden = false,
                Bounces = false
            };
            SetRewardTableViewForCategory();
            View.AddSubview(_rewardsScrollView);
            if (DeviceHelper.IsIOS10AndBelow)
            {
                _rewardsScrollView.SetContentOffset(new CGPoint(_rewardsScrollView.Frame.Width * _selectedCategoryIndex
                    , NavigationController.NavigationBar.Frame.Height + DeviceHelper.GetStatusBarHeight()), true);
            }
        }

        private void SetRewardTableViewForCategory()
        {
            nfloat width = _rewardsScrollView.Frame.Width;
            for (int i = 0; i < _categoryList.Count; i++)
            {
                UIView viewContainer = new UIView(_rewardsScrollView.Bounds)
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = RewardsConstants.Tag_ViewContainer * (i + 1)
                };

                UITableView rewardsTableView = new UITableView(viewContainer.Bounds)
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = RewardsConstants.Tag_TableView,
                    SeparatorStyle = UITableViewCellSeparatorStyle.None
                };

                rewardsTableView.RegisterClassForCellReuse(typeof(RewardsCell), RewardsConstants.Cell_Rewards);
                viewContainer.AddSubview(rewardsTableView);

                _rewardsScrollView.AddSubview(viewContainer);

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);

                var filteredList = i == 0 ? _rewardsList : FilteredRewards(i);
                rewardsTableView.Source = new RewardsDataSource(this, filteredList, GetI18NValue);
                rewardsTableView.ReloadData();
            }
            _rewardsScrollView.ContentSize = new CGSize(_rewardsScrollView.Frame.Width * _categoryList.Count, _rewardsScrollView.Frame.Height);
        }

        private List<RewardsModel> FilteredRewards(int index)
        {
            List<RewardsModel> filteredRewards = new List<RewardsModel>();
            var activeCatId = _categoryList[index].CategoryID;
            filteredRewards = _rewardsList.FindAll(x => x.CategoryID.Equals(activeCatId)).ToList();
            return filteredRewards;
        }

        private void ScrollViewHasPaginated()
        {
            CategorySelectionUpdate(_selectedCategoryIndex);
        }

        private class ScrollViewDelegate : UIScrollViewDelegate
        {
            RewardsViewController _controller;
            public ScrollViewDelegate(RewardsViewController controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._rewardsScrollView.ContentOffset.X / _controller._rewardsScrollView.Frame.Width);
                if (newPageIndex == _controller._selectedCategoryIndex)
                    return;

                _controller._selectedCategoryIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
        #endregion

        #region CATEGORY TOP BAR MENU
        private void CreateCategoryTopBar(nfloat addtlPadding)
        {
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            _topBarScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height - addtl
                , ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White,
                Tag = 1001
            };
            _topBarScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_topBarScrollView);
            SetCategoryTopBarValues(addtlPadding);
        }

        private void SetCategoryTopBarValues(nfloat addtlPadding)
        {
            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(16F);
            nfloat padding = GetScaledWidth(10F) + addtlPadding;
            CustomUIView lastView = new CustomUIView();

            for (int i = 0; i < _categoryList.Count; i++)
            {
                CustomUIView categoryView = new CustomUIView(_topBarScrollView.Bounds)
                {
                    BackgroundColor = UIColor.White,
                    Tag = i
                };
                categoryView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSelectCategoryAction((int)categoryView.Tag);
                }));
                UILabel categoryLabel = new UILabel(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), 0, labelHeight))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = i == 0 ? MyTNBColor.WaterBlue : MyTNBColor.WarmGrey,
                    TextAlignment = UITextAlignment.Center,
                    Text = _categoryList[i].CategoryName,
                    Tag = RewardsConstants.Tag_CategoryLabel
                };

                CGSize labelNewSize = categoryLabel.SizeThatFits(new CGSize(500F, labelHeight));
                ViewHelper.AdjustFrameSetWidth(categoryLabel, labelNewSize.Width);
                ViewHelper.AdjustFrameSetWidth(categoryView, categoryLabel.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);

                UIView lineView = new UIView(new CGRect(padding
                    , _topBarScrollView.Frame.Height - GetScaledHeight(2F), labelNewSize.Width, GetScaledHeight(2)))
                {
                    BackgroundColor = MyTNBColor.WaterBlue,
                    Tag = RewardsConstants.Tag_SelectedCategory,
                    Hidden = i != 0
                };
                lineView.Layer.CornerRadius = GetScaledHeight(1F);
                categoryView.AddSubview(lineView);

                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(categoryLabel);
                _topBarScrollView.AddSubview(categoryView);
                lastView = categoryView;
            }
            _topBarScrollView.ContentSize = new CGSize(xPos, _topBarScrollView.Frame.Height);
            AdjustCategoryMenuUI(lastView);
        }

        private void AdjustCategoryMenuUI(CustomUIView lastView)
        {
            if (lastView != null)
            {
                if (Convert.ToInt32(lastView.Frame.GetMaxX()) < Convert.ToInt32(ViewWidth))
                {
                    nfloat diff = ViewWidth - lastView.Frame.GetMaxX();
                    nfloat xtraPadding = diff / (_categoryList.Count * 2);

                    if (_topBarScrollView != null)
                    {
                        _topBarScrollView.RemoveFromSuperview();
                        _topBarScrollView = null;
                    }
                    CreateCategoryTopBar(xtraPadding);
                }
            }
        }

        private void OnSelectCategoryAction(int index)
        {
            _selectedCategoryIndex = index;
            _rewardsScrollView.SetContentOffset(new CGPoint(_rewardsScrollView.Frame.Width * _selectedCategoryIndex, 0), true);
        }

        private void CategorySelectionUpdate(int index)
        {
            foreach (UIView catView in _topBarScrollView.Subviews)
            {
                if (catView != null)
                {
                    UILabel label = catView.ViewWithTag(RewardsConstants.Tag_CategoryLabel) as UILabel;
                    if (label != null)
                    {
                        label.TextColor = catView.Tag == index ? MyTNBColor.WaterBlue : MyTNBColor.WarmGrey;
                    }
                    UIView line = catView.ViewWithTag(RewardsConstants.Tag_SelectedCategory) as UIView;
                    if (line != null)
                    {
                        line.Hidden = catView.Tag != index;
                    }
                }

                try
                {
                    if (catView != null && catView.GetType() != typeof(UIImageView))
                    {
                        if (catView.Tag == _selectedCategoryIndex)
                        {
                            nfloat maxXPos = catView.Frame.X - _topBarScrollView.ContentOffset.X + catView.Frame.Width;
                            nfloat minXPos = catView.Frame.X;
                            if (maxXPos > ViewWidth)
                            {
                                nfloat addtl = 0;
                                UIView nView = _topBarScrollView.ViewWithTag(_selectedCategoryIndex + 1) as UIView;
                                if (nView != null && nView.GetType() != typeof(UIImageView))
                                {
                                    addtl = nView.Frame.Width * 0.30F;
                                }
                                _topBarScrollView.SetContentOffset(new CGPoint(catView.Frame.X + catView.Frame.Width - ViewWidth + addtl, 0), true);
                            }
                            else if (minXPos < _topBarScrollView.ContentOffset.X)
                            {
                                nfloat addtl = 0;
                                UIView pView = _topBarScrollView.ViewWithTag(_selectedCategoryIndex - 1) as UIView;
                                if (pView != null && pView.GetType() != typeof(UIImageView))
                                {
                                    addtl = pView.Frame.Width * 0.30F;
                                }
                                _topBarScrollView.SetContentOffset(new CGPoint(minXPos - addtl, 0), true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
            }
        }
        #endregion

        #region ACTIONS
        public void OnRewardSelection(RewardsModel reward)
        {
            if (reward != null)
            {
                RewardDetailsViewController rewardDetailView = new RewardDetailsViewController();
                DateTime? rDate = RewardsCache.GetRedeemedDate(reward.ID);
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
                rewardDetailView.IsFromSavedRewards = false;
                rewardDetailView.RedeemedDate = rDateStr;
                rewardDetailView.RewardModel = reward;
                UINavigationController navController = new UINavigationController(rewardDetailView)
                {
                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                };
                PresentViewController(navController, true, null);
            }
        }

        public void OnSaveUnsaveAction(RewardsModel reward)
        {
            if (reward != null)
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeInBackground(async () =>
                    {
                        await RewardsServices.UpdateRewards(reward, RewardsServices.RewardProperties.Favourite, reward.IsSaved);
                        RefreshRewardsList();
                    });
                }
                else
                {
                    AlertHandler.DisplayNoDataAlert(this);
                }
            }
        }

        public void OnUpdateReadRewards(RewardsModel reward)
        {
            if (reward == null)
                return;

            var entity = RewardsEntity.GetItem(reward.ID);
            if (entity != null)
            {
                var entityModel = reward.ToEntity();
                RewardsEntity rewardsEntity = new RewardsEntity();
                rewardsEntity.UpdateItem(entityModel);
            }
        }

        private void RefreshRewardsList()
        {
            InvokeInBackground(async () =>
            {
                GetUserRewardsResponseModel userRewards = await RewardsServices.GetUserRewards();
                InvokeOnMainThread(() =>
                {
                    if (userRewards != null && userRewards.d != null && userRewards.d.IsSuccess)
                    {
                        if (userRewards.d.data != null && userRewards.d.data.UserRewards != null)
                        {
                            List<RewardsModel> rList = _rewardsList.DeepClone();
                            foreach (RewardsItemModel item in userRewards.d.data.UserRewards)
                            {
                                int index = rList.FindIndex(x => x.ID == item.RewardId);
                                if (index > -1)
                                {
                                    rList[index].IsRead = item.Read;
                                    rList[index].IsSaved = item.Favourite;
                                    rList[index].IsUsed = item.Redeemed;
                                }
                            }
                            _rewardsList = rList;
                            RefreshTable();
                        }
                    }
                    else
                    {
                        SetRefreshScreen();
                    }
                });
            });
        }

        private void RefreshTable()
        {
            if (_rewardsList != null && _rewardsList.Count > 0)
            {
                for (int c = 0; c < _categoryList.Count; c++)
                {
                    UIView viewContainer = _rewardsScrollView.Subviews[c];
                    if (viewContainer != null && viewContainer.Subviews.Count() > 0)
                    {
                        if (viewContainer.Subviews[0] is UITableView table)
                        {
                            var filteredList = c == 0 ? _rewardsList : FilteredRewards(c);
                            table.ClearsContextBeforeDrawing = true;
                            table.Source = new RewardsDataSource(this, filteredList, GetI18NValue);
                            UIView.PerformWithoutAnimation(() =>
                            {
                                table.BeginUpdates();
                                table.ReloadData();
                                table.EndUpdates();
                            });
                        }
                    }
                }
            }
        }
        #endregion

        #region TUTORIAL OVERLAY
        public void CheckTutorialOverlay()
        {
            if (!RewardsCache.RewardIsAvailable) { return; }

            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(RewardsConstants.Pref_RewardsTutorialOverlay);

            if (tutorialOverlayHasShown) { return; }

            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading && _rewardsList != null && _rewardsList.Count > 0)
            {
                InvokeOnMainThread(() =>
                {
                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is RewardsViewController)
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

            RewardsTutorialOverlay tutorialView = new RewardsTutorialOverlay(_tutorialContainer, this, _hotspotIsOn)
            {
                OnDismissAction = HideTutorialOverlay,
                GetI18NValue = GetI18NValue
            };
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is RewardsViewController && _tutorialContainer != null && !_tutorialContainer.IsDescendantOfView(currentWindow))
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
                sharedPreference.SetBool(true, RewardsConstants.Pref_RewardsTutorialOverlay);
                sharedPreference.Synchronize();
            }
        }

        public nfloat GetFirstRewardYPos()
        {
            nfloat yPos = 0;
            if (_rewardsScrollView != null)
            {
                yPos = _rewardsScrollView.Frame.Y + GetScaledHeight(17F);
            }
            return yPos;
        }

        public nfloat GetNavigationMaxYPos()
        {
            try
            {
                return NavigationController.NavigationBar.Frame.GetMaxY();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in services: " + e.Message);
                return 0;
            }
        }

        public CGRect GetSavedRewardFrame()
        {
            try
            {
                return NavigationController.NavigationItem.RightBarButtonItem.AccessibilityFrame;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in services: " + e.Message);
                return new CGRect();
            }
        }

        public bool CategoryMenuIsVisible()
        {
            return _categoryList != null && _categoryList.Count > 2;
        }
        #endregion
    }
}
