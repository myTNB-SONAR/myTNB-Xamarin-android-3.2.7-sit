using Foundation;
using System;
using UIKit;
using myTNB.Payment.SelectBills;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model.GetMultiAccountDueAmount;
using System.Collections.Generic;
using myTNB.Model;
using System.Globalization;
using System.Drawing;
using myTNB.Extensions;

namespace myTNB
{
    public partial class SelectBillsViewController : UIViewController
    {
        public SelectBillsViewController(IntPtr handle) : base(handle)
        {
        }

        public double SelectedAccountDueAmount = 0;

        string selectedAccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
        MultiAccountDueAmountResponseModel _multiAccountDueAmount = new MultiAccountDueAmountResponseModel();
        List<CustomerAccountRecordModel> _accounts = new List<CustomerAccountRecordModel>();
        List<CustomerAccountRecordModel> _accountsForDisplay = new List<CustomerAccountRecordModel>();
        CustomerAccountRecordModel _selectedAccount = new CustomerAccountRecordModel();
        public List<CustomerAccountRecordModel> _accountsForPayment = new List<CustomerAccountRecordModel>();

        UIView _viewAmount;
        UILabel _lblCurrency;
        public UILabel _lblTotalAmountValue;
        UIView _viewFooter;

        int loadMoreCount = 0;
        int lastStartIndex = 0;
        int lastEndIndex = 0;
        bool isViewDidLoad = false;
        public double totalAmount = 0.00;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetDefaultTableFrame();
            InitializedSubViews();
            AddBackButton();
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate._selectBillsVC = this;
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                Console.WriteLine("Keyboard Show");
                var userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                SelectBillsTableView.Frame = new CGRect(0
                                                        , 0
                                                        , View.Frame.Width
                                                        , View.Frame.Height - (keyboardRectangle.Height));
            });
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidHideNotification, (NSNotification obj) =>
            {
                Console.WriteLine("Keyboard Hide");
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
                    UpdateFromCachedDues(accountsForQuery);
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

        /// <summary>
        /// Updates from cached dues.
        /// </summary>
        /// <param name="accountsForQuery">Accounts for query.</param>
        private void UpdateFromCachedDues(List<string> accountsForQuery)
        {
            var acctsToRemove = new List<string>();

            foreach (var accNum in accountsForQuery)
            {
                var acct = DataManager.DataManager.SharedInstance.GetDue(accNum);
                if (acct != null)
                {
                    // update due values
                    int itemIndex = _accounts.FindIndex(x => x.accNum.Equals(accNum));
                    if (itemIndex > -1)
                    {
                        _accounts[itemIndex].Amount = acct.amountDue;
                        _accounts[itemIndex].AmountDue = acct.amountDue;

                        acctsToRemove.Add(accNum);

                        int displayIndex = _accountsForDisplay.FindIndex(x => x.accNum.Equals(accNum));

                        if (displayIndex > -1)
                        {
                            _accountsForDisplay[itemIndex].Amount = acct.amountDue;
                            _accountsForDisplay[itemIndex].AmountDue = acct.amountDue;
                        }
                        else
                        {
                            _accountsForDisplay.Add(_accounts[itemIndex]);
                        }
                    }
                }
            }

            if (acctsToRemove?.Count > 0)
            {
                accountsForQuery.RemoveAll(x => acctsToRemove.FindIndex(removeItem => removeItem == x) > -1);
            }

        }

        void SetDefaultTableFrame()
        {
            SelectBillsTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (136));
        }

        void ResetValues()
        {
            SelectBillsTableView.Source = new SelectBillsDataSource(this, new List<CustomerAccountRecordModel>());
            SelectBillsTableView.ReloadData();
            _multiAccountDueAmount = new MultiAccountDueAmountResponseModel();
            _accounts = new List<CustomerAccountRecordModel>();
            _accountsForDisplay = new List<CustomerAccountRecordModel>();
            _selectedAccount = new CustomerAccountRecordModel();
            lastStartIndex = 0;
            lastEndIndex = 0;
            loadMoreCount = 0;
        }

        void UpdateAccountListWithAmount()
        {
            foreach (var item in _multiAccountDueAmount.d.data)
            {
                int itemIndex = _accounts.FindIndex(x => x.accNum.Equals(item.accNum));
                if (itemIndex > -1)
                {
                    _accounts[itemIndex].Amount = item.amountDue;
                    _accounts[itemIndex].AmountDue = item.amountDue;
                    //if (item.amountDue > 0)
                    //{
                    _accountsForDisplay.Add(_accounts[itemIndex]);
                    //}
                }
            }
        }

        void GetAccountsForDisplay()
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
            int selectedAccountIndex = _accounts.FindIndex(x => x.accNum.Equals(selectedAccountNumber));
            if (selectedAccountIndex > -1)
            {
                _selectedAccount = _accounts[selectedAccountIndex];
                _selectedAccount.IsAccountSelected = SelectedAccountDueAmount > 0;//true;
                _accounts.RemoveAt(selectedAccountIndex);
                _accounts.Insert(0, _selectedAccount);
            }
        }

        List<string> GetAccountsForQuery(int start, int end)
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
            UpdateFromCachedDues(accountsForQuery);
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
                if (item.IsAccountSelected)
                {
                    selectedAccountCount++;
                    //FOR UAT TESTING
                    /*if(item.Amount > 0 && item.Amount >= item.AmountDue){
                        totalAmount += item.Amount;
                    }else{
                        hasInvalidSelection = true;
                    }*/

                    if (item.Amount < TNBGlobal.PaymentMinAmnt)
                    {
                        hasInvalidSelection = true;
                    }

                    totalAmount += item.Amount;
                }
            }
            _lblTotalAmountValue.Text = totalAmount.ToString("N2", CultureInfo.InvariantCulture);
            AdjustAmountFrame();
            var title = (selectedAccountCount > 0) ? string.Format("PayBillBtnMltple".Translate(), selectedAccountCount.ToString()) : "PayBillBtnSngle".Translate();
            BtnPayBill.SetTitle(title, UIControlState.Normal);

            bool isValid = (selectedAccountCount > 0 && totalAmount > 0) && !hasInvalidSelection;

            BtnPayBill.BackgroundColor = isValid
                ? myTNBColor.FreshGreen()
                : myTNBColor.SilverChalice();
            BtnPayBill.Enabled = isValid;
        }

        void OnGetMultiAccountDueAmountServiceCall(List<string> accountsForQuery)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        GetMultiAccountDueAmount(accountsForQuery).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_multiAccountDueAmount != null && _multiAccountDueAmount.d != null
                                    && _multiAccountDueAmount.d.data != null)
                                {
                                    UpdateAccountListWithAmount();
                                    UpdateDuesDisplay();
                                    ActivityIndicator.Hide();
                                }
                                else
                                {
                                    UpDateTotalAmount();
                                    string msg = _multiAccountDueAmount?.d?.message;
                                    var alert = UIAlertController.Create(string.Empty, !string.IsNullOrEmpty(msg) ? msg : "DefaultServerErrorMessage".Translate(), UIAlertControllerStyle.Alert);
                                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                    PresentViewController(alert, animated: true, completionHandler: null);
                                    ActivityIndicator.Hide();
                                }
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
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
            BtnPayBill.BackgroundColor = myTNBColor.SilverChalice();
            BtnPayBill.Layer.CornerRadius = 4.0f;
            BtnPayBill.SetTitleColor(UIColor.White, UIControlState.Normal);
            BtnPayBill.TitleLabel.Font = myTNBFont.MuseoSans16_500();
            BtnPayBill.TouchUpInside += (sender, e) =>
            {
                _accountsForPayment = new List<CustomerAccountRecordModel>();
                foreach (var item in _accountsForDisplay)
                {
                    if (item.IsAccountSelected)
                    {
                        _accountsForPayment.Add(item);
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

            BottomContainerView.BackgroundColor = myTNBColor.LightGrayBG();

            _viewAmount = new UIView(new CGRect(18, 20, View.Frame.Width - 36, 24));

            UILabel lblTotalAmountTitle = new UILabel(new CGRect(0, 6, 120, 18));
            lblTotalAmountTitle.TextColor = myTNBColor.TunaGrey();
            lblTotalAmountTitle.Font = myTNBFont.MuseoSans14_500();
            lblTotalAmountTitle.Text = "Total Amount";

            _lblCurrency = new UILabel(new CGRect(0, 6, 24, 18));
            _lblCurrency.TextColor = myTNBColor.TunaGrey();
            _lblCurrency.Font = myTNBFont.MuseoSans14_500();
            _lblCurrency.Text = "RM";
            _lblCurrency.TextAlignment = UITextAlignment.Right;

            _lblTotalAmountValue = new UILabel(new CGRect(0, 0, (View.Frame.Width - 36) / 2, 24));
            _lblTotalAmountValue.TextColor = myTNBColor.TunaGrey();
            _lblTotalAmountValue.Font = myTNBFont.MuseoSans24_500();
            _lblTotalAmountValue.Text = "0.00";
            _lblTotalAmountValue.TextAlignment = UITextAlignment.Right;

            _viewAmount.AddSubviews(lblTotalAmountTitle, _lblCurrency, _lblTotalAmountValue);
            AdjustAmountFrame();
            BottomContainerView.AddSubview(_viewAmount);

            _viewFooter = new UIView(new CGRect(0, 0, View.Frame.Width, 40));
            _viewFooter.BackgroundColor = myTNBColor.LightGrayBG();
            _viewFooter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnLoadMore();
            }));
            UILabel lblLoadMore = new UILabel(new CGRect(0, 0, _viewFooter.Frame.Width, 16));
            lblLoadMore.TextAlignment = UITextAlignment.Center;
            lblLoadMore.AttributedText = new NSAttributedString("Load More"
                , font: myTNBFont.MuseoSans12_300()
                , foregroundColor: myTNBColor.SilverChalice()
                , strokeWidth: 0
                , underlineStyle: NSUnderlineStyle.Single
            );

            _viewFooter.AddSubview(lblLoadMore);
            _viewFooter.Hidden = true;
            SelectBillsTableView.TableFooterView = _viewFooter;
        }

        void AdjustAmountFrame()
        {
            CGSize newSize = GetLabelSize(_lblTotalAmountValue
                                          , _viewAmount.Frame.Width / 2
                                          , 24);
            _lblTotalAmountValue.Frame = new CGRect(_viewAmount.Frame.Width - newSize.Width - 9
                                                    , 0
                                                    , newSize.Width
                                                    , 24);
            _lblCurrency.Frame = new CGRect(_lblTotalAmountValue.Frame.X - 27
                                            , _lblCurrency.Frame.Y
                                            , _lblCurrency.Frame.Width
                                            , _lblCurrency.Frame.Height);
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                View.EndEditing(true);
                DismissViewController(true, null);
                ResetValues();
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal Task GetMultiAccountDueAmount(List<string> accountList)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accounts = accountList
                };
                _multiAccountDueAmount = serviceManager.GetMultiAccountDueAmount("GetMultiAccountDueAmount", requestParameter);
            });
        }
    }
}