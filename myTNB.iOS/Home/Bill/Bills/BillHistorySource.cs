using System;
using System.Collections.Generic;
using CoreGraphics;
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

        //For Filter
        public Action OnShowFilter;
        public bool IsFiltered;
        public bool NoData { set; private get; }

        //For Refresh
        public bool IsFailedService { set; private get; } = false;
        public bool IsPlanned { set; private get; } = false;
        public string FailMessage { set; private get; } = string.Empty;
        public Action OnRefresh { set; private get; }

        private List<BillPayHistoryModel> _historyResponseList;
        private List<BillPayHistoryDataModel> _historyList = new List<BillPayHistoryDataModel>();
        private Dictionary<int, string> _historyDictionary = new Dictionary<int, string>();
        private bool _isLoading, _hideSection;

        public BillHistorySource(List<BillPayHistoryModel> historyResponseList, bool isLoading)
        {
            _historyResponseList = historyResponseList;
            if (_historyResponseList != null)
            {
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
            _isLoading = isLoading;
            _hideSection = false;
        }

        public BillHistorySource(bool hideSection)
        {
            _hideSection = hideSection;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _hideSection ? 0 : 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_hideSection)
            {
                return 0;
            }
            int rowCount = 1;//Default to 1 for section view
            if (_isLoading || IsEmptyHistory || IsFailedService)
            {
                rowCount++;
            }
            else
            {
                rowCount += _historyList != null ? _historyList.Count : 0;
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
                cell.Layer.ZPosition = 1;
                cell.SetFilterImage(IsFiltered);
                cell.DisplayFilterIcon = !IsFailedService && !NoData;
                cell.EnableFilter = !_isLoading;
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
                if (IsFailedService)
                {
                    RefreshViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_Refresh) as RefreshViewCell;
                    RefreshComponent refreshComponent = new RefreshComponent(FailMessage
                        , LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshNow)
                        , () => { if (OnRefresh != null) { OnRefresh.Invoke(); } })
                    {
                        PageName = "Bill",
                        IsPlannedDownTime = IsPlanned
                    };
                    UIView refreshView = refreshComponent.GetUI(cell._view);
                    cell._view.AddSubview(refreshView);
                    cell._view.Frame = new CGRect(new CGPoint(0, 0), new CGSize(tableView.Frame.Width, refreshView.Frame.Height));
                    cell.Rescale();
                    return cell;
                }
                else if (IsEmptyHistory)
                {
                    NoDataViewCell cell = tableView.DequeueReusableCell(Constants.Cell_NoHistoryData) as NoDataViewCell;
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
                    cell.Amount = item.Amount;
                    cell.Source = item.PaidVia;
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
                    cell.IsLineHidden = index == 0;
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
            if (index > -1 && index < _historyList.Count)
            {
                BillPayHistoryDataModel item = _historyList[index];
                if (item != null)
                {
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