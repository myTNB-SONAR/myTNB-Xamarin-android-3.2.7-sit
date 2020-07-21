using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class SavedRewardsDataSource : UITableViewSource
    {
        private readonly SavedRewardsViewController _controller;
        private List<RewardsModel> _savedRewardsList = new List<RewardsModel>();
        public Func<string, string> GetI18NValue;

        public SavedRewardsDataSource(SavedRewardsViewController controller,
            List<RewardsModel> savedRewardsList,
            Func<string, string> getI18NValue)
        {
            _controller = controller;
            _savedRewardsList = savedRewardsList.OrderByDescending(x => DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss"
                   , CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();
            GetI18NValue = getI18NValue;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _savedRewardsList.Count;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat addtl = 0;
            if (indexPath.Row == _savedRewardsList.Count - 1)
            {
                addtl = ScaleUtility.GetScaledHeight(17F);
            }
            return ScaleUtility.GetScaledHeight(177F) + addtl;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SavedRewardCell cell = tableView.DequeueReusableCell(RewardsConstants.Cell_SavedRewards) as SavedRewardCell;
            cell.Tag = indexPath.Row;
            if (cell.Tag > -1 && cell.Tag < _savedRewardsList.Count)
            {
                RewardsModel reward = _savedRewardsList[(int)cell.Tag];
                cell.CellIndex = (int)cell.Tag;
                cell.GetI18NValue = GetI18NValue;
                cell.SetAccountCell(reward);
                if (reward.Image.IsValid())
                {
                    if (cell.Tag == indexPath.Row)
                    {
                        try
                        {
                            NSData imgData = RewardsCache.GetImage(reward.ID);
                            if (imgData != null)
                            {
                                cell.RewardImageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                using (var image = UIImage.LoadFromData(imgData))
                                {
                                    cell.RewardImageView.Image = image;
                                }
                                if (reward.IsUsed)
                                {
                                    cell.RewardImageView.Image = RewardsServices.ConvertToGrayScale(cell.RewardImageView.Image);
                                }
                                cell.UsedView.Hidden = !reward.IsUsed;
                            }
                            else
                            {
                                cell.SetLoadingImageView();
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
                                                cell.RewardImageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                                using (var image = UIImage.LoadFromData(data))
                                                {
                                                    cell.RewardImageView.Image = image;
                                                }
                                                if (reward.IsUsed)
                                                {
                                                    cell.RewardImageView.Image = RewardsServices.ConvertToGrayScale(cell.RewardImageView.Image);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cell.RewardImageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                                            if (reward.IsUsed)
                                            {
                                                cell.RewardImageView.Image = RewardsServices.ConvertToGrayScale(cell.RewardImageView.Image);
                                            }
                                        }
                                        cell.ShowDowloadedImage(reward);
                                    });
                                });
                                dataTask.Resume();
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            cell.RewardImageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                            if (reward.IsUsed)
                            {
                                cell.RewardImageView.Image = RewardsServices.ConvertToGrayScale(cell.RewardImageView.Image);
                            }
                        }
                    }
                }
                else
                {
                    cell.RewardImageView.Image = UIImage.FromBundle(RewardsConstants.Img_RewardDefaultBanner);
                    if (reward.IsUsed)
                    {
                        cell.RewardImageView.Image = RewardsServices.ConvertToGrayScale(cell.RewardImageView.Image);
                    }
                }
                cell.Title.TextColor = reward.IsUsed ? MyTNBColor.GreyishBrown : MyTNBColor.WaterBlue;
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var index = indexPath.Row;
            if (index > -1 && index < _savedRewardsList.Count)
            {
                if (_controller != null)
                {
                    _controller.OnRewardSelection(_savedRewardsList[index]);
                }
            }
        }
    }
}
