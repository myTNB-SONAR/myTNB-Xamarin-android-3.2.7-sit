using System; using System.Collections.Generic; using System.Diagnostics;
using Foundation; using myTNB.Enums;
using myTNB.Model;
using myTNB.Model.Feedback; using UIKit;  namespace myTNB.Home.Feedback {     public class FeedbackDataSource : UITableViewSource     {         readonly List<FeedbackRowModel> _feedbacks = new List<FeedbackRowModel>();         readonly List<SubmittedFeedbackDataModel> _submittedFeedbackList = new List<SubmittedFeedbackDataModel>();         readonly bool _isFromPrelogin;         readonly bool _isBcrmAvailable;         readonly FeedbackViewController _controller;          public FeedbackDataSource(FeedbackViewController controller, List<SubmittedFeedbackDataModel> submittedFeedbackList             , bool isFromPrelogin, bool isBcrmAvailable)         {             if (DataManager.DataManager.SharedInstance.FeedbackCategory != null)
            {                 foreach (var f in DataManager.DataManager.SharedInstance.FeedbackCategory)                 {                     Debug.WriteLine("FeedbackCategoryId: " + f.FeedbackCategoryId);                     Debug.WriteLine("FeedbackCategoryName: " + f.FeedbackCategoryName);                      FeedbackRowModel feedBackRowModel = new FeedbackRowModel
                    {
                        Name = f.FeedbackCategoryName,
                        ID = f.FeedbackCategoryId
                    };                      if (f.FeedbackCategoryId == "1")                     {                         feedBackRowModel.Icon = "Feedback-Bill";                         feedBackRowModel.Subtitle = "Feedback_BillSubTitle".Translate();                     }                     else if (f.FeedbackCategoryId == "2")                     {                         feedBackRowModel.Icon = "Feedback-Streetlamp";                         feedBackRowModel.Subtitle = "Feedback_StreetLampSubTitle".Translate();                     }                     else if (f.FeedbackCategoryId == "3")                     {                         feedBackRowModel.Icon = "Feedback-Others";                         feedBackRowModel.Subtitle = "Feedback_OthersSubtitle".Translate();                     }                     _feedbacks.Add(feedBackRowModel);                 }             }              FeedbackRowModel submittedfeedBackRowModel = new FeedbackRowModel
            {
                Name = "Feedback_SubmittedFeedbackTitle".Translate(),
                ID = "4",
                Icon = "Feedback-Submitted",
                Subtitle = "Feedback_SubmittedFeedbackSubTitle".Translate()
            };             _feedbacks.Add(submittedfeedBackRowModel);              _controller = controller;             _submittedFeedbackList = submittedFeedbackList;             _isFromPrelogin = isFromPrelogin;             _isBcrmAvailable = isBcrmAvailable;         }          public override nint NumberOfSections(UITableView tableView)         {             return 1;         }          public override nint RowsInSection(UITableView tableview, nint section)         {
            return _feedbacks.Count;         }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)         {
            var feedBack = _feedbacks[indexPath.Row];             var cell = tableView.DequeueReusableCell("FeedbackViewCell", indexPath) as FeedbackViewCell;              cell.lblTitle.Text = feedBack.Name;             cell.lblSubtTitle.Text = feedBack.Subtitle;             cell.imgViewIcon.Image = UIImage.FromBundle(feedBack.Icon);             if (indexPath.Section == 1)
            {                 if (_submittedFeedbackList != null)
                {                     cell.lblCount.Hidden = false;                     cell.lblCount.Text = _submittedFeedbackList.Count.ToString();                 }             }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;              return cell;         }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)         {             var feedback = _feedbacks[indexPath.Row];             if (feedback.ID == "4")
            {
                _controller.DisplaySubmittedFeedback();
            }
            else
            {                 if (!_isBcrmAvailable)
                {
                    ShowBRCMAlert();                     return;
                }
                UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                FeedbackEntryViewController feedbackEntryViewController =
                    storyBoard.InstantiateViewController("FeedbackEntryViewController") as FeedbackEntryViewController;                 feedbackEntryViewController.FeedbackID = feedback.ID;                 feedbackEntryViewController.IsLoggedIn = !_isFromPrelogin;
                var navController = new UINavigationController(feedbackEntryViewController);
                _controller.PresentViewController(navController, true, null);
            }
        }
         internal void ShowBRCMAlert()         {             var status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);             //Todo: Confirm alert will not show if msg is empty?
            ToastHelper.DisplayAlertView(_controller, "Error_DefaultTitle".Translate(), status?.DowntimeTextMessage ?? "Error_DefaultMessage".Translate());         }     } }