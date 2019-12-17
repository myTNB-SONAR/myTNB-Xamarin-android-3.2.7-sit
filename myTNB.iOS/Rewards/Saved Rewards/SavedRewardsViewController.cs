using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public class SavedRewardsViewController : CustomUIViewController
    {
        private UITableView _savedRewardsTableView;
        public List<RewardsModel> SavedRewardsList;
        private bool _isViewDidLoad;

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_SavedRewards;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            _isViewDidLoad = true;
            SetNavigationBar();
            SetTableView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!_isViewDidLoad)
            {
                UpdateSavedRewardList();
            }
            _isViewDidLoad = false;
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
            if (SavedRewardsList != null && SavedRewardsList.Count > 0)
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
            else
            {
                SetEmptySavedRewardView();
            }
        }

        private void SetEmptySavedRewardView()
        {
            if (_savedRewardsTableView != null)
            {
                _savedRewardsTableView.RemoveFromSuperview();
                _savedRewardsTableView = null;
            }
            nfloat iconWidth = GetScaledWidth(102F);
            nfloat iconHeight = GetScaledHeight(94F);
            UIImageView emptyIcon = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth), DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + GetScaledHeight(88F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(RewardsConstants.Img_EmptySavedRewardIcon)
            };
            UITextView emptyDesc = new UITextView(new CGRect(GetScaledWidth(32F), GetYLocationFromFrame(emptyIcon.Frame, 24F), ViewWidth - (GetScaledWidth(32F) * 2), GetScaledHeight(70F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(RewardsConstants.I18N_EmptySavedReward)
            };
            emptyDesc.TextContainer.LineFragmentPadding = 0F;

            View.AddSubviews(new UIView { emptyIcon, emptyDesc });
        }

        #region ACTIONS
        private void UpdateSavedRewardList()
        {
            RewardsEntity rewardsEntity = new RewardsEntity();
            List<RewardsModel> rewardsList = rewardsEntity.GetAllItems();
            if (rewardsList != null && rewardsList.Count > 0)
            {
                SavedRewardsList = rewardsList.FindAll(x => x.IsSaved);
                InvokeOnMainThread(() =>
                {
                    _savedRewardsTableView.ClearsContextBeforeDrawing = true;
                    _savedRewardsTableView.Source = new SavedRewardsDataSource(
                        this,
                        SavedRewardsList,
                        GetI18NValue);
                    _savedRewardsTableView.ReloadData();
                });
            }
        }

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
                rewardDetailView.IsFromSavedRewards = true;
                rewardDetailView.RedeemedDate = rDateStr;
                rewardDetailView.RewardModel = reward;
                UINavigationController navController = new UINavigationController(rewardDetailView);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
        #endregion
    }
}

