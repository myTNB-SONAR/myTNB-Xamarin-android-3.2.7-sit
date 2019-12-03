using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsDataSource : UITableViewSource
    {
        private readonly RewardsViewController _controller;
        private List<RewardsModel> _rewardsList = new List<RewardsModel>();
        private readonly bool _isLoading;
        public Func<string, string> GetI18NValue;

        public RewardsDataSource(RewardsViewController controller,
            List<RewardsModel> rewardsList,
            Func<string, string> getI18NValue,
            bool isLoading = false)
        {
            _controller = controller;
            _rewardsList = rewardsList;
            GetI18NValue = getI18NValue;
            _isLoading = isLoading;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _rewardsList.Count;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat addtl = 0;
            if (indexPath.Row == _rewardsList.Count - 1)
            {
                addtl = ScaleUtility.GetScaledHeight(17F);
            }
            return RewardsConstants.RewardsCellHeight + addtl;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RewardsCell cell = tableView.DequeueReusableCell(RewardsConstants.Cell_Rewards) as RewardsCell;
            cell.Tag = indexPath.Row;
            if (cell.Tag > -1 && cell.Tag < _rewardsList.Count)
            {
                RewardsModel reward = _rewardsList[(int)cell.Tag];
                cell.CellIndex = (int)cell.Tag;
                cell.GetI18NValue = GetI18NValue;
                cell.SetAccountCell(reward);
                if (reward.Image.IsValid())
                {
                    if (cell.Tag == indexPath.Row)
                    {
                        try
                        {
                            if ((bool)cell.ActivityIndicator?.GetView?.IsDescendantOfView(cell.RewardImageVIew))
                            {
                                cell.ActivityIndicator.GetView.RemoveFromSuperview();
                                cell.ActivityIndicator.GetView = null;
                                cell.ActivityIndicator = null;
                                cell.ActivityIndicator = new ActivityIndicatorComponent(cell.RewardImageVIew);
                            }
                            cell.ActivityIndicator.Show();
                            NSUrl url = new NSUrl(reward.Image);
                            NSUrlSession session = NSUrlSession
                                .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                            NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    if (error == null && response != null && data != null)
                                    {
                                        if (cell.Tag == indexPath.Row)
                                        {
                                            cell.RewardImageVIew.Image = UIImage.LoadFromData(data);
                                        }
                                    }
                                    else
                                    {
                                        cell.RewardImageVIew.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                    }
                                    cell.ActivityIndicator.Hide();
                                });
                            });
                            dataTask.Resume();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            cell.RewardImageVIew.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                        }
                    }
                }
                else
                {
                    cell.RewardImageVIew.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                }
            }
            cell.SaveIcon.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var indx = indexPath.Row;
                if (indx > -1 && indx < _rewardsList.Count)
                {
                    InvokeOnMainThread(() =>
                    {
                        if (cell.Tag == indx)
                        {
                            RewardsModel reward = _rewardsList[indx];
                            reward.IsSaved = !reward.IsSaved;
                            cell.SaveIcon.Image = UIImage.FromBundle(reward.IsSaved ? RewardsConstants.Img_HeartSaveIcon : RewardsConstants.Img_HeartUnsaveIcon);
                            if (_controller != null)
                            {
                                _controller.OnSaveUnsaveAction(reward);
                            }
                        }
                    });
                }
            }));
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (!_isLoading)
            {
                var index = indexPath.Row;
                if (index > -1 && index < _rewardsList.Count)
                {
                    if (_controller != null)
                    {
                        _rewardsList[index].IsRead = true;
                        //_controller.OnUpdateReadRewards(_rewardsList[index]);
                        _controller.OnRewardSelection(_rewardsList[index]);
                        //_controller.OnReloadTableAction(_rewardsList, tableView);
                        _controller.SetReloadProperties(tableView, index);
                    }
                }
            }
        }
    }
}