using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using myTNB.Model.RequestPayBill;
using myTNB.Payment.AddCard;
using UIKit;

namespace myTNB
{
    public partial class SelectPaymentMethodViewController : UIViewController
    {
        public SelectPaymentMethodViewController(IntPtr handle) : base(handle)
        {
        }

        RegisteredCardsResponseModel _registeredCards = new RegisteredCardsResponseModel();
        RequestPayBillResponseModel _requestPayBill = new RequestPayBillResponseModel();
        TitleBarComponent titleBarComponent;
        public string TotalAmount = string.Empty;
        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();
        public List<CustomerAccountRecordModel> AccountsForPayment = new List<CustomerAccountRecordModel>();
        public bool IsFromNavigation = false;
        bool _isAMEX = false;

        UIView viewCVVContainer;
        UIView viewCVVWrapper;
        UIView viewCVVBackground;
        public UITextField txtFieldAmountValue;
        bool _isKeyboardDismissed = false;

        string _selectedSavedCardType = string.Empty;
        string _selectedSavedCardID = string.Empty;
        string _cardCVV = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();

            var headerView = new UIView(new CGRect(0, 0, (float)View.Frame.Width, 100));
            headerView.BackgroundColor = myTNBColor.SectionGrey();
            selectPaymentTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - 64);
            selectPaymentTableView.TableHeaderView = headerView;

            var footerView = new UIView(new CGRect(0, 0, (float)View.Frame.Width, 50));
            footerView.BackgroundColor = myTNBColor.SectionGrey();
            selectPaymentTableView.TableFooterView = footerView;

            //Credit Card Number 

            var lblAmountTitle = new UILabel(new CGRect(18, 20, View.Frame.Width, 12));
            lblAmountTitle.TextColor = myTNBColor.SilverChalice();
            lblAmountTitle.Font = myTNBFont.MuseoSans9();
            lblAmountTitle.TextAlignment = UITextAlignment.Left;
            lblAmountTitle.Text = "TOTAL AMOUNT (RM)";
            headerView.AddSubview(lblAmountTitle);

            txtFieldAmountValue = new UITextField(new CGRect(18, 40, View.Frame.Width - 36, 24));
            txtFieldAmountValue.TextColor = myTNBColor.TunaGrey();
            txtFieldAmountValue.Font = myTNBFont.MuseoSans16();
            //txtFieldAmountValue.Text = DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal.ToString();
            txtFieldAmountValue.Text = TotalAmount;
            txtFieldAmountValue.TextAlignment = UITextAlignment.Right;
            txtFieldAmountValue.KeyboardType = UIKeyboardType.DecimalPad;
            txtFieldAmountValue.Enabled = false;
            TextFieldHelper txtFieldHelper = new TextFieldHelper();
            txtFieldHelper.CreateDoneButton(txtFieldAmountValue);

            headerView.AddSubview(txtFieldAmountValue);

            var lineView = new UIView((new CGRect(18, 66, View.Frame.Width - 36, 1)));
            lineView.BackgroundColor = myTNBColor.PlatinumGrey();
            headerView.AddSubview(lineView);

