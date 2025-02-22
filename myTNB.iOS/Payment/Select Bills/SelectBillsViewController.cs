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
using myTNB.Payment;

namespace myTNB
{
    public partial class SelectBillsViewController : CustomUIViewController
    {
        public SelectBillsViewController(IntPtr handle) : base(handle) { }

        public double SelectedAccountDueAmount;
        public List<CustomerAccountRecordModel> _accountsForPayment = new List<CustomerAccountRecordModel>();
        public double totalAmount;
        public bool IsFromBillDetails { set; get; }

        private List<CustomerAccountRecordModel> _accounts = new List<CustomerAccountRecordModel>();
        private List<PaymentRecordModel> _accountsForDisplay = new List<PaymentRecordModel>();
        private CustomerAccountRecordModel _selectedAccount = new CustomerAccountRecordModel();
        private GetAccountsChargesResponseModel _accountChargesResponse = new GetAccountsChargesResponseModel();
        private UIView _viewAmount, _viewFooter;
        private UILabel _lblTotalAmountValue, _lblCurrency;
        private string _selectedAccountNumber = string.Empty;
        private int _loadMoreCount, _lastStartIndex, _lastEndIndex;
        private bool _isViewDidLoad;
        private bool _isLoadmore;

        internal List<string> _displayedPopup;

        public override void ViewDidLoad()
        {
            PageName = PaymentConstants.Pagename_SelectBills;
            base.ViewDidLoad();
            Title = GetI18NValue(PaymentConstants.I18N_Title);
            _displayedPopup = new List<string>();
            AccountChargesCache.Clear();
            SetDefaultTableFrame();
            InitializedSubViews();
            AddBackButton();
            AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            if (appDelegate != null)
            {
                appDelegate._selectBillsVC = this;
            }
            NotifCenterUtility.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                NSDictionary userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                SelectBillsTableView.Frame = new CGRect(0, 0, View.Frame.Width
                    , View.Frame.Height - (keyboardRectangle.Height));
            });
            NotifCenterUtility.AddObserver(UIKeyboard.DidHideNotification, (NSNotification obj) =>
            {
                SetDefaultTableFrame();
            });

