using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using UIKit;
using myTNB.SQLite.SQLiteDataManager;
using System.Diagnostics;

namespace myTNB.Registration.CustomerAccounts
{
    public partial class AccountsViewController : UIViewController
    {
        UIButton btnAddAccount;
        UIButton btnConfirm;
        UIView _progressView;
        public bool isDashboardFlow = false;
        const int accountLimit = 10;

        public AccountsViewController(IntPtr handle) : base(handle)
        {
        }
        CustomerAccountResponseModel _customerAccountResponseModel = new CustomerAccountResponseModel();
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        CustomerAccountResponseModel _addMultipleSupplyAccountsResponseModel = new CustomerAccountResponseModel();
        public bool _needsUpdate = false;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = "Common_AddElectricityAccount".Translate();
            InitializedSubviews();
            SetStaticFields();
            SetEvents();
            lblDetails.Hidden = true;
            btnAddAccount.Hidden = true;
            btnConfirm.Hidden = true;
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel
                {
                    d = new List<CustomerAccountRecordModel>()
                };

                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                PresentViewController(loginVC, true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (isDashboardFlow)
            {
                UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    DataManager.DataManager.SharedInstance.AccountsToBeAddedList = new CustomerAccountRecordListModel();
                    DismissViewController(true, null);
                });
                NavigationItem.LeftBarButtonItem = btnBack;
            }
            else
            {
                AddBackButton();
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_needsUpdate)
            {
                ActivityIndicator.Show();
                btnAddAccount.Hidden = true;
                btnConfirm.Hidden = true;
                accountRecordsTableView.Hidden = true;
                lblSubDetails.Hidden = true;
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteGetCustomerRecordsCall();
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                            ActivityIndicator.Hide();
                        }
                    });
                });
            }
            else
            {
                SetAccountTable();
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _needsUpdate = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        internal void SetAccountTable()
        {
            accountRecordsTableView.Source = new AccountRecordDataSource(DataManager.DataManager.SharedInstance.AccountsToBeAddedList, this);
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            accountRecordsTableView.BackgroundColor = MyTNBColor.SectionGrey;
            accountRecordsTableView.ReloadData();
            int length = DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count ?? 0;
            btnAddAccount.Hidden = (length >= accountLimit);
            btnConfirm.Hidden = false;
            SetRecordCount();
        }

        internal void ReloadTableViewData()
        {
            accountRecordsTableView.ReloadData();
        }

        internal void SetStaticFields()
        {
            SetRecordCount();
            lblSubDetails.Text = "Registration_NoAccountsFoundMessage".Translate();
        }

        internal void SetRecordCount()
        {
            int recordCount = (int)(DataManager.DataManager.SharedInstance.AccountRecordsList != null
                                         && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                                         ? DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count : 0);
            lblDetails.Hidden = false;
            lblDetails.Text = recordCount > 0 ? string.Format("{0} {1}", recordCount.ToString()
                , "Registration_SupplyAccountCount".Translate())
                : "Registration_NoAccountsFoundMessage".Translate();
            lblSubDetails.Hidden = recordCount == 0;
        }

        internal void SetEvents()
        {
            btnAddAccount.TouchUpInside += (sender, e) =>
            {
                NavigateToPage("AccountRecords", "SelectAccountByICNumberViewController");
            };

            btnConfirm.TouchUpInside += (sender, e) =>
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null
                               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count > 0)
                            {
                                if (IsEmptyNicknameExist())
                                {
                                    AlertHandler.DisplayGenericAlert(this, string.Empty, "Invalid_NicknameEntry".Translate());
                                }
                                else
                                {
                                    ExecuteAddMultipleSupplyAccountsToUserReg();
                                }
                            }
                            else
                            {
                                if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                                   || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                                {
                                    DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                                    DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                                }

#if true // CREATE_TABBAR
                                if (isDashboardFlow)
                                {
                                    ViewHelper.DismissControllersAndSelectTab(this, 0, true);
                                }
                                else
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                    UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                    PresentViewController(loginVC, true, null);
                                }

#else
                                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                PresentViewController(loginVC, true, null);
#endif
                            }
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                    });
                });
            };
        }

        /// <summary>
        /// Updates the control states.
        /// </summary>
        public void UpdateControlStates()
        {
            var txtFieldHelper = new TextFieldHelper();
            bool isValid = true;
            for (int i = 0; i < DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count; i++)
            {
                var acc = DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[i];
                bool isFormatValid = !string.IsNullOrWhiteSpace(acc.accountNickName)
                    && txtFieldHelper.ValidateTextField(acc.accountNickName, TNBGlobal.ACCOUNT_NAME_PATTERN);
                bool isUnique = DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(acc.accountNickName, acc.accNum);
                isValid = isFormatValid && isUnique;
                if (!isValid)
                {
                    break;
                }
            }

            if (!btnConfirm.Enabled && isValid)
            {
                try
                {
                    // if previously invalid and now all are valid, reset control states
                    accountRecordsTableView.BeginUpdates();
                    nuint count = (nuint)accountRecordsTableView.NumberOfSections();
                    for (nuint i = 0; i < count; i++)
                    {
                        accountRecordsTableView.ReloadSections(new NSIndexSet(i), UITableViewRowAnimation.None);
                    }
                    accountRecordsTableView.EndUpdates();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("accounts table exception: " + ex.Message);
                }
            }

            btnConfirm.Enabled = isValid;
            btnConfirm.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        internal bool IsEmptyNicknameExist()
        {
            for (int i = 0; i < DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count; i++)
            {
                CustomerAccountRecordModel acc = DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[i];
                if (string.IsNullOrEmpty(acc.accountNickName) || string.IsNullOrWhiteSpace(acc.accountNickName))
                {
                    return true;
                }
            }
            return false;
        }

        internal void ExecuteGetCustomerRecordsCall()
        {

            GetCustomerRecords().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_customerAccountResponseModel?.d != null
                       && _customerAccountResponseModel?.d?.didSucceed == true
                       && _customerAccountResponseModel?.d?.status == "success")
                    {
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d = _customerAccountResponseModel?.d?.data;
                        int currentCount = ServiceCall.GetAccountListCount();
                        int index = currentCount + 1;
                        foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d)
                        {
                            item.accDesc = string.Format("{0} {1}", "Common_Account".Translate(), index);
                            item.accountNickName = string.Format("{0} {1}", "Common_Account".Translate(), index);
                            index++;
                        }
                        SetAccountTable();
                        accountRecordsTableView.Hidden = false;
                        if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                            && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null
                            && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count == 0)
                        {
                            NavigateToPage("AccountRecords", "SelectAccountByICNumberViewController");
                        }
                    }
                    else
                    {
                        if (!isDashboardFlow)
                        {
                            AlertHandler.DisplayServiceError(this, _customerAccountResponseModel?.d?.message, (obj) =>
                             {
                                 if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                                    || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                                 {
                                     DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                                     DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                                 }
                                 UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                 UIViewController homeTabBarVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                 PresentViewController(homeTabBarVC, true, null);
                             });
                        }
                        else
                        {
                            AlertHandler.DisplayServiceError(this, _customerAccountResponseModel?.d?.message, (obj) =>
                            {
                                DismissViewController(true, null);
                            });
                        }
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task GetCustomerRecords()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();

                var currentLinkedAccountNumbers = string.Empty;
                List<String> _linkedAccountNumbers = new List<string>();

                if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                    && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                   && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
                {

                    List<CustomerAccountRecordModel> linkedAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d;
                    CustomerAccountRecordModel _linkedAccount = new CustomerAccountRecordModel();

                    var counter = 1;
                    foreach (var acct in linkedAccountList)
                    {
                        if (linkedAccountList.Count > 1)
                        {
                            _linkedAccountNumbers.Add(acct.accNum);

                            if (counter < linkedAccountList.Count)
                            {
                                currentLinkedAccountNumbers += acct.accNum + ",";
                            }
                            else
                            {
                                currentLinkedAccountNumbers += acct.accNum + string.Empty;
                            }
                        }
                        else if (linkedAccountList.Count == 1)
                        {
                            currentLinkedAccountNumbers = acct.accNum;
                        }
                        counter++;
                    }
                }

                var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0] : default(UserEntity);

                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    currentLinkedAccounts = currentLinkedAccountNumbers,
                    email = user?.email,
                    identificationNo = user?.identificationNo
                };
                _customerAccountResponseModel = serviceManager.OnExecuteAPI<CustomerAccountResponseModel>("GetCustomerAccountsForICNum", requestParameter);
            });
        }



        internal void ExecuteAddMultipleSupplyAccountsToUserReg()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DisplayProgessView((int)DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count);
                        AddMultipleSupplyAccountsToUserReg().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_addMultipleSupplyAccountsResponseModel != null && _addMultipleSupplyAccountsResponseModel?.d != null
                                    && _addMultipleSupplyAccountsResponseModel?.d?.data != null
                                    && _addMultipleSupplyAccountsResponseModel?.d?.didSucceed == true)
                                {
                                    var count = DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count;
                                    DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d = _addMultipleSupplyAccountsResponseModel?.d?.data;

                                    UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                                    AddAccountSuccessViewController addAccountSuccessVC
                                        = storyBoard.InstantiateViewController("AddAccountSuccessViewController") as AddAccountSuccessViewController;
                                    if (addAccountSuccessVC != null)
                                    {
                                        addAccountSuccessVC.CreateNewlyAddedList();
                                        AddRecords();

                                        addAccountSuccessVC.AccountsAddedCount = (int)count;
                                        addAccountSuccessVC.IsDashboardFlow = isDashboardFlow;
                                        PresentViewController(addAccountSuccessVC, true, null);
                                    }
                                    HideProgressView();
                                }
                                else
                                {
                                    HideProgressView();
                                    AlertHandler.DisplayServiceError(this, _addMultipleSupplyAccountsResponseModel?.d?.message);
                                }
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        /// <summary>
        /// Adds the records.
        /// </summary>
        private void AddRecords()
        {

            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null)
            {
                if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                   || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                {
                    DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                    DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                }
                foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d)
                {
                    int itemIndex = DataManager.DataManager.SharedInstance.AccountRecordsList
                                               .d.FindIndex(x => x.accNum.Equals(item.accNum));
                    if (itemIndex == -1)
                    {
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d.Insert(0, item);
                    }
                }
            }
            UserAccountsEntity uaManager = new UserAccountsEntity();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
            {
                uaManager.DeleteTable();
                uaManager.CreateTable();
                uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);

                // updates the cache when user entity data is updated
                DataManager.DataManager.SharedInstance.RefreshDataFromAccountUpdate();
            }
        }

        internal Task AddMultipleSupplyAccountsToUserReg()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();

                List<CustomerAccountRecordModel> accountList = DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d;

                if (accountList != null)
                {
                    List<object> billAccs = new List<object>();
                    foreach (var item in accountList)
                    {
                        billAccs.Add(new
                        {
                            accNum = item.accNum ?? string.Empty,
                            accountTypeId = item.accountTypeId ?? "1",
                            accountStAddress = item.accountStAddress ?? string.Empty,
                            icNum = item.icNum ?? string.Empty,
                            item.isOwned,
                            accountNickName = item.accountNickName ?? string.Empty,
                            accountCategoryId = item.accountCategoryId ?? string.Empty
                        });
                    }

                    var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                        ? DataManager.DataManager.SharedInstance.UserEntity[0] : default(UserEntity);

                    object requestParams = new
                    {
                        apiKeyID = TNBGlobal.API_KEY_ID,
                        sspUserId = DataManager.DataManager.SharedInstance.User?.UserID,
                        billAccounts = billAccs,
                        user?.email
                    };
                    _addMultipleSupplyAccountsResponseModel = serviceManager.OnExecuteAPI<CustomerAccountResponseModel>("AddMultipleSupplyAccountsToUserReg", requestParams);
                }
            });

        }

        internal Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.OnExecuteAPI<BillingAccountDetailsResponseModel>("GetBillingAccountDetails", requestParameter);
            });
        }

        internal void ShowPrelogin()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(preloginVC, true, null);
        }

        internal void NavigateToPage(string storyboardName, string viewControllerName)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
            NavigationController?.PushViewController(viewController, true);
        }

        internal void InitializedSubviews()
        {

            var footerView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 335));
            footerView.BackgroundColor = UIColor.Clear;
            accountRecordsTableView.TableFooterView = footerView;

            btnAddAccount = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, 24, View.Frame.Width - 36, 48),
                BackgroundColor = UIColor.Clear
            };
            btnAddAccount.Layer.BorderWidth = 1.0f;
            btnAddAccount.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnAddAccount.Layer.CornerRadius = 5.0f;
            btnAddAccount.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
            btnAddAccount.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            footerView.AddSubview(btnAddAccount);

            btnConfirm = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, 78, View.Frame.Width - 36, 48),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            btnConfirm.Layer.CornerRadius = 5.0f;
            btnConfirm.SetTitle("Common_Confirm".Translate(), UIControlState.Normal);
            btnConfirm.SetTitleColor(UIColor.White, UIControlState.Normal);
            footerView.AddSubview(btnConfirm);
        }

        void DisplayProgessView(int count)
        {
            _progressView = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = new UIColor(0, .75F)
            };

            UIView progressContainer = new UIView(new CGRect((_progressView.Frame.Width / 2) - 127
                , (_progressView.Frame.Height / 2) - 87, 254, 174))
            {
                BackgroundColor = UIColor.White,
                Alpha = 1F
            };
            progressContainer.Layer.CornerRadius = 4;

            UILabel lblTitle = new UILabel(new CGRect(13, 23, progressContainer.Frame.Width - 26, 18));
            lblTitle.Font = MyTNBFont.MuseoSans16;
            lblTitle.TextColor = MyTNBColor.PowerBlue;
            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.Text = string.Format(count > 1 ? "Registration_AddingAccountsTitle".Translate()
                : "Registration_AddingAccountTitle".Translate(), count);

            UIImageView imgLoading = new UIImageView(new CGRect(105, 57, 44, 44))
            {
                Image = UIImage.FromBundle("Loading-Circle")
            };

            CABasicAnimation rotationAnimation = new CABasicAnimation
            {
                KeyPath = "transform.rotation.z",
                To = new NSNumber(Math.PI * 2),
                Duration = 2,
                Cumulative = true,
                RepeatCount = float.MaxValue
            };
            imgLoading.Layer.AddAnimation(rotationAnimation, "rotationAnimation");

            UILabel lblSubTitle = new UILabel(new CGRect(13, 112, progressContainer.Frame.Width - 26, 36))
            {
                Font = MyTNBFont.MuseoSans14,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Center,
                Lines = 2,
                Text = count > 1 ? "Registration_AddingAccountsMessage".Translate()
                : "Registration_AddingAccountMessage".Translate()
            };

            progressContainer.AddSubviews(new UIView[] { lblTitle, imgLoading, lblSubTitle });
            _progressView.AddSubview(progressContainer);

            UIApplication.SharedApplication.KeyWindow.AddSubview(_progressView);
            _progressView.Hidden = false;
        }

        void HideProgressView()
        {
            if (_progressView != null)
            {
                _progressView.Hidden = true;
                _progressView.RemoveFromSuperview();
            }
        }
    }
}