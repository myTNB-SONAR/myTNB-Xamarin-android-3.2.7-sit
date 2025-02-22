﻿using System;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackDetails
{
    public class FeedbackDetailsDataSource : UITableViewSource
    {
        private FeedbackDetailsViewController _controller;
        private SubmittedFeedbackDetailsDataModel _feedbackDetails;
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
            string feedbackCatId = _feedbackDetails.FeedbackCategoryId;
            int rowSection;
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
                FeedbackDetailsViewCell cell = tableView.DequeueReusableCell("feedbackDetailsCell", indexPath) as FeedbackDetailsViewCell;

                int rowIndex = indexPath.Row;
                switch (rowIndex)
                {
                    case 0:
                        {
                            cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_FeedbackID).ToUpper();
                            cell.lblValue.Text = _feedbackDetails.ServiceReqNo;
                            break;
                        }
                    case 1:
                        {
                            cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_FeedbackStatus).ToUpper();
                            cell.lblValue.Text = _feedbackDetails.StatusDesc;

                            string statusCode = _feedbackDetails.StatusCode;
                            switch (statusCode)
                            {
                                case "CL01":
                                    {
                                        //Created
                                        cell.lblValue.TextColor = MyTNBColor.PowerBlue;
                                        break;
                                    }
                                case "CL02":
                                    {
                                        //In Progress
                                        cell.lblValue.TextColor = MyTNBColor.SunGlow;
                                        break;
                                    }
                                case "CL03":
                                case "CL04":
                                    {
                                        //Completed
                                        cell.lblValue.TextColor = MyTNBColor.FreshGreen;
                                        break;
                                    }
                                case "CL06":
                                    {
                                        //Cancelled
                                        cell.lblValue.TextColor = MyTNBColor.Tomato;
                                        break;
                                    }
                            }

                            break;
                        }
                    case 2:
                        {
                            cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_DateTimeTitle).ToUpper();
                            cell.lblValue.Text = GetFormattedDate(_feedbackDetails.DateCreated);
                            break;
                        }
                    case 3:
                        {
                            cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_DateTimeTitle).ToUpper();
                            cell.lblValue.Text = GetFormattedDate(_feedbackDetails.DateCreated);

                            string feedbackCatId = _feedbackDetails.FeedbackCategoryId;
                            switch (feedbackCatId)
                            {
                                case "1":
                                    {
                                        cell.lblTitle.Text = GetCommonI18NValue(Constants.Common_AccountNo).ToUpper();
                                        cell.lblValue.Text = _feedbackDetails.AccountNum;
                                        break;
                                    }
                                case "2":
                                    {
                                        cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_State).ToUpper();
                                        cell.lblValue.Text = _feedbackDetails.StateName;
                                        break;
                                    }
                                case "3":
                                    {
                                        cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_FeedbackType).ToUpper();
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
                                        cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_Feedback).ToUpper();
                                        cell.lblValue.Text = _feedbackDetails.FeedbackMessage;
                                        CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
                                        cell.lblValue.Frame = new CGRect(18, 30, tableView.Frame.Width - 36, newSize.Height);
                                        break;
                                    }
                                default:
                                    {
                                        cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_Location).ToUpper();
                                        cell.lblValue.Text = _feedbackDetails.Location;
                                        break;
                                    }
                            }

                            break;
                        }
                    case 6:
                        {
                            cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_Title).ToUpper();
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
                        FeedbackDetailsViewImageCell cell = tableView.DequeueReusableCell("feedbackDetailsImageCell", indexPath) as FeedbackDetailsViewImageCell;
                        cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_PhotoTitle).ToUpper();
                        int x = 0;
                        UIImageHelper imgHelper = new UIImageHelper();
                        foreach (FeedbackImageModel item in _feedbackDetails.FeedbackImage)
                        {
                            UIView viewContainer = new UIView(new CGRect(x, 0, 94, 94));
                            UIImageView imgView = new UIImageView(new CGRect(0, 0, 94, 94))
                            {
                                Image = imgHelper.ConvertHexToUIImage(item.imageHex)
                            };
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
                    FeedbackDetailsViewCell cell = tableView.DequeueReusableCell("feedbackDetailsCell", indexPath) as FeedbackDetailsViewCell;
                    cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_PoleNumber).ToUpper();
                    cell.lblValue.Text = _feedbackDetails.PoleNum;
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    return cell;
                }
            }
            else if (indexPath.Row == 7)
            {
                if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
                {
                    FeedbackDetailsViewImageCell cell = tableView.DequeueReusableCell("feedbackDetailsImageCell", indexPath) as FeedbackDetailsViewImageCell;
                    cell.lblTitle.Text = GetI18NValue(FeedbackConstants.I18N_PhotoTitle).ToUpper();
                    int x = 0;
                    UIImageHelper imgHelper = new UIImageHelper();
                    foreach (FeedbackImageModel item in _feedbackDetails.FeedbackImage)
                    {
                        UIView viewContainer = new UIView(new CGRect(x, 0, 94, 94));
                        UIImageView imgView = new UIImageView(new CGRect(0, 0, 94, 94))
                        {
                            Image = imgHelper.ConvertHexToUIImage(item.imageHex)
                        };
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
            nfloat rowHeight;
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
                        rowHeight = _feedbackDetails.FeedbackCategoryId == "2" ? 50f : 130f;
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
                Font = MyTNBFont.MuseoSans14,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = text
            };
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

        private string GetFormattedDate(string DateCreated)
        {
            if (string.IsNullOrEmpty(DateCreated) || string.IsNullOrWhiteSpace(DateCreated))
            {
                return TNBGlobal.EMPTY_DATE;
            }
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
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }

        private string GetI18NValue(string key)
        {
            return _controller.GetI18NValue(key);
        }

        private string GetCommonI18NValue(string key)
        {
            return _controller.GetCommonI18NValue(key);
        }
    }
}