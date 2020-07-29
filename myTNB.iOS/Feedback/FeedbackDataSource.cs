using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using myTNB.Enums;
using myTNB.Model;
using myTNB.Model.Feedback;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class FeedbackDataSource : UITableViewSource
    {
        private readonly List<FeedbackRowModel> _feedbackList = new List<FeedbackRowModel>();
        private readonly List<SubmittedFeedbackDataModel> _submittedFeedbackList = new List<SubmittedFeedbackDataModel>();
        private readonly bool _isBcrmAvailable;
        private readonly FeedbackViewController _controller;

        public FeedbackDataSource(FeedbackViewController controller, List<SubmittedFeedbackDataModel> submittedFeedbackList, bool isBcrmAvailable)
        {
            if (DataManager.DataManager.SharedInstance.FeedbackCategory != null)
            {
                foreach (FeedbackCategoryDataModel f in DataManager.DataManager.SharedInstance.FeedbackCategory)
                {
                    FeedbackRowModel feedBackRowModel = new FeedbackRowModel
                    {
                        Name = f.FeedbackCategoryName,
                        ID = f.FeedbackCategoryId,
                        Subtitle = f.FeedbackCategoryDesc
                    };
                    switch (f.FeedbackCategoryId)
                    {
                        case "1":
                            feedBackRowModel.Icon = "IC-Tile-FeedbackBill";//Feedback-Generic
                            break;
                        case "2":
                            feedBackRowModel.Icon = "Feedback-Streetlamp";
                            break;
                        case "3":
                            feedBackRowModel.Icon = "Feedback-Others";
                            break;
                        case "10":
                            feedBackRowModel.Icon = "Feedback-Submitted";
                            break;
                        default:
                            feedBackRowModel.Icon = string.Empty;
                            break;
                    }
                    _feedbackList.Add(feedBackRowModel);
                }
            }
            _controller = controller;
            _submittedFeedbackList = submittedFeedbackList;
            _isBcrmAvailable = isBcrmAvailable;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _feedbackList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackRowModel feedBack = _feedbackList[indexPath.Row];
            FeedbackViewCell cell = tableView.DequeueReusableCell("FeedbackViewCell", indexPath) as FeedbackViewCell;

            cell.lblTitle.Text = feedBack.Name;
            cell.Subtitle = feedBack.Subtitle;
            cell.imgViewIcon.Image = UIImage.FromBundle(feedBack.Icon ?? string.Empty);
            if (indexPath.Section == 1)
            {
                if (_submittedFeedbackList != null)
                {
                    cell.lblCount.Hidden = false;
                    cell.lblCount.Text = _submittedFeedbackList.Count.ToString();
                }
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackRowModel feedback = _feedbackList[indexPath.Row];
            if (feedback.ID == "10")
            {
                _controller.DisplaySubmittedFeedback(feedback.Name);
            }
            else
            {
                if (!_isBcrmAvailable)
                {
                    ShowBRCMAlert();
                    return;
                }
                _controller.DisplayFeedbackEntry(feedback.ID);
            }
        }

        private void ShowBRCMAlert()
        {
            DowntimeDataModel status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
            _controller.DisplayServiceError(status?.DowntimeTextMessage ?? string.Empty);
        }
    }
}