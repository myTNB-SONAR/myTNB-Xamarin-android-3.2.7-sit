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
        readonly List<FeedbackRowModel> _feedbackList = new List<FeedbackRowModel>();
        readonly List<SubmittedFeedbackDataModel> _submittedFeedbackList = new List<SubmittedFeedbackDataModel>();
        readonly bool _isBcrmAvailable;
        readonly FeedbackViewController _controller;

        public FeedbackDataSource(FeedbackViewController controller, List<SubmittedFeedbackDataModel> submittedFeedbackList, bool isFromPrelogin, bool isBcrmAvailable)
        {
            if (DataManager.DataManager.SharedInstance.FeedbackCategory != null)
            {
                foreach (FeedbackCategoryDataModel f in DataManager.DataManager.SharedInstance.FeedbackCategory)
                {
                    Debug.WriteLine("FeedbackCategoryId: " + f.FeedbackCategoryId);
                    Debug.WriteLine("FeedbackCategoryName: " + f.FeedbackCategoryName);

                    FeedbackRowModel feedBackRowModel = new FeedbackRowModel
                    {
                        Name = f.FeedbackCategoryName,
                        ID = f.FeedbackCategoryId,
                        Subtitle = f.FeedbackCategoryDesc
                    };
                    switch (f.FeedbackCategoryId)
                    {
                        case "1":
                            feedBackRowModel.Icon = "Feedback-Generic";
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
            var feedBack = _feedbackList[indexPath.Row];
            var cell = tableView.DequeueReusableCell("FeedbackViewCell", indexPath) as FeedbackViewCell;

            cell.lblTitle.Text = feedBack.Name;
            cell.lblSubtTitle.Text = feedBack.Subtitle;
            cell.imgViewIcon.Image = UIImage.FromBundle(feedBack.Icon ?? string.Empty);
            if (indexPath.Section == 1)
            {
                if (_submittedFeedbackList != null)
                {
                    cell.lblCount.Hidden = false;
                    cell.lblCount.Text = _submittedFeedbackList.Count.ToString();
                }
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var feedback = _feedbackList[indexPath.Row];
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

        internal void ShowBRCMAlert()
        {
            var status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
            AlertHandler.DisplayServiceError(_controller, status?.DowntimeTextMessage);
        }
    }
}