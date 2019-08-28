using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillHistorySource : UITableViewSource
    {
        public EventHandler OnTableViewScroll;
        public Func<string, string> GetI18NValue;
        private List<BillPayHistoryModel> _historyResponseList;
        private List<BillPayHistoryDataModel> _historyList = new List<BillPayHistoryDataModel>();
        private Dictionary<int, string> _historyDictionary = new Dictionary<int, string>();

        public BillHistorySource(List<BillPayHistoryModel> historyResponseList)
        {
            _historyResponseList = historyResponseList;
            for (int i = 0; i < _historyResponseList.Count; i++)
            {
                BillPayHistoryModel item = _historyResponseList[i];
                if (i == 0)
                {
                    _historyDictionary.Add(i, item.MonthYear);
                }
                else
                {
                    _historyDictionary.Add(_historyList.Count, item.MonthYear);
                }
                for (int j = 0; j < item.BillPayHistoryData.Count; j++)
                {
                    BillPayHistoryDataModel jItem = item.BillPayHistoryData[j];
                    _historyList.Add(jItem);
                }
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            int rowCount = 1;//Default to 1 for section view
            if (IsEmptyHistory)
            {
                rowCount++;
            }
            else
            {
                rowCount += _historyList.Count;
            }
            return rowCount;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                BillSectionViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_BillSection) as BillSectionViewCell;
                cell.SectionTitle = GetI18NValue(BillConstants.I18N_MyHistory);
                return cell;
            }
            else
            {
                if (IsEmptyHistory)
                {
                    NoDataViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_NoHistoryData) as NoDataViewCell;
                    cell.Image = BillConstants.IMG_NoHistoryData;
                    cell.Message = GetI18NValue(BillConstants.I18N_NoHistoryData);
                    return cell;
                }
                else
                {
                    int index = indexPath.Row - 1;
                    BillPayHistoryDataModel item = _historyList[index];
                    BillHistoryViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_BillHistory) as BillHistoryViewCell;

                    cell.Type = item.DateAndHistoryType;
                    cell.Source = item.PaidVia;
                    cell.Amount = item.Amount;
                    cell.IsArrowHidden = !item.IsViaMobileApp;
                    cell.IsPayment = item.IsPayment;

                    if (_historyDictionary.ContainsKey(index))
                    {
                        cell.Date = _historyDictionary[index];
                        cell.SetWidgetHeight(true, true, _historyDictionary.ContainsKey(index + 1));
                    }
                    else if (_historyDictionary.ContainsKey(index + 1))
                    {
                        cell.SetWidgetHeight(true, false, true);
                    }

                    cell.ClipsToBounds = false;
                    cell.Layer.ZPosition = 10 + indexPath.Section + indexPath.Row;
                    return cell;
                }
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return ScaleUtility.GetScaledHeight(60);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            OnTableViewScroll?.Invoke(scrollView, null);
        }

        private bool IsEmptyHistory
        {
            get
            {
                return _historyResponseList == null || _historyResponseList.Count == 0;
            }
        }
    }
}