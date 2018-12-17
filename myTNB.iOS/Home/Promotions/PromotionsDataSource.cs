using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB.Home.Promotions
{
    public class PromotionsDataSource : UITableViewSource
    {
        PromotionsViewController _controller;
        List<PromotionsModelV2> _promotionList = new List<PromotionsModelV2>();
        public PromotionsDataSource(PromotionsViewController controller, List<PromotionsModelV2> promotionList)
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
                NSUrl url = new NSUrl(promotion.LandscapeImage);//Image);
                NSUrlSession session = NSUrlSession
                    .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                {
                    if (error == null && response != null && data != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            cell.imgBanner.Image = UIImage.LoadFromData(data);
                            _activityIndicator.Hide();
                        });
                    }
                });
                dataTask.Resume();
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
            string formattedDate = rawDate;
            try
            {
                int year = Int32.Parse(rawDate.Substring(0, 4));
                int month = Int32.Parse(rawDate.Substring(4, 2));
                int day = Int32.Parse(rawDate.Substring(6, 2));

                DateTime dt = new DateTime(year, month, day);
                formattedDate = dt.ToString("dd MMM yyyy");
            }
            catch (Exception e)
            {
                Console.WriteLine("Parse Error: " + e.Message);
                formattedDate = rawDate;
            }
            return formattedDate;
        }
    }
}