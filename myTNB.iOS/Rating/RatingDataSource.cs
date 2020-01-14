using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class RatingDataSource : UITableViewSource
    {
        private List<FeedbackQuestionModel> questions;
        private int defaultRating;
        private RatingViewController _controller;

        private string CellIdentifier = "FeedbackInputCell";

        public RatingDataSource(List<FeedbackQuestionModel> inputQuestions, int defRating, RatingViewController controller)
        {
            questions = inputQuestions != null ? inputQuestions : new List<FeedbackQuestionModel>();
            defaultRating = defRating;
            _controller = controller;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackInputCell cell = tableView.DequeueReusableCell(CellIdentifier) as FeedbackInputCell;
            FeedbackQuestionModel question = questions[indexPath.Row];
            if (cell == null)
            {
                cell = new FeedbackInputCell(CellIdentifier);
            }
            int defRating = indexPath.Row == 0 ? defaultRating : 0;
            cell.UpdateCell(question, indexPath.Row, defRating, tableView.Frame.Width);
            return cell;
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <returns>The in section.</returns>
        /// <param name="tableview">Table view.</param>
        /// <param name="section">Section.</param>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return questions?.Count ?? 0;
        }
        /// <summary>
        /// Rows the selected.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackInputCell cell = tableView.CellAt(indexPath) as FeedbackInputCell;
            if (cell != null)
            {
                if (cell.QuestionType == Enums.QuestionTypeEnum.MultilineComment)
                {
                    cell.feedbackTextView.BecomeFirstResponder();
                }
            }
        }

        /// <summary>
        /// Rows the deselected.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackInputCell cell = tableView.CellAt(indexPath) as FeedbackInputCell;
            if (cell != null)
            {
                if (cell.QuestionType == Enums.QuestionTypeEnum.MultilineComment)
                {
                    questions[indexPath.Row].Answer = cell.feedbackTextView.Text;
                    cell.feedbackTextView.ResignFirstResponder();
                }
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            FeedbackQuestionModel question = questions[indexPath.Row];
            if (question.Kind == Enums.QuestionTypeEnum.Rating)
            {
                UILabel lbl = new UILabel(new CGRect(0, 0, _controller.ViewWidth, 16F))
                {
                    Font = MyTNBFont.MuseoSans18_500,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    Text = question.Question
                };
                CGSize lblSize = lbl.SizeThatFits(new CGSize(lbl.Frame.Width, 1000F));
                return 64F + 32F + 16F + lblSize.Height + 10F;
            }
            return 220f;
        }
    }
}