            _isViewDidLoad = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            BtnPayBill.SetTitle(GetI18NValue(PaymentConstants.I18N_PaySingle), UIControlState.Normal);
            if (_isViewDidLoad)
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
                _isViewDidLoad = false;
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
            _accounts = new List<CustomerAccountRecordModel>();
            _accountsForDisplay = new List<PaymentRecordModel>();
            _selectedAccount = new CustomerAccountRecordModel();
            _lastStartIndex = 0;
            _lastEndIndex = 0;
            _loadMoreCount = 0;
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
        }

        private void UpdateAccountsForDisplay(CustomerAccountRecordModel customerAccount
            , AccountChargesModel dueAmtData)
        {
            _accountsForDisplay.Add(new PaymentRecordModel
            {
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
            foreach (CustomerAccountRecordModel item in DataManager.DataManager.SharedInstance.AccountRecordsList.d)
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
                _selectedAccount.IsAccountSelected = true;
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
                int accountChargesIndex = _accountChargesResponse.d.data.AccountCharges.FindIndex(x => x.ContractAccount.Equals(_selectedAccount.accNum));
                if (accountChargesIndex > -1)
                {
                    _selectedAccount.IsAccountSelected = _accountChargesResponse.d.data.AccountCharges[selectedAccountIndex].AmountDue > 0;//SelectedAccountDueAmount > 0;//true;
                }
            }
        }

        private List<string> GetAccountsForQuery(int start, int end)
        {
            _lastStartIndex = start;
            _lastEndIndex = end;
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

        private void OnLoadMore()
        {
            _isLoadmore = true;
            View.EndEditing(true);
            _lastStartIndex += (_loadMoreCount > 0 ? 4 : 5);
            _lastEndIndex = _lastStartIndex + 4;
            List<string> accountsForQuery = GetAccountsForQuery(_lastStartIndex, _lastEndIndex);
            if (accountsForQuery?.Count > 0)
            {
                ActivityIndicator.Show();
                OnGetMultiAccountDueAmountServiceCall(accountsForQuery);
            }
            else
            {
                UpdateDuesDisplay();
            }
            _loadMoreCount++;
        }

        internal void UpDateTotalAmount()
        {
            totalAmount = 0;
            int selectedAccountCount = 0;
            bool hasInvalidSelection = false;
            foreach (PaymentRecordModel item in _accountsForDisplay)
            {
                if (_accountChargesResponse != null && _accountChargesResponse.d != null && _accountChargesResponse.d.IsSuccess
                       && _accountChargesResponse.d.data != null && _accountChargesResponse.d.data.AccountCharges != null
                       && item.IsAccountSelected)
                {
                    selectedAccountCount++;
                    if (item.Amount < TNBGlobal.PaymentMinAmnt)
                    {
                        hasInvalidSelection = true;
                    }
                    /*if (AccountChargesCache.HasMandatory(item.accNum))
                    {
                        MandatoryChargesModel mandatoryCharges = AccountChargesCache.GetMandatoryCharges(item.accNum);
                        if (item.Amount < mandatoryCharges.TotalAmount)
                        {
                            hasInvalidSelection = true;
                        }
                    }*/

                    totalAmount += item.Amount;
                }
            }
            _lblTotalAmountValue.Text = totalAmount.ToString("N2", CultureInfo.InvariantCulture);
            AdjustAmountFrame();
            string title = (selectedAccountCount > 0) ? string.Format(GetI18NValue(PaymentConstants.I18N_PayMultiple)
                , selectedAccountCount.ToString()) : GetI18NValue(PaymentConstants.I18N_PaySingle);
            BtnPayBill.SetTitle(title, UIControlState.Normal);

            bool isValid = (selectedAccountCount > 0 && totalAmount > 0) && !hasInvalidSelection;

            BtnPayBill.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            BtnPayBill.Enabled = isValid;
        }

        internal void OnShowItemisedTooltip(string accNum)
        {
            bool isExist = _displayedPopup.FindIndex(x => x == accNum) > -1;
            if (!isExist)
            {
                _displayedPopup.Add(accNum);
            }
            PopupModel popupData = AccountChargesCache.GetPopupDetailsByType(BillConstants.Popup_MandatoryPaymentKey);
            MandatoryChargesModel mandatoryCharges = AccountChargesCache.GetMandatoryCharges(accNum);
            _selectedAccountNumber = accNum;
            string amount = string.Format("{0}{1}", TNBGlobal.UNIT_CURRENCY, mandatoryCharges.TotalAmount.ToString("N2", CultureInfo.InvariantCulture));
            string nickname = accNum;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
            {
                int accIndex = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == accNum);
                if (accIndex > -1 && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.AccountRecordsList.d[accIndex].accountNickName)
                    && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.AccountRecordsList.d[accIndex].accountNickName))
                {
                    nickname = DataManager.DataManager.SharedInstance.AccountRecordsList.d[accIndex].accountNickName;
                    _selectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[accIndex];
                }
            }
            string description = string.Format(popupData.Description, amount, nickname);
            string[] cta = popupData.CTA.Split(',');
            Dictionary<string, Action> ctaDictionary = new Dictionary<string, Action>();
            for (int i = 0; i < cta.Length; i++)
            {
                Action action = null;
                if (i == 0)
                {
                    action = OnViewDetails;
                }
                ctaDictionary.Add(cta[i], action);
            }
            DisplayCustomAlert(popupData.Title, description, ctaDictionary);
        }

        private void OnViewDetails()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
            BillDetailsViewController viewController =
                storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
            if (viewController != null)
            {
                viewController.IsRoot = true;
                viewController.IsFromBillSelection = true;
                viewController.AccountNumber = _selectedAccountNumber;
                viewController.SelectedAccount = _selectedAccount;
                NavigationController.PushViewController(viewController, true);
            }
        }

        private void OnGetMultiAccountDueAmountServiceCall(List<string> accountsForQuery)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        InvokeInBackground(async () =>
                        {
                            _accountChargesResponse = await GetAccountsCharges(accountsForQuery);
                            InvokeOnMainThread(() =>
                            {
                                if (_accountChargesResponse != null && _accountChargesResponse.d != null && _accountChargesResponse.d.IsSuccess
                                    && _accountChargesResponse.d.data != null && _accountChargesResponse.d.data.AccountCharges != null)
                                {
                                    AccountChargesCache.SetData(_accountChargesResponse);
                                    SetInitialSelectedAccount();
                                    UpdateAccountListWithAmount();
                                    UpdateDuesDisplay();
                                }
                                else
                                {
                                    UpDateTotalAmount();
                                    DisplayServiceError(_accountChargesResponse?.d?.DisplayMessage ?? string.Empty);
                                }
                                ActivityIndicator.Hide();
                            });
                        });

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

        private void InitializedTableView()
        {
            SelectBillsTableView.Source = new SelectBillsDataSource(this, _accountsForDisplay);
            SelectBillsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            SelectBillsTableView.ReloadData();
            if (_isLoadmore) { return; }
            if (_accountsForDisplay != null && _accountsForDisplay.Count > 0 && _accountsForDisplay[0] != null)
            {
                if (AccountChargesCache.HasMandatory(_accountsForDisplay[0].accNum) && !IsFromBillDetails)
                {
                    OnShowItemisedTooltip(_accountsForDisplay[0].accNum);
                }
            }
        }

        private void InitializedSubViews()
        {
            BottomContainerView.BackgroundColor = UIColor.White;

            BtnPayBill.BackgroundColor = MyTNBColor.SilverChalice;
            BtnPayBill.Layer.CornerRadius = 4.0f;
            BtnPayBill.SetTitleColor(UIColor.White, UIControlState.Normal);
            BtnPayBill.TitleLabel.Font = TNBFont.MuseoSans_16_500;
            BtnPayBill.TouchUpInside += (sender, e) =>
            {
                _accountsForPayment = new List<CustomerAccountRecordModel>();
                foreach (PaymentRecordModel item in _accountsForDisplay)
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
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            };

            _viewAmount = new UIView(new CGRect(18, 20, View.Frame.Width - 36, 24));

            UILabel lblTotalAmountTitle = new UILabel(new CGRect(0, 6, _viewAmount.Frame.Width / 2, 18))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                Text = GetCommonI18NValue(Constants.Common_TotalAmount)
            };

            _lblCurrency = new UILabel(new CGRect(0, 6, 24, 18))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                Text = TNBGlobal.UNIT_CURRENCY,
                TextAlignment = UITextAlignment.Right
            };

            _lblTotalAmountValue = new UILabel(new CGRect(0, 0, (View.Frame.Width - 36) / 2, 24))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_24_300,
                Text = TNBGlobal.DEFAULT_VALUE,
                TextAlignment = UITextAlignment.Right
            };

            _viewAmount.AddSubviews(lblTotalAmountTitle, _lblCurrency, _lblTotalAmountValue);
            AdjustAmountFrame();
            BottomContainerView.AddSubview(_viewAmount);

            _viewFooter = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(32)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            _viewFooter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnLoadMore();
            }));
            UILabel lblLoadMore = new UILabel(new CGRect(0, 0, _viewFooter.Frame.Width, GetScaledHeight(16)));
            lblLoadMore.TextAlignment = UITextAlignment.Center;
            lblLoadMore.AttributedText = new NSAttributedString(GetI18NValue(PaymentConstants.I18N_LoadMore)
                , font: TNBFont.MuseoSans_12_300
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

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnBack();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void OnBack()
        {
            View.EndEditing(true);
            ResetValues();
            DismissViewController(true, null);
        }

        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges(List<string> accountList)
        {
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