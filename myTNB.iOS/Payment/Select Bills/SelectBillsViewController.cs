using Foundation;
using System;
using UIKit;
using myTNB.Payment.SelectBills;
using CoreGraphics;
using System.Threading.Tasks;
using System.Collections.Generic;
using myTNB.Model;
using System.Globalization;
using myTNB.Home.Bill;

namespace myTNB
{
    public partial class SelectBillsViewController : CustomUIViewController
    {
        public SelectBillsViewController(IntPtr handle) : base(handle)
        {
        }

        public double SelectedAccountDueAmount;
        public List<CustomerAccountRecordModel> _accountsForPayment = new List<CustomerAccountRecordModel>();
        public double totalAmount;

        // private MultiAccountDueAmountResponseModel _multiAccountDueAmount = new MultiAccountDueAmountResponseModel();
        private List<CustomerAccountRecordModel> _accounts = new List<CustomerAccountRecordModel>();
        private List<PaymentRecordModel> _accountsForDisplay = new List<PaymentRecordModel>();
        private CustomerAccountRecordModel _selectedAccount = new CustomerAccountRecordModel();
        private GetAccountsChargesResponseModel _accountChargesResponse = new GetAccountsChargesResponseModel();

        private UIView _viewAmount, _viewFooter;
        private UILabel _lblTotalAmountValue, _lblCurrency;
        private int loadMoreCount, lastStartIndex, lastEndIndex;
        private bool isViewDidLoad, isItemisedTooltipDisplayed;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetDefaultTableFrame();
            InitializedSubViews();
            AddBackButton();
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            if (appDelegate != null)
            {
                appDelegate._selectBillsVC = this;
            }
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                var userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                SelectBillsTableView.Frame = new CGRect(0, 0, View.Frame.Width
                    , View.Frame.Height - (keyboardRectangle.Height));
            });
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidHideNotification, (NSNotification obj) =>
            {
                SetDefaultTableFrame();
            });

            isViewDidLoad = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.IsPaymentDone)
            {
                DataManager.DataManager.SharedInstance.IsPaymentDone = false;
                DismissViewController(true, null);
            }
            else
            {
                if (isViewDidLoad)
                {
                    SetDefaultTableFrame();
                    ResetValues();
                    GetAccountsForDisplay();
                    List<string> accountsForQuery = GetAccountsForQuery(0, 5);
                    if (accountsForQuery?.Count > 0)
                    {
                        ActivityIndicator.Show();
                        OnGetMultiAccountDueAmountServiceCall(accountsForQuery);
                    }
                    else
                    {
                        UpdateDuesDisplay();
                    }
                    isViewDidLoad = false;
                }
            }
        }

        private void SetDefaultTableFrame()
        {
            SelectBillsTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (136));
        }

        private void ResetValues()
        {
            SelectBillsTableView.Source = new SelectBillsDataSource(this, new List<PaymentRecordModel>());
            SelectBillsTableView.ReloadData();
            // _multiAccountDueAmount = new MultiAccountDueAmountResponseModel();
            _accounts = new List<CustomerAccountRecordModel>();
            _accountsForDisplay = new List<PaymentRecordModel>();
            _selectedAccount = new CustomerAccountRecordModel();
            lastStartIndex = 0;
            lastEndIndex = 0;
            loadMoreCount = 0;
            DataManager.DataManager.SharedInstance.ClearPaidList();
        }

        private void UpdateAccountListWithAmount()
        {
            foreach (AccountChargesModel item in _accountChargesResponse.d.data.AccountCharges)
            {
                int itemIndex = _accounts.FindIndex(x => x.accNum.Equals(item.ContractAccount));
                if (itemIndex > -1)
                {
                    _accounts[itemIndex].Amount = item.AmountDue;
                    _accounts[itemIndex].AmountDue = item.AmountDue;
                    UpdateAccountsForDisplay(_accounts[itemIndex], item);
                }
            }

            /* foreach (var item in _multiAccountDueAmount.d.data)
             {
                 int itemIndex = _accounts.FindIndex(x => x.accNum.Equals(item.accNum));
                 if (itemIndex > -1)
                 {
                     _accounts[itemIndex].Amount = item.amountDue;
                     _accounts[itemIndex].AmountDue = item.amountDue;
                     UpdateAccountsForDisplay(_accounts[itemIndex], item);
                 }
             }*/
        }

        private void UpdateAccountsForDisplay(CustomerAccountRecordModel customerAccount
            , AccountChargesModel dueAmtData)
        {
            _accountsForDisplay.Add(new PaymentRecordModel
            {
                //ItemizedBillings = dueAmtData.ItemizedBillings,
                //OpenChargesTotal = dueAmtData.OpenChargesTotal,
                Amount = dueAmtData.AmountDue,
                AmountDue = dueAmtData.AmountDue,
                accNum = customerAccount.accNum,
                userAccountID = customerAccount.userAccountID,
                accDesc = customerAccount.accDesc,
                icNum = customerAccount.icNum,
                amCurrentChg = customerAccount.amCurrentChg,
                isRegistered = customerAccount.isRegistered,
                isPaid = customerAccount.isPaid,
                isError = customerAccount.isError,
                message = customerAccount.message,
                isOwned = customerAccount.isOwned,
                isLocal = customerAccount.isLocal,
                accountTypeId = customerAccount.accountTypeId,
                accountStAddress = customerAccount.accountStAddress,
                accountNickName = customerAccount.accountNickName,
                ownerName = customerAccount.ownerName,
                accountCategoryId = customerAccount.accountCategoryId,
                accountOwnerName = customerAccount.accountOwnerName,
                smartMeterCode = customerAccount.smartMeterCode,
                IsAccountSelected = customerAccount.IsAccountSelected
            });
        }

        private void GetAccountsForDisplay()
        {
            //Clone account record List
            foreach (var item in DataManager.DataManager.SharedInstance.AccountRecordsList.d)
            {
                if (item.accountCategoryId != "2")
                {
                    item.IsAccountSelected = false;
                    _accounts.Add(item);
                }
            }
            int selectedAccountIndex = _accounts.FindIndex(x => x.accNum.Equals(DataManager.DataManager.SharedInstance.SelectedAccount.accNum));
            if (selectedAccountIndex > -1)
            {
                _selectedAccount = _accounts[selectedAccountIndex];
                _selectedAccount.IsAccountSelected = true;//SelectedAccountDueAmount > 0;//true;
                _accounts.RemoveAt(selectedAccountIndex);
                _accounts.Insert(0, _selectedAccount);
            }
        }

        private void SetInitialSelectedAccount()
        {
            int selectedAccountIndex = _accounts.FindIndex(x => x.accNum.Equals(DataManager.DataManager.SharedInstance.SelectedAccount.accNum));
            if (selectedAccountIndex > -1)
            {
                _selectedAccount = _accounts[selectedAccountIndex];
                _selectedAccount.IsAccountSelected = _accounts[selectedAccountIndex].Amount > 0;//SelectedAccountDueAmount > 0;//true;
                //_accounts.RemoveAt(selectedAccountIndex);
                //_accounts.Insert(0, _selectedAccount);
            }
        }

        private List<string> GetAccountsForQuery(int start, int end)
        {
            lastStartIndex = start;
            lastEndIndex = end;
            List<string> accountsForQuery = new List<string>();
            for (int i = start; i < end; i++)
            {
                if (i >= _accounts.Count)
                {
                    break;
                }
                accountsForQuery.Add(_accounts[i].accNum);
            }
            return accountsForQuery;
        }

        void OnLoadMore()
        {
            View.EndEditing(true);
            lastStartIndex += (loadMoreCount > 0 ? 4 : 5);
            lastEndIndex = lastStartIndex + 4;
            List<string> accountsForQuery = GetAccountsForQuery(lastStartIndex, lastEndIndex);
            if (accountsForQuery?.Count > 0)
            {
                ActivityIndicator.Show();
                OnGetMultiAccountDueAmountServiceCall(accountsForQuery);
            }
            else
            {
                UpdateDuesDisplay();
            }
            loadMoreCount++;
        }

        internal void UpDateTotalAmount()
        {
            totalAmount = 0;
            int selectedAccountCount = 0;
            bool hasInvalidSelection = false;
            foreach (var item in _accountsForDisplay)
            {
                if (_accountChargesResponse != null && _accountChargesResponse.d != null && _accountChargesResponse.d.IsSuccess
                       && _accountChargesResponse.d.data != null && _accountChargesResponse.d.data.AccountCharges != null
                       && item.IsAccountSelected)
                //          if (_multiAccountDueAmount != null && _multiAccountDueAmount.d != null
                //        && _multiAccountDueAmount.d.data != null && item.IsAccountSelected)
                {
                    double otherCharges = 0;
                    selectedAccountCount++;
#if false
                    int accIndex = _multiAccountDueAmount.d.data.FindIndex(x => x.accNum == item.accNum);
                    if (accIndex > -1 && _multiAccountDueAmount.d.data[accIndex].IsItemisedBilling)
                    {
                        otherCharges = _multiAccountDueAmount.d.data[accIndex].OpenChargesTotal;
                    }
#endif
                    if (item.Amount < TNBGlobal.PaymentMinAmnt)
                    {
                        hasInvalidSelection = true;
                    }

                    totalAmount += item.Amount;// + otherCharges;
                }
            }
            _lblTotalAmountValue.Text = totalAmount.ToString("N2", CultureInfo.InvariantCulture);
            AdjustAmountFrame();
            var title = (selectedAccountCount > 0) ? string.Format("Payment_Multiple".Translate()
                , selectedAccountCount.ToString()) : "Payment_Single".Translate();
            BtnPayBill.SetTitle(title, UIControlState.Normal);

            bool isValid = (selectedAccountCount > 0 && totalAmount > 0) && !hasInvalidSelection;

            BtnPayBill.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            BtnPayBill.Enabled = isValid;
        }

        internal void OnShowItemisedTooltip(string accNum)
        {
            if (!isItemisedTooltipDisplayed)
            {
                /*var data = _multiAccountDueAmount.d;
                DisplayCustomAlert(data.MandatoryChargesTitle, data.MandatoryChargesMessage, new Dictionary<string, Action>() {
                    {
                        data.MandatoryChargesPriButtonText, ()=>{
                            if (DataManager.DataManager.SharedInstance.AccountRecordsList!=null
                                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
                            {
                                DataManager.DataManager.SharedInstance.SelectedAccount
                                    = DataManager.DataManager.SharedInstance.AccountRecordsList.d.Find(x => x.accNum == accNum);
                                ViewHelper.DismissControllersAndSelectTab(this, 1, true);
                            }
                        }
                    }
                    ,{
                        data.MandatoryChargesSecButtonText, null
                    }
                });
                isItemisedTooltipDisplayed = true;
                */
            }
        }

        private void OnGetMultiAccountDueAmountServiceCall(List<string> accountsForQuery)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        _accountChargesResponse = await GetAccountsCharges(accountsForQuery);
                        if (_accountChargesResponse != null && _accountChargesResponse.d != null && _accountChargesResponse.d.IsSuccess
                        && _accountChargesResponse.d.data != null && _accountChargesResponse.d.data.AccountCharges != null)
                        {
                            SetInitialSelectedAccount();
                            UpdateAccountListWithAmount();
                            UpdateDuesDisplay();
                        }
                        else
                        {
                            UpDateTotalAmount();
                            //DisplayServiceError(_multiAccountDueAmount?.d?.message);
                            DisplayServiceError(_accountChargesResponse?.d?.ErrorMessage ?? string.Empty);
                        }
                        ActivityIndicator.Hide();


                        /*await GetMultiAccountDueAmount(accountsForQuery).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_multiAccountDueAmount != null && _multiAccountDueAmount.d != null
                                    && _multiAccountDueAmount.d.data != null)
                                {
                                    UpdateAccountListWithAmount();
                                    UpdateDuesDisplay();
                                }
                                else
                                {
                                    UpDateTotalAmount();
                                    DisplayServiceError(_multiAccountDueAmount?.d?.message);
                                }
                                ActivityIndicator.Hide();
                            });
                        });*/
                    }
                    else
                    {
                        DisplayNoDataAlert();
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        /// <summary>
        /// Updates the dues display.
        /// </summary>
        private void UpdateDuesDisplay()
        {
            InitializedTableView();
            UpDateTotalAmount();
            SetDefaultTableFrame();
            if (_accounts.Count == _accountsForDisplay.Count)
            {
                SelectBillsTableView.TableFooterView = null;
            }
            else
            {
                _viewFooter.Hidden = false;
                SelectBillsTableView.TableFooterView = _viewFooter;
            }
        }

        internal void InitializedTableView()
        {
            SelectBillsTableView.Source = new SelectBillsDataSource(this, _accountsForDisplay);
            SelectBillsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            SelectBillsTableView.ReloadData();
        }

        internal void InitializedSubViews()
        {
            BtnPayBill.BackgroundColor = MyTNBColor.SilverChalice;
            BtnPayBill.Layer.CornerRadius = 4.0f;
            BtnPayBill.SetTitleColor(UIColor.White, UIControlState.Normal);
            BtnPayBill.TitleLabel.Font = MyTNBFont.MuseoSans16_500;
            BtnPayBill.TouchUpInside += (sender, e) =>
            {
                _accountsForPayment = new List<CustomerAccountRecordModel>();
                foreach (var item in _accountsForDisplay)
                {
                    if (item.IsAccountSelected)
                    {
                        _accountsForPayment.Add(item);
                        DataManager.DataManager.SharedInstance.SetAccountNumberForPayment(item.accNum);
                    }
                }

                UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                SelectPaymentMethodViewController viewController =
                    storyBoard.InstantiateViewController("SelectPaymentMethodViewController") as SelectPaymentMethodViewController;
                viewController.AccountsForPayment = _accountsForPayment;
                viewController.TotalAmount = totalAmount;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            };

            BottomContainerView.BackgroundColor = MyTNBColor.LightGrayBG;

            _viewAmount = new UIView(new CGRect(18, 20, View.Frame.Width - 36, 24));

            UILabel lblTotalAmountTitle = new UILabel(new CGRect(0, 6, 120, 18));
            lblTotalAmountTitle.TextColor = MyTNBColor.TunaGrey();
            lblTotalAmountTitle.Font = MyTNBFont.MuseoSans14_500;
            lblTotalAmountTitle.Text = "Common_TotalAmount".Translate();

            _lblCurrency = new UILabel(new CGRect(0, 6, 24, 18));
            _lblCurrency.TextColor = MyTNBColor.TunaGrey();
            _lblCurrency.Font = MyTNBFont.MuseoSans14_500;
            _lblCurrency.Text = TNBGlobal.UNIT_CURRENCY;
            _lblCurrency.TextAlignment = UITextAlignment.Right;

            _lblTotalAmountValue = new UILabel(new CGRect(0, 0, (View.Frame.Width - 36) / 2, 24));
            _lblTotalAmountValue.TextColor = MyTNBColor.TunaGrey();
            _lblTotalAmountValue.Font = MyTNBFont.MuseoSans24_500;
            _lblTotalAmountValue.Text = TNBGlobal.DEFAULT_VALUE;
            _lblTotalAmountValue.TextAlignment = UITextAlignment.Right;

            _viewAmount.AddSubviews(lblTotalAmountTitle, _lblCurrency, _lblTotalAmountValue);
            AdjustAmountFrame();
            BottomContainerView.AddSubview(_viewAmount);

            _viewFooter = new UIView(new CGRect(0, 0, View.Frame.Width, 40));
            _viewFooter.BackgroundColor = MyTNBColor.LightGrayBG;
            _viewFooter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnLoadMore();
            }));
            UILabel lblLoadMore = new UILabel(new CGRect(0, 0, _viewFooter.Frame.Width, 16));
            lblLoadMore.TextAlignment = UITextAlignment.Center;
            lblLoadMore.AttributedText = new NSAttributedString("Payment_LoadMore".Translate()
                , font: MyTNBFont.MuseoSans12_300
                , foregroundColor: MyTNBColor.SilverChalice
                , strokeWidth: 0
                , underlineStyle: NSUnderlineStyle.Single
            );

            _viewFooter.AddSubview(lblLoadMore);
            _viewFooter.Hidden = true;
            SelectBillsTableView.TableFooterView = _viewFooter;
        }

        private void AdjustAmountFrame()
        {
            CGSize newSize = GetLabelSize(_lblTotalAmountValue, _viewAmount.Frame.Width / 2, 24);
            _lblTotalAmountValue.Frame = new CGRect(_viewAmount.Frame.Width - newSize.Width - 9
                , 0, newSize.Width, 24);
            _lblCurrency.Frame = new CGRect(_lblTotalAmountValue.Frame.X - 27
                , _lblCurrency.Frame.Y, _lblCurrency.Frame.Width, _lblCurrency.Frame.Height);
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnBack();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void OnBack()
        {
            View.EndEditing(true);
            DismissViewController(true, null);
            ResetValues();
        }

        /*
        private Task GetMultiAccountDueAmount(List<string> accountList)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accounts = accountList
                };
                _multiAccountDueAmount = serviceManager.OnExecuteAPI<MultiAccountDueAmountResponseModel>("GetMultiAccountDueAmount", requestParameter);
            });
        }*/

        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges(List<string> accountList)
        {
            //Thread.Sleep(5000);
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                accounts = accountList,
                isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount
            };
            GetAccountsChargesResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountsChargesResponseModel>(BillConstants.Service_GetAccountsCharges, request);
            return response;
        }
    }
}