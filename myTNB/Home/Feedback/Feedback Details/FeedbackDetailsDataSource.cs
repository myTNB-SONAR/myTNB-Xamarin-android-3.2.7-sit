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
            if (_feedbackDetails.FeedbackCategoryId == "1")
            {
                return 6;
            }
            else if (_feedbackDetails.FeedbackCategoryId == "2")
            {
                return 8;
            }
            else if (_feedbackDetails.FeedbackCategoryId == "3")
            {
                return 6;
            }
            else
            {
                return 0;
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("feedbackDetailsViewCell", indexPath);
            UILabel lblTitle = new UILabel(new CGRect(18, 16, tableView.Frame.Width - 36, 14));
            lblTitle.Font = myTNBFont.MuseoSans9();
            lblTitle.TextColor = myTNBColor.SilverChalice();

            UILabel lblValue = new UILabel(new CGRect(18, 30, tableView.Frame.Width - 36, 18));
            lblValue.Font = myTNBFont.MuseoSans14();
            lblValue.TextColor = myTNBColor.TunaGrey();
            lblValue.Lines = 0;
            lblValue.LineBreakMode = UILineBreakMode.WordWrap;

            if (indexPath.Row == 0)
            {
                lblTitle.Text = "FEEDBACK ID";
                lblValue.Text = _feedbackDetails.ServiceReqNo;
            }
            else if (indexPath.Row == 1)
            {
                lblTitle.Text = "FEEDBACK STATUS";
                lblValue.Text = _feedbackDetails.StatusDesc;


                if (_feedbackDetails.StatusCode == "CL01")
                {
                    //Created
                    lblValue.TextColor = myTNBColor.PowerBlue();
                }
                    else if (_feedbackDetails.StatusCode == "CL02")
                {
                    //In Progress
                    lblValue.TextColor = myTNBColor.SunGlow();
                }
                    else if (_feedbackDetails.StatusCode == "CL03")
                {
                    //Completed
                    lblValue.TextColor = myTNBColor.FreshGreen();

                }
                    else if (_feedbackDetails.StatusCode == "CL04")
                {
                    //Completed
                    lblValue.TextColor = myTNBColor.FreshGreen();

                }
                    else if (_feedbackDetails.StatusCode == "CL05")
                {
                    
                } 
                    else if (_feedbackDetails.StatusCode == "CL06")
                {
                    //Cancelled
                    lblValue.TextColor = myTNBColor.Tomato();
                }

            }
            else if (indexPath.Row == 2)
            {
                lblTitle.Text = "FEEDBACK DATE & TIME";
                lblValue.Text = GetFormattedDate(_feedbackDetails.DateCreated);
            }
            else if (indexPath.Row == 3)
            {
                if (_feedbackDetails.FeedbackCategoryId == "1")
                {
                    lblTitle.Text = "ACCOUNT NO.";
                    lblValue.Text = _feedbackDetails.AccountNum;
                }
                else if (_feedbackDetails.FeedbackCategoryId == "2")
                {
                    lblTitle.Text = "STATE";
                    lblValue.Text = _feedbackDetails.StateName;
                }
                else if (_feedbackDetails.FeedbackCategoryId == "3")
                {
                    lblTitle.Text = "FEEDBACK TYPE";
                    lblValue.Text = _feedbackDetails.FeedbackTypeName;
                }
            }
            else if (indexPath.Row == 4)
            {
                if (_feedbackDetails.FeedbackCategoryId == "1" || _feedbackDetails.FeedbackCategoryId == "3")
                {
                    lblTitle.Text = "FEEDBACK";
                    lblValue.Text = _feedbackDetails.FeedbackMessage;
                    CGSize newSize = GetLabelSize(_feedbackDetails.FeedbackMessage);
                    lblValue.Frame = new CGRect(18, 30, tableView.Frame.Width - 36, newSize.Height);
                }
                else
                {
                    lblTitle.Text = "LOCATION / STREET NAME";
                    lblValue.Text = _feedbackDetails.Location;
                }
            }
            else if (indexPath.Row == 5)
            {
                if (_feedbackDetails.FeedbackCategoryId == "1" || _feedbackDetails.FeedbackCategoryId == "3")
                {
                    if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
                    {
                        lblTitle.Text = "PHOTO / SCREENSHOT";
                        UIScrollView imgScrollView = new UIScrollView(new CGRect(18, 31, tableView.Frame.Width - 36, 94));
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
                            imgScrollView.AddSubview(viewContainer);
                            x += 94 + 7;
                        }
                        imgScrollView.ContentSize = new CGSize(x, 94);
                        cell.AddSubview(imgScrollView);
                    }
                }
                else
                {
                    lblTitle.Text = "POLE NO.";
                    lblValue.Text = _feedbackDetails.PoleNum;
                }
            }
            else if (indexPath.Row == 6)
            {
                lblTitle.Text = "FEEDBACK";
                lblValue.Text = _feedbackDetails.FeedbackMessage;
                CGSize newSize = GetLabelSize(_feedbackDetails.FeedbackMessage);
                lblValue.Frame = new CGRect(18, 30, tableView.Frame.Width - 36, newSize.Height);
            }
            else if (indexPath.Row == 7)
            {
                if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
                {
                    lblTitle.Text = "PHOTO / SCREENSHOT";
                    UIScrollView imgScrollView = new UIScrollView(new CGRect(18, 31, tableView.Frame.Width - 36, 94));
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
                        imgScrollView.AddSubview(viewContainer);
                        x += 94 + 7;

                    }
                    imgScrollView.ContentSize = new CGSize(x, 94);
                    cell.AddSubview(imgScrollView);
                }
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.AddSubviews(new UIView[] { lblTitle, lblValue });
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                return 50;
            }
            else if (indexPath.Row == 1)
            {
                return 50;
            }
            else if (indexPath.Row == 2)
            {
                return 50;
            }
            else if (indexPath.Row == 3)
            {
                return 50;
            }
            else if (indexPath.Row == 4)
            {
                if (_feedbackDetails.FeedbackCategoryId == "2")
                {
                    return 50;
                }
                else
                {
                    CGSize newSize = GetLabelSize(_feedbackDetails.FeedbackMessage);
                    return newSize.Height + 32;
                }
            }
            else if (indexPath.Row == 5)
            {
                if (_feedbackDetails.FeedbackCategoryId == "2")
                {
                    return 50;
                }
                else
                {
                    return 130;
                }
            }
            else if (indexPath.Row == 6)
            {
                CGSize newSize = GetLabelSize(_feedbackDetails.FeedbackMessage);
                return newSize.Height + 32;
            }
            else if (indexPath.Row == 6)
            {
                return 130;
            }
            return 50;
        }

        CGSize GetLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(18, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000));
            label.Font = myTNBFont.MuseoSans14();
            label.TextColor = myTNBColor.TunaGrey();
            label.Lines = 0;
            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.Text = text;
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