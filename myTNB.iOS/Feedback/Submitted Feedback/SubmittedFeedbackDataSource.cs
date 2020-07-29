using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class SubmittedFeedbackDataSource : UITableViewSource
    {
        private SubmittedFeedbackViewController _controller;
        private List<SubmittedFeedbackDataModel> _submittedFeedbacks = new List<SubmittedFeedbackDataModel>();

        public SubmittedFeedbackDataSource(SubmittedFeedbackViewController controller, List<SubmittedFeedbackDataModel> submittedFeedbacks)
        {
            _controller = controller;
            _submittedFeedbacks = submittedFeedbacks;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SubmittedFeedbackCell cell = tableView.DequeueReusableCell("SubmittedFeedbackCell", indexPath) as SubmittedFeedbackCell;
            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            cell.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            cell.UpdateStyle();
            if (_submittedFeedbacks != null && _submittedFeedbacks.Count != 0)
            {
                SubmittedFeedbackDataModel feedback = _submittedFeedbacks[indexPath.Row];
                cell.FeedbackTypeLabel.Text = feedback.FeedbackCategoryName;
                cell.FeedbackDateLabel.Text = GetFormattedDate(feedback.DateCreated);
                cell.FeedbackDetailsLabel.Text = feedback.FeedbackMessage;

                if (feedback.FeedbackCategoryId == "1")
                {
                    cell.imgViewIcon.Image = UIImage.FromBundle("IC-Tile-FeedbackBill");//"Feedback-Submitted-Generic"
                }
                else if (feedback.FeedbackCategoryId == "2")
                {
                    cell.imgViewIcon.Image = UIImage.FromBundle("Feedback-Submitted-Streetlamp");
                }
                else if (feedback.FeedbackCategoryId == "3")
                {
                    cell.imgViewIcon.Image = UIImage.FromBundle("Feedback-Submitted-Others");
                }
                else if (feedback.FeedbackCategoryId == "4")
                {
                    cell.imgViewIcon.Image = UIImage.FromBundle("Update-PersonalDetails");
                }
            }

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _submittedFeedbacks != null && _submittedFeedbacks.Count != 0 ? _submittedFeedbacks.Count : 0;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 65f;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _controller.ExecuteGetSubmittedFeedbackDetailsCall(_submittedFeedbacks[indexPath.Row]);
        }

        internal string GetFormattedDate(string DateCreated)
        {
            try
            {
                return DateHelper.GetFormattedDate(DateCreated.Split(' ')[0], "dd MMM yyyy");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}