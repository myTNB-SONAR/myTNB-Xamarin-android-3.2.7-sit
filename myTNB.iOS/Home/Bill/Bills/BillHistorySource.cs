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
        public Action<string> OnSelectBill;
        public Action<string> OnSelectPayment;
        public Action OnShowFilter;
        private List<BillPayHistoryModel> _historyResponseList;
        private List<BillPayHistoryDataModel> _historyList = new List<BillPayHistoryDataModel>();
        private Dictionary<int, string> _historyDictionary = new Dictionary<int, string>();
        private bool _isLoading;

        public BillHistorySource(List<BillPayHistoryModel> historyResponseList, bool isLoading)
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
            _isLoading = isLoading;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            int rowCount = 1;//Default to 1 for section view
            if (IsEmptyHistory || _isLoading)
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
                cell.filterAction = OnShowFilter;
                cell.IsLoading = _isLoading;
                cell.Layer.ZPosition = 1;
                return cell;
            }
            else
            {
                if (_isLoading)
                {
                    BillHistoryShimmerViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_BillHistoryShimmer) as BillHistoryShimmerViewCell;
                    cell.ClipsToBounds = false;
                    return cell;
                }
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
                    cell.IsArrowHidden = !item.IsDocumentAvailable;
                    cell.IsPayment = item.IsPayment;

                    if (_historyDictionary.ContainsKey(index) && _historyDictionary.ContainsKey(index + 1))
                    {
                        cell.Date = _historyDictionary[index];
                        cell.SetWidgetHeight(true, true, true);
                        cell.IsGroupedDateHidden = false;
                    }
                    else if (_historyDictionary.ContainsKey(index))
                    {
                        cell.Date = _historyDictionary[index];
                        cell.SetWidgetHeight(true);
                        cell.IsGroupedDateHidden = false;
                    }
                    else if (_historyDictionary.ContainsKey(index + 1))
                    {
                        cell.SetWidgetHeight(true, false, true);
                        cell.IsGroupedDateHidden = true;
                    }
                    else
                    {
                        cell.IsGroupedDateHidden = true;
                    }
                    cell.IsLineHidden = indexPath.Row == _historyList.Count;
                    cell.ClipsToBounds = false;
                    cell.Layer.ZPosition = 10 + indexPath.Section + indexPath.Row;
                    return cell;
                }
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
                return;

            int index = indexPath.Row - 1;
            BillPayHistoryDataModel item = _historyList[index];
            if (item.IsDocumentAvailable)
            {
                if (item.IsPayment)
                {
                    if (OnSelectPayment != null)
                    {
                        OnSelectPayment.Invoke(item.DetailedInfoNumber);
                    }
                }
                else
                {
                    if (OnSelectBill != null)
                    {
                        OnSelectBill.Invoke(item.DetailedInfoNumber);
                    }
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