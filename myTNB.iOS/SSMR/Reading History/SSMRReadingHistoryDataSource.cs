using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryDataSource : UITableViewSource
    {
        private List<MeterReadingHistoryItemModel> _readingHistoryList;
        private SSMRHelper _sSMRHelper = new SSMRHelper();
        EventHandler _onScroll;
        private readonly Dictionary<string, string> I18NDictionary;

        public SSMRReadingHistoryDataSource(EventHandler onScroll, List<MeterReadingHistoryItemModel> readingHistoryList)
        {
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("SSMRReadingHistory");
            _onScroll = onScroll;
            _readingHistoryList = readingHistoryList;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var readingHistory = _readingHistoryList[indexPath.Row];
            SSMRReadingHistoryCell cell = tableView.DequeueReusableCell(SSMRConstants.Cell_ReadingHistory) as SSMRReadingHistoryCell;
            cell._dateLabel.Text = readingHistory?.ReadingDate ?? string.Empty;
            cell._descLabel.Text = readingHistory?.ReadingType ?? string.Empty;
            cell._kwhLabel.Text = readingHistory?.Consumption ?? string.Empty;
            cell._monthYearLabel.Text = readingHistory?.ReadingForMonth ?? string.Empty;
            cell.UpdateCell(_sSMRHelper.IsEstimatedReading(readingHistory.ReadingType));
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _readingHistoryList?.Count ?? 0;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 67f;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 48.0f;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat padding = 13f;
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, 48.0f));
            sectionView.BackgroundColor = MyTNBColor.LightGrayBG;
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(padding, 0, sectionView.Frame.Width, sectionView.Frame.Height),
                Text = GetI18NValue(SSMRConstants.I18N_SectionTitle),
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            sectionView.AddSubviews(new UIView[] { lblTitle });
            return sectionView;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _onScroll?.Invoke(scrollView, null);
        }

        public string GetI18NValue(string key)
        {
            return I18NDictionary.ContainsKey(key) ? I18NDictionary[key] : string.Empty;
        }
    }
}
