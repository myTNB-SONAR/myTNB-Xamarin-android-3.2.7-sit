using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.Model;
using myTNB.Model.Usage;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public class UsageBaseViewController : CustomUIViewController
    {
        public UsageBaseViewController(IntPtr handle) : base(handle) { }

        TariffSelectionComponent _tariffSelectionComponent;
        UsageFooterViewComponent _footerViewComponent;
        REAmountComponent _rEAmountComponent;

        internal UIScrollView _scrollViewContent;
        internal CustomUIView _navbarContainer, _accountSelectorContainer, _viewSeparator, _viewStatus
            , _viewChart, _viewRE, _viewLegend, _viewToggle, _viewSSMR, _viewTips, _viewFooter, _rmKwhDropDownView
            , _chart, _tips, _RE, _status, _ssmr, _tariff, _legend;
        internal UILabel _lblAddress, _RMLabel, _kWhLabel;
        internal UIImageView _bgImageView;

        internal bool _rmkWhFlag, _tariffIsVisible = false;
        internal RMkWhEnum _rMkWhEnum;
        internal nfloat _lastContentOffset;
        internal bool isBcrmAvailable, isNormalChart, isREAccount, accountIsSSMR;

        internal CGRect _origViewFrame;

        protected AccountSelector _accountSelector;
        protected CustomUIView _viewAccountSelector;
        protected BaseChartView _chartView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBarHidden = true;
            InitializeValues();
            AddBackgroundImage();
            SetNavigation();
            AddScrollView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            InitializeValues();
            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
            {
                _rmkWhFlag = false;
                _tariffIsVisible = false;
                _rMkWhEnum = RMkWhEnum.RM;
                UpdateBackgroundImage(false);
                AddSubviews();
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void InitializeValues()
        {
            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            isNormalChart = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            accountIsSSMR = UsageHelper.IsSSMR(DataManager.DataManager.SharedInstance.SelectedAccount);
        }

        private void SetNavigation()
        {
            _navbarContainer = new CustomUIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + 8f, _navbarContainer.Frame.Width, GetScaledHeight(24f)));

            UILabel lblTitle = new UILabel(new CGRect(58, 0, _navbarContainer.Frame.Width - 116, GetScaledHeight(24f)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = "Usage"
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            nfloat imageWidth = GetScaledWidth(24f);
            UIView viewBack = new UIView(new CGRect(18, 0, imageWidth, imageWidth));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, imageWidth, imageWidth))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_Back)
            };
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);
            _navbarContainer.AddSubview(viewTitleBar);
            View.AddSubview(_navbarContainer);
        }

        private void AddBackgroundImage()
        {
            nfloat height = GetScaledHeight(543f);
            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, height))
            {
                Image = UIImage.FromBundle("Usage-Bg-Normal")
            };
            View.AddSubview(_bgImageView);
        }

        private void UpdateBackgroundImage(bool isLegendVisible = false)
        {
            nfloat height = isLegendVisible ? GetScaledHeight(691f) : GetScaledHeight(543f);
            ViewHelper.AdjustFrameSetHeight(_bgImageView, height);
            _bgImageView.Image = UIImage.FromBundle(isLegendVisible ? "Usage-Bg-Long" : "Usage-Bg-Normal");
        }

        private void AddScrollView()
        {
            nfloat height = UIScreen.MainScreen.Bounds.Height - _navbarContainer.Frame.GetMaxY() - GetScaledHeight(8F);
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                height -= 20f;
            }
            _scrollViewContent = new UIScrollView(new CGRect(0, GetYLocationFromFrame(_navbarContainer.Frame, 8F), ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = false
            };
            _scrollViewContent.Scrolled += OnScroll;
            View.AddSubview(_scrollViewContent);

            _accountSelectorContainer = new CustomUIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)));// { BackgroundColor = UIColor.Blue };
            _lblAddress = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth, 0))
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_10_300
            };
            _viewSeparator = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };
            _viewStatus = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewChart = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewRE = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewLegend = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewToggle = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewSSMR = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewTips = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };

            _scrollViewContent.AddSubviews(new UIView[] { _accountSelectorContainer
                , _lblAddress, _viewSeparator, _viewStatus, _viewChart, _viewRE, _viewLegend, _viewToggle, _viewSSMR, _viewTips });
        }

        private void SetContentView()
        {
            _lblAddress.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_accountSelectorContainer.Frame, 8F)), _lblAddress.Frame.Size);
            _viewSeparator.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewSeparator.Frame.Size);

            if (!AccountStatusCache.AccountStatusIsAvailable())
            {
                _viewStatus.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSeparator.Frame, 16F)), _viewStatus.Frame.Size);
                _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewStatus.Frame, 16F)), _viewChart.Frame.Size);
            }
            else
            {
                _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSeparator.Frame, 16F)), _viewChart.Frame.Size);
            }

            if (isREAccount)
            {
                _viewRE.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 24F)), _viewRE.Frame.Size);
            }
            else
            {
                _viewLegend.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, !_viewLegend.Hidden ? 16F : 0F)), _viewLegend.Frame.Size);
                _viewToggle.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewLegend.Frame, 16F)), _viewToggle.Frame.Size);

                if (accountIsSSMR)
                {
                    _viewSSMR.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 16F)), _viewSSMR.Frame.Size);
                    _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSSMR.Frame, 16F)), _viewTips.Frame.Size);
                }
                else
                {
                    _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 24F)), _viewTips.Frame.Size);
                }
            }
            _scrollViewContent.ContentSize = new CGSize(ViewWidth, GetAdditionalHeight(isREAccount ? _viewRE.Frame.GetMaxY() : _viewTips.Frame.GetMaxY()));
        }

        private nfloat GetAdditionalHeight(nfloat maxYPos)
        {
            nfloat height = maxYPos + GetScaledHeight(16F);
            nfloat totalHeight = maxYPos + _navbarContainer.Frame.GetMaxY();
            if (totalHeight < View.Frame.Height)
            {
                height += View.Frame.Height - totalHeight;
            }
            return height;
        }

        internal void AddSubviews()
        {
            AddAccountSelector();
            SetAddress();
            SetDisconnectionComponent();
            SetChartView(true);
            SetTariffSelectionComponent();
            SetSSMRComponent();
            SetEnergyTipsComponent();
            SetContentView();
            SetFooterView();
            SetREAmountView();
        }

        private void AddAccountSelector()
        {
            if (_viewAccountSelector != null)
            {
                _viewAccountSelector.RemoveFromSuperview();
            }

            _accountSelector = new AccountSelector();
            _viewAccountSelector = _accountSelector.GetUI();
            _accountSelector.SetAction(null);
            _accountSelectorContainer.AddSubview(_viewAccountSelector);
            _accountSelectorContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_rmKwhDropDownView != null)
                {
                    _rmKwhDropDownView.Hidden = true;
                }
                DataManager.DataManager.SharedInstance.IsSameAccount = true;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                viewController.OnSelect = OnSelectAccount;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }));

            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
        }

        internal virtual void OnSelectAccount(int index) { }

        private void SetAddress()
        {
            _lblAddress.Text = DataManager.DataManager.SharedInstance.SelectedAccount?.accountStAddress?.ToUpper();//AccountManager.Instance.Address.ToUpper();
            CGSize lblSize = GetLabelSize(_lblAddress, GetScaledHeight(42));
            ViewHelper.AdjustFrameSetHeight(_lblAddress, lblSize.Height);
        }

        public void SetChartView(bool isUpdating)
        {
            if (isREAccount)
            {
                _chartView = new REChartView();
            }
            else
            {
                _chartView = new NormalChartView();
            }

            if (_chart != null)
            {
                _chart.RemoveFromSuperview();
            }
            _chart = isUpdating ? _chartView.GetShimmerUI(isREAccount) : _chartView.GetUI();
            _viewChart.AddSubview(_chart);
            ViewHelper.AdjustFrameSetHeight(_viewChart, _chart.Frame.Height);
        }

        #region SSMR Methods
        internal void SetSSMRComponent()
        {
            if (!isREAccount && accountIsSSMR)
            {
                MeterReadingHistoryModel smrAcountInfo = SSMRActivityInfoCache.DashboardMeterReadingHistory;
                if (smrAcountInfo != null)
                {
                    if (_ssmr != null)
                    {
                        _ssmr.RemoveFromSuperview();
                    }
                    SSMRComponent sSMRComponent = new SSMRComponent(_viewSSMR);
                    _ssmr = sSMRComponent.GetUI();
                    _viewSSMR.AddSubview(_ssmr);
                    sSMRComponent.SetDescription(smrAcountInfo.DashboardMessage);
                    sSMRComponent.SetButtonText(smrAcountInfo.DashboardCTAText);
                    sSMRComponent.SetSRMButtonEnable(smrAcountInfo.IsDashboardCTADisabled);
                    sSMRComponent.ShowHistoryLink(smrAcountInfo.ShowReadingHistoryLink, smrAcountInfo.ReadingHistoryLinkText);
                    sSMRComponent._labelViewHistory.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        OnReadHistoryTap();
                    }));
                    sSMRComponent._smrButton.TouchUpInside += (sender, e) =>
                    {
                        var ctaChar = smrAcountInfo.DashboardCTAType.ToLower();
                        if (ctaChar == DashboardHomeConstants.CTA_ShowReadingHistory)
                        {
                            OnReadHistoryTap();
                        }
                        else if (ctaChar == DashboardHomeConstants.CTA_ShowSubmitReading)
                        {
                            OnSubmitMeterTap();
                        }
                    };
                    ViewHelper.AdjustFrameSetHeight(_viewSSMR, sSMRComponent.GetContainerHeight());
                    _viewSSMR.BackgroundColor = UIColor.Clear;
                    _viewSSMR.Hidden = false;
                    AddSSMRViewShadow(ref _viewSSMR);
                }
            }
            else
            {
                ViewHelper.AdjustFrameSetHeight(_viewSSMR, 0);
                _viewSSMR.Hidden = true;
            }
            SetContentView();
        }

        internal virtual void OnReadHistoryTap() { }

        internal virtual void OnSubmitMeterTap() { }

        private void AddSSMRViewShadow(ref CustomUIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = .32f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
        #endregion
        #region TARIFF LEGEND Methods
        public void SetTariffLegendComponent()
        {
            if (!isREAccount)
            {
                List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
                if (tariffList != null && tariffList.Count > 0)
                {
                    ViewHelper.AdjustFrameSetHeight(_viewLegend, 0);
                    _viewLegend.BackgroundColor = UIColor.Clear;
                    _viewLegend.Hidden = true;
                    if (_legend != null)
                    {
                        _legend.RemoveFromSuperview();
                    }
                    TariffLegendComponent tariffLegendComponent = new TariffLegendComponent(View, tariffList);
                    _legend = tariffLegendComponent.GetUI();
                    _viewLegend.AddSubview(_legend);
                }
            }
            else
            {
                ViewHelper.AdjustFrameSetHeight(_viewLegend, 0);
                _viewLegend.Hidden = true;
            }
            SetContentView();
        }
        private void ShowHideTariffLegends(bool isVisible)
        {
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            if (tariffList != null && tariffList.Count > 0)
            {
                UpdateBackgroundImage(isVisible);
                _viewLegend.Hidden = !isVisible;

                nfloat height = isVisible ? tariffList.Count * GetScaledHeight(25f) : 0;
                ViewHelper.AdjustFrameSetHeight(_viewLegend, height);
                SetContentView();
            }
        }
        #endregion
        #region RM/KWH & TARIFF Methods
        private void SetTariffSelectionComponent()
        {
            if (!isREAccount)
            {
                ViewHelper.AdjustFrameSetHeight(_viewToggle, GetScaledHeight(24f));
                _viewToggle.BackgroundColor = UIColor.Clear;
                _viewToggle.Hidden = false;

                if (_tariff != null)
                {
                    _tariff.RemoveFromSuperview();
                }
                _tariffSelectionComponent = new TariffSelectionComponent(View);
                _tariff = _tariffSelectionComponent.GetUI();
                _viewToggle.AddSubview(_tariff);
                _rMkWhEnum = RMkWhEnum.RM;
                _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
                _tariffSelectionComponent.SetGestureRecognizerFoRMKwH(new UITapGestureRecognizer(() =>
                {
                    ShowHideRMKwHDropDown();
                }));
                _tariffSelectionComponent.SetGestureRecognizerForTariff(new UITapGestureRecognizer(() =>
                {
                    if (_rmKwhDropDownView != null)
                    {
                        _rmKwhDropDownView.Hidden = true;
                    }
                    _tariffIsVisible = !_tariffIsVisible;
                    _tariffSelectionComponent.UpdateTariffButton(_tariffIsVisible);
                    ShowHideTariffLegends(_tariffIsVisible);
                    _chartView.ToggleTariffView(_tariffIsVisible);
                }));

                if (_rmKwhDropDownView == null)
                {
                    CreateRMKwhDropdown();
                }
            }
            else
            {
                ViewHelper.AdjustFrameSetHeight(_viewToggle, 0);
                _viewToggle.Hidden = true;
            }
            SetContentView();
        }

        private void CreateRMKwhDropdown()
        {
            nfloat dropDownXPos = GetScaledWidth(16f);
            nfloat dropDownYPos = _viewToggle.Frame.GetMinY() + GetScaledHeight(-56f);
            nfloat dropDownWidth = GetScaledWidth(60f);
            nfloat dropDownHeight = GetScaledHeight(48f);
            _rmKwhDropDownView = new CustomUIView(new CGRect(dropDownXPos, 200f, dropDownWidth, dropDownHeight))
            {
                BackgroundColor = UIColor.White,
                Hidden = true
            };
            _rmKwhDropDownView.Layer.CornerRadius = GetScaledHeight(5f);
            _scrollViewContent.AddSubview(_rmKwhDropDownView);

            nfloat lineXPos = GetScaledWidth(3.5f);
            nfloat lineYPos = GetScaledHeight(23.5f);
            nfloat lineWidth = GetScaledWidth(53f);
            nfloat lineHeight = GetScaledHeight(1f);
            UIView lineView = new UIView(new CGRect(lineXPos, lineYPos, lineWidth, lineHeight))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _rmKwhDropDownView.AddSubview(lineView);

            UIView kWhView = new UIView(new CGRect(0, 0, dropDownWidth, dropDownHeight / 2))
            {
                BackgroundColor = UIColor.Clear
            };
            kWhView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("kWh Selected!");
                _rMkWhEnum = RMkWhEnum.kWh;
                ShowHideRMKwHDropDown();
                _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
                UpdateRMkWhSelectionColour(_rMkWhEnum);
                //TO DO: Add Action here when KWH is selected....
                _chartView.ToggleRMKWHValues(_rMkWhEnum);
            }));
            _rmKwhDropDownView.AddSubview(kWhView);

            nfloat kWhXPos = GetScaledWidth(17.5f);
            nfloat kWhYPos = GetScaledHeight(4f);
            nfloat kWhWidth = GetScaledWidth(25f);
            nfloat kWhHeight = GetScaledHeight(16f);
            _kWhLabel = new UILabel(new CGRect(kWhXPos, kWhYPos, kWhWidth, kWhHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Center,
                Text = UsageHelper.GetRMkWhValueStringForEnum(RMkWhEnum.kWh)
            };
            kWhView.AddSubview(_kWhLabel);

            UIView rMView = new UIView(new CGRect(0, dropDownHeight / 2, dropDownWidth, dropDownHeight / 2))
            {
                BackgroundColor = UIColor.Clear
            };
            rMView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("RM Selected!");
                _rMkWhEnum = RMkWhEnum.RM;
                ShowHideRMKwHDropDown();
                _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
                UpdateRMkWhSelectionColour(_rMkWhEnum);
                //TO DO: Add Action here when RM is selected....
                _chartView.ToggleRMKWHValues(_rMkWhEnum);
            }));
            _rmKwhDropDownView.AddSubview(rMView);

            nfloat rmXPos = GetScaledWidth(20.5f);
            nfloat rmYPos = GetScaledHeight(4f);
            nfloat rmWidth = GetScaledWidth(19f);
            nfloat rmHeight = GetScaledHeight(16f);
            _RMLabel = new UILabel(new CGRect(rmXPos, rmYPos, rmWidth, rmHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center,
                Text = UsageHelper.GetRMkWhValueStringForEnum(RMkWhEnum.RM)
            };
            rMView.AddSubview(_RMLabel);
        }

        private void ShowHideRMKwHDropDown()
        {
            nfloat dropDownYPos = _viewToggle.Frame.GetMinY() + GetScaledHeight(-56f);
            ViewHelper.AdjustFrameSetY(_rmKwhDropDownView, dropDownYPos);
            _rmkWhFlag = !_rmkWhFlag;
            _rmKwhDropDownView.Hidden = !_rmkWhFlag;
        }

        private void UpdateRMkWhSelectionColour(RMkWhEnum rMkWhEnum)
        {
            switch (rMkWhEnum)
            {
                case RMkWhEnum.RM:
                    _kWhLabel.TextColor = MyTNBColor.WarmGrey;
                    _RMLabel.TextColor = MyTNBColor.WaterBlue;
                    break;
                case RMkWhEnum.kWh:
                    _kWhLabel.TextColor = MyTNBColor.WaterBlue;
                    _RMLabel.TextColor = MyTNBColor.WarmGrey;
                    break;
            }
        }
        #endregion
        #region ENERGY TIPS Methods
        private void SetEnergyTipsComponent()
        {
            if (!isREAccount)
            {
                List<TipsModel> tipsList;
                EnergyTipsEntity wsManager = new EnergyTipsEntity();
                var tips = wsManager.GetAllItems();
                tipsList = tips;
                if (tips.Count > UsageConstants.MaxRandomTips)
                {
                    tipsList = new List<TipsModel>();
                    var randomIndexes = UsageHelper.RandomizedTips(tips.Count, UsageConstants.MaxRandomTips);
                    for (int i = 0; i < randomIndexes.Length; i++)
                    {
                        tipsList.Add(tips[randomIndexes[i]]);
                    }
                }
                if (tipsList != null &&
                    tipsList.Count > 0)
                {
                    ViewHelper.AdjustFrameSetHeight(_viewTips, GetScaledHeight(100f));
                    _viewTips.BackgroundColor = UIColor.Clear;
                    _viewTips.Hidden = false;

                    if (_tips != null)
                    {
                        _tips.RemoveFromSuperview();
                    }
                    EnergyTipsComponent energyTipsComponent = new EnergyTipsComponent(_viewTips, tipsList);
                    _tips = energyTipsComponent.GetUI();
                    _viewTips.AddSubview(_tips);
                }
            }
            else
            {
                ViewHelper.AdjustFrameSetHeight(_viewTips, 0);
                _viewTips.Hidden = true;
            }
        }
        #endregion
        #region DISCONNECTION Methods
        public void SetDisconnectionComponent()
        {
            AccountStatusDataModel accountStatusData = AccountStatusCache.GetAccountStatusData();
            if (!AccountStatusCache.AccountStatusIsAvailable())
            {
                ViewHelper.AdjustFrameSetHeight(_viewStatus, GetScaledHeight(24f));
                _viewStatus.BackgroundColor = UIColor.Clear;
                _viewStatus.Hidden = false;

                if (_status != null)
                {
                    _status.RemoveFromSuperview();
                }

                DisconnectionComponent disconnectionComponent = new DisconnectionComponent(_scrollViewContent, accountStatusData);
                _status = disconnectionComponent.GetUI();
                _viewStatus.AddSubview(_status);
                disconnectionComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    var acctStatusTooltipBtnTitle = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalBtnText) ? accountStatusData.AccountStatusModalBtnText : "Common_GotIt".Translate();
                    var acctStatusTooltipMsg = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalMessage) ? accountStatusData.AccountStatusModalMessage : "Dashboard_AccountStatusMessage".Translate();
                    DisplayCustomAlert(string.Empty, acctStatusTooltipMsg, acctStatusTooltipBtnTitle, null);
                }));
            }
            else
            {
                _viewStatus.Hidden = true;
                ViewHelper.AdjustFrameSetHeight(_viewStatus, 0);
            }
            SetContentView();
        }
        #endregion
        #region RE Methods
        internal void SetREAmountView()
        {
            _viewRE.Hidden = !isREAccount;
            if (isREAccount)
            {
                ViewHelper.AdjustFrameSetHeight(_viewRE, GetScaledHeight(118f));
                _viewRE.BackgroundColor = UIColor.Clear;

                if (_RE != null)
                {
                    _RE.RemoveFromSuperview();
                }
                _rEAmountComponent = new REAmountComponent(_viewRE);
                _RE = _rEAmountComponent.GetUI();
                _viewRE.AddSubview(_RE);
                _rEAmountComponent._btnViewPaymentAdvice.TouchUpInside += (sender, e) =>
                {
                    OnCurrentBillButtonTap();
                };
            }
        }

        internal void UpdateREAmountViewUI(bool isUpdating)
        {
            if (_rEAmountComponent != null)
            {
                _rEAmountComponent.UpdateUI(isUpdating);
                if (!isUpdating)
                {
                    _rEAmountComponent.UpdateUI(isUpdating);
                    DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    _rEAmountComponent.SetAmount(dueData.amountDue);
                    _rEAmountComponent.SetDate(dueData.billDueDate);
                }
            }
        }
        #endregion
        #region FOOTER Methods
        internal void SetFooterView()
        {
            if (!isREAccount)
            {
                if (_viewFooter != null)
                {
                    _viewFooter.RemoveFromSuperview();
                }
                nfloat footerRatio = 136.0f / 320.0f;
                nfloat footerHeight = ViewWidth * footerRatio;
                nfloat footerYPos = UIScreen.MainScreen.Bounds.Height - footerHeight;
                if (DeviceHelper.IsIphoneXUpResolution())
                {
                    footerYPos -= 20f;
                }
                _viewFooter = new CustomUIView(new CGRect(0, footerYPos, ViewWidth, footerHeight))
                {
                    BackgroundColor = UIColor.White
                };
                _origViewFrame = _viewFooter.Frame;
                AddFooterShadow(ref _viewFooter);
                View.AddSubview(_viewFooter);
                _footerViewComponent = new UsageFooterViewComponent(View, footerHeight);
                _viewFooter.AddSubview(_footerViewComponent.GetUI());
                _footerViewComponent._btnViewBill.TouchUpInside += (sender, e) =>
                {
                    OnCurrentBillButtonTap();
                };
                _footerViewComponent._btnPay.TouchUpInside += (sender, e) =>
                {
                    DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    OnPayButtonTap(dueData.amountDue);
                };
            }
            else
            {
                if (_viewFooter != null)
                {
                    _viewFooter.RemoveFromSuperview();
                }
            }
        }

        internal void UpdateFooterUI(bool isUpdating)
        {
            if (_footerViewComponent != null)
            {
                _footerViewComponent.UpdateUI(isUpdating);
                if (!isUpdating)
                {
                    _footerViewComponent.UpdateUI(isUpdating);
                    DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    _footerViewComponent.SetAmount(dueData.amountDue);
                    _footerViewComponent.SetDate(dueData.billDueDate);
                }
            }
        }

        internal virtual void OnCurrentBillButtonTap() { }

        internal virtual void OnPayButtonTap(double amountDue) { }

        private void OnScroll(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            if (scrollView != null)
            {
                if (_lastContentOffset < 0 || _lastContentOffset < scrollView.ContentOffset.Y)
                {
                    AnimateFooterToHideAndShow(true);
                }
                else if (_lastContentOffset > scrollView.ContentOffset.Y)
                {
                    AnimateFooterToHideAndShow(false);
                }
                _lastContentOffset = scrollView.ContentOffset.Y;
            }
        }

        private void AnimateFooterToHideAndShow(bool isHidden)
        {
            if (_viewFooter != null)
            {
                UIView.Animate(0.3, 0, UIViewAnimationOptions.ShowHideTransitionViews
                , () =>
                {
                    if (isHidden)
                    {
                        var temp = _origViewFrame;
                        temp.Y = _scrollViewContent.Frame.GetMaxY();

                        _viewFooter.Frame = temp;
                    }
                    else
                    {
                        _viewFooter.Frame = _origViewFrame;
                    }
                }
                , () => { }
            );
            }
        }

        private void AddFooterShadow(ref CustomUIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue35.CGColor;
            view.Layer.ShadowOpacity = .16f;
            view.Layer.ShadowOffset = new CGSize(0, -8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
        #endregion
    }
}
