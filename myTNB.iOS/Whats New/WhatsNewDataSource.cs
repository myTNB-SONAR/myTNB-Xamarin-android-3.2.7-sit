using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB.WhatsNew
{
    public class WhatsNewDataSource : UITableViewSource
    {
        private WhatsNewViewController _controller;
        private List<WhatsNewModel> _whatsNewList = new List<WhatsNewModel>();
        private Func<string, string> GetI18NValue;

        public WhatsNewDataSource() { }

        public WhatsNewDataSource(WhatsNewViewController controller, List<WhatsNewModel> whatsNewList, Func<string, string> getI18NValue)
        {
            _controller = controller;
            _whatsNewList = whatsNewList.OrderByDescending(x => DateTime.ParseExact(x.PublishDate, "yyyyMMddTHHmmss"
                   , CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();
            GetI18NValue = getI18NValue;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _whatsNewList != null ? _whatsNewList.Count : 0;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat addtl = 0;
            if (indexPath.Row == _whatsNewList.Count - 1)
            {
                addtl = ScaleUtility.GetScaledHeight(17F);
            }
            return ScaleUtility.GetScaledHeight(177F) + addtl;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            WhatsNewCell cell = tableView.DequeueReusableCell(WhatsNewConstants.Cell_WhatsNew) as WhatsNewCell;
            cell.Tag = indexPath.Row;
            if (cell.Tag > -1 && cell.Tag < _whatsNewList.Count)
            {
                WhatsNewModel whatsNew = _whatsNewList[(int)cell.Tag];
                cell.CellIndex = (int)cell.Tag;
                cell.GetI18NValue = GetI18NValue;
                cell.SetAccountCell(whatsNew);
                if (whatsNew.Image.IsValid())
                {
                    if (cell.Tag == indexPath.Row)
                    {
                        try
                        {
                            NSData imgData = WhatsNewCache.GetImage(whatsNew.ID);
                            if (imgData != null)
                            {
                                cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                                using (var image = UIImage.LoadFromData(imgData))
                                {
                                    cell.BannerImageView.Image = image;
                                }
                            }
                            else
                            {
                                cell.SetLoadingImageView();
                                NSUrl url = new NSUrl(whatsNew.Image);
                                NSUrlSession session = NSUrlSession
                                    .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                                NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        if (error == null && response != null && data != null)
                                        {
                                            if (cell.Tag == indexPath.Row)
                                            {
                                                cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                                                using (var image = UIImage.LoadFromData(data))
                                                {
                                                    cell.BannerImageView.Image = image;
                                                }
                                                WhatsNewCache.SaveImage(whatsNew.ID, data);
                                            }
                                        }
                                        else
                                        {
                                            cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                                        }
                                        cell.ShowDowloadedImage();
                                    });
                                });
                                dataTask.Resume();
                            }
                        }
                        catch (MonoTouchException m)
                        {
                            Debug.WriteLine("Image load Error: " + m.Message);
                            cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                        }
                    }
                }
                else
                {
                    cell.BannerImageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                }
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var index = indexPath.Row;
            if (index > -1 && index < _whatsNewList.Count)
            {
                if (_controller != null)
                {
                    _controller.OnItemSelection(_whatsNewList[index]);
                }
            }
        }
    }
}
