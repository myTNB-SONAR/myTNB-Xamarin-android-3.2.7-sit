using System;
using System.Collections.Generic;
using System.Globalization;
using Foundation;
using myTNB.Extensions;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillTableViewDataSource : UITableViewSource
    {
        BillHistoryResponseModel _billHistory = new BillHistoryResponseModel();
        PaymentHistoryResponseModel _paymentHistory = new PaymentHistoryResponseModel();
        BillViewController _billViewController;
        bool _hasNetworkConnection;
        bool _isREAccount;
        bool _isOwner;

        public BillTableViewDataSource(BillHistoryResponseModel billHistory
                                       , PaymentHistoryResponseModel paymentHistory
                                       , BillViewController billViewController
                                       , bool hasNetworkConnection
                                       , bool isREAccount
                                       , bool isOwner)
        {
            if (billHistory != null && billHistory.d != null && billHistory.d.data != null)
            {
                _billHistory = billHistory;
                if (!isOwner && billHistory.d.data.Count > 1)
                {
                    _billHistory.d.data = billHistory.d.data.GetRange(0, 1);
                }

            }
            else
            {
                _billHistory.d = new BillHistoryModel();
                _billHistory.d.data = new List<BillHistoryDataModel>();
            }

            if (paymentHistory != null && paymentHistory.d != null && paymentHistory.d.data != null)
            {
                _paymentHistory = paymentHistory;
            }
            else
            {
                _paymentHistory.d = new PaymentHistoryModel();
                _paymentHistory.d.data = new List<PaymentHistoryDataModel>();
            }
            _billViewController = billViewController;
            _hasNetworkConnection = hasNetworkConnection;
            _isREAccount = isREAccount;
            _isOwner = isOwner;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (!_hasNetworkConnection)
            {
                return 1;
            }
            if (DataManager.DataManager.SharedInstance.selectedTag == 0)
            {
                int rowCount = _billHistory != null
                    && _billHistory.d != null
                    && _billHistory.d.data != null ? _billHistory.d.data.Count : 0;
                return rowCount == 0 ? 1 : rowCount;
            }
            else
            {
                int rowCount = _paymentHistory != null
                    && _paymentHistory.d != null
                    && _paymentHistory.d.data != null ? _paymentHistory.d.data.Count : 0;
                return rowCount == 0 ? 1 : rowCount;
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_hasNetworkConnection)
            {
                const string CELLIDENTIFIER = "billCell";

                if (DataManager.DataManager.SharedInstance.selectedTag == 0)
                {
                    if (HasBill())
                    {
                        var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as BillTableViewCell;
                        BillHistoryDataModel billHistoryModel = indexPath.Row < _billHistory?.d?.data?.Count
                                                                         ? _billHistory.d.data[indexPath.Row]
                                                                         : new BillHistoryDataModel();

                        var date = DateHelper.GetFormattedDate(billHistoryModel.DtBill, "dd MMM");
                        var title = DateHelper.GetFormattedDate(billHistoryModel.DtBill, "MMM yyyy");

                        double amountDbl = TextHelper.ParseStringToDouble(billHistoryModel.AmPayable);

                        if (_isREAccount)
                        {
                            title += string.Format(" {0}", "Bill_Advice".Translate());
                            amountDbl = ChartHelper.UpdateValueForRE(amountDbl);
                        }
                        cell.AdjustYlocation(true);
                        cell.imgArrow.Hidden = false;
                        cell.lblDate.Text = date;
                        cell.lblTitle.Text = title;
                        cell.lblDetails.Hidden = true;
                        cell.lblAmount.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, amountDbl.ToString("N2", CultureInfo.InvariantCulture));
                        cell.viewLine.Hidden = !(indexPath.Row < _billHistory?.d?.data?.Count - 1);
                        return cell;
                    }
                    else
                    {
                        var cell = tableView.DequeueReusableCell("NoDataViewCell", indexPath) as NoDataViewCell;
                        cell.imgViewState.Image = UIImage.FromBundle("Empty-Current-Bill");
                        cell.lblDescription.Text = "Bill_NoBill".Translate();
                        return cell;
                    }

                }
                else
                {
                    if (HasPayment())
                    {
                        var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as BillTableViewCell;
                        CreatePaymentCell(indexPath.Row, cell);
                        return cell;
                    }
                    else
                    {
                        var cell = tableView.DequeueReusableCell("NoDataViewCell", indexPath) as NoDataViewCell;
                        cell.imgViewState.Image = UIImage.FromBundle("Empty-Bill-Payment");
                        cell.lblDescription.Text = "Bill_NoPayment".Translate();
                        return cell;
                    }
                }
            }
            else
            {
                //tableView.TableFooterView.Hidden = true;
                var cell = tableView.DequeueReusableCell("NoDataViewCell", indexPath) as NoDataViewCell;
                cell.imgViewState.Image = UIImage.FromBundle("No-Internet-Connection");
                cell.lblDescription.Text = "Bill_NoDataConnection".Translate();
                return cell;
            }
        }


        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (_hasNetworkConnection)
            {
                if (DataManager.DataManager.SharedInstance.selectedTag == 0)
                {
                    return HasBill() ? 62F : 238F;
                }
                else
                {
                    return HasPayment() ? 62F : 238F;
                }
            }
            else
            {
                return 238F;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (!_hasNetworkConnection)
            {
                return;
            }
            if (DataManager.DataManager.SharedInstance.selectedTag == 0)
            {
                if (!HasBill())
                {
                    return;
                }
            }
            else
            {
                if (!HasPayment())
                {
                    return;
                }
            }
            if (NetworkUtility.isReachable)
            {
                if (DataManager.DataManager.SharedInstance.selectedTag == 0)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                    ViewBillViewController viewBillVC =
                        storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                    viewBillVC.selectedIndex = indexPath.Row;
                    var navController = new UINavigationController(viewBillVC);
                    _billViewController.PresentViewController(navController, true, null);

                }
                else if (DataManager.DataManager.SharedInstance.selectedTag == 1)
                {
                    if (!_isREAccount && indexPath.Row < _paymentHistory?.d?.data?.Count)
                    {
                        if (_paymentHistory.d.data[indexPath.Row].NmPBranch == "myTNB Mobile App")
                        {
                            string merchantTransactionID = _paymentHistory.d.data[indexPath.Row].MechantTransId;
                            _billViewController.ViewReceipt(merchantTransactionID);
                        }
                    }
                }
            }
            else
            {
                ErrorHandler.DisplayNoDataAlert(_billViewController);
            }
        }

        bool HasBill()
        {
            return _billHistory != null && _billHistory.d != null
                && _billHistory.d.data != null && _billHistory.d.data.Count > 0;
        }

        bool HasPayment()
        {
            return _paymentHistory != null && _paymentHistory.d != null
                && _paymentHistory.d.data != null && _paymentHistory.d.data.Count > 0;
        }

        /// <summary>
        /// Creates the payment cell.
        /// </summary>
        /// <param name="row">Row.</param>
        /// <param name="cell">Cell.</param>
        private void CreatePaymentCell(int row, BillTableViewCell cell)
        {
            if (row > -1 && row < _paymentHistory?.d?.data.Count)
            {
                PaymentHistoryDataModel paymentHistoryModel = _paymentHistory?.d?.data[row];
                cell.AdjustYlocation(false);
                cell.lblTitle.Text = "Common_Payment".Translate();
                cell.viewLine.Hidden = !(row < _paymentHistory?.d?.data?.Count - 1);
                cell.lblDetails.Hidden = false;
                string date = string.Empty;
                string details = string.Empty;
                if (!_isREAccount)
                {
                    cell.imgArrow.Hidden = paymentHistoryModel.NmPBranch != "myTNB Mobile App" ? true : false;
                    var strDate = paymentHistoryModel.DtEvent ?? string.Empty;
                    date = string.IsNullOrEmpty(strDate) || strDate.ToLower().Equals("n/a")
                                                 ? "Common_NA".Translate()
                                                 : DateHelper.GetFormattedDate(paymentHistoryModel.DtEvent, "dd MMM");
                    details = string.IsNullOrEmpty(paymentHistoryModel.NmPBranch) || string.IsNullOrWhiteSpace(paymentHistoryModel.NmPBranch)
                                            ? string.Empty : string.Format("{0} {1}", "Bill_Via".Translate(), paymentHistoryModel.NmPBranch);
                }
                else
                {
                    cell.imgArrow.Hidden = true;
                    var strDate = paymentHistoryModel.PaidDate ?? string.Empty;
                    date = string.IsNullOrEmpty(strDate) || strDate.ToLower().Equals("n/a")
                                                 ? "Common_NA".Translate()
                                                 : DateHelper.GetFormattedDate(paymentHistoryModel.PaidDate, "dd MMM");
                    details = "Bill_PaymentFrom".Translate();
                }

                cell.lblDate.Text = date;
                cell.lblDetails.Text = details;
                double amountDbl = TextHelper.ParseStringToDouble(_isREAccount ? paymentHistoryModel.Amount : paymentHistoryModel.AmPaid);
                cell.lblAmount.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, amountDbl.ToString("N2", CultureInfo.InvariantCulture));
            }
        }
    }
}