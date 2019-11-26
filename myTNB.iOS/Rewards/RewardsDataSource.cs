using System;
using System.Collections.Generic;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsDataSource : UITableViewSource
    {
        private List<RewardsChildModel> _rewardsList = new List<RewardsChildModel>();
        private readonly Action<RewardsChildModel> OnRowSelected;
        private readonly bool _isLoading;
        public Func<string, string> GetI18NValue;

        public RewardsDataSource(List<RewardsChildModel> rewardsList,
            Func<string, string> getI18NValue,
            Action<RewardsChildModel> onRowSelected,
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
            return RewardsConstants.RewardsCellHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RewardsCell cell = tableView.DequeueReusableCell(RewardsConstants.Cell_Rewards) as RewardsCell;
            var index = indexPath.Row;
            if (index > -1 && index < _rewardsList.Count)
            {
                cell.GetI18NValue = GetI18NValue;
                cell.SetAccountCell(_rewardsList[index]);
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
                    var acct = _rewardsList[index];
                    OnRowSelected?.Invoke(acct);
                }
            }
        }
    }
}
