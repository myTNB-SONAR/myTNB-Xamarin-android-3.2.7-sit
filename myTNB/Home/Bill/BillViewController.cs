using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Home.Bill;
using System.Drawing;
using CoreGraphics;
using CoreAnimation;
using System.Globalization;
using myTNB.DataManager;
using Foundation;

namespace myTNB
{
    public partial class BillViewController : UIViewController
    {
        const string CURRENCY = "RM";

        UILabel _lblAmount;
        UIView _viewAmount;

        UIView _headerView;
        UIView _historySelectionView;
        UIView _currentBillDetailsView;
        UIView _currentBillHeaderView;
        UIView _historyHeaderView;
        UIButton _btnPayment;
        UIButton _btnBills;
        UIButton _btnPay;

        UILabel _lblAccountName;
        UILabel _lblAccountNumber;
        UITextView _txtViewAddress;
        UILabel _lblCurrentChargesValue;
        UILabel _lblOutstandingChargesValue;
        UILabel _lblTotalPayableValue;
        UILabel _lblHistoryHeader;

        UILabel _lblCurrentBillHeader;
        UILabel _lblTotalDueAmountTitle;
        UIView _viewCharges;

        UIImageView _imgLeaf;

        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        PaymentHistoryResponseModel _paymentHistory = new PaymentHistoryResponseModel();
        BillHistoryResponseModel _billingHistory = new BillHistoryResponseModel();
        AccountSelectionComponent _accountSelectionComponent;
        TitleBarComponent titleBarComponent;

        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();
        public bool IsFromNavigation = false;
        bool _paymentNeedsUpdate = false;

        bool isREAccount = false;
        bool isOwnedAccount = false;

