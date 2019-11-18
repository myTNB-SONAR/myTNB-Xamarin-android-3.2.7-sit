﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Enums;
using myTNB.Home.Components;
using myTNB.Model;
using myTNB.Model.Usage;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;
using CoreAnimation;
using Foundation;
using System.Diagnostics;

namespace myTNB
{
    public class UsageBaseViewController : CustomUIViewController
    {
        public UsageBaseViewController(IntPtr handle) : base(handle) { }

        TariffSelectionComponent _tariffSelectionComponent;
        UsageFooterViewComponent _footerViewComponent;
        REAmountComponent _rEAmountComponent;
        public DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        internal UIScrollView _scrollViewContent, _refreshScrollView;
        internal CustomUIView _navbarContainer, _accountSelectorContainer, _viewStatus
            , _viewChart, _viewRE, _viewLegend, _viewDPCNote, _viewToggle, _viewSSMR, _viewSmartMeter, _viewTips, _viewFooter, _rmKwhDropDownView, _viewRefresh
            , _chart, _tips, _RE, _RERefresh, _status, _sm, _ssmr, _ssmrRefresh, _tariff, _legend, _refresh, _lastView;
        internal UILabel _lblAddress, _RMLabel, _kWhLabel;
        internal UIImageView _footerRefreshBGImage, _footerBGImage, _scrollIndicatorView;
        internal UIView _smOverlayParentView;

        internal bool _rmkWhFlag, _tariffIsVisible;
        internal RMkWhEnum _rMkWhEnum;
        internal SmartMeterViewEnum _smViewEnum;
        internal nfloat _lastContentOffset, _footerYPos, _scrollViewYPos;
        internal bool isBcrmAvailable, isNormalChart, isREAccount, isSmartMeterAccount, accountIsSSMR;
        internal bool _legendIsVisible, _footerIsDocked, _isEmptyData, _isDPCIndicator, _lastSelectedIsDPC;
        public bool SMChartIsLoading, NormalChartIsLoading;

        internal CGRect _origViewFrame;

        protected AccountSelector _accountSelector;
        protected CustomUIView _viewAccountSelector;
        protected BaseChartView _chartView;
        protected List<LegendItemModel> _tariffList;

        public AccountUsageResponseModel _accountUsageResponse;
        public SMRAccountActivityInfoResponseModel _smrAccountActivityInfoResponse;

        private UIView _tutorialContainer;
        private nfloat _smrCardYPos, _smrCardHeight;
        private MonthItemModel _lastSelectedMonthItem = new MonthItemModel();

        public override void ViewDidLoad()
        {
            PageName = UsageConstants.PageName;
            IsNewGradientRequired = true;
            base.ViewDidLoad();
            InitializeValues();
            SetNavigation();
            PrepareRefreshView();
            AddScrollView();
            SetDisconnectionComponent(false);
            SetSSMRComponent(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(true, true);
            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
            {
                ResetViews();
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

        private void ShowPinchOverlay()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat widthMargin = GetScaledWidth(18f);
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_smOverlayParentView != null)
            {
                _smOverlayParentView.RemoveFromSuperview();
            }
            _smOverlayParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            currentWindow.AddSubview(_smOverlayParentView);
            SmartMeterOverlayComponent overlay = new SmartMeterOverlayComponent(_smOverlayParentView, _scrollViewYPos + _viewChart.Frame.Y)
            {
                GetI18NValue = GetI18NValue
            };
            _smOverlayParentView.AddSubview(overlay.GetUI());
            overlay.SetGestureForButton(new UITapGestureRecognizer(() =>
            {
                _smOverlayParentView.RemoveFromSuperview();
            }));
        }

        private void PrepareRefreshView()
        {
            nfloat height = UIScreen.MainScreen.Bounds.Height - _scrollViewYPos;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                height -= 20f;
            }
            _refreshScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(44F), ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = true,
                CanCancelContentTouches = false,
                DelaysContentTouches = true,
                Hidden = true
            };
            _refreshScrollView.Scrolled += OnScroll;
            View.AddSubview(_refreshScrollView);
            AddFooterRefreshBGImage(_refreshScrollView);
            _viewRefresh = new CustomUIView(new CGRect(0, 0, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            _refreshScrollView.AddSubview(_viewRefresh);
            _ssmrRefresh = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true
            };
            _viewRefresh.AddSubview(_ssmrRefresh);
            _RERefresh = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true
            };
            _viewRefresh.AddSubview(_RERefresh);
        }

        private void InitializeValues()
        {
            _scrollViewYPos = DeviceHelper.GetStatusBarHeight() + GetScaledHeight(44F);
            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            isNormalChart = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;
            isSmartMeterAccount = !isREAccount && !isNormalChart;
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            accountIsSSMR = false;
        }

        private void ResetViews()
        {
            if (_refreshScrollView != null)
            {
                _refreshScrollView.Hidden = true;
            }

            if (_scrollViewContent != null)
            {
                _scrollViewContent.Hidden = false;
            }
            ScrollToTop();
            InitializeValues();
            _rmkWhFlag = false;
            _tariffIsVisible = false;
            _rMkWhEnum = RMkWhEnum.RM;
            UpdateRMkWhSelectionColour(_rMkWhEnum);
            HideTariffLegend();
            if (!accountIsSSMR)
            {
                HideSSMRView();
            }
            SetFooterView();
            AddSubviews();
            RemoveDPCNote();
            SetContentView();
        }

        internal virtual void InitiateAPICalls() { }

