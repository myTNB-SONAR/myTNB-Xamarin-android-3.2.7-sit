using System;
using UIKit;
using myTNB.Model;
using Foundation;

namespace myTNB.Home.Bill.Receipt
{
    public class ReceiptTableViewDataSource : UITableViewSource
    {
        ReceiptResponseModel _receipt = new ReceiptResponseModel();

        public ReceiptTableViewDataSource(ReceiptResponseModel receipt)
        {
            _receipt = receipt;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _receipt?.d?.data?.accMultiPay?.Count ?? 0;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 177f;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("receiptCell", indexPath) as ReceiptTableViewCell;
            var item = _receipt?.d?.data?.accMultiPay?[indexPath.Row];
            cell.lblAcctNoValue.Text = item?.accountNum;
            cell.lblAcctNameValue.Text = item?.AccountOwnerName ?? string.Empty;
            cell.lblAmountValue.Text = item?.itmAmt;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}
