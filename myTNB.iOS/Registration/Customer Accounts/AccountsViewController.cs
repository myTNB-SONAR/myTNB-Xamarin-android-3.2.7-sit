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
    public partial class AccountsViewController : CustomUIViewController
    {
        public AccountsViewController(IntPtr handle) : base(handle) { }

        private UIButton btnAddAccount, btnConfirm;
        private UIView _progressView, _footerView;
        public bool isDashboardFlow;

        private CustomerAccountResponseModel _customerAccountResponseModel = new CustomerAccountResponseModel();
        private CustomerAccountResponseModel _addMultipleSupplyAccountsResponseModel = new CustomerAccountResponseModel();
        public bool _needsUpdate;

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            InitializeViews();
            SetStaticFields();
            SetEvents();
            lblDetails.Hidden = true;
            btnAddAccount.Hidden = true;
            btnConfirm.Hidden = true;
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(AddAccountConstants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel
                {
                    d = new List<CustomerAccountRecordModel>()
                };

                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(loginVC, true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (isDashboardFlow)
            {
                UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(AddAccountConstants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
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
                            DisplayNoDataAlert();
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
        }

        internal void SetAccountTable()
        {
            accountRecordsTableView.Frame = new CGRect(0, 0, ViewWidth, View.Frame.Height - _footerView.Frame.Height);
            accountRecordsTableView.Source = new AccountRecordDataSource(DataManager.DataManager.SharedInstance.AccountsToBeAddedList, this)
            {
                GetI18NValue = GetI18NValue
            };
            accountRecordsTableView.ReloadData();

            int length = DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count ?? 0;
            btnAddAccount.Hidden = length >= AddAccountConstants.Int_AccountLimit;
            btnConfirm.Hidden = false;
            SetRecordCount();
        }

        private void SetStaticFields()
        {
            SetRecordCount();
            lblSubDetails.Text = GetI18NValue(AddAccountConstants.I18N_NoAcctFoundMsg);
        }

        private void SetRecordCount()
        {
            int recordCount = (int)(DataManager.DataManager.SharedInstance.AccountRecordsList != null
                                         && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                                         ? DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count : 0);
            lblDetails.Hidden = false;
            lblDetails.Text = recordCount > 0 ? string.Format("{0} {1}", recordCount.ToString()
                , GetI18NValue(AddAccountConstants.I18N_SupplyAcctCount))
                : GetI18NValue(AddAccountConstants.I18N_NoAcctFoundMsg);
            lblSubDetails.Hidden = recordCount == 0;
        }

        private void SetEvents()
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
                                    DisplayGenericAlert(string.Empty, GetErrorI18NValue(AddAccountConstants.I18N_EmptyNickname));
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
                                if (isDashboardFlow)
                                {
                                    ViewHelper.DismissControllersAndSelectTab(this, 0, true);
                                }
                                else
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                    UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                    loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                    PresentViewController(loginVC, true, null);
                                }
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
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

        private bool IsEmptyNicknameExist()
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

        private void ExecuteGetCustomerRecordsCall()
        {
            GetCustomerRecords().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_customerAccountResponseModel != null && _customerAccountResponseModel?.d != null
                       && _customerAccountResponseModel.d.IsSuccess)
                    {
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d = _customerAccountResponseModel?.d?.data;
                        int currentCount = ServiceCall.GetAccountListCount();
                        int index = currentCount + 1;
                        foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d)
                        {
                            item.accDesc = string.Format("{0} {1}", GetCommonI18NValue(AddAccountConstants.I18N_Account), index);
                            item.accountNickName = string.Format("{0} {1}", GetCommonI18NValue(AddAccountConstants.I18N_Account), index);
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
                            DisplayServiceError(_customerAccountResponseModel?.d?.DisplayMessage, (obj) =>
                            {
                                if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                                   || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                                {
                                    DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                                    DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                                }
                                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                UIViewController homeTabBarVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                homeTabBarVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                PresentViewController(homeTabBarVC, true, null);
                            });
                        }
                        else
                        {
                            DisplayServiceError(_customerAccountResponseModel?.d?.DisplayMessage, (obj) =>
                           {
                               DismissViewController(true, null);
                           });
                        }
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task GetCustomerRecords()
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

                UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0] : default(UserEntity);

                object requestParameter = new
                {
                    serviceManager.usrInf,
                    currentLinkedAccounts = currentLinkedAccountNumbers,
                    identificationNo = user?.identificationNo
                };
                _customerAccountResponseModel = serviceManager.OnExecuteAPIV6<CustomerAccountResponseModel>(AddAccountConstants.Service_GetCustomerAccountsForICNum, requestParameter);
            });
        }

        private void ExecuteAddMultipleSupplyAccountsToUserReg()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DisplayProgessView((int)DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count);
                        AddAccounts().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_addMultipleSupplyAccountsResponseModel != null && _addMultipleSupplyAccountsResponseModel?.d != null
                                    && _addMultipleSupplyAccountsResponseModel?.d?.data != null
                                    && _addMultipleSupplyAccountsResponseModel.d.IsSuccess)
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
                                        addAccountSuccessVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                        PresentViewController(addAccountSuccessVC, true, null);
                                    }
                                    HideProgressView();
                                }
                                else
                                {
                                    HideProgressView();
                                    DisplayServiceError(_addMultipleSupplyAccountsResponseModel?.d?.DisplayMessage);
                                }
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
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

        private Task AddAccounts()
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

                    object requestParams = new
                    {
                        billAccounts = billAccs,
                        serviceManager.usrInf
                    };
                    _addMultipleSupplyAccountsResponseModel = serviceManager.OnExecuteAPIV6<CustomerAccountResponseModel>(AddAccountConstants.Service_AddAccounts, requestParams);
                }
            });

        }

        private void NavigateToPage(string storyboardName, string viewControllerName)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
            NavigationController?.PushViewController(viewController, true);
        }

        private void InitializeViews()
        {
            var addtl = DeviceHelper.IsIphoneXUpResolution() ? GetScaledHeight(20) : 0;
            _footerView = new UIView(new CGRect(0, View.Frame.Height - GetScaledHeight(140) - NavigationController.NavigationBar.Frame.Height - addtl, ViewWidth, GetScaledHeight(126)))
            {
                BackgroundColor = UIColor.Clear
            };

            btnAddAccount = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(GetScaledWidth(18), GetScaledHeight(10F), ViewWidth - GetScaledWidth(36), GetScaledHeight(48)),
                BackgroundColor = UIColor.Clear
            };
            btnAddAccount.Layer.BorderWidth = GetScaledWidth(1.0f);
            btnAddAccount.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnAddAccount.Layer.CornerRadius = GetScaledHeight(5.0f);
            btnAddAccount.SetTitle(GetI18NValue(AddAccountConstants.I18N_AddAnotherAcct), UIControlState.Normal);
            btnAddAccount.Font = TNBFont.MuseoSans_16_500;
            btnAddAccount.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _footerView.AddSubview(btnAddAccount);

            btnConfirm = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(GetScaledWidth(18), btnAddAccount.Frame.GetMaxY() + GetScaledHeight(6), ViewWidth - GetScaledWidth(36), GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            btnConfirm.Layer.CornerRadius = GetScaledHeight(5.0f);
            btnConfirm.SetTitle(GetCommonI18NValue(AddAccountConstants.I18N_Confirm), UIControlState.Normal);
            btnConfirm.Font = TNBFont.MuseoSans_16_500;
            btnConfirm.SetTitleColor(UIColor.White, UIControlState.Normal);
            _footerView.AddSubview(btnConfirm);
            View.AddSubview(_footerView);

            accountRecordsTableView.Frame = new CGRect(0, 0, ViewWidth, View.Frame.Height - _footerView.Frame.Height);
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            accountRecordsTableView.BackgroundColor = MyTNBColor.SectionGrey;
        }

        private void DisplayProgessView(int count)
        {
            _progressView = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = new UIColor(0, .75F)
            };

            UIView progressContainer = new UIView(new CGRect((_progressView.Frame.Width / 2) - GetScaledWidth(127)
                , (_progressView.Frame.Height / 2) - GetScaledHeight(87), GetScaledWidth(254), GetScaledHeight(174)))
            {
                BackgroundColor = UIColor.White,
                Alpha = 1F
            };
            progressContainer.Layer.CornerRadius = GetScaledHeight(4);

            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(13), GetScaledHeight(23), progressContainer.Frame.Width - GetScaledWidth(26), GetScaledHeight(18)));
            lblTitle.Font = TNBFont.MuseoSans_16_500;
            lblTitle.TextColor = MyTNBColor.PowerBlue;
            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.Text = string.Format(count > 1 ? GetI18NValue(AddAccountConstants.I18N_AddAccounts)
                : GetI18NValue(AddAccountConstants.I18N_AddAccount), count);

            UIImageView imgLoading = new UIImageView(new CGRect(GetScaledWidth(105), GetScaledHeight(57), GetScaledWidth(44), GetScaledHeight(44)))
            {
                Image = UIImage.FromBundle(AddAccountConstants.IMG_LoadingCircle)
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

            UILabel lblSubTitle = new UILabel(new CGRect(GetScaledWidth(13), GetScaledHeight(112), progressContainer.Frame.Width - GetScaledWidth(26), GetScaledHeight(36)))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Center,
                Lines = 2,
                Text = count > 1 ? GetI18NValue(AddAccountConstants.I18N_AddAccountsMsg)
                : GetI18NValue(AddAccountConstants.I18N_AddAccountMsg)
            };

            progressContainer.AddSubviews(new UIView[] { lblTitle, imgLoading, lblSubTitle });
            _progressView.AddSubview(progressContainer);

            UIApplication.SharedApplication.KeyWindow.AddSubview(_progressView);
            _progressView.Hidden = false;
        }

        private void HideProgressView()
        {
            if (_progressView != null)
            {
                _progressView.Hidden = true;
                _progressView.RemoveFromSuperview();
            }
        }
    }
}