            /*NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                var userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                Console.WriteLine("height: " + keyboardRectangle.Height);
            });
            */
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ExecuteGetRegisteredCardsCall();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            UIView.AnimationsEnabled = true;
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetNavigationBar()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
            }
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Select Payment Method");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                if (IsFromNavigation)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                    NotificationDetailsViewController viewController =
                        storyBoard.InstantiateViewController("NotificationDetailsViewController") as NotificationDetailsViewController;
                    viewController.NotificationInfo = NotificationInfo;
                    NavigationController.PushViewController(viewController, true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void ExecuteGetRegisteredCardsCall()
        {
            ActivityIndicator.Show();
            GetRegisteredCards().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    InitializedTableView();
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task GetRegisteredCards()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email  //"rohanbomle@gmail.com" //For Testing
                };
                _registeredCards = serviceManager.GetRegisteredCards("GetRegisteredCards", requestParameter);
            });
        }

        internal void DisplayPaymentThreshold()
        {
            var alert = UIAlertController.Create(string.Empty, "For payments more than RM 5000, please use FPX payment mode.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        internal void ExecuteRequestPayBillCall(int thePlatform, string thePaymentMode, string cardID, bool isNewCard, string amountDue)
        {
            ActivityIndicator.Show();
            /*
            RequestPayBill(thePlatform, thePaymentMode, cardID, isNewCard, amountDue).ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    InitializedTableView();
                    ActivityIndicator.Hide();
                    if (isNewCard == false)
                    {
                        NavigateToVC(_requestPayBill, thePlatform, thePaymentMode);
                    }

                });
            });*/

            RequestMultiPayBill(thePlatform, thePaymentMode, cardID, isNewCard, amountDue).ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_requestPayBill != null && _requestPayBill.d != null
                       && _requestPayBill.d.data != null)
                    {
                        InitializedTableView();
                        if (isNewCard == false)
                        {
                            NavigateToVC(_requestPayBill, thePlatform, thePaymentMode);
                        }
                    }
                    else
                    {
                        string errMsg = "There is an error in the server, please try again.";
                        if (_requestPayBill != null && _requestPayBill.d != null && !string.IsNullOrEmpty(_requestPayBill.d.message))
                        {
                            errMsg = _requestPayBill.d.message;
                        }
                        var alert = UIAlertController.Create(string.Empty, errMsg, UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task RequestPayBill(int thePlatform, string thePaymentMode, string cardID, bool isNewCard, string amountDue)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    customerName = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                    accNum = DataManager.DataManager.SharedInstance.BillingAccountDetails.accNum,
                    amount = amountDue,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    phoneNo = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo != null ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : "",
                    sspUserId = DataManager.DataManager.SharedInstance.User.UserID,
                    platform = thePlatform,
                    registeredCardId = cardID,
                    paymentMode = thePaymentMode
                };
                _requestPayBill = serviceManager.RequestPayBill("RequestPayBill", requestParameter);
            });
        }

        internal Task RequestMultiPayBill(int thePlatform, string thePaymentMode, string cardID, bool isNewCard, string amountDue)
        {
            List<PaymentItemsModel> paymentItemList = new List<PaymentItemsModel>();
            PaymentItemsModel paymentItem;
            foreach (var item in AccountsForPayment)
            {
                paymentItem = new PaymentItemsModel();
                paymentItem.AccountNo = item.accNum;
                paymentItem.Amount = item.Amount.ToString();
                paymentItemList.Add(paymentItem);
            }

            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    customerName = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                    accNum = DataManager.DataManager.SharedInstance.BillingAccountDetails.accNum,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    phoneNo = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo != null ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : "",
                    sspUserId = DataManager.DataManager.SharedInstance.User.UserID,
                    platform = thePlatform,
                    registeredCardId = cardID,
                    paymentMode = thePaymentMode,
                    totalAmount = Convert.ToDouble(TotalAmount),
                    paymentItems = paymentItemList
                };
                _requestPayBill = serviceManager.RequestMultiPayBill("RequestMultiPayBill", requestParameter);
            });
        }

        internal void InitializedTableView()
        {
            selectPaymentTableView.Source = new SelectPaymentTableViewSource(_registeredCards, _requestPayBill, this);
            selectPaymentTableView.BackgroundColor = myTNBColor.SectionGrey();
            selectPaymentTableView.ReloadData();
        }

        internal void NavigateToVC(RequestPayBillResponseModel requestPayBillResponseModel, int platform, string paymentMode)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("MakePayment", null);
            MakePaymentViewController makePaymentVC =
                storyBoard.InstantiateViewController("MakePaymentViewController") as MakePaymentViewController;
            makePaymentVC._requestPayBillResponseModel = requestPayBillResponseModel;
            makePaymentVC._isNewCard = false;
            makePaymentVC._platform = platform;
            makePaymentVC._paymentMode = paymentMode;
            makePaymentVC._cardCVV = _cardCVV;

            var navController = new UINavigationController(makePaymentVC);
            NavigationController.PushViewController(makePaymentVC, true);
        }

        internal void ShowCVVField(string cardType, string cardID)
        {
            _selectedSavedCardType = cardType;
            _selectedSavedCardID = cardID;
            //cardType = "A";
            _isAMEX = cardType.ToLower().Equals("a");

            viewCVVBackground = new UIView(new CGRect(0
                                                             , 0
                                                             , View.Frame.Width
                                                             , UIScreen.MainScreen.Bounds.Height - (GetKeyboardHeight() + 175)));
            viewCVVBackground.BackgroundColor = UIColor.Black;
            viewCVVBackground.Alpha = 0.75F;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewCVVBackground);

            viewCVVContainer = new UIView(new CGRect(0
                                                     , View.Frame.Height - GetKeyboardHeight() - 175
                                                     , View.Frame.Width
                                                     , 175));
            viewCVVContainer.BackgroundColor = UIColor.White;
            viewCVVContainer.Alpha = 1.0f;

            UIButton btnBack = new UIButton(UIButtonType.Custom);
            btnBack.Frame = new CGRect(6, 10, 60, 24);
            btnBack.SetTitleColor(myTNBColor.PowerBlue(), UIControlState.Normal);
            btnBack.SetTitle("Back", UIControlState.Normal);
            btnBack.Font = myTNBFont.MuseoSans16();
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
            lblCVVDetails.Font = myTNBFont.MuseoSans14();
            lblCVVDetails.TextColor = myTNBColor.TunaGrey();
            lblCVVDetails.LineBreakMode = UILineBreakMode.WordWrap;
            lblCVVDetails.Lines = 2;
            lblCVVDetails.TextAlignment = UITextAlignment.Left;
            lblCVVDetails.Text = _isAMEX ? "Please enter the 4-digit CVV/CVC located at the front of your credit card."
                : "Please enter the 3-digit CVV/CVC located at the back of your credit card.";

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
                txtFieldCVV.TextColor = myTNBColor.TunaGrey();
                txtFieldCVV.Font = myTNBFont.MuseoSans16();
                txtFieldCVV.Tag = index + 1;
                txtFieldCVV.KeyboardType = UIKeyboardType.NumberPad;
                txtFieldCVV.AutocorrectionType = UITextAutocorrectionType.No;
                txtFieldCVV.AutocapitalizationType = UITextAutocapitalizationType.None;
                txtFieldCVV.SpellCheckingType = UITextSpellCheckingType.No;
                txtFieldCVV.ReturnKeyType = UIReturnKeyType.Done;
                txtFieldCVV.TextAlignment = UITextAlignment.Center;
                txtFieldCVV.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= 1;
                };
                SetTextFieldEvents(txtFieldCVV);

                viewLine = new UIView(new CGRect(xLocation, 25, 44, 1));
                viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
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

        int GetKeyboardHeight()
        {
            int deviceHeight = (int)UIScreen.MainScreen.NativeBounds.Height;
            int height = 226;
            if (deviceHeight == 1136)
            {
                height = 216;
            }
            else if (deviceHeight == 1334)
            {
                height = 216;
            }
            else if (deviceHeight == 2208)
            {
                height = 226;
            }
            else if (deviceHeight == 2436)
            {
                height = 291;
            }
            return height;
        }

        internal void SetTextFieldEvents(UITextField textField)
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

        internal void ValidateFields(bool isKeyboardDismissed)
        {
            UITextField txtFieldCVV1 = viewCVVWrapper.ViewWithTag(1) as UITextField;
            UITextField txtFieldCVV2 = viewCVVWrapper.ViewWithTag(2) as UITextField;
            UITextField txtFieldCVV3 = viewCVVWrapper.ViewWithTag(3) as UITextField;
            UITextField txtFieldCVV4 = viewCVVWrapper.ViewWithTag(4) as UITextField;

            if (!string.IsNullOrEmpty(txtFieldCVV1.Text) && !string.IsNullOrEmpty(txtFieldCVV2.Text)
                && !string.IsNullOrEmpty(txtFieldCVV3.Text) && isKeyboardDismissed)
            {
                string cvv = txtFieldCVV1.Text + txtFieldCVV2.Text + txtFieldCVV3.Text;
                if (_isAMEX)
                {
                    if (!string.IsNullOrEmpty(txtFieldCVV4.Text))
                    {
                        cvv += txtFieldCVV4.Text;
                    }
                    else
                    {
                        return;
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
                            Console.WriteLine("CVV: " + cvv);
                            ActivityIndicator.Hide();
                            _cardCVV = cvv;
                            ExecuteRequestPayBillCall(2, "CC", _selectedSavedCardID, false, txtFieldAmountValue.Text);
                        }
                        else
                        {
                            var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, true, null);
                            ActivityIndicator.Hide();
                        }
                    });
                });
            }
        }
    }
}