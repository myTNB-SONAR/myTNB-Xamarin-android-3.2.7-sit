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
        private List<RewardsModel> _rewardsList = new List<RewardsModel>();
        private readonly Action<RewardsModel> OnRowSelected;
        private readonly bool _isLoading;
        public Func<string, string> GetI18NValue;

        public RewardsDataSource(List<RewardsModel> rewardsList,
            Func<string, string> getI18NValue,
            Action<RewardsModel> onRowSelected,
            bool isLoading = false)
        {
            _rewardsList = rewardsList;
            GetI18NValue = getI18NValue;
            OnRowSelected = onRowSelected;
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
            var index = indexPath.Row;
            if (index > -1 && index < _rewardsList.Count)
            {
                RewardsModel reward = _rewardsList[index];
                cell.CellIndex = index;
                cell.GetI18NValue = GetI18NValue;
                cell.SetAccountCell(reward);
                if (reward.Image.IsValid())
                {
                    try
                    {
                        ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(cell.ViewImage);
                        _activityIndicator.Show();
                        NSUrl url = new NSUrl(reward.Image);
                        NSUrlSession session = NSUrlSession
                            .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                        NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                        {
                            if (error == null && response != null && data != null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    if (cell.CellIndex == indexPath.Row)
                                    {
                                        cell.RewardImageVIew.Image = UIImage.LoadFromData(data);
                                        _activityIndicator.Hide();
                                    }
                                });
                            }
                            else
                            {
                                // Default image goes here...
                                //InvokeOnMainThread(() =>
                                //{
                                //    if (cell.CellIndex == indexPath.Row)
                                //    {
                                //        cell.RewardImageVIew.Image = UIImage.LoadFromData(data);
                                //        _activityIndicator.Hide();
                                //    }
                                //});
                            }
                        });
                        dataTask.Resume();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                        // Default image goes here...
                    }
                }
                else
                {
                    // Default image goes here...
                }
            }
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
                    RewardsModel reward = _rewardsList[index];
                    OnRowSelected?.Invoke(reward);
                }
            }
        }
    }
}
