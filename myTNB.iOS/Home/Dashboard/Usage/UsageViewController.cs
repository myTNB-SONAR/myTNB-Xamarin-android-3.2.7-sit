using CoreGraphics;
using Foundation;
using myTNB.Home.Components.UsageView;
using myTNB.Home.Dashboard.Usage;
using myTNB.Model;
using myTNB.Model.Usage;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class UsageViewController : CustomUIViewController
    {
        public UsageViewController(IntPtr handle) : base(handle) { }

        TariffSelectionComponent _tariffSelectionComponent;

        UIView _navbarContainer, _tariffSelectionContainer, _energyTipsContainer, _footerViewContainer, _rmKwhDropDownView, _disconnectionContainer, _tariffLegendContainer;
        UILabel _RMLabel, _kWhLabel;

        nfloat _titleBarHeight = 24f;

        bool _rmkWhFlag, _tariffIsVisible = false;
        RMkWhEnum _rMkWhEnum;

        public override void ViewDidLoad()
        {
            PageName = "UsageView";
            base.ViewDidLoad();
            nfloat width = GetScaledWidth(320f);
            nfloat height = GetScaledHeight(543f);
            UIImageView bgImageView = new UIImageView(new CGRect(0, 0, width, height))
            {
                Image = UIImage.FromBundle("Stub-Usage-Bg")
            };
            View.AddSubview(bgImageView);
            NavigationController.NavigationBarHidden = true;
            CallGetAccountUsageAPI();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetNavigation();
            SetDisconnectionComponent();
            SetTariffSelectionComponent();
            SetEnergyTipsComponent();
            SetFooterView();
        }

        #region NAVIGATION BAR Methods
        private void SetNavigation()
        {
            _navbarContainer = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + 8f, _navbarContainer.Frame.Width, _titleBarHeight));

            UILabel lblTitle = new UILabel(new CGRect(58, 0, _navbarContainer.Frame.Width - 116, _titleBarHeight))
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
                Image = UIImage.FromBundle("Back-White")
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
        #endregion
        #region DISCONNECTION Methods
        private void SetDisconnectionComponent()
        {
            nfloat yPos = GetScaledHeight(90f);
            _disconnectionContainer = new UIView(new CGRect(0, yPos, ViewWidth, GetScaledHeight(24f)))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_disconnectionContainer);

            DisconnectionComponent disconnectionComponent = new DisconnectionComponent(View);
            _disconnectionContainer.AddSubview(disconnectionComponent.GetUI());
        }
        #endregion
        #region TARIFF LEGEND Methods
        private void SetTariffLegendComponent()
        {
            nfloat yPos = _tariffSelectionContainer.Frame.GetMaxY() + GetScaledHeight(30f);
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            nfloat height = 0;
            if (tariffList != null && tariffList.Count > 0)
            {
                height = tariffList.Count * GetScaledHeight(25f);
            }
            _tariffLegendContainer = new UIView(new CGRect(0, yPos, ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true
            };
            View.AddSubview(_tariffLegendContainer);

            TariffLegendComponent tariffLegendComponent = new TariffLegendComponent(View, tariffList);
            _tariffLegendContainer.AddSubview(tariffLegendComponent.GetUI());
        }
        private void ShowHideTariffLegends(bool isVisible)
        {
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            if (tariffList != null && tariffList.Count > 0)
            {
                _tariffLegendContainer.Hidden = !isVisible;
            }
        }
        #endregion
        #region RM/KWH & TARIFF Methods
        private void SetTariffSelectionComponent()
        {
            nfloat yPos = _disconnectionContainer.Frame.GetMaxY() + 40f;
            _tariffSelectionContainer = new UIView(new CGRect(0, yPos, ViewWidth, GetScaledHeight(24f)))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_tariffSelectionContainer);

            _tariffSelectionComponent = new TariffSelectionComponent(View);
            _tariffSelectionContainer.AddSubview(_tariffSelectionComponent.GetUI());
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
            nfloat dropDownYPos = _tariffSelectionContainer.Frame.GetMinY() + GetScaledHeight(-56f);
            nfloat dropDownWidth = GetScaledWidth(60f);
            nfloat dropDownHeight = GetScaledHeight(48f);
            _rmKwhDropDownView = new UIView(new CGRect(dropDownXPos, dropDownYPos, dropDownWidth, dropDownHeight))
            {
                BackgroundColor = UIColor.White,
                Hidden = true
            };
            _rmKwhDropDownView.Layer.CornerRadius = GetScaledHeight(5f);
            View.AddSubview(_rmKwhDropDownView);

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

            nfloat footerRatio = 136.0f / 320.0f;
            nfloat footerHeight = ViewWidth * footerRatio;
            nfloat footerYPos = View.Frame.Height - footerHeight;

            _energyTipsContainer = new UIView(new CGRect(0, footerYPos - GetScaledHeight(140f), ViewWidth, GetScaledHeight(100f)))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_energyTipsContainer);

            EnergyTipsComponent energyTipsComponent = new EnergyTipsComponent(_energyTipsContainer, tipsList);
            _energyTipsContainer.AddSubview(energyTipsComponent.GetUI());
        }
        #endregion
        #region FOOTER Methods
        private void SetFooterView()
        {
            nfloat footerRatio = 136.0f / 320.0f;
            nfloat footerHeight = ViewWidth * footerRatio;
            nfloat footerYPos = View.Frame.Height - footerHeight;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                footerYPos -= 20f;
            }
            _footerViewContainer = new UIView(new CGRect(0, footerYPos, ViewWidth, footerHeight))
            {
                BackgroundColor = UIColor.White
            };
            AddFooterShadow(ref _footerViewContainer);
            View.AddSubview(_footerViewContainer);
            UsageFooterViewComponent footerViewComponent = new UsageFooterViewComponent(View, footerHeight);
            _footerViewContainer.AddSubview(footerViewComponent.GetUI());
        }

        private void AddFooterShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue35.CGColor;
            view.Layer.ShadowOpacity = .16f;
            view.Layer.ShadowOffset = new CGSize(0, -8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
        #endregion
        #region API Calls
        private void CallGetAccountUsageAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        AccountUsageCache.ClearTariffLegendList();

                        AccountUsageResponseModel accountUsageResponse = await GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountUsageCache.AddTariffLegendList(accountUsageResponse);

                        SetTariffLegendComponent();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private async Task<AccountUsageResponseModel> GetAccountUsage(CustomerAccountRecordModel account)
        {
            AccountUsageResponseModel accountUsageResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf
            };

            accountUsageResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountUsageResponseModel>("GetAccountUsage", requestParameter);
            });

            return accountUsageResponse;
        }

        #endregion
    }
}
