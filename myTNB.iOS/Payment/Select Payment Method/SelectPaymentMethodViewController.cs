using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Enums;
using myTNB.Model;
using UIKit;
using Foundation;
using System.Globalization;
using System.Security;
using myTNB.Payment;
using System.Diagnostics;
using static myTNB.TextHelper;

namespace myTNB
{
    public partial class SelectPaymentMethodViewController : CustomUIViewController
    {
        public SelectPaymentMethodViewController(IntPtr handle) : base(handle) { }

        private RegisteredCardsResponseModel _registeredCards = new RegisteredCardsResponseModel();
        public double TotalAmount = 0.00;
        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();
        public List<CustomerAccountRecordModel> AccountsForPayment = new List<CustomerAccountRecordModel>();
        public bool IsFromNavigation = false;
        public UITextField txtFieldAmountValue;

        private bool _isAMEX = false;
        private UIView viewCVVContainer, viewCVVWrapper, viewCVVBackground;
        private bool _isKeyboardDismissed = false;

        private string _selectedSavedCardID = string.Empty;
        private SecureString _cardCVVStr;

        public override void ViewDidLoad()
        {
            PageName = PaymentConstants.Pagename_SelectPaymentMethod;
            base.ViewDidLoad();

            AddBackButton();

            UIView headerView = new UIView(new CGRect(0, 0, (float)View.Frame.Width, 100));
            headerView.BackgroundColor = UIColor.White;
            selectPaymentTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - 64);
            selectPaymentTableView.TableHeaderView = headerView;

            UIView footerView = new UIView(new CGRect(0, 0, (float)View.Frame.Width, 50));
            footerView.BackgroundColor = MyTNBColor.SectionGrey;
            selectPaymentTableView.TableFooterView = footerView;

            //Credit Card Number 

            UILabel lblAmountTitle = new UILabel(new CGRect(18, 20, View.Frame.Width, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetCommonI18NValue(Constants.Common_TotalAmountRM).ToUpper()
            };
            headerView.AddSubview(lblAmountTitle);

            txtFieldAmountValue = new UITextField(new CGRect(18, 40, View.Frame.Width - 36, 24))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300,
                Text = TotalAmount.ToString("N2", CultureInfo.InvariantCulture),
                TextAlignment = UITextAlignment.Right,
                KeyboardType = UIKeyboardType.DecimalPad,
                Enabled = false
            };
            TextFieldHelper txtFieldHelper = new TextFieldHelper();
            txtFieldHelper.CreateDoneButton(txtFieldAmountValue);

            headerView.AddSubview(txtFieldAmountValue);

            UIView lineView = new UIView((new CGRect(18, 66, View.Frame.Width - 36, 1)));
            lineView.BackgroundColor = MyTNBColor.PlatinumGrey;
            headerView.AddSubview(lineView);

            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // to show toast on top of navigation bar
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Layer.ZPosition = -1;
            }

