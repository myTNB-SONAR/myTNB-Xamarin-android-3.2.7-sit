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
        private EventHandler _onScroll;
        private List<MeterReadingHistoryItemModel> _readingHistoryList;
        private readonly Dictionary<string, string> I18NDictionary = new Dictionary<string, string>();
        private bool _isEmptyHistory;
        private bool _isSSMR;

        public SSMRReadingHistoryDataSource(EventHandler onScroll, List<MeterReadingHistoryItemModel> readingHistoryList, bool isSSMR = true)
        {
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("SSMRReadingHistory");
            _onScroll = onScroll;
            _readingHistoryList = readingHistoryList;
            _isSSMR = isSSMR;
            _isEmptyHistory = _readingHistoryList == null || _readingHistoryList.Count < 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_isEmptyHistory)
            {
                NoDataViewCell cell = tableView.DequeueReusableCell(Constants.Cell_NoHistoryData) as NoDataViewCell;
                cell.Image = SSMRConstants.IMG_NoHistory;
                cell.Message = GetI18NValue(SSMRConstants.I18N_NoHistoryData);
                return cell;
            }
            else
            {
                MeterReadingHistoryItemModel readingHistory = _readingHistoryList[indexPath.Row];
                SSMRReadingHistoryCell cell = tableView.DequeueReusableCell(SSMRConstants.Cell_ReadingHistory) as SSMRReadingHistoryCell;
                cell._dateLabel.Text = readingHistory?.ReadingDate ?? string.Empty;
                cell._descLabel.Text = readingHistory?.ReadingType ?? string.Empty;
                cell._kwhLabel.Text = readingHistory?.ReadingValue ?? string.Empty;
                cell._monthYearLabel.Text = readingHistory?.ReadingForMonth ?? string.Empty;
                cell.UpdateCell(readingHistory.IsEstimatedReading);
                return cell;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _isSSMR ? 1 : 0;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return (nint)(_isEmptyHistory ? 1 : _readingHistoryList?.Count);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return ScaleUtility.GetScaledHeight(48.0f);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat padding = ScaleUtility.GetScaledWidth(16);
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, ScaleUtility.GetScaledHeight(48.0f)));
            sectionView.BackgroundColor = MyTNBColor.LightGrayBG;
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(padding, 0, sectionView.Frame.Width, sectionView.Frame.Height),
                Text = GetI18NValue(SSMRConstants.I18N_SectionTitle),
                Font = TNBFont.MuseoSans_16_500,
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