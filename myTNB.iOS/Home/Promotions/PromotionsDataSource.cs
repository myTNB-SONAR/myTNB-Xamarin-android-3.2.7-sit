using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB.Home.Promotions
{
    public class PromotionsDataSource : UITableViewSource
    {
        PromotionsViewController _controller;
        List<PromotionsModel> _promotionList = new List<PromotionsModel>();
        public PromotionsDataSource(PromotionsViewController controller, List<PromotionsModel> promotionList)
        {
            _controller = controller;
            _promotionList = promotionList;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _promotionList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var promotion = _promotionList[indexPath.Row];
            var cell = tableView.DequeueReusableCell("PromotionsViewCell", indexPath) as PromotionsViewCell;

            if (promotion.LandscapeImage != null)
            {
                ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(cell.viewBanner);
                _activityIndicator.Show();
                cell.imgBanner.Image = UIImage.FromBundle(string.Empty);
                NSUrl url = new NSUrl(promotion.LandscapeImage);
                NSUrlSession session = NSUrlSession
                    .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (error == null && response != null && data != null)
                        {
                            cell.imgBanner.Image = UIImage.LoadFromData(data);
                        }
                        else
                        {
                            cell.imgBanner.Image = UIImage.FromBundle(PromotionConstants.Img_DefaultImage);
                        }
                        _activityIndicator.Hide();
                    });
                });
                dataTask.Resume();
            }
            else
            {
                cell.imgBanner.Image = UIImage.FromBundle(PromotionConstants.Img_DefaultImage);
            }

            cell.lblTitle.Text = promotion.Title;
            cell.lblDetails.Text = promotion.Text;
            cell.lblDate.Text = GetFormattedDate(promotion.PublishedDate);
            cell.imgUnread.Hidden = promotion.IsRead;

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //ActivityIndicator.Show();
            _promotionList[indexPath.Row].IsRead = true;
            DataManager.DataManager.SharedInstance.UpdatePromosDb(_promotionList);
            //PromotionsEntity wsManager = new PromotionsEntity();
            //wsManager.DeleteTable();
            //wsManager.CreateTable();
            //wsManager.InsertListOfItemsV2(_promotionList);
            //var test = wsManager.GetAllItemsV2();
            //ActivityIndicator.Hide();
            _controller.OnPromotionItemSelect(_promotionList[indexPath.Row]);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return (nfloat)(UIApplication.SharedApplication.KeyWindow.Frame.Width / 1.777) + 64;
        }

        string GetFormattedDate(string rawDate)
        {
            string formattedDate = string.Empty;
            if (!string.IsNullOrEmpty(rawDate))
            {
                try
                {
                    int year = Int32.Parse(rawDate.Substring(0, 4));
                    int month = Int32.Parse(rawDate.Substring(4, 2));
                    int day = Int32.Parse(rawDate.Substring(6, 2));

                    DateTime dt = new DateTime(year, month, day);
                    formattedDate = dt.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Parse Error: " + e.Message);
                    formattedDate = rawDate;
                }
            }
            return formattedDate;
        }
    }
}