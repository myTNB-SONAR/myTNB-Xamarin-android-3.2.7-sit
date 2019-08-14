using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Home.Dashboard.SelectAccounts;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.SelectAccounts
{
    public class SelectAccountsDataSource : UITableViewSource
    {
        private List<CustomerAccountRecordModel> _accountList = new List<CustomerAccountRecordModel>();
        private SelectAccountTableViewController _controller;
        private int _count;
        public SelectAccountsDataSource(SelectAccountTableViewController controller)
        {
            _controller = controller;
            if (_controller.IsFromSSMR)
            {
                _accountList = SSMRAccounts.GetAccounts();
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.AccountRecordsList != null && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
                {
                    _accountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d ?? new List<CustomerAccountRecordModel>();
                }
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            _count = _accountList != null ? _accountList.Count : 0;
            if (_controller.IsFromSSMR) { _count++; }
            return _count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("AccountsViewCell", indexPath) as AccountsViewCell;
            if (indexPath.Row < _accountList.Count)
            {
                CustomerAccountRecordModel item = new CustomerAccountRecordModel();
                item = _accountList[indexPath.Row];

                cell.lblAccountName.Text = item.accDesc;
                cell.ImageIcon = GetIcon(item);

                if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex
                   && item.accNum == _accountList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex].accNum)
                {
                    cell.Accessory = UITableViewCellAccessory.None;
                    cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                    UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24))
                    {
                        Image = UIImage.FromBundle("Table-Tick")
                    };
                    cell.AccessoryView.AddSubview(imgViewTick);
                }
                else
                {
                    if (cell != null && cell.AccessoryView != null && cell.AccessoryView.Subviews != null)
                    {
                        foreach (var subView in cell.AccessoryView.Subviews)
                        {
                            subView.RemoveFromSuperview();
                        }
                    }
                }
            }
            else
            {
                CustomUIView view = new CustomUIView(new CGRect(0, 0, cell.Frame.Width, cell.Frame.Height - 1))
                {
                    BackgroundColor = UIColor.White
                };
                CustomUIView viewInfo = new CustomUIView(new CGRect(16, (view.Frame.Height - 24) / 2, cell.Frame.Width - 32, 24))
                {
                    BackgroundColor = MyTNBColor.IceBlue
                };
                UIImageView imgView = new UIImageView(new CGRect(4, 4, 16, 16))
                {
                    Image = UIImage.FromBundle(SelectAccountConstants.IMG_Info)
                };
                UILabel lblDescription = new UILabel(new CGRect(28, 4, view.Frame.Width - 40, 16))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = _controller.GetI18NValue(SelectAccountConstants.I18N_AccountsMissing)
                };
                viewInfo.Layer.CornerRadius = 12;
                viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
                view.AddSubview(viewInfo);
                view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    _controller.DisplayCustomAlert(_controller.GetI18NValue(SelectAccountConstants.I18N_AccountsMissingTitle)
                        , _controller.GetI18NValue(SelectAccountConstants.I18N_AccountsMissingDescription)
                        , new Dictionary<string, Action> { { _controller.GetI18NValue(SelectAccountConstants.I18N_AccountsMissingCTA), null } }
                        , false);
                }));
                cell.AddSubview(view);
            }
            return cell;
        }

        private string GetIcon(CustomerAccountRecordModel account)
        {
            string iconName = DashboardHomeConstants.Img_SMIcon;
            if (account.IsREAccount)
            {
                iconName = DashboardHomeConstants.Img_REIcon;
            }
            else if (account.IsNormalMeter && account.IsSSMR && account.IsOwnedAccount)
            {
                iconName = DashboardHomeConstants.Img_SMRIcon;
            }
            else if (account.IsNormalMeter)
            {
                iconName = DashboardHomeConstants.Img_NormalIcon;
            }
            return iconName;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row < _accountList.Count)
            {
                var selected = _accountList[indexPath.Row];
                DataManager.DataManager.SharedInstance.IsSameAccount = DataManager.DataManager.SharedInstance.GetAccountsCount() > 1
                    && string.Compare(selected.accNum, DataManager.DataManager.SharedInstance.SelectedAccount?.accNum) == 0;
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                _controller.DismissViewController(true, null);
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 57F;
        }
    }
}