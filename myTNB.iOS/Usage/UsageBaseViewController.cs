using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
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

        internal UIScrollView _scrollViewContent;
        internal CustomUIView _navbarContainer, _accountSelector, _viewSeparator, _viewStatus
            , _viewChart, _viewLegend, _viewToggle, _viewTips, _viewFooter, _rmKwhDropDownView;
        internal UILabel _lblAddress, _RMLabel, _kWhLabel;

        internal bool _rmkWhFlag, _tariffIsVisible = false;
        internal RMkWhEnum _rMkWhEnum;
        internal nfloat _lastContentOffset;

        internal CGRect _origViewFrame;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBarHidden = true;
            AddBackgroundImage();
            SetNavigation();
            AddScrollView();
            AddSubviews();
            SetFooterView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
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
            UIImageView bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, height))
            {
                Image = UIImage.FromBundle("Stub-Usage-Bg")
            };
            View.AddSubview(bgImageView);
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

            _accountSelector = new CustomUIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)));// { BackgroundColor = UIColor.Blue };
            _lblAddress = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth, 0))
            {
                //BackgroundColor = UIColor.Red,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_10_300
            };
            _viewSeparator = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };
            _viewStatus = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewChart = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewLegend = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewToggle = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewTips = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));

            _scrollViewContent.AddSubviews(new UIView[] { _accountSelector
                , _lblAddress, _viewSeparator, _viewStatus, _viewChart, _viewLegend, _viewToggle, _viewTips });
        }

        private void SetContentView()
        {
            _lblAddress.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_accountSelector.Frame, 8F)), _lblAddress.Frame.Size);
            _viewSeparator.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewSeparator.Frame.Size);
            _viewStatus.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSeparator.Frame, (_viewStatus.Frame.Height > 0) ? 16F : 0F)), _viewStatus.Frame.Size);
            _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewStatus.Frame, 16F)), _viewChart.Frame.Size);
            _viewLegend.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, !_viewLegend.Hidden ? 16F : 0F)), _viewLegend.Frame.Size);
            _viewToggle.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewLegend.Frame, 16F)), _viewToggle.Frame.Size);
            _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 24F)), _viewTips.Frame.Size);

            _scrollViewContent.ContentSize = new CGSize(ViewWidth, GetAdditionalHeight(_viewTips.Frame.GetMaxY()));
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

        private void AddSubviews()
        {
            AddAccountSelector();
            SetAddress();
            SetChartView();
            SetTariffSelectionComponent();
            SetEnergyTipsComponent();
            SetContentView();
        }

        private void AddAccountSelector()
        {
            AccountSelector accountSelector = new AccountSelector();
            CustomUIView viewAccountSelector = accountSelector.GetUI();
            accountSelector.SetAction(null);
            accountSelector.Title = AccountManager.Instance.Nickname;
            _accountSelector.AddSubview(viewAccountSelector);
            _accountSelector.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.IsSameAccount = true;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }));
        }

        private void SetAddress()
        {
            _lblAddress.Text = AccountManager.Instance.Address.ToUpper();
            CGSize lblSize = GetLabelSize(_lblAddress, GetScaledHeight(42));
            CGRect lblFrame = _lblAddress.Frame;
            lblFrame.Height = lblSize.Height;
            _lblAddress.Frame = lblFrame;
        }

        private void SetChartView()
        {
            ChartView chartView = new ChartView();
            CustomUIView chart = chartView.GetUI();
            _viewChart.AddSubview(chart);
            CGRect chartFrame = _viewChart.Frame;
            chartFrame.Size = new CGSize(ViewWidth, chart.Frame.Height);
            _viewChart.Frame = chartFrame;
        }

        #region TARIFF LEGEND Methods
        public void SetTariffLegendComponent()
        {
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            if (tariffList != null && tariffList.Count > 0)
            {
                ViewHelper.AdjustFrameSetHeight(_viewLegend, 0);
                _viewLegend.BackgroundColor = UIColor.Clear;
                _viewLegend.Hidden = true;

                TariffLegendComponent tariffLegendComponent = new TariffLegendComponent(View, tariffList);
                _viewLegend.AddSubview(tariffLegendComponent.GetUI());
                SetContentView();
            }
        }
        private void ShowHideTariffLegends(bool isVisible)
        {
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            if (tariffList != null && tariffList.Count > 0)
            {
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
            ViewHelper.AdjustFrameSetHeight(_viewToggle, GetScaledHeight(24f));
            _viewStatus.BackgroundColor = UIColor.Clear;

            _tariffSelectionComponent = new TariffSelectionComponent(View);
            _viewToggle.AddSubview(_tariffSelectionComponent.GetUI());
            _rMkWhEnum = RMkWhEnum.RM;
            _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
            _tariffSelectionComponent.SetGestureRecognizerFoRMKwH(new UITapGestureRecognizer(() =>
            {
                ShowHideRMKwHDropDown();
            }));
            _tariffSelectionComponent.SetGestureRecognizerForTariff(new UITapGestureRecognizer(() =>
            {
                _tariffIsVisible = !_tariffIsVisible;
                _tariffSelectionComponent.UpdateTariffButton(_tariffIsVisible);
                ShowHideTariffLegends(_tariffIsVisible);
            }));

            CreateRMKwhDropdown();
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
            List<TipsModel> tipsList;
            EnergyTipsEntity wsManager = new EnergyTipsEntity();
            tipsList = wsManager.GetAllItems();

            if (tipsList != null &&
                tipsList.Count > 0)
            {
                ViewHelper.AdjustFrameSetHeight(_viewTips, GetScaledHeight(100f));
                _viewTips.BackgroundColor = UIColor.Clear;

                EnergyTipsComponent energyTipsComponent = new EnergyTipsComponent(_viewTips, tipsList);
                _viewTips.AddSubview(energyTipsComponent.GetUI());
            }
        }
        #endregion
        #region DISCONNECTION Methods
        public void SetDisconnectionComponent()
        {
            AccountStatusDataModel accountStatusData = AccountStatusCache.GetAccountStatusData();
            if (accountStatusData != null &&
                accountStatusData.DisconnectionStatus.ToLower() != "available")
            {
                ViewHelper.AdjustFrameSetHeight(_viewStatus, GetScaledHeight(24f));
                _viewStatus.BackgroundColor = UIColor.Clear;

                DisconnectionComponent disconnectionComponent = new DisconnectionComponent(_scrollViewContent, accountStatusData);
                _viewStatus.AddSubview(disconnectionComponent.GetUI());
                disconnectionComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    var acctStatusTooltipBtnTitle = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalBtnText) ? accountStatusData.AccountStatusModalBtnText : "Common_GotIt".Translate();
                    var acctStatusTooltipMsg = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalMessage) ? accountStatusData.AccountStatusModalMessage : "Dashboard_AccountStatusMessage".Translate();
                    DisplayCustomAlert(string.Empty, acctStatusTooltipMsg, acctStatusTooltipBtnTitle, null);
                }));
                SetContentView();
            }
        }
        #endregion
        #region FOOTER Methods
        private void SetFooterView()
        {
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
            UsageFooterViewComponent footerViewComponent = new UsageFooterViewComponent(View, footerHeight);
            _viewFooter.AddSubview(footerViewComponent.GetUI());
        }

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
