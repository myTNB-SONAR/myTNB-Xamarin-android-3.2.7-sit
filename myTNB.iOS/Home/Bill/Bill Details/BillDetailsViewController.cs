using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using CoreGraphics;
using Foundation;
using myTNB.Home.Bill;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public partial class BillDetailsViewController : CustomUIViewController
    {
        private UIView _viewDetails, _viewTitleSection, _viewBreakdown, _viewLine
            , _viewPayment, _toolTipParentView, _viewCTAContainer, _refreshViewContainer;
        private CustomUIView _viewMandatory, _viewTooltip;
        private UIScrollView _uiScrollView;
        private CustomUIButtonV2 _btnViewBill, _btnPay;
        private List<BillsTooltipModelEntity> _toolTipList;
        private List<BillsTooltipModel> _pageData;
        private bool isMandatoryExpanded;
        private AccountChargesModel _charges = new AccountChargesModel();
        private GetAccountsChargesResponseModel _accountCharges;

        //Created by Syahmi ICS 05052020
        private List<EppTooltipModelEntity> _eppToolTipList;
        public bool HasEppToolTip = false;

        public bool IsFreshCall;
        public bool IsRoot { set; private get; } = false;
        public string AccountNumber { set; private get; } = string.Empty;
        public CustomerAccountRecordModel SelectedAccount = new CustomerAccountRecordModel();
        public bool IsFromBillSelection { set; private get; }
        public bool IsPayBtnEnabled = true, HasPendingPayment;
        private UIView _tutorialContainer;
        private bool IsLoading = true;
        private Timer tutorialOverlayTimer;
        private bool _isTooltipCreated;

        public BillDetailsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = BillConstants.Pagename_BillDetails;
            NavigationController.SetNavigationBarHidden(false, true);
            base.ViewDidLoad();
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            SetNavigation();
            AddCTAs();
            IsLoading = true;
            if (IsFreshCall)
            {
                OnRefreshCall();
            }
            else
            {
                LoadViews();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void SetNavigation()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(BillConstants.IMG_BackIcon)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                HideTutorialOverlay();
                if (IsRoot)
                {
                    NavigationController.PopViewController(true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            });
            UIBarButtonItem btnInfo = new UIBarButtonItem(UIImage.FromBundle(BillConstants.IMG_Info)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                PrepareToolTipView();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnInfo;
            Title = GetI18NValue(BillConstants.I18N_NavTitle);
        }

        private void OnRefreshCall()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        InvokeInBackground(async () =>
                        {
                            PendingPaymentResponseModel pendingPaymentResponse = await UsageServiceCall.CheckPendingPayments(new List<string> { DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty });
                            HasPendingPayment =
                                            pendingPaymentResponse != null && pendingPaymentResponse.d != null &&
                                            pendingPaymentResponse.d.IsSuccess && pendingPaymentResponse.d.data != null &&
                                            pendingPaymentResponse.d.data.Count > 0 && pendingPaymentResponse.d.data[0].HasPendingPayment;
                            _accountCharges = await GetAccountsCharges();
                            InvokeOnMainThread(() =>
                            {
                                if (_accountCharges != null && _accountCharges.d != null && _accountCharges.d.IsSuccess
                                    && _accountCharges.d.data != null && _accountCharges.d.data.AccountCharges != null
                                    && _accountCharges.d.data.AccountCharges.Count > 0 && _accountCharges.d.data.AccountCharges[0] != null)
                                {
                                    if (_refreshViewContainer != null)
                                    {
                                        _refreshViewContainer.RemoveFromSuperview();
                                        _refreshViewContainer = null;
                                    }
                                    AccountChargesCache.SetData(_accountCharges);
                                    LoadViews();
                                }
                                else
                                {
                                    InitializeTooltip();
                                    AddDetails();
                                    AddSectionTitle();
                                    SetEvents();
                                    if (_btnPay != null && _accountCharges != null && _accountCharges.d != null && !_accountCharges.d.IsPayEnabled)
                                    {
                                        _btnPay.Enabled = false;
                                        _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
                                    }
                                    if (_refreshViewContainer == null)
                                    {
                                        _refreshViewContainer = new UIView(new CGRect(0
                                           , _viewTitleSection.Frame.GetMaxY(), ViewWidth, ScaleUtility.GetScaledHeight(220)));
                                        _uiScrollView.AddSubview(_refreshViewContainer);

                                        bool isPlannedDowntime = _accountCharges != null && _accountCharges.d != null ? _accountCharges.d.IsPlannedDownTime : false;

                                        string message = isPlannedDowntime ? _accountCharges?.d?.DisplayMessage ?? string.Empty : _accountCharges?.d?.RefreshMessage ?? string.Empty;
                                        message = message.IsValid() ? message : isPlannedDowntime ? GetI18NValue(UsageConstants.I18N_BcrmDownMessage) : GetErrorI18NValue(Constants.Refresh_BillDetails);

                                        RefreshComponent refreshComponent = new RefreshComponent(message
                                            , GetCommonI18NValue(Constants.Common_RefreshNow)
                                            , OnRefreshCall)
                                        {
                                            PageName = PageName,
                                            IsPlannedDownTime = isPlannedDowntime
                                        };
                                        _refreshViewContainer.AddSubview(refreshComponent.GetUI(_refreshViewContainer));
                                    }
                                }
                                ActivityIndicator.Hide();
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

        private void LoadViews()
        {
            _charges = AccountChargesCache.GetAccountCharges(AccountNumber);
            HasEppToolTip = _charges.ShowEppToolTip; //Created by Syahmi ICS 05052020
            InitializeTooltip();
            AddDetails();
            AddSectionTitle();
            AddBreakdown();
            SetEvents();
            IsLoading = false;
            if (IsFreshCall)
            {
                if (_btnPay != null && _accountCharges != null && _accountCharges.d != null && !_accountCharges.d.IsPayEnabled)
                {
                    _btnPay.Enabled = false;
                    _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
                }
            }
            else
            {
                if (_btnPay != null && !IsPayBtnEnabled)
                {
                    _btnPay.Enabled = false;
                    _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            CheckTutorialOverlay();
        }

        #region Tutorial Overlay Methods
        private void CheckTutorialOverlay()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(BillConstants.Pref_BillDetailsTutorialOverlay);

            if (tutorialOverlayHasShown)
                return;

            tutorialOverlayTimer = new Timer
            {
                Interval = 500F,
                AutoReset = true,
                Enabled = true
            };
            tutorialOverlayTimer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsLoading)
            {
                tutorialOverlayTimer.Enabled = false;
                InvokeOnMainThread(() =>
                {
                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is BillDetailsViewController)
                        {
                            ShowTutorialOverlay();
                        }
                    }
                });
            }
        }

        private void ShowTutorialOverlay()
        {
            if (_tutorialContainer != null)
                return;

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            currentWindow.AddSubview(_tutorialContainer);

            BillDetailsTutorialOverlay tutorialView = new BillDetailsTutorialOverlay(_tutorialContainer)
            {
                GetI18NValue = GetI18NValue,
                NavigationHeight = DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height,
                OnDismissAction = HideTutorialOverlay,
                ViewCTAContainerYPos = _viewCTAContainer.Frame.Y,
                ButtonHeight = _btnViewBill.Frame.Height
            };
            _tutorialContainer.AddSubview(tutorialView.GetView());
        }

        private void HideTutorialOverlay()
        {
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);

                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, BillConstants.Pref_BillDetailsTutorialOverlay);
            }
        }
        #endregion

        #region Tooltip
        private void SetTooltipFallbackData()
        {
            _pageData = new List<BillsTooltipModel>();
            var item1 = new BillsTooltipModel
            {
                Image = BillConstants.IMG_BGToolTip1,
                Title = GetI18NValue(BillConstants.I18N_ToolTipTitle1),
                Description = GetDescriptionList(GetI18NValue(BillConstants.I18N_ToolTipDesc1))
            };
            _pageData.Add(item1);

            var item2 = new BillsTooltipModel
            {
                Image = BillConstants.IMG_BGToolTip2,
                Title = GetI18NValue(BillConstants.I18N_ToolTipTitle2),
                Description = GetDescriptionList(GetI18NValue(BillConstants.I18N_ToolTipDesc2))
            };
            _pageData.Add(item2);
        }

        private List<string> GetDescriptionList(string description)
        {
            List<string> descList = new List<string>();
            var descSplit = description.Split('|');

            if (descSplit.Length > 0)
            {
                foreach (string str in descSplit)
                {
                    descList.Add(str);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
                {
                    descList.Add(description);
                }
            }
            return descList;
        }


        private void InitializeTooltip()
        {
            if (_isTooltipCreated) { return; }
            _isTooltipCreated = true;
            SetTooltipFallbackData();
            BillDetailsTooltipEntity wsManager = new BillDetailsTooltipEntity();
            _toolTipList = wsManager.GetAllItems();
            if (_toolTipList != null && _toolTipList.Count > 0)
            {
                if (_pageData != null)
                {
                    _pageData.Clear();
                }
                for (int i = 0; i < _toolTipList.Count; i++)
                {
                    BillsTooltipModel item = new BillsTooltipModel
                    {
                        Title = _toolTipList[i].Title,
                        Description = GetDescriptionList(_toolTipList[i].Description),
                        Image = _toolTipList[i].Image,
                        IsSitecoreData = true,
                        NSDataImage = _toolTipList[i].ImageByteArray.ToNSData()
                    };
                    _pageData.Add(item);
                }
            }
        }

        private void PrepareToolTipView()
        {
            if (_toolTipParentView == null)
            {
                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                nfloat height = currentWindow.Frame.Height;
                _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
                {
                    BackgroundColor = MyTNBColor.Black60
                };
                currentWindow.AddSubview(_toolTipParentView);
                PaginatedTooltipComponent tooltipComponent = new PaginatedTooltipComponent(_toolTipParentView);
                tooltipComponent.SetBillsToolTipData(_pageData);
                _toolTipParentView.AddSubview(tooltipComponent.GetBillDetailsTooltip());
                tooltipComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    MakeToolTipVisible(false);
                }));
            }
            _toolTipParentView.Hidden = false;
        }

        private void MakeToolTipVisible(bool isVisible)
        {
            if (_toolTipParentView != null)
            {
                _toolTipParentView.Hidden = !isVisible;
            }
        }
        #endregion

        private void AddDetails()
        {
            if (_uiScrollView != null) { return; }
            _uiScrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - _viewCTAContainer.Frame.Height));
            View.AddSubview(_uiScrollView);
            _viewDetails = new UIView { BackgroundColor = UIColor.White };

            UILabel lblAccountName = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = AccountManager.GetNickname(AccountNumber)
            };
            UILabel lblAddress = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblAccountName.Frame, 8)
                , BaseMarginedWidth, GetScaledHeight(32)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_300,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = AccountManager.GetAddress(AccountNumber)
            };
            nfloat height = lblAddress.GetLabelHeight(GetScaledHeight(100));
            lblAddress.Frame = new CGRect(lblAddress.Frame.Location, new CGSize(lblAddress.Frame.Width, height));
            _viewDetails.AddSubviews(new UIView[] { lblAccountName, lblAddress });
            _viewDetails.Frame = new CGRect(0, 0, ViewWidth, lblAddress.Frame.GetMaxY() + GetScaledHeight(16));
            _uiScrollView.AddSubview(_viewDetails);
        }

        private void AddSectionTitle()
        {
            if (_viewTitleSection != null) { return; }
            _viewTitleSection = new UIView(new CGRect(0, _viewDetails.Frame.GetMaxY(), ViewWidth, GetScaledHeight(48)));
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(BillConstants.I18N_BillDetails)
            };
            _viewTitleSection.AddSubview(lblSectionTitle);
            _uiScrollView.AddSubview(_viewTitleSection);
        }

        private void AddBreakdown()
        {
            _viewBreakdown = new UIView { BackgroundColor = UIColor.White };
            bool isOutstandingOverpaid = _charges.OutstandingCharges < 0;
            string outstandingTilte = isOutstandingOverpaid ? GetI18NValue(BillConstants.I18N_PaidExtra)
                : GetI18NValue(BillConstants.I18N_OutstandingCharges);
            UIView viewOutstanding = GetCommonLabelView(GetScaledHeight(16), outstandingTilte
                , Math.Abs(_charges.OutstandingCharges).ToString("N2", CultureInfo.InvariantCulture), isOutstandingOverpaid);
            UIView viewMonthBill = GetCommonLabelView(GetYLocationFromFrame(viewOutstanding.Frame, 16), GetI18NValue(BillConstants.I18N_BillThisMonth)
                , _charges.CurrentCharges.ToString("N2", CultureInfo.InvariantCulture));

            _viewMandatory = GetMandatoryView(GetYLocationFromFrame(viewMonthBill.Frame, 16));

            //True use _viewMandatory.Frame, False use viewMonthBill.Frame, 16
            nfloat lineYLoc = GetYLocationFromFrame(HasMandatory ? _viewMandatory.Frame : viewMonthBill.Frame, 16);
            _viewLine = new UIView(new CGRect(BaseMargin, lineYLoc, BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            _viewPayment = GetPaymentDetails(GetYLocationFromFrame(_viewLine.Frame, 20));
            _viewTooltip = GetTooltipView(GetYLocationFromFrame(_viewPayment.Frame, 16));

            _viewBreakdown.AddSubviews(new UIView[] { viewOutstanding, viewMonthBill, _viewLine, _viewPayment });

            nfloat breakDownViewHeight = _viewPayment.Frame.GetMaxY();
            if (HasMandatory || HasEppToolTip)
            {
                _viewBreakdown.AddSubviews(new UIView[] { _viewMandatory, _viewTooltip });
                breakDownViewHeight = _viewTooltip.Frame.GetMaxY();
            }

            _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                , breakDownViewHeight + GetScaledHeight(16)));

            _uiScrollView.AddSubview(_viewBreakdown);
            _uiScrollView.ContentSize = new CGSize(ViewWidth, _viewBreakdown.Frame.GetMaxY());
        }

        private UIView GetCommonLabelView(nfloat yLocation, string itemString, string valueString, bool isOverpaid = false)
        {
            UIView view = new UIView(new CGRect(0, yLocation, ViewWidth, GetScaledHeight(20)));
            UILabel item = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = itemString
            };
            nfloat itemWidth = item.GetLabelWidth(ViewWidth);
            item.Frame = new CGRect(item.Frame.Location, new CGSize(itemWidth, item.Frame.Height));
            UILabel value = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = isOverpaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY, valueString)
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, value });
            return view;
        }

        private UIView GetPaymentDetails(nfloat yLoc)
        {
            bool isOverPaid = _charges.AmountDue < 0;
            UIView viewAmount = new UIView();
            UILabel lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(11), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = isOverPaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            nfloat currencyWidth = lblCurrency.GetLabelWidth(ViewWidth);
            lblCurrency.Frame = new CGRect(lblCurrency.Frame.Location, new CGSize(currencyWidth, lblCurrency.Frame.Height));

            UILabel lblAmount = new UILabel(new CGRect(lblCurrency.Frame.GetMaxX() + GetScaledWidth(6)
                , 0, GetScaledWidth(100), GetScaledHeight(32)))
            {
                TextColor = isOverPaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_24_300,
                TextAlignment = UITextAlignment.Left,
                Text = Math.Abs(_charges.AmountDue).ToString("N2", CultureInfo.InvariantCulture)
            };

            nfloat amountWidth = lblAmount.GetLabelWidth(ViewWidth);
            lblAmount.Frame = new CGRect(lblAmount.Frame.Location, new CGSize(amountWidth, lblAmount.Frame.Height));
            viewAmount.AddSubviews(new UIView[] { lblCurrency, lblAmount });

            viewAmount.Frame = new CGRect(ViewWidth - (BaseMargin + currencyWidth + GetScaledWidth(6) + amountWidth)
                , 0, currencyWidth + amountWidth + GetScaledWidth(6), GetScaledHeight(32));

            nfloat maxWidth = BaseMarginedWidth - viewAmount.Frame.Width - GetScaledWidth(4);

            UIView viewPayment = new UIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(40)));
            nfloat statusYLoc = _charges.AmountDue <= 0 ? GetScaledHeight(10) : GetScaledHeight(-4);
            string statusString;
            if (_charges.AmountDue == 0)
            {
                statusString = BillConstants.I18N_ClearedBills;
            }
            else
            {
                statusString = _charges.AmountDue < 0 ? BillConstants.I18N_PaidExtra : BillConstants.I18N_NeedToPay;
            }
            UILabel lblStatus = new UILabel(new CGRect(BaseMargin, statusYLoc, maxWidth, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(statusString),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            if (HasPendingPayment)
            {
                lblCurrency.TextColor = MyTNBColor.LightOrange;
                lblAmount.TextColor = MyTNBColor.LightOrange;
                UIStringAttributes stringAttributes = new UIStringAttributes
                {
                    Font = TNBFont.MuseoSans_14_500,
                    ForegroundColor = MyTNBColor.GreyishBrown,
                    ParagraphStyle = new NSMutableParagraphStyle() { LineSpacing = 3.0f, Alignment = UITextAlignment.Left }
                };
                var text = GetCommonI18NValue(Constants.Common_PaymentPendingMsg);
                var AttributedText = new NSMutableAttributedString(text);
                AttributedText.AddAttributes(stringAttributes, new NSRange(0, text.Length));
                lblStatus.AttributedText = AttributedText;

                nfloat width = viewPayment.Frame.Width / 2 - BaseMarginWidth16;
                nfloat height = GetScaledHeight(40F);
                nfloat xPos = BaseMarginWidth16;
                nfloat yPos = GetScaledHeight(-4);
                lblStatus.Frame = new CGRect(xPos, yPos, width, height);
            }
            else
            {
                nfloat statusHeight = lblStatus.GetLabelHeight(1000);
                lblStatus.Frame = new CGRect(lblStatus.Frame.Location, new CGSize(lblStatus.Frame.Width, statusHeight));
            }

            string result = string.Empty;
            if (_charges.DueDate != null)
            {
                try
                {
                    result = DateTime.ParseExact(_charges.DueDate
                   , BillConstants.Format_DateParse, CultureInfo.InvariantCulture).ToString(BillConstants.Format_Date, DateHelper.DateCultureInfo);
                }
                catch (FormatException)
                {
                    Debug.WriteLine("Unable to parse '{0}'", _charges.DueDate);
                }
            }
            UILabel lblDue = new UILabel(new CGRect(BaseMargin, lblStatus.Frame.GetMaxY(), maxWidth, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = string.Format(BillConstants.Format_Default, GetI18NValue(BillConstants.I18N_By), result),
                Hidden = _charges.AmountDue <= 0 || HasPendingPayment,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            nfloat dueHeight = lblDue.GetLabelHeight(1000);
            lblDue.Frame = new CGRect(lblDue.Frame.Location, new CGSize(lblDue.Frame.Width, dueHeight));

            viewPayment.AddSubviews(new UIView[] { lblStatus, lblDue, viewAmount });
            nfloat viewPaymentHeigt = (lblDue.Hidden ? lblStatus.Frame.GetMaxY() : lblDue.Frame.GetMaxY());
            viewPayment.Frame = new CGRect(viewPayment.Frame.Location, new CGSize(viewPayment.Frame.Width, viewPaymentHeigt));

            viewAmount.Frame = new CGRect(new CGPoint(viewAmount.Frame.X
                , (viewPayment.Frame.Height - viewAmount.Frame.Height) / 2), viewAmount.Frame.Size);
            return viewPayment;
        }

        private CustomUIView GetMandatoryView(nfloat yLoc)
        {
            if (!HasMandatory)
            {
                return new CustomUIView();
            }
            isMandatoryExpanded = false;
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(20)))
            {
                ClipsToBounds = true,
                PageName = PageName,
                EventName = BillConstants.Event_MandatoryDetails
            };
            UILabel item = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(BillConstants.I18N_ApplicationCharges)
            };
            nfloat itemWidth = item.GetLabelWidth(ViewWidth);
            item.Frame = new CGRect(item.Frame.Location, new CGSize(itemWidth, item.Frame.Height));

            UIImageView imgIndicator = new UIImageView(new CGRect(item.Frame.GetMaxX() + GetScaledWidth(4)
                , 0, GetScaledWidth(20), GetScaledWidth(20)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_ArrowDown),
                Tag = 99
            };

            UILabel value = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY
                , _charges.MandatoryCharges.TotalAmount.ToString("N2", CultureInfo.InvariantCulture))
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, imgIndicator, value });

            UIView mandatoryView = new UIView(new CGRect(GetScaledWidth(24), GetYLocationFromFrame(item.Frame, 4)
                , ViewWidth - GetScaledWidth(40), 0))
            { Tag = 98 };
            nfloat subYLoc = 0;
            int itemCount = 0;
            if (_charges != null && _charges.MandatoryCharges != null && _charges.MandatoryCharges.Charges != null)
            {
                for (int i = 0; i < _charges.MandatoryCharges.Charges.Count; i++)
                {
                    ChargesModel chargeItem = _charges.MandatoryCharges.Charges[i];
                    if (chargeItem.Amount > 0)
                    {
                        UILabel subItem = new UILabel(new CGRect(0, subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                        {
                            TextAlignment = UITextAlignment.Left,
                            Font = TNBFont.MuseoSans_14_300,
                            TextColor = MyTNBColor.GreyishBrown,
                            Text = chargeItem.Title
                        };
                        nfloat subItemWidth = subItem.GetLabelWidth(ViewWidth);
                        subItem.Frame = new CGRect(subItem.Frame.Location, new CGSize(subItemWidth, subItem.Frame.Height));

                        UILabel subValue = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2)
                            , subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                        {
                            TextAlignment = UITextAlignment.Right,
                            Font = TNBFont.MuseoSans_14_300,
                            TextColor = MyTNBColor.GreyishBrown,
                            Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY, chargeItem.Amount.ToString("N2", CultureInfo.InvariantCulture))
                        };
                        nfloat subValueWidth = subValue.GetLabelWidth(ViewWidth);
                        subValue.Frame = new CGRect(new CGPoint(mandatoryView.Frame.Width - subValueWidth, subValue.Frame.Y)
                            , new CGSize(subValueWidth, subValue.Frame.Height));
                        mandatoryView.AddSubviews(new UIView[] { subItem, subValue });
                        itemCount++;
                        subYLoc += GetScaledHeight(20);
                    }
                }
            }

            mandatoryView.Frame = new CGRect(mandatoryView.Frame.Location, new CGSize(mandatoryView.Frame.Width, itemCount * GetScaledHeight(20)));

            view.AddSubview(mandatoryView);
            CGRect newFrame = view.Frame;
            newFrame.Height = mandatoryView.Frame.GetMaxY();
            return view;
        }

        //Code Start Here
        private CustomUIView GetTooltipView(nfloat yLoc)
        {
            if (!HasMandatory && !HasEppToolTip)
            {
                return new CustomUIView();
            }
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
                PageName = PageName,
                EventName = BillConstants.Event_MandatoryChargesPopup
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            //Created by Syahmi ICS 05052020
            if (HasEppToolTip)
            {
                UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                    , GetScaledHeight(4), view.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = TNBFont.MuseoSans_11_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetCommonI18NValue("eppToolTipTitle")

                };
                viewInfo.Layer.CornerRadius = GetScaledHeight(12);
                viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
                view.AddSubview(viewInfo);
                view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    EppInfoTooltipEntity wsEppManager = new EppInfoTooltipEntity();
                    _eppToolTipList = wsEppManager.GetAllItems();

                    DisplayCustomAlert(
                    _eppToolTipList[0].PopUpTitle,
                    _eppToolTipList[0].PopUpBody,
                    new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null }, { GetCommonI18NValue("viewBill"), () => ViewBill() } },
                    UIImage.LoadFromData(NSData.FromArray(_eppToolTipList[0].ImageByteArray))
                    );
                }));
            }
            else if (HasEppToolTip.Equals(false) && HasMandatory)
            {
                UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                    , GetScaledHeight(4), view.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(BillConstants.I18N_MinimumChargeDescription)

                };
                viewInfo.Layer.CornerRadius = GetScaledHeight(12);
                viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
                view.AddSubview(viewInfo);
                view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    PopupModel popupData = AccountChargesCache.GetPopupDetailsByType(BillConstants.Popup_MandatoryChargesKey);
                    if (popupData != null)
                    {
                        DisplayCustomAlert(popupData.Title, popupData.Description
                            , new Dictionary<string, Action> { { popupData.CTA, null } }
                            , false);
                    }
                }));
            }
            return view;
        }

        private void AddCTAs()
        {
            nfloat containerHeight = GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;
            nfloat yLoc = View.Frame.Height - DeviceHelper.TopSafeAreaInset - NavigationController.NavigationBar.Frame.Height - containerHeight;
            if (DeviceHelper.IsIOS10AndBelow)
            {
                yLoc = ViewHeight - containerHeight;
            }
            _viewCTAContainer = new UIView(new CGRect(0, yLoc, ViewWidth, containerHeight)) { BackgroundColor = UIColor.White };

            nfloat btnWidth = (BaseMarginedWidth - GetScaledWidth(4)) / 2;
            _btnViewBill = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, GetScaledHeight(16), btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnViewBill.SetTitle(GetCommonI18NValue(BillConstants.I18N_ViewBill), UIControlState.Normal);
            _btnViewBill.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnViewBill.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnPay = new CustomUIButtonV2
            {
                Frame = new CGRect(_btnViewBill.Frame.GetMaxX() + GetScaledWidth(4), GetScaledHeight(16), btnWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnPay.SetTitle(GetI18NValue(BillConstants.I18N_Pay), UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);

            if (IsFreshCall)
            {
                if (_btnPay != null && _accountCharges != null && _accountCharges.d != null && !_accountCharges.d.IsPayEnabled)
                {
                    _btnPay.Enabled = false;
                    _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
                }
            }
            else
            {
                if (_btnPay != null && !IsPayBtnEnabled)
                {
                    _btnPay.Enabled = false;
                    _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
                }
            }

            _viewCTAContainer.AddSubviews(new UIView[] { _btnViewBill, _btnPay });

            View.AddSubview(_viewCTAContainer);
        }

        private void SetEvents()
        {
            if (_viewMandatory != null)
            {
                _viewMandatory.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    nfloat mandatoryViewHeight = GetScaledHeight(20);
                    if (!isMandatoryExpanded)
                    {
                        UIView view = _viewMandatory.ViewWithTag(98) as UIView;
                        if (view != null)
                        {
                            mandatoryViewHeight = view.Frame.GetMaxY();
                        }
                    }
                    isMandatoryExpanded = !isMandatoryExpanded;
                    UIImageView imgView = _viewMandatory.ViewWithTag(99) as UIImageView;
                    if (imgView != null)
                    {
                        imgView.Image = UIImage.FromBundle(isMandatoryExpanded ? BillConstants.IMG_ArrowUp : BillConstants.IMG_ArrowDown);
                    }
                    UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn
                        , () =>
                        {
                            _viewMandatory.Frame = new CGRect(_viewMandatory.Frame.Location, new CGSize(_viewMandatory.Frame.Width
                                , mandatoryViewHeight));
                            _viewLine.Frame = new CGRect(new CGPoint(_viewLine.Frame.X
                                , GetYLocationFromFrame(_viewMandatory.Frame, 16)), _viewLine.Frame.Size);
                            _viewPayment.Frame = new CGRect(new CGPoint(_viewPayment.Frame.X
                                , GetYLocationFromFrame(_viewLine.Frame, 20)), _viewPayment.Frame.Size);
                            _viewTooltip.Frame = new CGRect(new CGPoint(_viewTooltip.Frame.X
                                , GetYLocationFromFrame(_viewPayment.Frame, 16)), _viewTooltip.Frame.Size);
                            _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                                , _viewTooltip.Frame.GetMaxY() + GetScaledHeight(16)));
                        }
                        , () =>
                        {
                            _uiScrollView.ContentSize = new CGSize(ViewWidth, _viewBreakdown.Frame.GetMaxY());
                        }
                    );
                }));
            }

            //Created by Syahmi ICS 05052020
            _btnViewBill.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewBill();
            }));

            _btnPay.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (IsFromBillSelection)
                {
                    NavigationController.PopViewController(true);
                    return;
                }
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Debug.WriteLine("_btnPay");
                            UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                            SelectBillsViewController selectBillsVC =
                                storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                            if (selectBillsVC != null)
                            {
                                selectBillsVC.IsFromBillDetails = true;
                                UINavigationController navController = new UINavigationController(selectBillsVC);
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                PresentViewController(navController, true, null);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                    });
                });
            }));
        }

        //Created by Syahmi ICS 05052020
        private void ViewBill()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        Debug.WriteLine("_btnViewBill");
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                        ViewBillViewController viewController =
                            storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                        if (viewController != null)
                        {
                            viewController.IsFromUsage = true;
                            viewController.IsFromBillSelection = IsFromBillSelection;
                            viewController.SelectedAccount = SelectedAccount;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
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

        private bool HasMandatory
        {
            get
            {
                return AccountChargesCache.HasMandatory(AccountNumber);
            }
        }

        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges()
        {
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
    }
}