            ExecuteGetRegisteredCardsCall();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Layer.ZPosition = 0;
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            UIView.AnimationsEnabled = true;
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(PaymentConstants.I18N_Title);
        }

        private void ExecuteGetRegisteredCardsCall()
        {
            ActivityIndicator.Show();
            GetRegisteredCards().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    InitializedTableView();
                    ActivityIndicator.Hide();

                    if (_registeredCards != null && _registeredCards.d != null)
                    {
                        if (!_registeredCards.d.IsSuccess)
                        {
                            DisplayCustomAlert(GetErrorI18NValue(Constants.Error_PaymentCCErrorTitle),
                            GetErrorI18NValue(Constants.Error_PaymentCCErrorMsg),
                            new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                        }
                    }
                    else
                    {
                        DisplayCustomAlert(GetErrorI18NValue(Constants.Error_PaymentCCErrorTitle),
                            GetErrorI18NValue(Constants.Error_PaymentCCErrorMsg),
                            new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                    }
                });
            });
        }

        private Task GetRegisteredCards()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    isOwnedAccount = true
                };
                _registeredCards = serviceManager.OnExecuteAPIV6<RegisteredCardsResponseModel>(PaymentConstants.Service_GetRegisteredCards, requestParameter);
            });
        }

        internal void ExecuteRequestPayBillCall(int thePlatform, string thePaymentMode, string cardID, bool isNewCard, string amountDue)
        {
            InvokeOnMainThread(() =>
            {
                RemoveCachedAccountRecords();
                ActivityIndicator.Show();
                InvokeInBackground(async () =>
                {
                    GetPaymentTransactionIdResponseModel _paymentTransaction = await GetPaymentTransactionId(thePlatform, thePaymentMode, cardID);
                    InvokeOnMainThread(() =>
                    {
                        if (_paymentTransaction != null && _paymentTransaction.d != null && _paymentTransaction.d.IsSuccess
                        && _paymentTransaction.d.data != null)
                        {
                            Debug.WriteLine("Success");
                            NavigateToVC(_paymentTransaction, thePlatform, thePaymentMode);
                        }
                        else
                        {
                            DisplayServiceError(_paymentTransaction?.d?.DisplayMessage ?? string.Empty);
                        }
                        ActivityIndicator.Hide();
                    });
                });
            });
        }

        private void InitializedTableView()
        {
            selectPaymentTableView.Source = new SelectPaymentTableViewSource(_registeredCards
                , this, OnSelectUnavailablePaymentMethod);
            selectPaymentTableView.BackgroundColor = MyTNBColor.SectionGrey;
            selectPaymentTableView.ReloadData();
        }

        /// <summary>
        /// Handles the selection of unavailable payment method.
        /// </summary>
        /// <param name="methodType">Method type.</param>
        private void OnSelectUnavailablePaymentMethod(SystemEnum methodType)
        {
            ShowError(methodType);
        }

        /// <summary>
        /// Shows the error message.
        /// </summary>
        private void ShowError(SystemEnum methodType)
        {
            string errMsg = GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage);
            DowntimeDataModel status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == methodType);
            if (status != null && !string.IsNullOrEmpty(status?.DowntimeTextMessage))
            {
                errMsg = status?.DowntimeMessage;
            }
            DisplayGenericAlert(string.Empty, errMsg);
        }

        private void NavigateToVC(GetPaymentTransactionIdResponseModel paymentTransactionIDResponse, int platform, string paymentMode)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("MakePayment", null);
            MakePaymentViewController makePaymentVC =
                storyBoard.InstantiateViewController("MakePaymentViewController") as MakePaymentViewController;
            if (makePaymentVC != null)
            {
                makePaymentVC._paymentTransactionIDResponseModel = paymentTransactionIDResponse;
                makePaymentVC._isNewCard = false;
                makePaymentVC._platform = platform;
                makePaymentVC._paymentMode = paymentMode;
                makePaymentVC._cardCVV = ConvertSecureStringToString(_cardCVVStr);
                NavigationController.PushViewController(makePaymentVC, true);
            }
        }

        internal void ShowCVVField(string cardType, string cardID)
        {
            _selectedSavedCardID = cardID;
            //cardType = "A";
            _isAMEX = cardType.ToLower().Equals("a");

            viewCVVBackground = new UIView(new CGRect(0, 0, View.Frame.Width, UIScreen.MainScreen.Bounds.Height - 175));
            viewCVVBackground.BackgroundColor = UIColor.Black;
            viewCVVBackground.Alpha = 0.75F;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewCVVBackground);

            viewCVVContainer = new UIView(new CGRect(0, View.Frame.Height - 175, View.Frame.Width, 175));
            viewCVVContainer.BackgroundColor = UIColor.White;
            viewCVVContainer.Alpha = 1.0f;

            UIButton btnBack = new UIButton(UIButtonType.Custom);
            btnBack.Frame = new CGRect(6, 10, 60, 24);
            btnBack.SetTitleColor(MyTNBColor.PowerBlue, UIControlState.Normal);
            btnBack.SetTitle(GetCommonI18NValue(Constants.Common_Back), UIControlState.Normal);
            btnBack.Font = MyTNBFont.MuseoSans16;
            btnBack.TouchUpInside += (sender, e) =>
            {
                View.EndEditing(true);
                viewCVVContainer.Hidden = true;
                viewCVVContainer.RemoveFromSuperview();
                viewCVVBackground.Hidden = true;
                viewCVVBackground.RemoveFromSuperview();
                _isAMEX = false;
            };

            UILabel lblCVVDetails = new UILabel(new CGRect(18, 60, viewCVVContainer.Frame.Width - 36, 40));
            lblCVVDetails.Font = MyTNBFont.MuseoSans14;
            lblCVVDetails.TextColor = MyTNBColor.TunaGrey();
            lblCVVDetails.LineBreakMode = UILineBreakMode.WordWrap;
            lblCVVDetails.Lines = 2;
            lblCVVDetails.TextAlignment = UITextAlignment.Left;
            lblCVVDetails.Text = GetI18NValue(_isAMEX ? PaymentConstants.I18N_CVVFour : PaymentConstants.I18N_CVVThree);

            viewCVVWrapper = new UIView(new CGRect(0, 124, viewCVVContainer.Frame.Width, 26));

            viewCVVContainer.AddSubviews(new UIView[] { btnBack, lblCVVDetails, viewCVVWrapper });

            UITextField txtFieldCVV;
            UIView viewLine;
            int cvvCount = _isAMEX ? 4 : 3;
            float xLocation = (float)(viewCVVWrapper.Frame.Width / cvvCount); //0;
            for (int i = 0; i < cvvCount; i++)
            {
                int index = i;
                txtFieldCVV = new UITextField(new CGRect(xLocation, 0, 44, 24));
                txtFieldCVV.Placeholder = "-";
                txtFieldCVV.TextColor = MyTNBColor.TunaGrey();
                txtFieldCVV.Font = MyTNBFont.MuseoSans16;
                txtFieldCVV.Tag = index + 1;
                txtFieldCVV.KeyboardType = UIKeyboardType.NumberPad;
                txtFieldCVV.AutocorrectionType = UITextAutocorrectionType.No;
                txtFieldCVV.AutocapitalizationType = UITextAutocapitalizationType.None;
                txtFieldCVV.SpellCheckingType = UITextSpellCheckingType.No;
                txtFieldCVV.ReturnKeyType = UIReturnKeyType.Done;
                txtFieldCVV.TextAlignment = UITextAlignment.Center;
                txtFieldCVV.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    nint newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= 1;
                };
                SetTextFieldEvents(txtFieldCVV);

                viewLine = new UIView(new CGRect(xLocation, 25, 44, 1));
                viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
                viewLine.Tag = 6;

                viewCVVWrapper.AddSubview(viewLine);
                viewCVVWrapper.AddSubview(txtFieldCVV);

                xLocation += 14 + 34;
                if (i == 0)
                {
                    txtFieldCVV.BecomeFirstResponder();
                }
            }

            View.AddSubview(viewCVVContainer);
        }

        /// <summary>
        /// Handles the keyboard notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (visible)
            {
                CGRect r = UIKeyboard.BoundsFromNotification(notification);
                CGRect viewFrame = View.Bounds;
                nfloat currentViewHeight = viewFrame.Height - r.Height;
                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                if (viewCVVContainer != null && viewCVVContainer.IsDescendantOfView(View))
                {
                    ViewHelper.AdjustFrameSetHeight(viewCVVBackground, UIScreen.MainScreen.Bounds.Height - (r.Height + 175));
                    ViewHelper.AdjustFrameSetY(viewCVVContainer, View.Frame.Height - r.Height - 175);
                }

            }

            UIView.CommitAnimations();
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            textField.EditingChanged += (sender, e) =>
            {
                if (textField.Text.Length == 1)
                {
                    int maxTag = _isAMEX ? 5 : 4;
                    textField.ResignFirstResponder();
                    _isKeyboardDismissed = true;
                    int nextTag = (int)textField.Tag + 1;
                    if (nextTag < maxTag)
                    {
                        UIResponder nextResponder = textField.Superview.ViewWithTag(nextTag);
                        nextResponder.BecomeFirstResponder();
                        _isKeyboardDismissed = false;
                    }
                }
                ValidateFields(_isKeyboardDismissed);
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                UIView.AnimationsEnabled = false;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        private void ValidateFields(bool isKeyboardDismissed)
        {
            UITextField txtFieldCVV1 = viewCVVWrapper.ViewWithTag(1) as UITextField;
            UITextField txtFieldCVV2 = viewCVVWrapper.ViewWithTag(2) as UITextField;
            UITextField txtFieldCVV3 = viewCVVWrapper.ViewWithTag(3) as UITextField;
            UITextField txtFieldCVV4 = viewCVVWrapper.ViewWithTag(4) as UITextField;

            if (txtFieldCVV1 != null && txtFieldCVV2 != null && txtFieldCVV3 != null)
            {
                if (!string.IsNullOrEmpty(txtFieldCVV1.Text) && !string.IsNullOrEmpty(txtFieldCVV2.Text)
                && !string.IsNullOrEmpty(txtFieldCVV3.Text) && isKeyboardDismissed)
                {
                    SecureString cvv = new SecureString();
                    cvv = ConvertStringToSecureString(txtFieldCVV1.Text + txtFieldCVV2.Text + txtFieldCVV3.Text);

                    if (_isAMEX)
                    {
                        if (txtFieldCVV4 != null)
                        {
                            if (!string.IsNullOrEmpty(txtFieldCVV4.Text))
                            {
                                cvv = ConvertStringToSecureString(ConvertSecureStringToString(cvv) + txtFieldCVV4.Text);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    ActivityIndicator.Show();
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                View.EndEditing(true);
                                viewCVVContainer.Hidden = true;
                                viewCVVContainer.RemoveFromSuperview();
                                viewCVVBackground.Hidden = true;
                                viewCVVBackground.RemoveFromSuperview();
                                _isAMEX = false;
                                ActivityIndicator.Hide();
                                _cardCVVStr = new SecureString();
                                _cardCVVStr = cvv;
                                ExecuteRequestPayBillCall(2, "CC", _selectedSavedCardID, false, txtFieldAmountValue.Text);
                            }
                            else
                            {
                                DisplayNoDataAlert();
                                ActivityIndicator.Hide();
                            }
                        });
                    });
                }
            }
        }

        /// <summary>
        /// Removes the cached account records.
        /// </summary>
        private void RemoveCachedAccountRecords()
        {
            foreach (CustomerAccountRecordModel item in AccountsForPayment)
            {
                DataManager.DataManager.SharedInstance.DeleteDue(item.accNum);
                DataManager.DataManager.SharedInstance.DeleteDetailsFromPaymentHistory(item.accNum);
            }
        }

        private async Task<GetPaymentTransactionIdResponseModel> GetPaymentTransactionId(int platform, string paymentMode, string cardID)
        {
            int count = AccountsForPayment?.Count ?? 0;
            string ownerName = count == 1 ? AccountsForPayment[0].accountOwnerName : string.Empty;
            List<object> paymentItems = new List<object>();

            foreach (CustomerAccountRecordModel item in AccountsForPayment)
            {
                if (AccountChargesCache.HasMandatory(item.accNum))
                {
                    paymentItems.Add(new
                    {
                        AccountOwnerName = count > 1 ? item.accountOwnerName : DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                        AccountNo = item?.accNum ?? string.Empty,
                        AccountAmount = item.Amount.ToString(CultureInfo.InvariantCulture),
                        AccountPayments = AccountChargesCache.GetAccountPayments(item.accNum)
                    });
                }
                else
                {
                    paymentItems.Add(new
                    {
                        AccountOwnerName = count > 1 ? item.accountOwnerName : DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                        AccountNo = item?.accNum ?? string.Empty,
                        AccountAmount = item.Amount.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }

            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                customerName = count > 1 ? DataManager.DataManager.SharedInstance.UserEntity[0].displayName : ownerName,
                phoneNo = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo != null
                    ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : string.Empty,
                platform,
                registeredCardId = cardID,
                paymentMode,
                totalAmount = TotalAmount,
                paymentItems
            };
            GetPaymentTransactionIdResponseModel response = serviceManager.OnExecuteAPIV6<GetPaymentTransactionIdResponseModel>("GetPaymentTransactionId", request);
            return response;
        }
    }
}