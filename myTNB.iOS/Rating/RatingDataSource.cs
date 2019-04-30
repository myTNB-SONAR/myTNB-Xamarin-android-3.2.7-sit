using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class RatingDataSource : UITableViewSource
    {
        List<FeedbackQuestionModel> questions;
        int defaultRating;

        string CellIdentifier = "FeedbackInputCell";

        public RatingDataSource(List<FeedbackQuestionModel> inputQuestions, int defRating)
        {
            questions = inputQuestions != null ? inputQuestions : new List<FeedbackQuestionModel>();
            defaultRating = defRating;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) as FeedbackInputCell;
            var question = questions[indexPath.Row];
            if (cell == null)
            {
                cell = new FeedbackInputCell(CellIdentifier);
            }
            cell.UpdateCell(question, indexPath.Row, defaultRating, tableView.Frame.Width);
            return cell;
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <returns>The in section.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">Section.</param>
        public override nint RowsInSection(UITableView tableView, nint section)
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
            var cell = tableView.CellAt(indexPath) as FeedbackInputCell;

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
            var cell = tableView.CellAt(indexPath) as FeedbackInputCell;

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
            var question = questions[indexPath.Row];

            if (question.Kind == Enums.QuestionTypeEnum.Rating)
            {
                return 140f;
            }
            return 220f;
        }
    }
}