        private void SetNavigation()
        {
            _navbarContainer = new CustomUIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8f), _navbarContainer.Frame.Width, GetScaledHeight(24f)));

            nfloat imageWidth = GetScaledWidth(24f);
            UIView viewBack = new UIView(new CGRect(BaseMarginWidth16, 0, imageWidth, imageWidth));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, imageWidth, imageWidth))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_Back)
            };
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationController.PopViewController(true);
            }));
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            nfloat xPos = viewBack.Frame.GetMaxX() + GetScaledWidth(5);
            _accountSelectorContainer = new CustomUIView(new CGRect(xPos, 0, ViewWidth - xPos - BaseMarginWidth16, GetScaledHeight(24)));
            AddAccountSelector();
            viewTitleBar.AddSubview(_accountSelectorContainer);

            _navbarContainer.AddSubview(viewTitleBar);
            View.AddSubview(_navbarContainer);
        }

        private void AddFooterBGImage(UIScrollView scrollView)
        {
            _footerBGImage = new UIImageView(new CGRect(0, View.Frame.Height, ViewWidth, GetScaledHeight(1000F)))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_FooterBG)
            };
            scrollView.AddSubview(_footerBGImage);
        }

        private void AddFooterRefreshBGImage(UIScrollView scrollView)
        {
            _footerRefreshBGImage = new UIImageView(new CGRect(0, View.Frame.Height, ViewWidth, GetScaledHeight(1000F)))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_FooterBG)
            };
            scrollView.AddSubview(_footerRefreshBGImage);
        }

        internal void UpdateFooterBGImageYPos()
        {
            if (isSmartMeterAccount)
            {
                if (!_viewSmartMeter.Hidden)
                {
                    _footerBGImage.Hidden = false;
                    ViewHelper.AdjustFrameSetY(_footerBGImage, GetYPosForBG(_viewSmartMeter));
                }
                else
                {
                    _footerBGImage.Hidden = true;
                }
            }
            else if (isREAccount)
            {
                _footerBGImage.Hidden = false;
                ViewHelper.AdjustFrameSetY(_footerBGImage, GetYPosForBG(_viewRE));
            }
            else if (accountIsSSMR)
            {
                if (!_viewSSMR.Hidden)
                {
                    _footerBGImage.Hidden = false;
                    ViewHelper.AdjustFrameSetY(_footerBGImage, GetYPosForBG(_viewSSMR));
                }
                else
                {
                    _footerBGImage.Hidden = true;
                }
            }
            else
            {
                _footerBGImage.Hidden = true;
            }
        }

        private nfloat GetYPosForBG(CustomUIView view)
        {
            nfloat yPos = 0;
            if (view != null)
            {
                yPos = view.Frame.GetMinY() + view.Frame.Height / 2;
            }
            return yPos;
        }

        private void AddScrollView()
        {
            nfloat height = UIScreen.MainScreen.Bounds.Height - _scrollViewYPos;
            if (TabBarController != null && TabBarController.TabBar != null)
            {
                height -= TabBarController.TabBar.Frame.Height;
            }
            _scrollViewContent = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(44F), ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = true,
                CanCancelContentTouches = false,
                DelaysContentTouches = true
            };
            _scrollViewContent.Scrolled += OnScroll;
            View.AddSubview(_scrollViewContent);

            AddFooterBGImage(_scrollViewContent);

            _lblAddress = new UILabel(new CGRect(GetScaledWidth(32F), 0, ViewWidth - (GetScaledWidth(32F) * 2), 0))
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_10_300
            };
            _viewStatus = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewChart = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewRE = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
            {
                Hidden = true
            };
            _viewDPCNote = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
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

            _viewSmartMeter = new CustomUIView(new CGRect(0, 0, ViewWidth, 0))
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

            _scrollViewContent.AddSubviews(new UIView[] { _lblAddress, _viewStatus, _viewChart, _viewDPCNote, _viewSmartMeter, _viewRE, _viewLegend, _viewToggle, _viewSSMR, _viewTips });
        }

        private void SetContentView()
        {
            _lblAddress.Frame = new CGRect(new CGPoint(GetScaledWidth(32F), 0), _lblAddress.Frame.Size);
            if (!_isEmptyData)
            {
                if (!AccountStatusCache.AccountStatusIsAvailable() && !_viewStatus.Hidden)
                {
                    _viewStatus.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewStatus.Frame.Size);
                    _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewStatus.Frame, 16F)), _viewChart.Frame.Size);
                }
                else
                {
                    _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_lblAddress.Frame, 12F)), _viewChart.Frame.Size);
                }

                if (isREAccount)
                {
                    _viewRE.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 24F)), _viewRE.Frame.Size);
                    _lastView = _viewRE;
                }
                else
                {
                    if (_isDPCIndicator)
                    {
                        _viewDPCNote.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 12F)), _viewDPCNote.Frame.Size);
                        _viewToggle.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewDPCNote.Frame, 24F)), _viewToggle.Frame.Size);
                    }
                    else
                    {
                        _viewLegend.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, _legendIsVisible && _tariffList?.Count > 0 ? 16F : 0F)), _viewLegend.Frame.Size);
                        _viewToggle.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_legendIsVisible ? _viewLegend.Frame : _viewChart.Frame, 24F)), _viewToggle.Frame.Size);
                    }
                    _lastView = _viewToggle;
                    if (accountIsSSMR)
                    {
                        _viewSSMR.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 16F)), _viewSSMR.Frame.Size);
                        _lastView = _viewSSMR;
                        if (!AppLaunchMasterCache.IsEnergyTipsDisabled)
                        {
                            _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSSMR.Frame, 16F)), _viewTips.Frame.Size);
                            _lastView = _viewTips;
                        }
                    }
                    else if (isSmartMeterAccount)
                    {
                        _viewSmartMeter.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 24F)), _viewSmartMeter.Frame.Size);
                        _lastView = _viewSmartMeter;
                        if (!AppLaunchMasterCache.IsEnergyTipsDisabled)
                        {
                            _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewSmartMeter.Frame, 16F)), _viewTips.Frame.Size);
                            _lastView = _viewTips;
                        }
                    }
                    else
                    {
                        if (!AppLaunchMasterCache.IsEnergyTipsDisabled)
                        {
                            _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 24F)), _viewTips.Frame.Size);
                            _lastView = _viewTips;
                        }
                    }
                }
            }
            else
            {
                if (!AccountStatusCache.AccountStatusIsAvailable() && !_viewStatus.Hidden)
                {
                    _viewStatus.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewStatus.Frame.Size);
                    _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewStatus.Frame, 16F)), _viewChart.Frame.Size);
                }
                else
                {
                    _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewChart.Frame.Size);
                }

                if (isSmartMeterAccount)
                {
                    _viewSmartMeter.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 16F)), _viewSmartMeter.Frame.Size);
                    _lastView = _viewSmartMeter;
                }
                else
                {
                    if (isREAccount)
                    {
                        _viewRE.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 24F)), _viewRE.Frame.Size);
                        _lastView = _viewRE;
                    }
                    else
                    {
                        if (_viewSSMR.Hidden)
                        {
                            _lastView = _viewChart;
                        }
                        else
                        {
                            _viewSSMR.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 32F)), _viewSSMR.Frame.Size);
                            _lastView = _viewSSMR;
                        }
                    }
                }
            }

            _footerIsDocked = (_lastView.Frame.GetMaxY() + _scrollViewYPos) < _footerYPos + GetScaledHeight(10);
            if (_footerViewComponent != null)
            {
                var view = _footerViewComponent.GetView();
                AddFooterViewShadow(ref view, _footerIsDocked);
            }
            if (_scrollIndicatorView != null)
            {
                _scrollIndicatorView.Hidden = _footerIsDocked;
            }
            if (_footerIsDocked && _viewFooter.Frame != _origViewFrame)
            {
                AnimateFooterToHideAndShow(false);
            }
            UpdateFooterBGImageYPos();
            _scrollViewContent.ContentSize = new CGSize(ViewWidth, isREAccount ? _viewRE.Frame.GetMaxY() : GetAdditionalHeight(_lastView.Frame.GetMaxY()));
            _smrCardYPos = (nfloat)_viewSSMR?.Frame.Y;

            if (_viewFooter != null)
            {
                if (_viewFooter.Frame == _origViewFrame && !_footerIsDocked)
                {
                    HideShowBottomCards(true);
                }
            }
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
            SetChartView(true);
            SetTariffSelectionComponent();
            SetSmartMeterComponent(true);
            if (!AppLaunchMasterCache.IsEnergyTipsDisabled)
            {
                SetEnergyTipsComponent();
            }
            SetREAmountView();
        }

        private void AddAccountSelector()
        {
            if (_viewAccountSelector != null)
            {
                _viewAccountSelector.RemoveFromSuperview();
            }

            _accountSelector = new AccountSelector(_accountSelectorContainer);
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
                viewController.IsRoot = false;
                viewController.IsFromUsage = true;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }));

            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
        }

        private void SetAddress()
        {
            UIStringAttributes stringAttributes = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_10_300,
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle() { LineSpacing = 3.0f, Alignment = UITextAlignment.Center }
            };
            var text = DataManager.DataManager.SharedInstance.SelectedAccount?.accountStAddress?.ToUpper();//AccountManager.Instance.Address.ToUpper();
            var AttributedText = new NSMutableAttributedString(text);
            AttributedText.AddAttributes(stringAttributes, new NSRange(0, text.Length));
            _lblAddress.AttributedText = AttributedText;
            CGSize lblSize = _lblAddress.SizeThatFits(new CGSize(_lblAddress.Frame.Width, GetScaledHeight(42F)));
            ViewHelper.AdjustFrameSetHeight(_lblAddress, lblSize.Height);
        }

        public void SetChartView(bool isUpdating)
        {
            _isEmptyData = false;
            if (isREAccount)
            {
                _chartView = new REChartView();
            }
            else if (isNormalChart)
            {
                _chartView = new NormalChartView();
                _chartView.LoadTariffLegendWithIndex = LoadTariffLegendWithIndex;
            }
            else
            {
                _chartView = new SmartMeterChartView()
                {
                    PinchOverlayAction = ShowPinchOverlay,
                    LoadTariffLegendWithIndex = LoadTariffLegendWithIndex,
                    LoadTariffLegendWithBlockIds = LoadTariffLegendWithBlockIds,
                    ShowMissedReadToolTip = ShowMissedReadTooltip,
                    GetI18NValue = GetI18NValue,
                    OnMDMSIconTap = OnMDMSIconTap,
                    SetDPCNoteForMDMSDown = SetDPCNoteForMDMSDown
                };
            }

            if (_chart != null)
            {
                _chart.RemoveFromSuperview();
            }
            _chart = isUpdating ? _chartView.GetShimmerUI() : _chartView.GetUI();
            _viewChart.AddSubview(_chart);
            ViewHelper.AdjustFrameSetHeight(_viewChart, _chart.Frame.Height);
        }

        private void OnMDMSIconTap()
        {
            string title = AccountUsageSmartCache.ErrorTitle;
            string message = AccountUsageSmartCache.ErrorMessage;
            string ctaTitle = AccountUsageSmartCache.ErrorCTA;
            DisplayCustomAlert(title, message, new Dictionary<string, Action> { { ctaTitle, null } });
        }

        #region DPC Methods
        private void SetDPCNoteOnBarTap(MonthItemModel item)
        {
            if (item.DPCIndicator && _rMkWhEnum == RMkWhEnum.kWh || (_rMkWhEnum == RMkWhEnum.RM && _tariffIsVisible && item.DPCIndicator))
            {
                var msg = _tariffIsVisible ? item.DPCIndicatorTariffMessage : item.DPCIndicatorUsageMessage;
                SetDPCNote(msg);
            }
            else
            {
                RemoveDPCNote();
            }
            SetContentView();
        }

        private void SetCPCNoteForShowHideTariff()
        {
            var msg = _tariffIsVisible ? _lastSelectedMonthItem.DPCIndicatorTariffMessage : _lastSelectedMonthItem.DPCIndicatorUsageMessage;
            SetDPCNote(msg);
            SetContentView();
        }

        private void SetDPCNoteForRMKwHToggle()
        {
            if (_rMkWhEnum == RMkWhEnum.kWh)
            {
                if (_lastSelectedIsDPC)
                {
                    var msg = _tariffIsVisible ? _lastSelectedMonthItem.DPCIndicatorTariffMessage : _lastSelectedMonthItem.DPCIndicatorUsageMessage;
                    SetDPCNote(msg);
                }
            }
            else
            {
                if (_lastSelectedIsDPC)
                {
                    RemoveDPCNote();
                }
            }
            SetContentView();
        }

        private void SetDPCNoteForMDMSDown(bool isHidden)
        {
            if (isHidden)
            {
                if (_lastSelectedIsDPC)
                {
                    RemoveDPCNote();
                }
            }
            else
            {
                if (_lastSelectedIsDPC)
                {
                    var msg = _tariffIsVisible ? _lastSelectedMonthItem.DPCIndicatorTariffMessage : _lastSelectedMonthItem.DPCIndicatorUsageMessage;
                    SetDPCNote(msg);
                }
            }
            SetContentView();
        }

        private void SetDPCNote(string dpcMessage)
        {
            _isDPCIndicator = true;
            UITextView textView = _viewDPCNote.ViewWithTag(1001) as UITextView;
            if (textView != null) { textView.RemoveFromSuperview(); }

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(dpcMessage
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(10F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView dpcNote = new UITextView(new CGRect(GetScaledWidth(24f), 0, _viewDPCNote.Frame.Width - (GetScaledWidth(24f) * 2), GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextContainerInset = UIEdgeInsets.Zero,
                Tag = 1001
            };
            CGSize cGSize = dpcNote.SizeThatFits(new CGSize(dpcNote.Frame.Width, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetHeight(dpcNote, cGSize.Height);
            _viewDPCNote.AddSubview(dpcNote);
            ViewHelper.AdjustFrameSetHeight(_viewDPCNote, dpcNote.Frame.Height);
            _viewDPCNote.Hidden = false;
        }

        private void RemoveDPCNote()
        {
            _isDPCIndicator = false;
            ViewHelper.AdjustFrameSetHeight(_viewDPCNote, 0);
            _viewDPCNote.BackgroundColor = UIColor.Clear;
            _viewDPCNote.Hidden = true;
        }
        #endregion
        #region TUTORIAL OVERLAY Methods
        public void CheckTutorialOverlay()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(UsageConstants.Pref_UsageSSMRTutorialOverlay);

            if (tutorialOverlayHasShown)
                return;

            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is UsageViewController)
                {
                    if (accountIsSSMR)
                    {
                        ShowTutorialOverlay();
                    }
                }
            }
        }

        private void ShowTutorialOverlay()
        {
            ScrollToTop();
            AnimateFooterToHideAndShow(true);
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_tutorialContainer != null)
            {
                _tutorialContainer.RemoveFromSuperview();
            }
            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            currentWindow.AddSubview(_tutorialContainer);

            nfloat addtl = 0;
            if (DeviceHelper.IsIphone5())
            {
                addtl = -GetScaledHeight(8F);
            }
            else if (DeviceHelper.IsIphone678PlusResolution())
            {
                addtl = GetScaledHeight(3F);
            }

            UsageSSMRTutorialOverlay tutorialView = new UsageSSMRTutorialOverlay(_tutorialContainer)
            {
                GetI18NValue = GetI18NValue,
                NavigationHeight = _navbarContainer.Frame.Height,
                SSMRCardYPos = _smrCardYPos + GetScaledHeight(8F) + addtl,
                SSMRCardHeight = _smrCardHeight,
                OnDismissAction = HideTutorialOverlay
            };
            _tutorialContainer.AddSubview(tutorialView.GetView());

            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(true, UsageConstants.Pref_UsageSSMRTutorialOverlay);
        }

        private void HideTutorialOverlay()
        {
            AnimateFooterToHideAndShow(false);
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);
            }
        }

        private void ScrollToTop()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(UsageConstants.Pref_UsageSSMRTutorialOverlay);
            if (tutorialOverlayHasShown || !accountIsSSMR)
                return;
            Debug.WriteLine("ScrollToTop()");
            CGPoint topOffset = new CGPoint(0, 0);
            _scrollViewContent.SetContentOffset(topOffset, false);
        }
        #endregion
        #region EMPTY DATA Methods
        internal void SetEmptyDataComponent(string message)
        {
            _isEmptyData = true;
            _viewChart.BackgroundColor = UIColor.Clear;
            var yPos = isREAccount || accountIsSSMR ? GetHeightByScreenSize(16F) : GetHeightByScreenSize(50F);
            EmptyUsageComponent emptyUsageComponent = new EmptyUsageComponent(_viewChart, yPos)
            {
                GetI18NValue = GetI18NValue
            };
            if (_chart != null)
            {
                _chart.RemoveFromSuperview();
            }
            _chart = emptyUsageComponent.GetUI(message);
            ViewHelper.AdjustFrameSetHeight(_viewChart, emptyUsageComponent.GetView().Frame.Height);
            _viewChart.AddSubview(_chart);
            _viewToggle.Hidden = _isEmptyData;
            SetContentView();
        }
        #endregion
        #region SMART METER Methods
        internal void SetSmartMeterComponent(bool isUpdating, List<UsageCostItemModel> usageCostModel = null)
        {
            if (isSmartMeterAccount)
            {
                ViewHelper.AdjustFrameSetHeight(_viewSmartMeter, _rMkWhEnum == RMkWhEnum.RM ? GetScaledHeight(169F) : GetScaledHeight(129F));
                _viewSmartMeter.BackgroundColor = UIColor.Clear;
                _viewSmartMeter.Hidden = false;

                if (_sm != null)
                {
                    _sm.RemoveFromSuperview();
                }
                SmartMeterCardComponent smartMeterComponent = new SmartMeterCardComponent(_viewSmartMeter, usageCostModel, _rMkWhEnum);
                if (isUpdating)
                {
                    _sm = smartMeterComponent.GetShimmerUI();
                    _viewSmartMeter.AddSubview(_sm);
                    AddSmartMeterViewShadow(ref _sm);
                }
                else
                {
                    List<ToolTipItemModel> toolTips = AccountUsageSmartCache.GetTooltips();
                    ToolTipItemModel toolTipItem = new ToolTipItemModel();
                    if (toolTips != null && toolTips.Count > 0 &&
                        usageCostModel != null && usageCostModel.Count > 1)
                    {
                        toolTipItem = toolTips.Find(x => x.UsageCostType == usageCostModel[1].UsageCostType);
                    }
                    var toolTipMsg = GetI18NValue(UsageConstants.I18N_ProjectedCostMessage);
                    var toolTipBtnTitle = GetI18NValue(UsageConstants.I18N_GotIt);
                    var toolTipProjCostTitle = GetI18NValue(UsageConstants.I18N_ProjectCostTitle);

                    if (toolTipItem != null)
                    {
                        if (toolTipItem.Message != null)
                        {
                            if (toolTipItem.Message.Count > 0)
                            {
                                toolTipMsg = toolTipItem.Message[0];
                            }
                        }
                        toolTipBtnTitle = toolTipItem.SMBtnText;
                        toolTipProjCostTitle = toolTipItem.SMLink;
                    }
                    _sm = smartMeterComponent.GetUI();
                    smartMeterComponent.SetTooltipText(toolTipProjCostTitle);
                    smartMeterComponent.SetTooltipTapRecognizer(new UITapGestureRecognizer(() =>
                    {
                        DisplayCustomAlert(string.Empty, toolTipMsg, toolTipBtnTitle, null);
                    }));
                    _viewSmartMeter.AddSubview(_sm);
                    AddSmartMeterViewShadow(ref _sm);
                }
            }
            else
            {
                ViewHelper.AdjustFrameSetHeight(_viewSmartMeter, 0);
                _viewSmartMeter.Hidden = true;
            }
            UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn
                    , () =>
                    {
                        SetContentView();
                    }
                    , () =>
                    {
                    }
                );
        }

        private void AddSmartMeterViewShadow(ref CustomUIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = .32f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private void ShowMissedReadTooltip()
        {
            List<ToolTipItemModel> toolTips = AccountUsageSmartCache.GetTooltips();
            ToolTipItemModel toolTipItem = toolTips.Find(x => x.Type.Equals(UsageConstants.STR_MissingReading));

            var toolTipMsg = GetI18NValue(UsageConstants.I18N_MissedReadMessage);
            var toolTipBtnTitle = GetI18NValue(UsageConstants.I18N_GotIt);
            var toolTipTitle = GetI18NValue(UsageConstants.I18N_MissedReadTitle);

            if (toolTipItem != null)
            {
                if (toolTipItem.Message != null)
                {
                    if (toolTipItem.Message.Count > 0)
                    {
                        toolTipMsg = toolTipItem.Message[0];
                    }
                }
                toolTipBtnTitle = toolTipItem.SMBtnText;
                toolTipTitle = toolTipItem.Title;
                DisplayCustomAlert(toolTipTitle, toolTipMsg, toolTipBtnTitle, null);
            }
        }

        internal void HideSmartMeterComponent()
        {
            ViewHelper.AdjustFrameSetHeight(_viewSmartMeter, 0);
            _viewSmartMeter.Hidden = true;
        }
        #endregion
        #region SSMR Methods
        internal void SetSSMRComponent(bool isUpdating, bool forRefreshScreen = false)
        {
            if (!isREAccount && accountIsSSMR)
            {
                ViewHelper.AdjustFrameSetHeight(_viewSSMR, GetScaledHeight(116f));
                _viewSSMR.BackgroundColor = UIColor.Clear;
                _viewSSMR.Hidden = false;

                if (_ssmr != null)
                {
                    _ssmr.RemoveFromSuperview();
                }
                SSMRComponent sSMRComponent = new SSMRComponent(forRefreshScreen ? _viewRefresh : _viewSSMR)
                {
                    GetI18NValue = GetI18NValue
                };

                if (isUpdating)
                {
                    _ssmr = sSMRComponent.GetShimmerUI();
                    _viewSSMR.AddSubview(_ssmr);
                    if (forRefreshScreen)
                    {
                        if (_refresh != null)
                        {
                            _ssmrRefresh.Hidden = false;
                            _ssmrRefresh.AddSubview(_ssmr);
                            ViewHelper.AdjustFrameSetY(_ssmrRefresh, GetYLocationFromFrame(_refresh.Frame, GetScaledHeight(8F)));
                            ViewHelper.AdjustFrameSetHeight(_ssmrRefresh, GetScaledHeight(116f));
                            _viewRefresh.AddSubview(_ssmrRefresh);
                        }
                    }
                }
                else
                {
                    MeterReadingHistoryModel smrAcountInfo = SSMRActivityInfoCache.DashboardMeterReadingHistory;
                    if (smrAcountInfo != null)
                    {
                        _ssmr = sSMRComponent.GetUI();

                        sSMRComponent.SetDescription(smrAcountInfo.DashboardMessage);
                        sSMRComponent.SetButtonText(smrAcountInfo.DashboardCTAText);
                        sSMRComponent.SetSRMButtonEnable(smrAcountInfo.IsDashboardCTADisabled);
                        sSMRComponent.ShowHistoryLink(smrAcountInfo.ShowReadingHistoryLink, smrAcountInfo.ReadingHistoryLinkText);
                        if (sSMRComponent._labelViewHistory != null)
                        {
                            sSMRComponent._labelViewHistory.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                            {
                                OnReadHistoryTap();
                            }));
                        }
                        if (sSMRComponent._smrButton != null)
                        {
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
                        }
                        _viewSSMR.AddSubview(_ssmr);
                        ViewHelper.AdjustFrameSetHeight(_viewSSMR, sSMRComponent.GetContainerHeight());
                        _viewSSMR.BackgroundColor = UIColor.Clear;
                        _viewSSMR.Hidden = false;
                        AddSSMRViewShadow(ref _ssmr);
                        if (forRefreshScreen)
                        {
                            if (_refresh != null)
                            {
                                _ssmrRefresh.Hidden = false;
                                _ssmrRefresh.AddSubview(_ssmr);
                                _viewRefresh.AddSubview(_ssmrRefresh);
                                ViewHelper.AdjustFrameSetY(_ssmrRefresh, GetYLocationFromFrame(_refresh.Frame, GetScaledHeight(8F)));
                                ViewHelper.AdjustFrameSetHeight(_ssmrRefresh, sSMRComponent.GetContainerHeight());
                                ViewHelper.AdjustFrameSetHeight(_viewRefresh, _refresh.Frame.Height + sSMRComponent.GetContainerHeight() + GetScaledHeight(16F));
                                AddSSMRViewShadow(ref _ssmr);
                                SetContentViewForRefresh();
                            }
                        }
                        _smrCardHeight = sSMRComponent.GetContainerHeight();
                    }
                }
            }
            else
            {
                if (forRefreshScreen)
                {
                    if (_ssmr != null)
                    {
                        _ssmr.RemoveFromSuperview();
                    }
                }
                else
                {
                    ViewHelper.AdjustFrameSetHeight(_viewSSMR, 0);
                    _viewSSMR.Hidden = true;
                }
            }
            SetContentView();
        }

        internal void HideSSMRView()
        {
            ViewHelper.AdjustFrameSetHeight(_viewSSMR, 0);
            _viewSSMR.Hidden = true;
        }

        internal void HideSSMRViewForRefresh()
        {
            if (_ssmrRefresh != null)
            {
                _ssmrRefresh.Hidden = true;
                ViewHelper.AdjustFrameSetHeight(_ssmrRefresh, 0);
            }
        }

        internal void HideTariffLegend()
        {
            ViewHelper.AdjustFrameSetHeight(_viewLegend, 0);
            _viewLegend.Hidden = true;
            _legendIsVisible = false;
        }

        internal virtual void OnReadHistoryTap()
        {
            DataManager.DataManager.SharedInstance.IsSameAccount = false;
        }

        internal virtual void OnSubmitMeterTap()
        {
            DataManager.DataManager.SharedInstance.IsSameAccount = false;
        }

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
        public void SetTariffLegendComponent(List<LegendItemModel> tariffList = null)
        {
            if (!isREAccount)
            {
                if (tariffList != null && tariffList.Count > 0)
                {
                    ViewHelper.AdjustFrameSetHeight(_viewLegend, 0);
                    _viewLegend.BackgroundColor = UIColor.Clear;
                    if (_legend != null)
                    {
                        _legend.RemoveFromSuperview();
                    }
                    TariffLegendComponent tariffLegendComponent = new TariffLegendComponent(View, tariffList)
                    {
                        GetI18NValue = GetI18NValue
                    };
                    _legend = tariffLegendComponent.GetUI();
                    _viewLegend.AddSubview(_legend);
                    if (_legendIsVisible)
                    {
                        ShowHideTariffLegends(_legendIsVisible);
                    }
                }
                else
                {
                    ShowHideTariffLegends(_legendIsVisible);
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
            _legendIsVisible = isVisible;
            _viewLegend.Hidden = true;
            UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn
                , () =>
                {
                    nfloat addtl = isVisible && _tariffList.Count > 0 ? GetScaledHeight(28F) + GetScaledHeight(20F) : 0;
                    nfloat height = isVisible ? _tariffList.Count * GetScaledHeight(25f) : 0;
                    ViewHelper.AdjustFrameSetHeight(_viewLegend, height + addtl);
                    SetContentView();
                    if (!isVisible)
                    {
                        ScrollToTop();
                        if (_viewFooter != null)
                        {
                            if (!_footerIsDocked && _viewFooter.Frame != _origViewFrame)
                            {
                                AnimateFooterToHideAndShow(false);
                            }
                        }
                    }
                }
                , () =>
                {
                    if (isVisible && _tariffList != null && _tariffList.Count > 0)
                    {
                        _viewLegend.Hidden = false;
                        if ((_viewToggle.Frame.GetMaxY() + _scrollViewYPos) > _footerYPos + GetScaledHeight(10))
                        {
                            AnimateFooterToHideAndShow(true);
                        }
                    }
                    else
                    {
                        _viewLegend.Hidden = true;
                    }
                    UpdateFooterBGImageYPos();
                }
            );
        }
        private void LoadTariffLegendWithIndex(int index)
        {
            List<MonthItemModel> usageData = isSmartMeterAccount ? AccountUsageSmartCache.ByMonthUsage : AccountUsageCache.ByMonthUsage;
            if (usageData != null && usageData.Count > 0)
            {
                if (index > -1 && index < usageData.Count)
                {
                    MonthItemModel item = usageData[index];
                    if (item != null)
                    {
                        _lastSelectedIsDPC = item.DPCIndicator;
                        _lastSelectedMonthItem = item;
                        if (!item.DPCIndicator)
                        {
                            if (item.tariffBlocks != null &&
                            item.tariffBlocks.Count > 0)
                            {
                                List<LegendItemModel> tariffLegend = new List<LegendItemModel>(isSmartMeterAccount ? AccountUsageSmartCache.GetTariffLegendList() : AccountUsageCache.GetTariffLegendList());
                                if (tariffLegend != null && tariffLegend.Count > 0)
                                {
                                    _tariffList = new List<LegendItemModel>();
                                    foreach (var legend in tariffLegend)
                                    {
                                        var res = false;
                                        foreach (var tBlock in item.tariffBlocks)
                                        {
                                            if (tBlock.BlockId != null)
                                            {
                                                if (tBlock.BlockId.Equals(legend.BlockId) && tBlock.Usage > 0)
                                                {
                                                    res = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (res)
                                        {
                                            _tariffList.Add(legend);
                                        }
                                    }
                                    _legendIsVisible = _tariffIsVisible;
                                    SetTariffLegendComponent(_tariffList);
                                }
                            }
                        }
                        else
                        {
                            ShowHideTariffLegends(false);
                        }
                        SetDPCNoteOnBarTap(item);
                    }
                }
            }
        }
        public void LoadTariffLegendWithBlockIds(List<String> blockIdList = null)
        {
            if (blockIdList != null && blockIdList.Count > 0)
            {
                List<LegendItemModel> tariffLegend = new List<LegendItemModel>(isSmartMeterAccount ? AccountUsageSmartCache.GetTariffLegendList() : AccountUsageCache.GetTariffLegendList());
                if (tariffLegend != null && tariffLegend.Count > 0)
                {
                    _tariffList = new List<LegendItemModel>();
                    foreach (var legend in tariffLegend)
                    {
                        var res = false;
                        foreach (var blockId in blockIdList)
                        {
                            if (blockId.Equals(legend.BlockId))
                            {
                                res = true;
                                break;
                            }
                        }
                        if (res)
                        {
                            _tariffList.Add(legend);
                        }
                    }
                    SetTariffLegendComponent(_tariffList);
                }
            }
        }
        #endregion
        #region RM/KWH & TARIFF Methods
        public void SetTariffButtonState()
        {
            if (_tariffSelectionComponent != null)
            {
                List<LegendItemModel> tariffList;
                bool isDisable;
                if (isSmartMeterAccount)
                {
                    tariffList = new List<LegendItemModel>(AccountUsageSmartCache.GetTariffLegendList());
                    isDisable = AccountUsageSmartCache.IsMDMSDown || AccountUsageSmartCache.IsMonthlyTariffDisable || AccountUsageSmartCache.IsMonthlyTariffUnavailable || tariffList == null || tariffList.Count == 0;
                }
                else
                {
                    tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
                    isDisable = AccountUsageCache.IsMonthlyTariffDisable || AccountUsageCache.IsMonthlyTariffUnavailable || tariffList == null || tariffList.Count == 0;
                }
                _tariffSelectionComponent.SetTariffButtonDisable(isDisable);
            }
        }

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
                _tariffSelectionComponent = new TariffSelectionComponent(View)
                {
                    GetI18NValue = GetI18NValue
                };
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
                    if (!_tariffSelectionComponent.isTariffDisabled)
                    {
                        ValidateTariffLegend();
                    }
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

        private void ValidateTariffLegend()
        {
            if (_lastSelectedIsDPC)
            {
                if (_tariffIsVisible)
                {
                    _tariffIsVisible = !_tariffIsVisible;
                    if (_rMkWhEnum == RMkWhEnum.RM)
                    {
                        RemoveDPCNote();
                        SetContentView();
                    }
                    else
                    {
                        SetCPCNoteForShowHideTariff();
                    }
                }
                else
                {
                    _tariffIsVisible = !_tariffIsVisible;
                    SetCPCNoteForShowHideTariff();
                }
                _tariffSelectionComponent.UpdateTariffButton(_tariffIsVisible);
                _chartView.ToggleTariffView(_tariffIsVisible);
            }
            else
            {
                RemoveDPCNote();
                List<LegendItemModel> tariffList = new List<LegendItemModel>(isSmartMeterAccount ? AccountUsageSmartCache.GetTariffLegendList() : AccountUsageCache.GetTariffLegendList());
                if (tariffList != null && tariffList.Count > 0)
                {
                    if (_rmKwhDropDownView != null)
                    {
                        _rmKwhDropDownView.Hidden = true;
                    }
                    _tariffIsVisible = !_tariffIsVisible;
                    _tariffSelectionComponent.UpdateTariffButton(_tariffIsVisible);
                    ShowHideTariffLegends(_tariffIsVisible);
                    _chartView.ToggleTariffView(_tariffIsVisible);
                }
            }
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
                _rMkWhEnum = RMkWhEnum.kWh;
                ShowHideRMKwHDropDown();
                _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
                UpdateRMkWhSelectionColour(_rMkWhEnum);
                ToggleRMkWh();
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
                _rMkWhEnum = RMkWhEnum.RM;
                ShowHideRMKwHDropDown();
                _tariffSelectionComponent.SetRMkWhLabel(_rMkWhEnum);
                UpdateRMkWhSelectionColour(_rMkWhEnum);
                ToggleRMkWh();
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
            if (isSmartMeterAccount && SMChartIsLoading)
                return;

            if (!isREAccount && isNormalChart && NormalChartIsLoading)
                return;

            nfloat dropDownYPos = _viewToggle.Frame.GetMinY() + GetScaledHeight(-56f);
            ViewHelper.AdjustFrameSetY(_rmKwhDropDownView, dropDownYPos);
            _rmkWhFlag = !_rmkWhFlag;
            _rmKwhDropDownView.Hidden = !_rmkWhFlag;
        }

        private void UpdateRMkWhSelectionColour(RMkWhEnum rMkWhEnum)
        {
            if (_kWhLabel != null & _RMLabel != null)
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
        }

        private void ToggleRMkWh()
        {
            if (isSmartMeterAccount)
            {
                OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                if (model != null && (model.Cost != null || model.Usage != null))
                {
                    if (_chartView != null)
                    {
                        _chartView.ToggleRMKWHValues(_rMkWhEnum);
                    }
                    if (!AccountUsageSmartCache.IsMDMSDown)
                    {
                        SetSmartMeterComponent(false, (_rMkWhEnum == RMkWhEnum.RM) ? model.Cost : model.Usage);
                    }
                }
            }
            else
            {
                if (_chartView != null)
                {
                    _chartView.ToggleRMKWHValues(_rMkWhEnum);
                }
            }
            SetDPCNoteForRMKwHToggle();
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
        public void SetDisconnectionComponent(bool isUpdating)
        {
            AccountStatusDataModel accountStatusData = AccountStatusCache.GetAccountStatusData();
            if (_status != null)
            {
                _status.RemoveFromSuperview();
            }
            DisconnectionComponent disconnectionComponent = new DisconnectionComponent(_scrollViewContent, accountStatusData);
            if (isUpdating)
            {
                _viewStatus.Hidden = true;
                ViewHelper.AdjustFrameSetHeight(_viewStatus, 0);
            }
            else
            {
                if (!AccountStatusCache.AccountStatusIsAvailable())
                {
                    ViewHelper.AdjustFrameSetHeight(_viewStatus, GetScaledHeight(24f));
                    _viewStatus.BackgroundColor = UIColor.Clear;
                    _viewStatus.Hidden = false;
                    _status = disconnectionComponent.GetUI();
                    disconnectionComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        var acctStatusTooltipBtnTitle = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalBtnText) ? accountStatusData.AccountStatusModalBtnText : GetI18NValue(UsageConstants.I18N_GotIt);
                        var acctStatusTooltipMsg = !string.IsNullOrWhiteSpace(accountStatusData.AccountStatusModalMessage) ? accountStatusData.AccountStatusModalMessage : GetI18NValue(UsageConstants.I18N_DisconnectionMessage);
                        DisplayCustomAlert(string.Empty, acctStatusTooltipMsg, acctStatusTooltipBtnTitle, null);
                    }));
                    _viewStatus.AddSubview(_status);
                }
                else
                {
                    _viewStatus.Hidden = true;
                    ViewHelper.AdjustFrameSetHeight(_viewStatus, 0);
                }
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
                _rEAmountComponent = new REAmountComponent(_viewRE)
                {
                    GetI18NValue = GetI18NValue
                };
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
                    DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    _rEAmountComponent.SetValues(dueData.billDueDate, dueData.amountDue);
                }
            }
        }

        internal void UpdateREAmountViewForRefreshState()
        {
            if (_rEAmountComponent != null)
            {
                _rEAmountComponent.SetRefreshState();
            }
        }

        internal void SetREAmountViewForRefresh()
        {
            if (_RERefresh != null && _RE != null)
            {
                _RERefresh.Hidden = false;
                _RERefresh.AddSubview(_RE);
                ViewHelper.AdjustFrameSetY(_RERefresh, GetYLocationFromFrame(_refresh.Frame, GetScaledHeight(8F)));
                ViewHelper.AdjustFrameSetHeight(_RERefresh, _RE.Frame.Height);
                _viewRefresh.AddSubview(_RERefresh);
                ViewHelper.AdjustFrameSetHeight(_viewRefresh, _refresh.Frame.Height + _RE.Frame.Height + GetScaledHeight(16F));
                SetContentViewForRefresh();
            }
        }

        internal void HideREAmountView()
        {
            if (_RERefresh != null)
            {
                _RERefresh.Hidden = true;
                ViewHelper.AdjustFrameSetHeight(_RERefresh, 0);
            }
        }
        #endregion
        #region FOOTER Methods
        internal void SetFooterView(bool isRefreshScreen = false)
        {
            if (!isREAccount)
            {
                if (_viewFooter != null)
                {
                    _viewFooter.RemoveFromSuperview();
                }
                nfloat componentHeight = GetScaledHeight(136);
                nfloat indicatorHeight = !isRefreshScreen || (isRefreshScreen && accountIsSSMR) ? GetScaledHeight(48) : 0;
                nfloat footerHeight = indicatorHeight + componentHeight;
                nfloat footerYPos = _scrollViewContent.Frame.GetMaxY() - footerHeight;
                _footerYPos = footerYPos;
                _viewFooter = new CustomUIView(new CGRect(0, footerYPos, ViewWidth, footerHeight))
                {
                    BackgroundColor = UIColor.Clear
                };
                _origViewFrame = _viewFooter.Frame;
                View.AddSubview(_viewFooter);
                _footerViewComponent = new UsageFooterViewComponent(View, footerHeight, indicatorHeight)
                {
                    GetI18NValue = GetI18NValue
                };
                _viewFooter.AddSubview(_footerViewComponent.GetUI());
                if (_footerViewComponent._btnViewBill != null)
                {
                    _footerViewComponent._btnViewBill.TouchUpInside += (sender, e) =>
                    {
                        OnViewDetailsButtonTap();
                    };
                }
                if (_footerViewComponent._btnPay != null)
                {
                    _footerViewComponent._btnPay.TouchUpInside += (sender, e) =>
                    {
                        DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount?.accNum);
                        OnPayButtonTap(dueData != null ? dueData.amountDue : 0);
                    };
                }
                if (!isRefreshScreen || (isRefreshScreen && accountIsSSMR))
                {
                    if (_scrollIndicatorView != null)
                    {
                        _scrollIndicatorView.RemoveFromSuperview();
                    }
                    _scrollIndicatorView = new UIImageView(new CGRect(0, 0, ViewWidth, GetScaledHeight(48)))
                    {
                        UserInteractionEnabled = true
                    };
                    _scrollIndicatorView.Image = UIImage.FromBundle(UsageConstants.IMG_ScrollIndicator);
                    _scrollIndicatorView.ContentMode = UIViewContentMode.ScaleAspectFill;
                    _scrollIndicatorView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        if (!_footerIsDocked)
                        {
                            AnimateFooterToHideAndShow(true);
                        }
                    }));
                    _viewFooter.AddSubview(_scrollIndicatorView);
                }
                else
                {
                    var view = _footerViewComponent.GetView();
                    AddFooterViewShadow(ref view, true);
                    UpdateFooterUI(false);
                }
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
                    DueAmountDataModel dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    if (dueData != null)
                    {
                        _footerViewComponent.SetAmount(dueData.amountDue);
                        _footerViewComponent.SetDate(dueData.billDueDate);
                    }
                }
            }
        }

        internal void UpdateFooterForRefreshState()
        {
            if (isSmartMeterAccount)
            {
                if (_viewFooter != null)
                {
                    _viewFooter.RemoveFromSuperview();
                }
            }
            else
            {
                if (_footerViewComponent != null)
                {
                    _footerViewComponent.SetRefreshState();
                }
            }
        }

        internal virtual void OnViewDetailsButtonTap() { }

        internal virtual void OnCurrentBillButtonTap() { }

        internal virtual void OnPayButtonTap(double amountDue) { }

        private void OnScroll(object sender, EventArgs e)
        {
            if (_footerIsDocked)
                return;

            UIScrollView scrollView = sender as UIScrollView;
            if (scrollView != null)
            {
                nfloat scrollViewHeight = scrollView.Frame.Size.Height;
                nfloat scrollContentSizeHeight = scrollView.ContentSize.Height;
                nfloat scrollOffset = scrollView.ContentOffset.Y;

                if (scrollOffset <= 0)
                {
                    AnimateFooterToHideAndShow(false);
                }
                else if ((scrollOffset + scrollViewHeight) >= scrollContentSizeHeight)
                {
                    AnimateFooterToHideAndShow(true);
                }
            }
        }

        private void AnimateFooterToHideAndShow(bool isHidden)
        {
            if (_viewFooter != null)
            {
                UIView.Animate(0.3, 0, UIViewAnimationOptions.ShowHideTransitionViews
                , () =>
                {
                    HideShowBottomCards(!isHidden);
                    if (isHidden)
                    {
                        var temp = _origViewFrame;
                        temp.Y = _scrollViewContent.Frame.GetMaxY();
                        _viewFooter.Frame = temp;
                        _viewFooter.Layer.ShadowColor = UIColor.Clear.CGColor;
                    }
                    else
                    {
                        _viewFooter.Frame = _origViewFrame;
                        _viewFooter.Layer.ShadowColor = MyTNBColor.BabyBlue35.CGColor;
                    }
                }
                , () => { }
            );
            }
        }

        private void HideShowBottomCards(bool isHidden)
        {
            if (accountIsSSMR)
            {
                if (_viewSSMR != null)
                {
                    _viewSSMR.Hidden = isHidden;
                    if (_footerBGImage != null)
                    {
                        _footerBGImage.Hidden = isHidden;
                    }
                }
            }
            if (isSmartMeterAccount && !AccountUsageSmartCache.IsMDMSDown)
            {
                if (_viewSmartMeter != null)
                {
                    _viewSmartMeter.Hidden = isHidden;
                    if (_footerBGImage != null)
                    {
                        _footerBGImage.Hidden = isHidden;
                    }
                }
            }
        }

        private void AddFooterViewShadow(ref UIView view, bool isDocked = false)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = isDocked ? MyTNBColor.BabyBlue35.CGColor : UIColor.Clear.CGColor;
            view.Layer.ShadowOpacity = 1f;
            view.Layer.ShadowOffset = new CGSize(0, -8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
        #endregion
        #region Refresh Methods
        internal void SetRefreshScreen()
        {
            SetFooterView(true);
            if (_scrollViewContent != null)
            {
                _scrollViewContent.Hidden = true;
            }
            if (_refreshScrollView != null)
            {
                _refreshScrollView.Hidden = false;
            }
            var bcrm = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
            var bcrmMsg = !string.IsNullOrEmpty(bcrm?.DowntimeMessage) && !string.IsNullOrWhiteSpace(bcrm?.DowntimeMessage) ? bcrm?.DowntimeMessage : GetCommonI18NValue(Constants.Common_BCRMMessage);
            var refreshMsg = isSmartMeterAccount ? AccountUsageSmartCache.GetRefreshDataModel()?.RefreshMessage ?? string.Empty : AccountUsageCache.GetRefreshDataModel()?.RefreshMessage ?? string.Empty;
            var refreshBtnTxt = isSmartMeterAccount ? AccountUsageSmartCache.GetRefreshDataModel()?.RefreshBtnText ?? string.Empty : AccountUsageCache.GetRefreshDataModel()?.RefreshBtnText ?? string.Empty;
            string desc = isBcrmAvailable ? refreshMsg : bcrmMsg;

            if (_refresh != null)
            {
                _refresh.RemoveFromSuperview();
            }
            RefreshScreenComponent refreshScreenComponent = new RefreshScreenComponent(View, GetScaledHeight(32F));
            refreshScreenComponent.SetIsBCRMDown(!isBcrmAvailable);
            refreshScreenComponent.SetRefreshButtonHidden(!isBcrmAvailable);
            refreshScreenComponent.SetButtonText(refreshBtnTxt);
            refreshScreenComponent.SetDescription(desc);
            refreshScreenComponent.CreateComponent();
            refreshScreenComponent.OnButtonTap = RefreshButtonOnTap;
            _refresh = refreshScreenComponent.GetView();
            _viewRefresh.AddSubview(_refresh);
            ViewHelper.AdjustFrameSetHeight(_viewRefresh, _refresh.Frame.Height);
        }
        internal virtual void RefreshButtonOnTap()
        {
            if (!isREAccount && accountIsSSMR)
            {
                _viewSSMR.AddSubview(_ssmr);
            }
            ResetViews();
        }
        internal void SetContentViewForRefresh()
        {
            if (accountIsSSMR)
            {
                _refreshScrollView.ContentSize = new CGSize(ViewWidth, GetAdditionalHeight(_ssmrRefresh.Frame.GetMaxY()) + GetScaledHeight(40F));
                _footerRefreshBGImage.Hidden = false;
                ViewHelper.AdjustFrameSetY(_footerRefreshBGImage, GetYPosForBG(_ssmrRefresh));
            }
            else
            {
                _refreshScrollView.ContentSize = new CGSize(ViewWidth, _viewRefresh.Frame.GetMaxY() + BaseMarginHeight16);
                if (isREAccount)
                {
                    _footerRefreshBGImage.Hidden = false;
                    ViewHelper.AdjustFrameSetY(_footerRefreshBGImage, GetYPosForBG(_RERefresh));
                }
                else
                {
                    _footerRefreshBGImage.Hidden = true;
                }
            }

        }
        #endregion
    }
}
