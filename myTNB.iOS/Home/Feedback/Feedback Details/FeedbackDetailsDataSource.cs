using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackDetails
{
    public class FeedbackDetailsDataSource : UITableViewSource
    {
        FeedbackDetailsViewController _controller;
        SubmittedFeedbackDetailsDataModel _feedbackDetails;
        public FeedbackDetailsDataSource(FeedbackDetailsViewController controller
                                         , SubmittedFeedbackDetailsDataModel feedbackDetails)
        {
            _feedbackDetails = feedbackDetails;
            _controller = controller;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            int rowSection = 0;
            string feedbackCatId = _feedbackDetails.FeedbackCategoryId;
            switch (feedbackCatId)
            {
                case "1":
                case "3":
                    {
                        rowSection = 6;
                        break;
                    }
                case "2":
                    {
                        rowSection = 8;
                        break;
                    }
                default:
                    {
                        rowSection = 0;
                        break;
                    }
            }
            return rowSection;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row != 5 && indexPath.Row != 7)
            {
                var cell = tableView.DequeueReusableCell("feedbackDetailsCell", indexPath) as FeedbackDetailsViewCell;

                int rowIndex = indexPath.Row;
                switch (rowIndex)
                {
                    case 0:
                        {
                            cell.lblTitle.Text = "FEEDBACK ID";
                            cell.lblValue.Text = _feedbackDetails.ServiceReqNo;
                            break;
                        }
                    case 1:
                        {
                            cell.lblTitle.Text = "FEEDBACK STATUS";
                            cell.lblValue.Text = _feedbackDetails.StatusDesc;

                            string statusCode = _feedbackDetails.StatusCode;
                            switch (statusCode)
                            {
                                case "CL01":
                                    {
                                        //Created
                                        cell.lblValue.TextColor = myTNBColor.PowerBlue();
                                        break;
                                    }
                                case "CL02":
                                    {
                                        //In Progress
                                        cell.lblValue.TextColor = myTNBColor.SunGlow();
                                        break;
                                    }
                                case "CL03":
                                case "CL04":
                                    {
                                        //Completed
                                        cell.lblValue.TextColor = myTNBColor.FreshGreen();
                                        break;
                                    }
                                case "CL06":
                                    {
                                        //Cancelled
                                        cell.lblValue.TextColor = myTNBColor.Tomato();
                                        break;
                                    }
                            }

                            break;
                        }
                    case 2:
                        {
                            cell.lblTitle.Text = "FEEDBACK DATE & TIME";
                            cell.lblValue.Text = GetFormattedDate(_feedbackDetails.DateCreated);
                            break;
                        }
                    case 3:
                        {
                            cell.lblTitle.Text = "FEEDBACK DATE & TIME";
                            cell.lblValue.Text = GetFormattedDate(_feedbackDetails.DateCreated);

                            string feedbackCatId = _feedbackDetails.FeedbackCategoryId;
                            switch (feedbackCatId)
                            {
                                case "1":
                                    {
                                        cell.lblTitle.Text = "ACCOUNT NO.";
                                        cell.lblValue.Text = _feedbackDetails.AccountNum;
                                        break;
                                    }
                                case "2":
                                    {
                                        cell.lblTitle.Text = "STATE";
                                        cell.lblValue.Text = _feedbackDetails.StateName;
                                        break;
                                    }
                                case "3":
                                    {
                                        cell.lblTitle.Text = "FEEDBACK TYPE";
                                        cell.lblValue.Text = _feedbackDetails.FeedbackTypeName;
                                        break;
                                    }
                            }

                            break;
                        }
                    case 4:
                        {
                            string feedbackCatId = _feedbackDetails.FeedbackCategoryId;
                            switch (feedbackCatId)
                            {
                                case "1":
                                case "3":
                                    {
                                        cell.lblTitle.Text = "FEEDBACK";
                                        cell.lblValue.Text = _feedbackDetails.FeedbackMessage;
                                        CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
                                        cell.lblValue.Frame = new CGRect(18, 30, tableView.Frame.Width - 36, newSize.Height);
                                        break;
                                    }
                                default:
                                    {
                                        cell.lblTitle.Text = "LOCATION / STREET NAME";
                                        cell.lblValue.Text = _feedbackDetails.Location;
                                        break;
                                    }
                            }

                            break;
                        }
                    case 6:
                        {
                            cell.lblTitle.Text = "FEEDBACK";
                            cell.lblValue.Text = _feedbackDetails.FeedbackMessage;
                            CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
                            cell.lblValue.Frame = new CGRect(18, 30, tableView.Frame.Width - 36, newSize.Height);
                            break;
                        }
                }

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
            else if (indexPath.Row == 5)
            {
                if (_feedbackDetails.FeedbackCategoryId == "1" || _feedbackDetails.FeedbackCategoryId == "3")
                {
                    if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
                    {
                        var cell = tableView.DequeueReusableCell("feedbackDetailsImageCell", indexPath) as FeedbackDetailsViewImageCell;
                        cell.lblTitle.Text = "PHOTO / SCREENSHOT";
                        int x = 0;
                        UIImageHelper imgHelper = new UIImageHelper();
                        foreach (var item in _feedbackDetails.FeedbackImage)
                        {
                            UIView viewContainer = new UIView(new CGRect(x, 0, 94, 94));
                            UIImageView imgView = new UIImageView(new CGRect(0, 0, 94, 94));
                            imgView.Image = imgHelper.ConvertHexToUIImage(item.imageHex);
                            viewContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                            {
                                _controller.OnImageClick(imgView.Image, item.fileName);
                            }));
                            viewContainer.AddSubview(imgView);
                            cell.imgScrollView.AddSubview(viewContainer);
                            x += 94 + 7;
                        }
                        cell.imgScrollView.ContentSize = new CGSize(x, 94);
                        cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                        return cell;
                    }
                }
                else
                {
                    var cell = tableView.DequeueReusableCell("feedbackDetailsCell", indexPath) as FeedbackDetailsViewCell;
                    cell.lblTitle.Text = "POLE NO.";
                    cell.lblValue.Text = _feedbackDetails.PoleNum;
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    return cell;
                }
            }
            else if (indexPath.Row == 7)
            {
                if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
                {
                    var cell = tableView.DequeueReusableCell("feedbackDetailsImageCell", indexPath) as FeedbackDetailsViewImageCell;
                    cell.lblTitle.Text = "PHOTO / SCREENSHOT";
                    int x = 0;
                    UIImageHelper imgHelper = new UIImageHelper();
                    foreach (var item in _feedbackDetails.FeedbackImage)
                    {
                        UIView viewContainer = new UIView(new CGRect(x, 0, 94, 94));
                        UIImageView imgView = new UIImageView(new CGRect(0, 0, 94, 94));
                        imgView.Image = imgHelper.ConvertHexToUIImage(item.imageHex);
                        viewContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                        {
                            _controller.OnImageClick(imgView.Image, item.fileName);
                        }));
                        viewContainer.AddSubview(imgView);
                        cell.imgScrollView.AddSubview(viewContainer);
                        x += 94 + 7;

                    }
                    cell.imgScrollView.ContentSize = new CGSize(x, 94);
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    return cell;
                }
            }
            return new UITableViewCell();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat rowHeight = 50f;

            int rowIndex = indexPath.Row;
            switch (rowIndex)
            {
                case 4:
                    {
                        if (_feedbackDetails.FeedbackCategoryId == "2")
                        {
                            rowHeight = 50f;
                        }
                        else
                        {
                            CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
                            rowHeight = newSize.Height + 32f;
                        }
                        break;
                    }
                case 5:
                    {
                        if (_feedbackDetails.FeedbackCategoryId == "2")
                        {
                            rowHeight = 50f;
                        }
                        else
                        {
                            rowHeight = 130f;
                        }
                        break;
                    }
                case 6:
                    {
                        CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
                        rowHeight = newSize.Height + 32f;
                        break;
                    }
                case 7:
                    {
                        rowHeight = 136f;
                        break;
                    }
                default:
                    {
                        rowHeight = 50f;
                        break;
                    }
            }

            return rowHeight;
        }

        /// <summary>
        /// Gets the size of the title label.
        /// </summary>
        /// <returns>The title label size.</returns>
        /// <param name="text">Text.</param>
        private CGSize GetTitleLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(18, 30, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000))
            {
                Font = myTNBFont.MuseoSans14(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = text
            };
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

        internal string GetFormattedDate(string DateCreated)
        {
            try
            {
                string date = DateHelper.GetFormattedDate(DateCreated.Split(' ')[0], "dd MMM yyyy");
                string time = DateCreated.Split(' ')[1];
                int hr = int.Parse(time.Split(':')[0]);
                int min = int.Parse(time.Split(':')[1]);
                int sec = int.Parse(time.Split(':')[2]);

                TimeSpan timespan = new TimeSpan(hr, min, sec);
                DateTime dt = DateTime.Today.Add(timespan);
                string displayTime = dt.ToString("hh:mm tt");
                string formattedDate = date + " " + displayTime;
                return formattedDate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}