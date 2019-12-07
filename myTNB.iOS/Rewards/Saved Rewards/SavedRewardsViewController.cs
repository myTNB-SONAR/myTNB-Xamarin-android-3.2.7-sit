﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class SavedRewardsViewController : CustomUIViewController
    {
        private UITableView _savedRewardsTableView;
        public List<RewardsModel> SavedRewardsList;

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_SavedRewards;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            SetNavigationBar();
            SetTableView();
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
        }

        private void SetTableView()
        {
            if (_savedRewardsTableView != null)
            {
                _savedRewardsTableView.RemoveFromSuperview();
                _savedRewardsTableView = null;
            }
            _savedRewardsTableView = new UITableView(new CGRect(0, 0, ViewWidth, ViewHeight))
            { BackgroundColor = UIColor.Clear };
            _savedRewardsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _savedRewardsTableView.RegisterClassForCellReuse(typeof(SavedRewardCell), RewardsConstants.Cell_SavedRewards);
            View.AddSubview(_savedRewardsTableView);

            _savedRewardsTableView.Source = new SavedRewardsDataSource(
                    this,
                    SavedRewardsList,
                    GetI18NValue);
            _savedRewardsTableView.ReloadData();
        }

        #region ACTIONS
        public void UpdateSavedRewardList()
        {

        }

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
        #endregion
    }
}

