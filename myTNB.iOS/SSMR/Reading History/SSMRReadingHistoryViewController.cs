using CoreAnimation;
using CoreGraphics;
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
        private nfloat _headerHeight, _maxHeaderHeight, _navBarHeight, _previousScrollOffset;
        private nfloat _minHeaderHeight = 0.1f;
        private nfloat _tableViewOffset = 64f;
        private nfloat titleBarHeight = 24f;
        private int _currentIndex = -1;
        private bool _isFromSelection;

        public bool IsFromHome;
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
            NavigationController.SetNavigationBarHidden(true, true);
            SetNoSSMR();
            string accName;
            if (_isFromSelection)
            {
                accName = _currAcc?.accountNickName ?? string.Empty;
                _ssmrHeaderComponent.AccountName = accName;
                EvaluateEntry();
            }
            else
            {
                if (IsFromHome)
                {
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
                    }
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
            NavigationController.SetNavigationBarHidden(false, true);
        }

        private void EvaluateEntry()
        {
            if (_currAcc.IsSSMR)
            {
                InvokeOnMainThread(async () =>
                {
                    ActivityIndicator.Show();
                    await LoadSMRAccountActivityInfo(_currAcc);
                    ActivityIndicator.Hide();
                });
            }
            else
            {
                SetEnableSSMR();
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
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, _readingHistoryList);
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
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, null);
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

            int yLocation = 26;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 50;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + _navBarHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_ReadingHistoryBanner)
            };

            View.AddSubview(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, yLocation, _navbarView.Frame.Width, titleBarHeight));

            UIView viewBack = new UIView(new CGRect(18, 0, 24, titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
            };
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            UILabel lblTitle = new UILabel(new CGRect(58, 0, _navbarView.Frame.Width - 116, titleBarHeight))
            {
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetI18NValue(SSMRConstants.I18N_NavTitle)
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            UIImageView imgViewRightBtn = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_PrimaryIcon)
            };

            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (IsFromHome)
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
            _headerHeight = _headerView.Frame.Height;
            _maxHeaderHeight = _headerView.Frame.Height;
        }

        private void AddTableView()
        {
            _readingHistoryTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, ViewHeight));
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
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
            _meterReadingHistory = IsFromHome || _isFromSelection
                ? SSMRActivityInfoCache.ViewMeterReadingHistory : SSMRActivityInfoCache.DashboardMeterReadingHistory;
            _readingHistoryList = IsFromHome || _isFromSelection
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
            NavigationController.PushViewController(viewController, true);
        }

        private void OnTapDropDown()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await GetEligibility();
                        if (_eligibilityAccount != null && _eligibilityAccount.d != null
                            && _eligibilityAccount.d.didSucceed && _eligibilityAccount.d.data != null
                            && _eligibilityAccount.d.data.accountEligibilities != null)
                        {
                            SSMRAccounts.SetData(_eligibilityAccount.d);
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
                                UIStoryboard storyBoard = UIStoryboard.FromName("GenericNoData", null);
                                GenericNodataViewController viewController = (GenericNodataViewController)storyBoard
                                    .InstantiateViewController("GenericNoData");
                                viewController.NavTitle = GetI18NValue(SSMRConstants.I18N_SelectAccountNavTitle);
                                viewController.IsRootPage = true;
                                viewController.Image = SSMRConstants.IMG_NoData;
                                viewController.Message = GetI18NValue(SSMRConstants.I18N_NoEligibleAccount);
                                NavigationController.PushViewController(viewController, true);
                            }
                        }
                        else
                        {
                            DisplayServiceError(_eligibilityAccount?.d?.ErrorMessage);
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
                        if (_contactDetails != null && _contactDetails.d != null
                        && _contactDetails.d.IsSuccess && _contactDetails.d.data != null)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                            SSMRApplicationViewController viewController =
                                storyBoard.InstantiateViewController("SSMRApplicationViewController") as SSMRApplicationViewController;
                            viewController.IsApplication = isEnable;
                            viewController.SelectedAccount = _currAcc;
                            viewController.ContactDetails = _contactDetails;
                            NavigationController.PushViewController(viewController, true);
                        }
                        else
                        {
                            DisplayServiceError(_contactDetails?.d?.ErrorMessage);
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
                        SSMRActivityInfoCache.SetReadingHistoryCache(_smrActivityInfoResponse);
                        UpdateTable();
                        _readingHistoryTableView.Hidden = false;
                    }
                    else
                    {
                        _readingHistoryTableView.Hidden = true;
                        DisplayRefresh();
                    }
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