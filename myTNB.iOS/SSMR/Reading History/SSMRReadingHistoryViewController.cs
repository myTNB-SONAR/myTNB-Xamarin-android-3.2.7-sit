using CoreAnimation;
using CoreGraphics;
using myTNB.Home.Bill;
using myTNB.Model;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadingHistoryViewController : CustomUIViewController
    {
        private SSMRReadingHistoryHeaderComponent _ssmrHeaderComponent;
        private UITableView _readingHistoryTableView;
        private UIView _headerView, _footerView, _navbarView, _viewRefreshContainer;
        private CustomUIButtonV2 _btnDisable;
        private UIImageView _bgImageView;
        private MeterReadingHistoryModel _meterReadingHistory;
        private List<MeterReadingHistoryItemModel> _readingHistoryList;
        private CAGradientLayer _gradientLayer;
        private AccountsSMREligibilityResponseModel _eligibilityAccount;
        private SMRAccountActivityInfoResponseModel _smrActivityInfoResponse;
        private CustomerAccountRecordModel _currAcc;
        private ContactDetailsResponseModel _contactDetails;
        private nfloat _navBarHeight, _previousScrollOffset;
        private nfloat _tableViewOffset = 64f;
        private nfloat titleBarHeight = ScaleUtility.GetScaledHeight(24f);
        private int _currentIndex = -1;
        private bool _isFromSelection;
        private bool _rootNavigation;

        public bool IsFromHome;
        public bool IsFromNotification;
        public bool FromStatusPage;
        public bool IsRoot;
        public SSMRReadingHistoryViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = SSMRConstants.Pagename_SSMRReadingHistory;
            NavigationController.NavigationBarHidden = false;
            base.ViewDidLoad();
            SetNavigation();
            SSMRAccounts.SetFilteredEligibleAccounts();
            PrepareHeaderView();
            PrepareFooterView();
            AddTableView();
            View.BackgroundColor = MyTNBColor.LightGrayBG;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _rootNavigation = false;
            NavigationController.SetNavigationBarHidden(true, true);
            SetNoSSMR();
            string accName;
            if (SSMRAccounts.FilteredListCount == 0)
            {
                _currentIndex = -1;
            }
            else if (_isFromSelection)
            {
                accName = _currAcc?.accountNickName ?? string.Empty;
                _ssmrHeaderComponent.AccountName = accName;
                EvaluateEntry();
            }
            else if (FromStatusPage)
            {
                _currAcc = SSMRActivityInfoCache.SubmittedAccount;
                accName = _currAcc?.accountNickName ?? string.Empty;
                _ssmrHeaderComponent.AccountName = accName;
                EvaluateEntry();
            }
            else
            {
                if (IsFromHome || IsFromNotification)
                {
                    InvokeOnMainThread(async () =>
                    {
                        ActivityIndicator.Show();
                        await GetEligibility();
                        if (_eligibilityAccount != null && _eligibilityAccount.d != null
                            && _eligibilityAccount.d.didSucceed && _eligibilityAccount.d.data != null
                            && _eligibilityAccount.d.data.accountEligibilities != null)
                        {
                            SSMRAccounts.SetData(_eligibilityAccount.d);
                            SSMRAccounts.SetFilteredEligibleAccounts(true);
                            if (SSMRAccounts.HasSSMRAccount)
                            {
                                _currentIndex = 0;
                                _currAcc = SSMRAccounts.GetFirstSSMRAccount();
                                accName = _currAcc?.accountNickName ?? string.Empty;
                                _ssmrHeaderComponent.AccountName = accName;
                                EvaluateEntry();
                            }
                            else
                            {
                                _currentIndex = -1;
                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            DisplayServiceError(_eligibilityAccount?.d?.ErrorMessage ?? string.Empty);
                            ActivityIndicator.Hide();
                        }
                    });
                }
                else
                {
                    _currAcc = DataManager.DataManager.SharedInstance.SelectedAccount;
                    _ssmrHeaderComponent.AccountName = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;
                    UpdateTable();
                }
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (IsFromNotification && !_rootNavigation)
            {
                NavigationController.SetNavigationBarHidden(true, true);
            }
            else
            {
                NavigationController.SetNavigationBarHidden(false, true);
            }
        }

        private void EvaluateEntry()
        {
            if (_currAcc.IsSSMR)
            {
                InvokeOnMainThread(async () =>
                {
                    await LoadSMRAccountActivityInfo(_currAcc);
                });
            }
            else
            {
                SetEnableSSMR();
                ActivityIndicator.Hide();
            }
        }

        private void SetNoSSMR()
        {
            if (_ssmrHeaderComponent != null)
            {
                _ssmrHeaderComponent.SetNoSSMRHeader();
                AdjustHeader();
            }
            _readingHistoryTableView.TableFooterView = null;
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, _readingHistoryList, false);
            _readingHistoryTableView.ReloadData();
        }

        private void SetEnableSSMR()
        {
            _ssmrHeaderComponent.ActionTitle = string.Empty;
            _ssmrHeaderComponent.SetDescription(GetI18NValue(SSMRConstants.I18N_EnableSSMRDescription));
            _ssmrHeaderComponent.SetSubmitButtonHidden(null, true, GetI18NValue(SSMRConstants.I18N_EnableSSMRCTA));
            _ssmrHeaderComponent.OnButtonTap = OnEnableSSMR;
            AdjustHeader();

            _readingHistoryTableView.TableFooterView = null;
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, null, false);
            _readingHistoryTableView.ReloadData();
        }

        #region Navigation
        private void SetNavigation()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
                _navBarHeight = NavigationController.NavigationBar.Frame.Height;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + _navBarHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_ReadingHistoryBanner),
                BackgroundColor = UIColor.White
            };

            View.AddSubview(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8f), _navbarView.Frame.Width, titleBarHeight));

            UIView viewBack = new UIView(new CGRect(BaseMarginWidth16, 0, GetScaledWidth(24F), titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), titleBarHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
            };
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(56F), 0, _navbarView.Frame.Width - (GetScaledWidth(56F) * 2), titleBarHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(SSMRConstants.I18N_NavTitle)
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (IsRoot)
                {
                    NavigationController.PopViewController(true);
                }
                else if (IsFromHome)
                {
                    ViewHelper.DismissControllersAndSelectTab(this, 0, true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            }));

            _navbarView.AddSubview(viewTitleBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _gradientLayer.Opacity = 0f;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }

        private void AddViewWithOpacity(float opacity)
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            gradientLayer.Frame = _navbarView.Bounds;
            gradientLayer.Opacity = opacity;
            _navbarView.Layer.ReplaceSublayer(_gradientLayer, gradientLayer);
            _gradientLayer = gradientLayer;
        }
        #endregion

        private void PrepareHeaderView()
        {
            _headerView = new UIView
            {
                ClipsToBounds = true
            };
            _ssmrHeaderComponent = new SSMRReadingHistoryHeaderComponent(View, _navBarHeight);
            _headerView.AddSubview(_ssmrHeaderComponent.GetUI());
            _ssmrHeaderComponent.SetTitle(GetI18NValue(SSMRConstants.I18N_SubTitle));
            _ssmrHeaderComponent.ActionTitle = _meterReadingHistory?.HistoryViewTitle ?? string.Empty;
            _ssmrHeaderComponent.SetDescription(_meterReadingHistory?.HistoryViewMessage ?? string.Empty);
            _ssmrHeaderComponent.OnButtonTap = ShowSubmitMeterView;
            _ssmrHeaderComponent.DropdownAction = OnTapDropDown;
            AdjustHeader();
        }

        private void PrepareFooterView()
        {
            _footerView = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(104))) { BackgroundColor = MyTNBColor.LightGrayBG };
            UIView viewButton = new UIView(new CGRect(0, GetScaledHeight(24), ViewWidth, GetScaledHeight(80))) { BackgroundColor = UIColor.White };
            _btnDisable = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, BaseMargin, BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_DisableSelfMeterReading
            };
            _btnDisable.SetTitle(GetI18NValue(SSMRConstants.I18N_DisableSSMRCTA), UIControlState.Normal);
            _btnDisable.Layer.BorderColor = MyTNBColor.Tomato.CGColor;
            _btnDisable.Layer.BorderWidth = GetScaledWidth(1);
            _btnDisable.SetTitleColor(MyTNBColor.Tomato, UIControlState.Normal);
            _btnDisable.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnDisableSSMR();
            }));
            viewButton.AddSubview(_btnDisable);
            _footerView.AddSubview(viewButton);
        }

        private void OnSelectAccount(int index)
        {
            if (index > -1)
            {
                _currentIndex = index;
                _currAcc = SSMRAccounts.GetAccountByIndex(index);
                _isFromSelection = true;
            }
        }

        private void AdjustHeader()
        {
            _headerView.Frame = new CGRect(0, 0, _ssmrHeaderComponent.GetView().Frame.Width, _ssmrHeaderComponent.GetView().Frame.GetMaxY());
        }

        private void AddTableView()
        {
            _readingHistoryTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, ViewHeight));
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(NoDataViewCell), Constants.Cell_NoHistoryData);
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, _readingHistoryList);
            _readingHistoryTableView.BackgroundColor = UIColor.Clear;
            _readingHistoryTableView.RowHeight = UITableView.AutomaticDimension;
            _readingHistoryTableView.EstimatedRowHeight = GetScaledHeight(68);
            _readingHistoryTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _readingHistoryTableView.Bounces = false;
            _readingHistoryTableView.SectionFooterHeight = 0;
            _readingHistoryTableView.TableHeaderView = _headerView;
            _readingHistoryTableView.TableFooterView = _footerView;
            View.AddSubview(_readingHistoryTableView);
        }

        private void UpdateTable()
        {
            _meterReadingHistory = IsFromHome || IsFromNotification || _isFromSelection || FromStatusPage
                ? SSMRActivityInfoCache.ViewMeterReadingHistory : SSMRActivityInfoCache.DashboardMeterReadingHistory;
            _readingHistoryList = IsFromHome || IsFromNotification || _isFromSelection || FromStatusPage
                ? SSMRActivityInfoCache.ViewReadingHistoryList : SSMRActivityInfoCache.DashboardReadingHistoryList;

            _ssmrHeaderComponent.SetSubmitButtonHidden(_meterReadingHistory);
            _ssmrHeaderComponent.ActionTitle = _meterReadingHistory?.HistoryViewTitle ?? string.Empty;
            _ssmrHeaderComponent.SetDescription(_meterReadingHistory?.HistoryViewMessage ?? string.Empty);
            _ssmrHeaderComponent.OnButtonTap = ShowSubmitMeterView;
            AdjustHeader();

            _readingHistoryTableView.TableFooterView = _footerView;
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, _readingHistoryList);
            _readingHistoryTableView.ReloadData();
        }

        #region Events
        public void OnTableViewScrolled(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            CGRect frame = _bgImageView.Frame;
            if ((nfloat)Math.Abs(frame.Y) == frame.Height) { return; }

            nfloat newYLoc = 0 - scrollView.ContentOffset.Y;
            frame.Y = newYLoc;
            _bgImageView.Frame = frame;

            _previousScrollOffset = _readingHistoryTableView.ContentOffset.Y;
            var opac = _previousScrollOffset / _tableViewOffset;
            var absOpacity = Math.Abs((float)opac);
            AddViewWithOpacity(absOpacity);
        }

        private void ShowSubmitMeterView()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRReadMeterViewController viewController =
                storyBoard.InstantiateViewController("SSMRReadMeterViewController") as SSMRReadMeterViewController;
            viewController.IsRoot = true;
            _rootNavigation = true;
            NavigationController.PushViewController(viewController, true);
        }

        private void OnTapDropDown()
        {
            if (SSMRAccounts.FilteredListCount > 0)
            {
                _rootNavigation = true;
                if (SSMRAccounts.GetEligibleAccountList().Count > 0)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                    SelectAccountTableViewController viewController = storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                    viewController.IsFromSSMR = true;
                    viewController.IsRoot = true;
                    viewController.CurrentSelectedIndex = _currentIndex;
                    viewController.OnSelect = OnSelectAccount;
                    NavigationController.PushViewController(viewController, true);
                }
                else
                {
                    DisplayNoData();
                }
            }
            else
            {
                DisplayNoData();
            }
        }

        private void DisplayNoData()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("GenericNoData", null);
            GenericNodataViewController viewController = (GenericNodataViewController)storyBoard
                .InstantiateViewController("GenericNoData");
            viewController.NavTitle = GetI18NValue(SSMRConstants.I18N_SelectAccountNavTitle);
            viewController.IsRootPage = true;
            viewController.Image = SSMRConstants.IMG_NoData;
            viewController.Message = GetI18NValue(SSMRConstants.I18N_NoEligibleAccount);
            NavigationController.PushViewController(viewController, true);
        }

        private void OnEnableSSMR()
        {
            DisplayApplciationForm(true);
        }

        private void OnDisableSSMR()
        {
            DisplayApplciationForm(false);
        }

        private void DisplayApplciationForm(bool isEnable)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await GetContactInfo();
                        if (_contactDetails != null && _contactDetails.d != null)
                        {
                            if (_contactDetails.d.IsSuccess && _contactDetails.d.data != null)
                            {
                                _rootNavigation = true;
                                UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                                SSMRApplicationViewController viewController =
                                    storyBoard.InstantiateViewController("SSMRApplicationViewController") as SSMRApplicationViewController;
                                if (viewController != null)
                                {
                                    viewController.IsApplication = isEnable;
                                    viewController.SelectedAccount = _currAcc;
                                    viewController.ContactDetails = _contactDetails;
                                    NavigationController.PushViewController(viewController, true);
                                }
                            }
                            else if (_contactDetails.d.IsBusinessFail)
                            {
                                DisplayCustomAlert(_contactDetails.d.DisplayTitle, _contactDetails.d.DisplayMessage, _contactDetails.d.RefreshBtnText, null);
                            }
                            else
                            {
                                DisplayServiceError(_contactDetails?.d?.ErrorMessage);
                            }
                        }
                        else
                        {
                            DisplayServiceError(GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage));
                        }
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        #endregion
        #region Services
        private async Task GetContactInfo()
        {
            await Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                string ICNumber = DataManager.DataManager.SharedInstance.UserEntity != null
                    && DataManager.DataManager.SharedInstance.UserEntity.Count > 0
                    && DataManager.DataManager.SharedInstance.UserEntity[0] != null
                    && DataManager.DataManager.SharedInstance.UserEntity[0].identificationNo != null
                    ? DataManager.DataManager.SharedInstance.UserEntity[0].identificationNo ?? string.Empty : string.Empty;
                object request = new
                {
                    serviceManager.usrInf,
                    contractAccount = _currAcc?.accNum ?? string.Empty,
                    isOwnedAccount = _currAcc?.IsOwnedAccount,
                    ICNumber
                };
                _contactDetails = serviceManager.OnExecuteAPIV6<ContactDetailsResponseModel>(SSMRConstants.Service_GetCARegisteredContact, request);
            });
        }

        private async Task GetEligibility()
        {
            await Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object request = new
                {
                    serviceManager.usrInf,
                    contractAccounts = SSMRAccounts.GetFilteredAccountNumberList()
                };
                _eligibilityAccount = serviceManager.OnExecuteAPIV6<AccountsSMREligibilityResponseModel>(SSMRConstants.Service_GetAccountsSMREligibility, request);
            });
        }

        private async Task LoadSMRAccountActivityInfo(CustomerAccountRecordModel account)
        {
            ActivityIndicator.Show();
            await GetSMRAccountActivityInfo(account).ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_smrActivityInfoResponse != null &&
                        _smrActivityInfoResponse.d != null &&
                        _smrActivityInfoResponse.d.data != null &&
                        _smrActivityInfoResponse.d.IsSuccess)
                    {
                        _bgImageView.Image = UIImage.FromBundle(SSMRConstants.IMG_ReadingHistoryBanner);
                        if (_viewRefreshContainer != null)
                        {
                            _viewRefreshContainer.RemoveFromSuperview();
                        }
                        SSMRActivityInfoCache.SetReadingHistoryCache(_smrActivityInfoResponse, _currAcc);
                        UpdateTable();
                        _readingHistoryTableView.Hidden = false;
                    }
                    else
                    {
                        _readingHistoryTableView.Hidden = true;
                        DisplayRefresh();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task GetSMRAccountActivityInfo(CustomerAccountRecordModel account)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object request = new
                {
                    contractAccount = account.accNum,
                    isOwnedAccount = account.isOwned,
                    serviceManager.usrInf
                };
                _smrActivityInfoResponse = serviceManager.OnExecuteAPIV6<SMRAccountActivityInfoResponseModel>(SSMRConstants.Service_GetSMRAccountActivityInfo, request);
            });
        }
        #endregion

        #region Refresh
        private void DisplayRefresh()
        {
            _bgImageView.Image = UIImage.FromBundle(SSMRConstants.IMG_Refresh);
            if (_viewRefreshContainer != null) { _viewRefreshContainer.RemoveFromSuperview(); }
            _viewRefreshContainer = new UIView(new CGRect(BaseMargin, GetYLocationFromFrame(_bgImageView.Frame, 16)
                , BaseMarginedWidth, GetScaledHeight(112)))
            { Tag = 10 };
            UILabel lblDescription = new UILabel(new CGRect(0, 0, BaseMarginedWidth, GetScaledHeight(48)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_16_300,
                Text = GetCommonI18NValue(SSMRConstants.I18N_RefreshDescription),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextColor = MyTNBColor.BrownGreyThree
            };
            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2()
            {
                Frame = new CGRect(0, GetYLocationFromFrame(lblDescription.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_Refresh
            };
            btnRefresh.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_RefreshNow), UIControlState.Normal);
            btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                EvaluateEntry();
            }));

            _viewRefreshContainer.AddSubview(lblDescription);
            _viewRefreshContainer.AddSubview(btnRefresh);
            View.AddSubview(_viewRefreshContainer);
        }
        #endregion
    }
}