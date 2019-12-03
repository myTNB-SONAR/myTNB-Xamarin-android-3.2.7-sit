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
        private List<RewardsModel> _categoryList;
        private List<RewardsModel> _rewardsList;
        private int _selectedCategoryIndex;

        public RewardsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName;
            base.ViewDidLoad();
            NotifCenterUtility.AddObserver((NSString)"OnReceiveRewardsNotification", OnReceiveRewards);
            ViewHeight += GetBottomPadding;
            View.BackgroundColor = MyTNBColor.SectionGrey;
            SetNavigationBar();
            CreateLoadingCategoryTopBar();
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
            RewardsEntity rewardsEntity = new RewardsEntity();
            _rewardsList = rewardsEntity.GetAllItems();
            if (_rewardsList != null && _rewardsList.Count > 0)
            {
                _categoryList = new List<RewardsModel>();
                RewardsModel viewAllModel = new RewardsModel()
                {
                    CategoryID = "1001",
                    CategoryName = "View All"
                };
                _categoryList = _rewardsList.GroupBy(x => x.CategoryID).Select(x => x.First()).ToList();
                _categoryList.Insert(0, viewAllModel);
                _selectedCategoryIndex = 0;
                CreateCategoryTopBar();
                AddRewardsScrollView();
            }
            else
            {
                // Empty rewards handling here....
            }
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            //NavigationItem.Title = GetI18NValue(RewardsConstants.I18N_Rewards);
            NavigationItem.Title = "Rewards";

            UIBarButtonItem btnSavedRewards = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_HeartIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnSavedRewards");
            });
            NavigationItem.RightBarButtonItem = btnSavedRewards;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        #region REWARDS SCROLL VIEW
        private void AddRewardsScrollView()
        {
            if (_rewardsScrollView != null)
            {
                _rewardsScrollView.RemoveFromSuperview();
                _rewardsScrollView = null;
            }
            _rewardsScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + GetScaledHeight(44F)
                , ViewWidth, ViewHeight - GetScaledHeight(44F)))
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
                UIView viewContainer = new UIView(_rewardsScrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.Clear;

                UITableView rewardsTableView = new UITableView(viewContainer.Bounds)
                { BackgroundColor = UIColor.Clear };
                rewardsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                rewardsTableView.RegisterClassForCellReuse(typeof(RewardsCell), RewardsConstants.Cell_Rewards);
                viewContainer.AddSubview(rewardsTableView);

                _rewardsScrollView.AddSubview(viewContainer);

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);

                var filteredList = i == 0 ? _rewardsList : FilteredRewards(i);
                rewardsTableView.Source = new RewardsDataSource(
                    this,
                    filteredList,
                    GetI18NValue,
                    false);
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
        private void CreateLoadingCategoryTopBar()
        {
            if (_loadingScrollView != null)
            {
                _loadingScrollView.RemoveFromSuperview();
                _loadingScrollView = null;
            }
            _loadingScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            _loadingScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_loadingScrollView);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, _loadingScrollView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, _loadingScrollView.Frame.Height))
            { BackgroundColor = UIColor.Clear };

            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);
            for (int i = 0; i < 5; i++)
            {
                UIView categoryView = new UIView(_loadingScrollView.Bounds);
                categoryView.BackgroundColor = UIColor.White;

                UIView itemView = new UIView(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), GetScaledWidth(52F), labelHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGrey25
                };
                ViewHelper.AdjustFrameSetWidth(categoryView, itemView.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);
                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(itemView);
                _loadingScrollView.AddSubview(categoryView);

                viewShimmerContent.AddSubview(categoryView);
            }

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            _loadingScrollView.AddSubview(viewShimmerParent);

            UIView lineView = new UIView(new CGRect(padding, _loadingScrollView.Frame.Height - GetScaledHeight(2F), GetScaledWidth(52F), GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            lineView.Layer.CornerRadius = GetScaledHeight(1F);
            _loadingScrollView.AddSubview(lineView);
        }

        private void CreateCategoryTopBar()
        {
            if (_loadingScrollView != null)
            {
                _loadingScrollView.RemoveFromSuperview();
            }
            if (_topBarScrollView != null)
            {
                _topBarScrollView.RemoveFromSuperview();
            }
            _topBarScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, GetScaledHeight(44F)))
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

                UIView lineView = new UIView(new CGRect(padding, _loadingScrollView.Frame.Height - GetScaledHeight(2F), labelNewSize.Width, GetScaledHeight(2)))
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

        public void OnSaveUnsaveAction(RewardsModel reward)
        {
            if (reward != null)
            {
                // Call Save/Unsave Service API here...
                Debug.WriteLine("OnSaveUnsaveAction");
                Debug.WriteLine(reward.ID);
                Debug.WriteLine(reward.RewardName);
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

        public void OnReloadTableAction(List<RewardsModel> rewardsList, UITableView tableView)
        {
            if (rewardsList != null && rewardsList.Count > 0)
            {
                _rewardsList = rewardsList;
                if (tableView != null)
                {
                    tableView.ClearsContextBeforeDrawing = true;
                    tableView.Source = new RewardsDataSource(
                    this,
                    _rewardsList,
                    GetI18NValue,
                    false);
                    tableView.ReloadData();
                }
            }
        }
        #endregion
    }
}