using CoreGraphics;
using Foundation;
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
        internal UIView _skeletonLoadingView;
        private List<RewardsModel> _categoryList;
        private List<RewardsModel> _rewardsList;
        private int _selectedCategoryIndex, props_index;

        private bool props_needsUpdate;
        private bool _isViewDidLoad;
        private List<RewardsModel> props_rewardsList;

        public RewardsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_Rewards;
            base.ViewDidLoad();
            _isViewDidLoad = true;
            NotifCenterUtility.AddObserver((NSString)"OnReceiveRewardsNotification", OnReceiveRewards);
            ViewHeight += GetBottomPadding;
            View.BackgroundColor = MyTNBColor.VeryLightPinkEight;
            SetNavigationBar();
            SetSkeletonLoading();
            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading)
            {
                ValidateRewards();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!_isViewDidLoad)
            {
                props_needsUpdate = true;
                OnTableReload();
            }
            _isViewDidLoad = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            OnTableReload();
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> Rewards LanguageDidChange");
            base.LanguageDidChange(notification);
            Title = GetI18NValue(RewardsConstants.I18N_Title);
            SetSkeletonLoading();
            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading)
            {
                ValidateRewards();
            }
        }

        private void OnReceiveRewards(NSNotification notification)
        {
            Debug.WriteLine("OnReceiveRewardsNotification");
            ValidateRewards();
        }

        private void ValidateRewards()
        {
            InvokeOnMainThread(async () =>
            {
                if (_skeletonLoadingView != null)
                {
                    _skeletonLoadingView.RemoveFromSuperview();
                    _skeletonLoadingView = null;
                }
                RewardsEntity rewardsEntity = new RewardsEntity();
                _rewardsList = rewardsEntity.GetAllItems();
                if (_rewardsList != null && _rewardsList.Count > 0)
                {
                    _categoryList = new List<RewardsModel>();
                    RewardsModel viewAllModel = new RewardsModel()
                    {
                        CategoryID = "1001",
                        CategoryName = GetI18NValue(RewardsConstants.I18N_ViewAll)
                    };
                    _categoryList = _rewardsList.GroupBy(x => x.CategoryID).Select(x => x.First()).ToList();
                    _categoryList.Insert(0, viewAllModel);
                    _selectedCategoryIndex = 0;
                    CreateCategoryTopBar();
                    AddRewardsScrollView();
                }
                else
                {
                    SetEmptyRewardView();
                }
            });
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(RewardsConstants.I18N_Title);
            UIBarButtonItem btnSavedRewards = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_HeartIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnSavedRewards");
                SavedRewardsViewController savedRewardsView = new SavedRewardsViewController
                {
                    SavedRewardsList = _rewardsList.FindAll(x => x.IsSaved)
                };
                UINavigationController navController = new UINavigationController(savedRewardsView);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            });
            NavigationItem.RightBarButtonItem = btnSavedRewards;
        }

        private void SetSkeletonLoading()
        {
            if (_skeletonLoadingView != null)
            {
                _skeletonLoadingView.RemoveFromSuperview();
                _skeletonLoadingView = null;
            }
            _skeletonLoadingView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, ViewHeight))
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

            View.AddSubviews(new UIView { emptyIcon, emptyDesc });
        }

        #region REWARDS SCROLL VIEW
        private void AddRewardsScrollView()
        {
            if (_rewardsScrollView != null)
            {
                _rewardsScrollView.RemoveFromSuperview();
                _rewardsScrollView = null;
            }

            nfloat yDelta = DeviceHelper.IsIphoneXUpResolution() ? 0 : DeviceHelper.GetStatusBarHeight();
            _rewardsScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + GetScaledHeight(44F)
                , ViewWidth, ViewHeight - GetScaledHeight(44F) + yDelta))
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

            View.AddSubview(_rewardsScrollView);
            SetRewardTableViewForCategory();
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
        private void CreateCategoryTopBar()
        {
            if (_topBarScrollView != null)
            {
                _topBarScrollView.RemoveFromSuperview();
            }
            _topBarScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height
                , ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            _topBarScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_topBarScrollView);
            SetCategoryTopBarValues();
        }

        private void SetCategoryTopBarValues()
        {
            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);

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
            }
            _topBarScrollView.ContentSize = new CGSize(xPos, _topBarScrollView.Frame.Height);
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
            }
        }
        #endregion

        #region ACTIONS
        public void OnRewardSelection(RewardsModel reward)
        {
            if (reward != null)
            {
                RewardDetailsViewController rewardDetailView = new RewardDetailsViewController();
                rewardDetailView.RewardModel = reward;
                UINavigationController navController = new UINavigationController(rewardDetailView);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        public void OnSaveUnsaveAction(List<RewardsModel> rewardsList, RewardsModel reward, int index)
        {
            if (reward != null)
            {
                // Call Save/Unsave Service API here...
                Debug.WriteLine("OnSaveUnsaveAction");
                Debug.WriteLine(reward.ID);
                Debug.WriteLine(reward.RewardName);
                InvokeInBackground(async () =>
                {
                    await RewardsServices.UpdateRewards(reward, RewardsServices.RewardProperties.Favourite, reward.IsSaved);
                    InvokeOnMainThread(() =>
                    {
                        OnReloadTableAction(rewardsList, index);
                    });
                });
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

        internal void SetReloadProperties(List<RewardsModel> rewardsList, int index)
        {
            props_needsUpdate = true;
            props_rewardsList = rewardsList;
            props_index = index;
        }

        private void OnTableReload()
        {
            InvokeOnMainThread(async () =>
            {
                GetUserRewardsResponseModel _userRewards = await RewardsServices.GetUserRewards();
                if (props_needsUpdate && props_rewardsList != null)
                {
                    if (_userRewards != null && _userRewards.d != null && _userRewards.d.IsSuccess
                        && _userRewards.d.data != null && _userRewards.d.data.UserRewards != null)
                    {
                        foreach (RewardsItemModel item in _userRewards.d.data.UserRewards)
                        {
                            int index = props_rewardsList.FindIndex(x => x.ID == item.RewardId);
                            if (index > -1)
                            {
                                props_rewardsList[index].IsRead = item.Read;
                                props_rewardsList[index].IsSaved = item.Favourite;
                                props_rewardsList[index].IsUsed = item.Redeemed;
                            }
                        }
                    }
                    OnReloadTableAction(props_rewardsList, props_index);
                    props_needsUpdate = false;
                }

            });
        }

        public void OnReloadTableAction(List<RewardsModel> rewardsList, int index)
        {
            if (rewardsList != null && rewardsList.Count > 0 && index > -1 && index < rewardsList.Count)
            {
                RewardsModel reward = rewardsList[index];
                var catIndx = _categoryList.FindIndex(x => x.CategoryID.Equals(reward.CategoryID));

                if (catIndx > -1 && catIndx < _rewardsScrollView.Subviews.Count())
                {
                    UIView viewContainer = _rewardsScrollView.Subviews[catIndx];
                    if (viewContainer != null && viewContainer.Subviews.Count() > 0)
                    {
                        if (viewContainer.Subviews[0] is UITableView table)
                        {
                            var filteredList = catIndx == 0 ? _rewardsList : FilteredRewards(catIndx);
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

                UIView viewAllView = _rewardsScrollView.Subviews[0];
                if (viewAllView != null && viewAllView.Subviews.Count() > 0)
                {
                    if (viewAllView.Subviews[0] is UITableView table)
                    {
                        table.ClearsContextBeforeDrawing = true;
                        table.Source = new RewardsDataSource(this, _rewardsList, GetI18NValue);
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

        #endregion
    }
}