        public BillViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
            }
            SetSubviews();
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
        }

        void HandleAppWillEnterForeground(NSNotification notification)
        {
            Console.WriteLine("HandleAppWillEnterForeground");
            DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;
            ViewWillAppear(true);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            InitializedSubviews();
            titleBarComponent.SetBackVisibility(!IsFromNavigation);
            DataManager.DataManager.SharedInstance.selectedTag = 0;
            if (_lblAmount != null)
            {
                _lblAmount.Text = "0.00";
            }

            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.accountCategoryId != null
                                     ? DataManager.DataManager.SharedInstance.SelectedAccount.accountCategoryId.Equals("2")
                                     : false;
            isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount;

            SetDetailsView();
            if (!ServiceCall.HasAccountList())
            {
                _billingHistory = new BillHistoryResponseModel();
                _paymentHistory = new PaymentHistoryResponseModel();
                InitializeBillTableView();
                SetupContent();
                AdjustFrames();
                ToggleButtons();
            }
            else
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            if (ServiceCall.HasAccountList() && ServiceCall.HasSelectedAccount())
                            {
                                _paymentNeedsUpdate = true;
                                if (DataManager.DataManager.SharedInstance.IsBillUpdateNeeded)
                                {
                                    ExecuteGetBillAccountDetailsCall();
                                }
                                else
                                {
                                    DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;
                                    ExecuteGetBillHistoryCall();
                                }
                            }
                            else
                            {
                                InitializeBillTableView();
                                SetupContent();
                                AdjustFrames();
                                ToggleButtons();
                                SetButtonPayEnable();
                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            _billingHistory = new BillHistoryResponseModel();
                            _paymentHistory = new PaymentHistoryResponseModel();
                            InitializeBillTableView();
                            SetupContent();
                            AdjustFrames();
                            ToggleButtons();
                        }
                    });
                });
            }
            SetEvents();
        }

        void SetDetailsView()
        {
            if (_lblCurrentBillHeader != null)
            {
                _lblCurrentBillHeader.Text = isREAccount ? "Current Payment Advice" : "Current Bill";
            }
            if (_lblTotalDueAmountTitle != null)
            {
                _lblTotalDueAmountTitle.Text = isREAccount ? "Payment Advice Amount" : "Total Amount Due";
            }

            if (isREAccount)
            {
                _viewCharges.RemoveFromSuperview();
                _lblTotalDueAmountTitle.Frame = new CGRect(18, 20, 160, 18);
                _viewAmount.Frame = new CGRect(View.Frame.Width - 120, 14, 0, 24);
                _headerView.Frame = new RectangleF(0, 0, (float)View.Frame.Width, 393);
                _btnPay.Frame = new CGRect(18, 54, View.Frame.Width - 36, 48);
                _historyHeaderView.Frame = new RectangleF(0, 297, (float)View.Frame.Width, 48);

            }
            else
            {
                _currentBillDetailsView.AddSubview(_viewCharges);
                _lblTotalDueAmountTitle.Frame = new CGRect(18, 136, 160, 18);
                _viewAmount.Frame = new CGRect(View.Frame.Width - 120, 130, 0, 24);
                _headerView.Frame = new RectangleF(0, 0, (float)View.Frame.Width, 509);
                _btnPay.Frame = new CGRect(18, 170, View.Frame.Width - 36, 48);
                _historyHeaderView.Frame = new RectangleF(0, 413, (float)View.Frame.Width, 48);
            }
        }

        void SetButtonPayEnable()
        {
            bool isEnabled = false;
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                isEnabled = true;//DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal > 0;
            }
            if (!ServiceCall.HasAccountList())
            {
                isEnabled = false;
            }
            _btnPay.Enabled = isEnabled;
            _btnPay.BackgroundColor = isEnabled ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 89, true);
            UIView headerView = gradientViewComponent.GetUI();
            titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Bills");
            titleBarComponent.SetNotificationVisibility(true);

            titleBarComponent.SetBackVisibility(!IsFromNavigation);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                NotificationDetailsViewController viewController =
                    storyBoard.InstantiateViewController("NotificationDetailsViewController") as NotificationDetailsViewController;
                viewController.NotificationInfo = NotificationInfo;
                NavigationController.PushViewController(viewController, true);
            }));

            headerView.AddSubview(titleBarView);

            _accountSelectionComponent = new AccountSelectionComponent(headerView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            headerView.AddSubview(accountSelectionView);

            View.AddSubview(headerView);
        }

        void ExecuteGetBillHistoryCall()
        {
            GetBillHistory().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    InitializeBillTableView();
                    SetupContent();
                    AdjustFrames();
                    ToggleButtons();
                    SetButtonPayEnable();
                    ActivityIndicator.Hide();
                });
            });
        }

        Task GetBillHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email
                };
                _billingHistory = serviceManager.GetBillHistory("GetBillHistory", requestParameter);
            });
        }

        Task GetPaymentHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,//220706336302
                    isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email
                };
                _paymentHistory = serviceManager.GetPaymentHistory("GetPaymentHistory", requestParameter);
            });
        }

        void SetupContent()
        {
            _accountSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.SelectedAccount.accDesc);
            if (IsFromNavigation)
            {
                _accountSelectionComponent.SetDropdownVisibility(IsFromNavigation);
            }
            else
            {
                _accountSelectionComponent.SetDropdownVisibility(false);//ServiceCall.GetAccountListCount() > 1 ? IsFromNavigation : true);
            }
            _accountSelectionComponent.SetLeafVisibility(!isREAccount);

            _lblAccountName.Text = DataManager.DataManager.SharedInstance.AccountRecordsList
                .d[DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex].ownerName;
            _lblAccountNumber.Text = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
            _txtViewAddress.Text = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.SelectedAccount.accountStAddress : string.Empty;

            _lblCurrentChargesValue.Text = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.BillingAccountDetails.amCurrentChg.ToString("N2", CultureInfo.InvariantCulture) : "0.00";
            _lblOutstandingChargesValue.Text = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.BillingAccountDetails.amOutstandingChg.ToString("N2", CultureInfo.InvariantCulture) : "0.00";
            _lblTotalPayableValue.Text = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.BillingAccountDetails.amPayableChg.ToString("N2", CultureInfo.InvariantCulture) : "0.00";
            _lblAmount.Text = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal.ToString("N2", CultureInfo.InvariantCulture) : "0.00";

            _imgLeaf.Hidden = !isREAccount;
            _lblHistoryHeader.Text = isREAccount ? "Payment Advice History" : "Bill / Payment History";
            if (isREAccount)
            {
                _headerView.Frame = new CGRect(0, 0, View.Frame.Width, 509 - 116 - 48 - 48);
                _currentBillDetailsView.Frame = new RectangleF(0, 170, (float)View.Frame.Width, 218 - 116 - 48);
                _historyHeaderView.Frame = new RectangleF(0, 413 - 116 - 48, (float)View.Frame.Width, 48);
                if (_historySelectionView != null)
                {
                    _historySelectionView.RemoveFromSuperview();
                }
                if (_btnPay != null)
                {
                    _btnPay.RemoveFromSuperview();
                }
            }
            else
            {
                _headerView.Frame = new CGRect(0, 0, View.Frame.Width, 509);
                _currentBillDetailsView.Frame = new RectangleF(0, 170, (float)View.Frame.Width, 218);
                _historyHeaderView.Frame = new RectangleF(0, 413, (float)View.Frame.Width, 48);
                if (_historySelectionView != null)
                {
                    _historySelectionView.RemoveFromSuperview();
                }
                if (_btnPay != null)
                {
                    _btnPay.RemoveFromSuperview();
                }
                _headerView.AddSubview(_historySelectionView);
                _currentBillDetailsView.AddSubview(_btnPay);
            }

            //if (!isOwnedAccount)
            //{
            //    ShowNonOwnerView();
            //}
        }

        void InitializeBillTableView()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    billTableView.Source = new BillTableViewDataSource(_billingHistory
                                                                       , _paymentHistory
                                                                       , this
                                                                       , NetworkUtility.isReachable
                                                                       , isREAccount
                                                                       , isOwnedAccount);
                    billTableView.ReloadData();
                });
            });
        }

        void InitializedSubviews()
        {
            //BillTableView Header
            _headerView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 509));
            _headerView.BackgroundColor = UIColor.White;
            billTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            billTableView.TableHeaderView = _headerView;

            //Acount Details
            var accountDetailsView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 122));
            accountDetailsView.BackgroundColor = UIColor.White;
            _headerView.AddSubview(accountDetailsView);

            _lblAccountName = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 18));
            _lblAccountName.Font = myTNBFont.MuseoSans14();
            _lblAccountName.TextAlignment = UITextAlignment.Left;
            _lblAccountName.TextColor = myTNBColor.TunaGrey();
            _lblAccountName.BackgroundColor = UIColor.Clear;
            accountDetailsView.AddSubview(_lblAccountName);

            _lblAccountNumber = new UILabel(new CGRect(18, 34, View.Frame.Width - 36, 16));
            _lblAccountNumber.Font = myTNBFont.MuseoSans12_300();
            _lblAccountNumber.TextAlignment = UITextAlignment.Left;
            _lblAccountNumber.TextColor = myTNBColor.TunaGrey();
            _lblAccountNumber.BackgroundColor = UIColor.Clear;
            accountDetailsView.AddSubview(_lblAccountNumber);

            _txtViewAddress = new UITextView(new CGRect(18, 66, View.Frame.Width - 36, 32));
            _txtViewAddress.Font = myTNBFont.MuseoSans12_300();
            _txtViewAddress.TextAlignment = UITextAlignment.Left;
            _txtViewAddress.TextColor = myTNBColor.TunaGrey();
            _txtViewAddress.BackgroundColor = UIColor.Clear;
            _txtViewAddress.TextContainerInset = new UIEdgeInsets(0, -4, 0, 0);
            _txtViewAddress.UserInteractionEnabled = false;
            _txtViewAddress.Editable = false;
            _txtViewAddress.ScrollEnabled = false;
            accountDetailsView.AddSubview(_txtViewAddress);

            //RE Account Leaf
            _imgLeaf = new UIImageView(new CGRect(View.Frame.Width - 42, 16, 24, 24));
            _imgLeaf.Image = UIImage.FromBundle("IC-RE-Leaf-Green");
            _imgLeaf.Hidden = true;
            accountDetailsView.AddSubview(_imgLeaf);

            //Current Bill Header
            _currentBillHeaderView = new UIView(new RectangleF(0, 122, (float)View.Frame.Width, 48));
            _currentBillHeaderView.BackgroundColor = myTNBColor.SectionGrey();
            _headerView.AddSubview(_currentBillHeaderView);

            _lblCurrentBillHeader = new UILabel(new CGRect(18, 24, View.Frame.Width - 36, 18));
            _lblCurrentBillHeader.Font = myTNBFont.MuseoSans16();
            _lblCurrentBillHeader.TextAlignment = UITextAlignment.Left;
            _lblCurrentBillHeader.Text = "Current Bill";
            _lblCurrentBillHeader.TextColor = myTNBColor.PowerBlue();
            _lblCurrentBillHeader.BackgroundColor = UIColor.Clear;
            _currentBillHeaderView.AddSubview(_lblCurrentBillHeader);

            //Current Bill Details
            _currentBillDetailsView = new UIView(new RectangleF(0, 170, (float)View.Frame.Width, 218));
            _currentBillDetailsView.BackgroundColor = UIColor.White;
            _headerView.AddSubview(_currentBillDetailsView);

            _viewCharges = new UIView(new CGRect(0, 0, View.Frame.Width, 113));
            _viewCharges.BackgroundColor = UIColor.White;
            //_currentBillDetailsView.AddSubview(_viewCharges);

            #region _viewCharges
            var lblCurrentChargesTitle = new UILabel(new CGRect(18, 16, 119, 16));
            lblCurrentChargesTitle.Font = myTNBFont.MuseoSans12();
            lblCurrentChargesTitle.TextAlignment = UITextAlignment.Left;
            lblCurrentChargesTitle.Text = "Current Charges";
            lblCurrentChargesTitle.TextColor = myTNBColor.TunaGrey();
            lblCurrentChargesTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(lblCurrentChargesTitle);

            _lblCurrentChargesValue = new UILabel(new CGRect(137, 16, View.Frame.Width - 155, 16));
            _lblCurrentChargesValue.Font = myTNBFont.MuseoSans12();
            _lblCurrentChargesValue.TextAlignment = UITextAlignment.Right;
            _lblCurrentChargesValue.TextColor = myTNBColor.TunaGrey();
            _lblCurrentChargesValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblCurrentChargesValue);

            var lblOutstandingChargesTitle = new UILabel(new CGRect(18, 48, 119, 16));
            lblOutstandingChargesTitle.Font = myTNBFont.MuseoSans12();
            lblOutstandingChargesTitle.TextAlignment = UITextAlignment.Left;
            lblOutstandingChargesTitle.Text = "Oustanding Charges";
            lblOutstandingChargesTitle.TextColor = myTNBColor.TunaGrey();
            lblOutstandingChargesTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(lblOutstandingChargesTitle);

            _lblOutstandingChargesValue = new UILabel(new CGRect(137, 48, View.Frame.Width - 155, 16));
            _lblOutstandingChargesValue.Font = myTNBFont.MuseoSans12();
            _lblOutstandingChargesValue.TextAlignment = UITextAlignment.Right;
            _lblOutstandingChargesValue.TextColor = myTNBColor.TunaGrey();
            _lblOutstandingChargesValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblOutstandingChargesValue);

            var lblTotalPayableTitle = new UILabel(new CGRect(18, 80, 119, 16));
            lblTotalPayableTitle.Font = myTNBFont.MuseoSans12();
            lblTotalPayableTitle.TextAlignment = UITextAlignment.Left;
            lblTotalPayableTitle.Text = "Total Payable";
            lblTotalPayableTitle.TextColor = myTNBColor.TunaGrey();
            lblTotalPayableTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(lblTotalPayableTitle);

            _lblTotalPayableValue = new UILabel(new CGRect(137, 80, View.Frame.Width - 155, 16));
            _lblTotalPayableValue.Font = myTNBFont.MuseoSans12();
            _lblTotalPayableValue.TextAlignment = UITextAlignment.Right;
            _lblTotalPayableValue.Text = "0.70";
            _lblTotalPayableValue.TextColor = myTNBColor.TunaGrey();
            _lblTotalPayableValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblTotalPayableValue);

            var lineView = new UIView(new RectangleF(18, 112, (float)View.Frame.Width - 36, 1));
            lineView.BackgroundColor = UIColor.LightGray;
            _viewCharges.AddSubview(lineView);
            #endregion

            _lblTotalDueAmountTitle = new UILabel(new CGRect(18, 136, 160, 18));
            _lblTotalDueAmountTitle.Font = myTNBFont.MuseoSans14();
            _lblTotalDueAmountTitle.TextAlignment = UITextAlignment.Left;
            _lblTotalDueAmountTitle.Text = "Total Amount Due";
            _lblTotalDueAmountTitle.TextColor = myTNBColor.TunaGrey();
            _lblTotalDueAmountTitle.BackgroundColor = UIColor.Clear;
            _currentBillDetailsView.AddSubview(_lblTotalDueAmountTitle);

            _viewAmount = new UIView(new CGRect(View.Frame.Width - 120, 130, 0, 24));             var lblCurrency = new UILabel(new CGRect(0, 6, 24, 18));             lblCurrency.Font = myTNBFont.MuseoSans14();             lblCurrency.TextColor = myTNBColor.TunaGrey();             lblCurrency.TextAlignment = UITextAlignment.Right;             lblCurrency.Text = CURRENCY;
            _viewAmount.BackgroundColor = UIColor.Clear;             _viewAmount.AddSubview(lblCurrency);              _lblAmount = new UILabel(new CGRect(24, 0, 75, 24));             _lblAmount.Font = myTNBFont.MuseoSans24();             _lblAmount.TextColor = myTNBColor.TunaGrey();             _lblAmount.TextAlignment = UITextAlignment.Right;             //_lblAmount.Text = "0.00";
            _lblAmount.BackgroundColor = UIColor.Clear;             _viewAmount.AddSubview(_lblAmount);              _currentBillDetailsView.AddSubview(_viewAmount);

            _btnPay = new UIButton(UIButtonType.Custom);
            _btnPay.Frame = new CGRect(18, 170, View.Frame.Width - 36, 48);
            _btnPay.SetTitle("Pay", UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnPay.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnPay.BackgroundColor = myTNBColor.SilverChalice();
            _btnPay.Layer.CornerRadius = 4.0f;
            _btnPay.Enabled = false;
            _currentBillDetailsView.AddSubview(_btnPay);

            //Bills and Payment History Header
            _historyHeaderView = new UIView(new RectangleF(0, 413, (float)View.Frame.Width, 48));
            _historyHeaderView.BackgroundColor = myTNBColor.SectionGrey();
            _headerView.AddSubview(_historyHeaderView);

            _lblHistoryHeader = new UILabel(new CGRect(18, 24, View.Frame.Width - 36, 18));
            _lblHistoryHeader.Font = myTNBFont.MuseoSans16();
            _lblHistoryHeader.TextAlignment = UITextAlignment.Left;
            _lblHistoryHeader.Text = isREAccount ? "Payment Advice History" : "Bill / Payment History";
            _lblHistoryHeader.TextColor = myTNBColor.PowerBlue();
            _lblHistoryHeader.BackgroundColor = UIColor.Clear;
            _historyHeaderView.AddSubview(_lblHistoryHeader);

            //Bills and Payment History Selection Buttons
            _historySelectionView = new UIView(new RectangleF(0, 461, (float)View.Frame.Width, 48));
            _historySelectionView.BackgroundColor = myTNBColor.SectionGrey();

            var btnWidth = (View.Frame.Width) / 2;

            _btnBills = new UIButton(UIButtonType.Custom);
            _btnBills.Frame = new CGRect(0, 0, btnWidth, 48);
            _btnBills.SetTitle("Bills", UIControlState.Normal);
            _btnBills.SetTitleColor(myTNBColor.PowerBlue(), UIControlState.Normal);
            _btnBills.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnBills.BackgroundColor = UIColor.White;
            MakeTopCornerRadius(_btnBills);
            _btnBills.Tag = 0;
            _historySelectionView.AddSubview(_btnBills);

            _btnPayment = new UIButton(UIButtonType.Custom);
            _btnPayment.Frame = new CGRect(_btnBills.Frame.Width, 0, btnWidth, 48);
            _btnPayment.SetTitle("Payment", UIControlState.Normal);
            _btnPayment.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
            _btnPayment.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnPayment.BackgroundColor = myTNBColor.SelectionGrey();
            MakeTopCornerRadius(_btnPayment);
            _btnPayment.Tag = 1;
            _historySelectionView.AddSubview(_btnPayment);

            //BillTableView Header
            /*var footerView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 62));
            footerView.BackgroundColor = myTNBColor.SectionGrey();
            billTableView.TableFooterView = footerView;

            var lblFooterDetails = new UILabel(new CGRect(12, 16, View.Frame.Width - 24, 18));
            lblFooterDetails.Font = myTNBFont.MuseoSans12();
            lblFooterDetails.TextAlignment = UITextAlignment.Center;
            lblFooterDetails.Text = "For more Bill/Payment History, visit myTNB Self-Service Portal.";
            lblFooterDetails.TextColor = UIColor.Gray;
            lblFooterDetails.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(lblFooterDetails);

            var lblFooterSubDetail = new UILabel(new CGRect(12, 34, View.Frame.Width - 24, 18));
            lblFooterSubDetail.Font = myTNBFont.MuseoSans12();
            lblFooterSubDetail.TextAlignment = UITextAlignment.Center;
            lblFooterSubDetail.Text = "www.mytnb.com.my";
            lblFooterSubDetail.TextColor = myTNBColor.PowerBlue();
            lblFooterSubDetail.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(lblFooterSubDetail);*/

            _btnPay.Enabled = false;
            _btnPay.BackgroundColor = myTNBColor.SilverChalice();
        }

        /// <summary>
        /// Shows the non owner view.
        /// </summary>
        public void ShowNonOwnerView()
        {
            _txtViewAddress.Hidden = true;
            float adjY = ((float)_txtViewAddress.Frame.Height + 16.0f) * -1.0f;

            AdjustFrameY(_currentBillHeaderView, adjY);
            AdjustFrameY(_currentBillDetailsView, adjY);
            AdjustFrameY(_historyHeaderView, adjY);
            AdjustFrameY(_historySelectionView, adjY);

            var temp = _headerView.Frame;
            temp.Height += adjY;
            _headerView.Frame = temp;
            billTableView.TableHeaderView = _headerView;
        }

        private void AdjustFrameY(UIView adjView, float adjY)
        {
            var temp = adjView.Frame;
            temp.Y += adjY;
            adjView.Frame = temp;
        }

        void MakeTopCornerRadius(UIButton btn)
        {
            var maskPath = UIBezierPath.FromRoundedRect(btn.Bounds
                                                        , UIRectCorner.TopLeft | UIRectCorner.TopRight
                                                        , new CGSize(10.0, 10.0));
            var maskLayer = new CAShapeLayer
            {
                Frame = btn.Bounds,
                Path = maskPath.CGPath
            };
            btn.Layer.Mask = maskLayer;
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)         {             return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));         }          void AdjustFrames()
        {             CGSize newSize = GetLabelSize(_lblAmount, View.Frame.Width / 2, _lblAmount.Frame.Height);             double newWidth = Math.Ceiling(newSize.Width);             _lblAmount.Frame = new CGRect(24, 0, newWidth, _lblAmount.Frame.Height);             _viewAmount.Frame = new CGRect(View.Frame.Width - (newWidth + 24 + 17), _viewAmount.Frame.Y, newWidth + 24, 24);         }

        void SetEvents()
        {
            if (_accountSelectionComponent != null && !IsFromNavigation)
            {
                if (ServiceCall.GetAccountListCount() > 0)
                {
                    UITapGestureRecognizer accountSelectionGesture = new UITapGestureRecognizer(() =>
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        SelectAccountTableViewController viewController =
                            storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    });
                    _accountSelectionComponent.SetSelectAccountEvent(accountSelectionGesture);
                }
            }

            _btnBills.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.selectedTag = 0;
                ToggleButtons();
            };

            _btnPayment.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.selectedTag = 1;
                ToggleButtons();
            };

            _btnPay.TouchUpInside += (sender, e) =>
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                            SelectBillsViewController selectBillsVC =
                                storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                            selectBillsVC.SelectedAccountDueAmount = DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal;
                            var navController = new UINavigationController(selectBillsVC);
                            PresentViewController(navController, true, null);
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            var alert = UIAlertController.Create("No Data Connection"
                                                                 , "Please check your data connection and try again."
                                                                 , UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                        }
                    });
                });
            };
        }

        void ToggleButtons()
        {
            bool isBillSelected = DataManager.DataManager.SharedInstance.selectedTag == 0;
            _btnBills.SetTitleColor(isBillSelected ? myTNBColor.PowerBlue() : UIColor.LightGray, UIControlState.Normal);
            _btnBills.BackgroundColor = isBillSelected ? UIColor.White : myTNBColor.SelectionGrey();
            _btnPayment.SetTitleColor(isBillSelected ? UIColor.LightGray : myTNBColor.PowerBlue(), UIControlState.Normal);
            _btnPayment.BackgroundColor = isBillSelected ? myTNBColor.SelectionGrey() : UIColor.White;

            if (_paymentNeedsUpdate && !isBillSelected)
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            GetPaymentHistory().ContinueWith(task =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    _paymentNeedsUpdate = false;
                                    InitializeBillTableView();
                                    ActivityIndicator.Hide();
                                });
                            });
                        }
                    });
                });
            }
            else
            {
                InitializeBillTableView();
            }
        }

        internal void ViewReceipt(string merchantTransactionID)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                        ReceiptViewController viewController =
                            storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                        viewController.MerchatTransactionID = merchantTransactionID;//"MYTN201801041414";//
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });
        }

        void ExecuteGetBillAccountDetailsCall()
        {
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList.d != null
                        && _billingAccountDetailsList.d.data != null)
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                        ExecuteGetBillHistoryCall();
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        var alert = UIAlertController.Create("Error in Response", "There is an error in the server, please try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }
    }
}