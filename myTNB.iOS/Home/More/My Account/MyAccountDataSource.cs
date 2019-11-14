using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.MyAccount;
using UIKit;

namespace myTNB.Home.More.MyAccount
{
    public class MyAccountDataSource : UITableViewSource
    {
        private readonly RegisteredCardsResponseModel _registeredCards = new RegisteredCardsResponseModel();
        private readonly MyAccountViewController _controller;
        private readonly List<string> SectionTitle;
        private readonly List<string> DetailContent;

        public MyAccountDataSource(MyAccountViewController controller)
        {
            _controller = controller;
            if (DataManager.DataManager.SharedInstance.RegisteredCards != null
                && DataManager.DataManager.SharedInstance.RegisteredCards.d != null
                && DataManager.DataManager.SharedInstance.RegisteredCards?.d?.didSucceed == true)
            {
                _registeredCards = DataManager.DataManager.SharedInstance.RegisteredCards;
            }
            else
            {
                _registeredCards.d = new RegisteredCardsModel();
                _registeredCards.d.data = new List<RegisteredCardsDataModel>();
            }
            SectionTitle = new List<string>{
                GetI18NValue(MyAccountConstants.I18N_DetailSectionTitle)
                , GetI18NValue(MyAccountConstants.I18N_AccountSectionTitle)
            };
            DetailContent = new List<string>{
                GetCommonI18NValue(Constants.Common_Name).ToUpper(),
                GetCommonI18NValue(Constants.Common_IDNumber).ToUpper(),
                GetCommonI18NValue(Constants.Common_Email).ToUpper(),
                GetCommonI18NValue(Constants.Common_MobileNo).ToUpper(),
                GetCommonI18NValue(Constants.Common_Password).ToUpper(),
                GetCommonI18NValue(Constants.Common_Cards).ToUpper()
            };
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return SectionTitle.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                return 6;
            }
            else if (section == 1)
            {
                return DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                    ? DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count : 0;
            }
            else
            {
                return 0;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 48));
            view.BackgroundColor = MyTNBColor.SectionGrey;

            UILabel lblSectionTitle = new UILabel(new CGRect(18, 16, tableView.Frame.Width, 18))
            {
                Text = SectionTitle[(int)section],
                Font = MyTNBFont.MuseoSans16,
                TextColor = MyTNBColor.PowerBlue
            };
            view.Add(lblSectionTitle);

            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                AccountDetailsViewCell cell = tableView.DequeueReusableCell("AccountDetailsViewCell", indexPath) as AccountDetailsViewCell;
                cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 64);

                int detailCount = DetailContent?.Count ?? 0;
                cell.lblTitle.Text = indexPath.Row < detailCount ? DetailContent[indexPath.Row] : string.Empty;
                cell.viewCTA.Hidden = true;
                cell.lblDetail.TextColor = MyTNBColor.SilverChalice;

                SQLite.SQLiteDataManager.UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0] : new SQLite.SQLiteDataManager.UserEntity();
                if (indexPath.Row == 0)
                {
                    cell.lblDetail.Text = userInfo?.displayName;
                }
                else if (indexPath.Row == 1)
                {
                    string icNo = userInfo?.identificationNo;
                    if (!string.IsNullOrEmpty(icNo) && icNo.Length > 4)
                    {
                        string lastDigit = icNo.Substring(icNo.Length - 4);
                        icNo = "•••••• •• " + lastDigit;
                    }
                    string maskedICNo = icNo;
                    cell.lblDetail.Text = maskedICNo;
                }
                else if (indexPath.Row == 2)
                {
                    cell.lblDetail.Text = userInfo?.email;
                }
                else if (indexPath.Row == 3)
                {
                    cell.lblDetail.Text = userInfo?.mobileNo;
                    cell.lblDetail.TextColor = MyTNBColor.TunaGrey();
                    cell.lblCTA.Text = GetCommonI18NValue(Constants.Common_Update);
                    cell.viewCTA.Hidden = false;
                    cell.AddGestureRecognizer(new UITapGestureRecognizer(_controller.UpdateMobileNumber));
                }
                else if (indexPath.Row == 4)
                {
                    cell.lblDetail.Text = "••••••••••••••";
                    cell.lblDetail.TextColor = MyTNBColor.TunaGrey();
                    cell.lblCTA.Text = GetCommonI18NValue(Constants.Common_Update);
                    cell.viewCTA.Hidden = false;
                    cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        _controller.UpdatePassword();
                    }));
                }
                else if (indexPath.Row == 5)
                {
                    int cardCount = _registeredCards?.d?.data?.Count ?? 0;
                    cell.lblDetail.Text = cardCount.ToString();
                    cell.lblDetail.TextColor = MyTNBColor.TunaGrey();
                    cell.lblCTA.Text = GetCommonI18NValue(Constants.Common_Manage);
                    UITapGestureRecognizer manageCards = new UITapGestureRecognizer(() =>
                    {
                        _controller.ManageRegisteredCards();
                    });
                    if (cardCount > 0)
                    {
                        cell.AddGestureRecognizer(manageCards);
                    }
                    else
                    {
                        cell.AddGestureRecognizer(new UITapGestureRecognizer(() => { }));
                    }
                    cell.lblCTA.TextColor = cardCount > 0
                        ? MyTNBColor.PowerBlue : MyTNBColor.SilverChalice;
                    cell.viewCTA.Hidden = false;
                }
                cell.viewLine.Hidden = !(indexPath.Row < detailCount - 1);
                return cell;
            }
            else if (indexPath.Section == 1)
            {
                SupplyAccountViewCell cell = tableView.DequeueReusableCell("SupplyAccountViewCell", indexPath) as SupplyAccountViewCell;
                cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 80);
                cell.lblName.Text = GetAccountModel(indexPath.Row).accDesc;
                cell.lblAccountNumber.Text = GetAccountModel(indexPath.Row).accNum;
                //cell.lblUsers.Text = "2 Users";
                cell.lblCTA.Text = GetCommonI18NValue(Constants.Common_Manage);

                nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
                CGSize newLabelSize = GetLabelSize(cell.lblName, cellWidth - 110, 18);
                cell.lblName.Frame = new CGRect(18, 16, newLabelSize.Width, 18);
                cell.imgLeaf.Frame = new CGRect(18 + cell.lblName.Frame.Width + 6, 14, 24, 24);

                if (GetAccountModel(indexPath.Row).accountCategoryId != null)
                {
                    cell.imgLeaf.Hidden = !GetAccountModel(indexPath.Row).accountCategoryId.Equals("2");
                }

                cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    _controller.ManageSupplyAccount(indexPath.Row);
                }));
                return cell;
            }
            return new UITableViewCell();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat rowHeight = 64;
            if (indexPath.Section == 0)
            {
                rowHeight = 64;
            }
            else if (indexPath.Section == 1)
            {
                rowHeight = 67;
            }
            return rowHeight;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 50f;
        }

        private CustomerAccountRecordModel GetAccountModel(int index)
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
               && index < DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count)
            {
                return DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
            }
            return new CustomerAccountRecordModel();
        }

        private CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        private string GetI18NValue(string key)
        {
            return _controller.GetI18NValue(key);
        }

        private string GetCommonI18NValue(string key)
        {
            return _controller.GetCommonI18NValue(key);
        }
    }
}