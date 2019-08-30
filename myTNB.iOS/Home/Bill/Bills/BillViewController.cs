using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using myTNB.Home.Bill;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public partial class BillViewController : CustomUIViewController
    {
        private UIView _headerViewContainer, _headerView, _navbarView
            , _shimmerView, _viewRefreshContainer;
        private UIImageView _bgImageView;
        private CAGradientLayer _gradientLayer;
        private CustomUIView _accountSelectorContainer, _viewFilter;
        private nfloat _navBarHeight, _previousScrollOffset;
        private nfloat _tableViewOffset;
        private UITableView _historyTableView;
        private UILabel _lblPaymentStatus, _lblCurrency, _lblAmount, _lblDate, _lblNavTitle;
        private UIView _viewAmount, _viewCTA;
        private CustomUIButtonV2 _btnMore, _btnPay;
        private AccountSelector _accountSelector;
        private CustomUIView _viewAccountSelector;

        private GetAccountsChargesResponseModel _accountCharges;
        private GetAccountBillPayHistoryResponseModel _billHistory;

        public BillViewController(IntPtr handle) : base(handle) { }

        #region Life Cycle
        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            NavigationController.NavigationBarHidden = true;
            PageName = BillConstants.Pagename_Bills;
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            SetNavigation();
            SetHeaderView();
            AddAccountSelector();
            AddTableView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            OnSelectAccount(0);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }
        #endregion

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
            _tableViewOffset = DeviceHelper.GetStatusBarHeight() + _navBarHeight;

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_Cleared),
                BackgroundColor = UIColor.White
            };

            View.AddSubview(_bgImageView);
            View.SendSubviewToBack(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(12)
                , _navbarView.Frame.Width, GetScaledHeight(24)));
            _lblNavTitle = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(BillConstants.I18N_NavTitle),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };
            _viewFilter = new CustomUIView(new CGRect(_navbarView.Frame.Width - GetScaledWidth(32), 0
                , GetScaledWidth(16), GetScaledWidth(16)))
            { Hidden = true };
            UIImageView imgFilter = new UIImageView(new CGRect(0, 0, GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle("IC-Action-Filter")
            };
            _viewFilter.AddSubview(imgFilter);
            viewTitleBar.AddSubview(_lblNavTitle);
            viewTitleBar.AddSubview(_viewFilter);
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
        #endregion

        #region Header
        private void SetHeaderView()
        {
            _headerViewContainer = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _accountSelectorContainer = new CustomUIView(new CGRect(0, GetScaledHeight(8), ViewWidth, GetScaledHeight(24)));
            _headerView = new CustomUIView(new CGRect(0, _bgImageView.Frame.GetMaxY()
                - (DeviceHelper.GetStatusBarHeight() + _navBarHeight), ViewWidth, 0))
            { BackgroundColor = UIColor.White };
            _lblPaymentStatus = new UILabel(new CGRect(0, GetScaledHeight(16), ViewWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(BillConstants.I18N_NeedToPay)
            };
            _viewAmount = new UIView(new CGRect(0, GetYLocationFromFrame(_lblPaymentStatus.Frame, 8)
                , ViewWidth, GetScaledHeight(36)));

            _lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(16), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_16_500,
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            _lblAmount = new UILabel(new CGRect(0, 0, GetScaledWidth(100), GetScaledHeight(36)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_36_500,
                TextAlignment = UITextAlignment.Left,
                Text = string.Empty
            };
            _viewAmount.AddSubviews(new UIView[] { _lblCurrency, _lblAmount });
            UpdateViewAmount();

            _lblDate = new UILabel(new CGRect(0, GetYLocationFromFrame(_viewAmount.Frame, 8), ViewWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.GreyishBrown,
                Font = TNBFont.MuseoSans_14_300,
                TextAlignment = UITextAlignment.Center,
                Text = string.Empty
            };
            nfloat btnWidth = (BaseMarginedWidth - GetScaledWidth(4)) / 2;
            _viewCTA = new UIView(new CGRect(0, GetYLocationFromFrame(_lblDate.Frame, 24), ViewWidth, GetScaledHeight(48)));
            _btnMore = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnMore.SetTitle(GetI18NValue(BillConstants.I18N_ViewMore), UIControlState.Normal);
            _btnMore.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnMore.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnMore.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
                BillDetailsViewController viewController =
                    storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
                if (viewController != null)
                {
                    viewController.Charges = _accountCharges.d.data;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                }
            }));

            _btnPay = new CustomUIButtonV2
            {
                Frame = new CGRect(_btnMore.Frame.GetMaxX() + GetScaledWidth(4), 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnPay.SetTitle(GetCommonI18NValue(BillConstants.I18N_Pay), UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);

            _viewCTA.AddSubviews(new CustomUIButtonV2[] { _btnMore, _btnPay });

            _shimmerView = GetShimmerView();

            _headerView.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA, _shimmerView });
            _headerViewContainer.AddSubviews(_headerView);
            _headerViewContainer.AddSubviews(_accountSelectorContainer);

            CGRect frame = _headerView.Frame;
            frame.Height = GetYLocationFromFrame(_viewCTA.Frame, 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
        }

        private void UpdateViewAmount(bool isExtra = false)
        {
            nfloat currencyWidth = _lblCurrency.GetLabelWidth(GetScaledWidth(ViewWidth / 2));
            _lblCurrency.Frame = new CGRect(0, _lblCurrency.Frame.Y, currencyWidth, _lblCurrency.Frame.Height);

            nfloat amountWidth = _lblAmount.GetLabelWidth(GetScaledWidth(ViewWidth - currencyWidth));
            _lblAmount.Frame = new CGRect(_lblCurrency.Frame.GetMaxX() + GetScaledWidth(6)
                , _lblAmount.Frame.Y, amountWidth, _lblAmount.Frame.Height);

            nfloat newXLoc = (ViewWidth - (currencyWidth + amountWidth + GetScaledWidth(6))) / 2;
            _viewAmount.Frame = new CGRect(newXLoc, _viewAmount.Frame.Y, _lblAmount.Frame.GetMaxY(), _viewAmount.Frame.Height);

            _lblCurrency.TextColor = isExtra ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey;
            _lblAmount.TextColor = isExtra ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey;
        }

        private void AddAccountSelector()
        {
            _accountSelector = new AccountSelector();
            _viewAccountSelector = _accountSelector.GetUI();
            _accountSelector.SetAction(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                viewController.OnSelect = OnSelectAccount;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            });
            _accountSelectorContainer.AddSubview(_viewAccountSelector);
            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
        }

        private UIView GetShimmerView()
        {
            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(108))) { BackgroundColor = UIColor.White };//.White };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), viewParent.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), viewParent.Frame.Size)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView viewStatus = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(120)) / 2
                , GetScaledHeight(16), ScaleUtility.GetWidthByScreenSize(120), GetScaledHeight(20)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            UIView viewAmt = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(260)) / 2
                , GetYLocationFromFrame(viewStatus.Frame, 8), ScaleUtility.GetWidthByScreenSize(260), GetScaledHeight(36)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            UIView viewDate = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(120)) / 2
                , GetYLocationFromFrame(viewAmt.Frame, 8), ScaleUtility.GetWidthByScreenSize(120), GetScaledHeight(20)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            viewShimmerContent.AddSubviews(new UIView[] { viewStatus, viewAmt, viewDate });

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return viewParent;
        }

        private void SetHeaderLoading(bool isLoading)
        {
            if (isLoading)
            {
                _bgImageView.Image = UIImage.FromBundle(BillConstants.IMG_LoadingBanner);
            }
            _shimmerView.Hidden = !isLoading;
            _btnMore.Enabled = !isLoading;
            _btnMore.Layer.BorderColor = (isLoading ? MyTNBColor.SilverChalice : MyTNBColor.FreshGreen).CGColor;
            _btnMore.SetTitleColor(isLoading ? MyTNBColor.SilverChalice : MyTNBColor.FreshGreen, UIControlState.Normal);

            _btnPay.Enabled = !isLoading;
            _btnPay.BackgroundColor = isLoading ? MyTNBColor.SilverChalice : MyTNBColor.FreshGreen;

            _viewCTA.Hidden = !isLoading;
            if (isLoading)
            {
                _viewCTA.Frame = new CGRect(new CGPoint(_viewCTA.Frame.X
                    , GetYLocationFromFrame(_shimmerView.Frame, 16)), _viewCTA.Frame.Size);

                CGRect frame = _headerView.Frame;
                frame.Height = GetYLocationFromFrame(_viewCTA.Frame, 16);
                _headerView.Frame = frame;

                _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
               , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
            }

            _historyTableView.ReloadData();
            _historyTableView.Hidden = false;
        }
        #endregion

        #region Refresh
        private void DisplayRefresh()
        {
            _bgImageView.Image = UIImage.FromBundle("SSMR-Refresh");
            if (_viewRefreshContainer != null) { _viewRefreshContainer.RemoveFromSuperview(); }
            _viewRefreshContainer = new UIView()
            { Tag = 10, BackgroundColor = UIColor.White };
            UILabel lblDescription = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
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
                Frame = new CGRect(BaseMargin, GetYLocationFromFrame(lblDescription.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_Refresh
            };
            btnRefresh.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_RefreshNow), UIControlState.Normal);
            btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                // call services again
                OnSelectAccount(0);
            }));

            _viewRefreshContainer.AddSubview(lblDescription);
            _viewRefreshContainer.AddSubview(btnRefresh);
            _viewRefreshContainer.Frame = new CGRect(0, _bgImageView.Frame.GetMaxY()
               , ViewWidth, btnRefresh.Frame.GetMaxY() + GetScaledHeight(16));
            View.AddSubview(_viewRefreshContainer);
        }
        #endregion

        private void OnSelectAccount(int index)
        {
            if (_accountSelector != null)
            {
                _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
               {
                   if (NetworkUtility.isReachable)
                   {
                       SetHeaderLoading(true);
                       if (_viewRefreshContainer != null)
                       {
                           _viewRefreshContainer.RemoveFromSuperview();
                       }
                       _historyTableView.Source = new BillHistorySource(new List<BillPayHistoryModel>(), true)
                       {
                           GetI18NValue = GetI18NValue
                       };
                       InvokeInBackground(async () =>
                   {
                       _accountCharges = await GetAccountsCharges();
                       InvokeOnMainThread(() =>
                       {
                           SetHeaderLoading(false);

                           if (_accountCharges != null && _accountCharges.d != null && _accountCharges.d.IsSuccess
                                && _accountCharges.d.data != null && _accountCharges.d.data.AccountCharges != null
                                && _accountCharges.d.data.AccountCharges.Count > 0 && _accountCharges.d.data.AccountCharges[0] != null)
                           {
                               UpdateHeaderData(_accountCharges.d.data.AccountCharges[0]);
                           }
                           else
                           {
                               _historyTableView.Hidden = true;
                               DisplayRefresh();
                           }
                       });
                   });
                       InvokeInBackground(async () =>
                       {
                           _billHistory = await GetAccountBillPayHistory();
                           InvokeOnMainThread(() =>
                           {
                               if (_billHistory != null && _billHistory.d != null && _billHistory.d.IsSuccess
                                    && _billHistory.d.data != null)
                               {
                                   List<BillPayHistoryModel> historyList = _billHistory.d.data.BillPayHistories;
                                   _historyTableView.Source = new BillHistorySource(historyList, false)
                                   {
                                       OnTableViewScroll = OnTableViewScroll,
                                       GetI18NValue = GetI18NValue,
                                       OnSelectBill = DisplayBillPDF,
                                       OnSelectPayment = DisplayReceipt
                                   };
                                   _historyTableView.ReloadData();
                               }
                               else
                               {
                                   //DisplayServiceError(_billHistory?.d?.ErrorMessage);
                                   _historyTableView.Hidden = true;
                                   DisplayRefresh();
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

        private void UpdateHeaderData(AccountChargesModel data)
        {
            bool isRe = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            _lblAmount.Text = Math.Abs(data.AmountDue).ToString("N2", CultureInfo.InvariantCulture);
            CGRect ctaFrame = _viewCTA.Frame;

            _bgImageView.Image = isRe ? UIImage.FromBundle(BillConstants.IMG_Cleared)
                : UIImage.FromBundle(data.AmountDue > 0 ? BillConstants.IMG_NeedToPay : BillConstants.IMG_Cleared);

            if (data.AmountDue > 0)
            {
                _lblPaymentStatus.Text = GetI18NValue(isRe ? BillConstants.I18N_MyEarnings : BillConstants.I18N_NeedToPay);
                string result = DateTime.ParseExact(data.DueDate, BillConstants.Format_DateParse
                    , CultureInfo.InvariantCulture).ToString(BillConstants.Format_Date);
                _lblDate.Text = string.Format(BillConstants.Format_Default
                    , GetI18NValue(isRe ? BillConstants.I18N_GetBy : BillConstants.I18N_By), result);
                _lblDate.Hidden = false;
                ctaFrame.Y = GetYLocationFromFrame(_lblDate.Frame, 24);
            }
            else
            {
                _lblPaymentStatus.Text = isRe ? GetI18NValue(BillConstants.I18N_BeenPaidExtra)
                    : GetI18NValue(data.AmountDue == 0 ? BillConstants.I18N_ClearedBills : BillConstants.I18N_PaidExtra);
                _lblDate.Hidden = true;
                ctaFrame.Y = GetYLocationFromFrame(_viewAmount.Frame, 24);
            }
            UpdateViewAmount(data.AmountDue < 0);

            _viewCTA.Frame = ctaFrame;
            _viewCTA.Hidden = isRe;

            nfloat headerHeight = isRe ? _lblDate.Hidden ? _viewAmount.Frame.GetMaxY() : _lblDate.Frame.GetMaxY() : _viewCTA.Frame.GetMaxY();
            CGRect frame = _headerView.Frame;
            frame.Height = headerHeight + GetScaledHeight(isRe ? 24 : 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
            _historyTableView.ReloadData();
        }

        #region Table
        private void AddTableView()
        {
            nfloat height = View.Frame.Height - _navbarView.Frame.Height - TabBarController.TabBar.Frame.Height;
            _historyTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, height));
            _historyTableView.RegisterClassForCellReuse(typeof(BillHistoryViewCell), BillConstants.Cell_BillHistory);
            _historyTableView.RegisterClassForCellReuse(typeof(BillSectionViewCell), BillConstants.Cell_BillSection);
            _historyTableView.RegisterClassForCellReuse(typeof(NoDataViewCell), BillConstants.Cell_NoHistoryData);
            _historyTableView.RegisterClassForCellReuse(typeof(BillHistoryShimmerViewCell), BillConstants.Cell_BillHistoryShimmer);
            _historyTableView.Source = new BillHistorySource(new List<BillPayHistoryModel>(), true)
            {
                OnTableViewScroll = OnTableViewScroll,
                GetI18NValue = GetI18NValue
            };
            _historyTableView.BackgroundColor = UIColor.Clear;
            _historyTableView.RowHeight = UITableView.AutomaticDimension;
            _historyTableView.EstimatedRowHeight = GetScaledHeight(90);
            _historyTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _historyTableView.Bounces = false;
            _historyTableView.TableHeaderView = _headerViewContainer;
            View.AddSubview(_historyTableView);
        }
        #endregion

        #region Events
        private void OnTableViewScroll(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            CGRect frame = _bgImageView.Frame;
            if ((nfloat)Math.Abs(frame.Y) == frame.Height) { return; }
            nfloat newYLoc = 0 - scrollView.ContentOffset.Y;
            FilterDisplay(scrollView.ContentOffset.Y > _headerViewContainer.Frame.Height + ScaleUtility.GetScaledHeight(36));
            frame.Y = newYLoc;
            _bgImageView.Frame = frame;
            _previousScrollOffset = _historyTableView.ContentOffset.Y;
            var opac = _previousScrollOffset / _tableViewOffset;
            var absOpacity = Math.Abs((float)opac);
            AddViewWithOpacity(absOpacity);
        }

        private void FilterDisplay(bool isHeader)
        {
            _lblNavTitle.Text = GetI18NValue(isHeader ? BillConstants.I18N_MyHistory : BillConstants.I18N_NavTitle);
            _viewFilter.Hidden = !isHeader;
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

        private void DisplayBillPDF(string DetailedInfoNumber)
        {
            if (string.IsNullOrEmpty(DetailedInfoNumber) || string.IsNullOrWhiteSpace(DetailedInfoNumber))
            {
                return;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                        ViewBillViewController viewController =
                            storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                        if (viewController != null)
                        {
                            viewController.BillingNumber = DetailedInfoNumber;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void DisplayReceipt(string DetailedInfoNumber)
        {
            if (string.IsNullOrEmpty(DetailedInfoNumber) || string.IsNullOrWhiteSpace(DetailedInfoNumber))
            {
                return;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                        ReceiptViewController viewController =
                            storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                        if (viewController != null)
                        {
                            viewController.DetailedInfoNumber = DetailedInfoNumber;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                        }
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
        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges()
        {
            //Thread.Sleep(5000);
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                accounts = new List<string> { DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty },
                isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount
            };
            GetAccountsChargesResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountsChargesResponseModel>(BillConstants.Service_GetAccountsCharges, request);
            return response;
        }

        private async Task<GetAccountBillPayHistoryResponseModel> GetAccountBillPayHistory()
        {
            //Thread.Sleep(5000);
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccount = DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty,
                isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount,
                accountType = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount ? BillConstants.Param_RE : BillConstants.Param_UTIL
            };
            GetAccountBillPayHistoryResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountBillPayHistoryResponseModel>(BillConstants.Service_GetAccountBillPayHistory, request);
            return response;
        }
        #endregion
    }
}