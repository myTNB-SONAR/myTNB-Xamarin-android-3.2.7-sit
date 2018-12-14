using System;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using myTNB.Model.RequestPayBill;

namespace myTNB
{
    public partial class AddCardViewController : UIViewController
    {
        public AddCardViewController(IntPtr handle) : base(handle)
        {
        }

        UILabel lblCardNumberTitle;
        UILabel lblCardNumberError;
        UITextField txtFieldCardNumber;
        UIView viewLineCardNumber;

        UILabel lblNameTitle;
        UILabel lblNameError;
        UITextField txtFieldName;
        UIView viewLineName;

        UILabel lblCardExpiryTitle;
        UILabel lblCardExpiryError;
        UITextField txtFieldCardExpiry;
        UIView viewLineCardExpiry;

        UILabel lblCVVTitle;
        UILabel lblCVVError;
        UITextField txtFieldCVV;
        UIView viewLineCVV;

        UIButton btnNext;
        UIButton btnCheckBox;

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        RequestPayBillResponseModel _requestPayBill = new RequestPayBillResponseModel();
        public double _amountDue = 0;
        public RegisteredCardsResponseModel _registeredCards;
        bool _cardAlreadySaved = false;
        public List<CustomerAccountRecordModel> AccountsForPayment = new List<CustomerAccountRecordModel>();
        public string TotalAmount = string.Empty;

        Dictionary<string, int[]> cardFormatPattern = new Dictionary<string, int[]>
        {
            {"V", new int[] { 4, 4, 4, 4 }},
            {"M", new int[] { 4, 4, 4, 4 }},
            {"A", new int[] { 4, 6, 5}}
        };

        Dictionary<string, string> cardPrefixPattern = new Dictionary<string, string>
        {
            {"V", @"^$|^4.*"},
            {"M", @"^$|^5.*"},
            {"A", @"^$|^3$|^3[47].*"}
        };

        Dictionary<string, string> cardValidationPattern = new Dictionary<string, string>
        {
            {"V", @"^4[0-9]{15}$"},
            {"M", @"^5[0-9]{15}$"},
            {"A", @"^3[47][0-9]{13}$"}
        };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            SetNavigationItems();
            SetSubviews();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.CreditCardInfo.CreditCardNumber))
            {
                txtFieldCardNumber.Text = DataManager.DataManager.SharedInstance.CreditCardInfo.CreditCardNumber;
                txtFieldName.Text = DataManager.DataManager.SharedInstance.CreditCardInfo.CardHolderName;
                DataManager.DataManager.SharedInstance.ClearCreditCardInfo();
            }
        }

        internal void SetNavigationItems()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(false);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetSubviews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 36));
            lblDescription.Font = myTNBFont.MuseoSans14();
            lblDescription.TextColor = myTNBColor.TunaGrey();
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Lines = 0;
            lblDescription.Text = "Only credit / debit cards issued in Malaysia are accepted.";
            lblDescription.TextAlignment = UITextAlignment.Left;
            View.AddSubview(lblDescription);

            //Credit Card Number 
            UIView viewCardNumber = new UIView((new CGRect(18, 68, View.Frame.Width - 36, 51)));
            viewCardNumber.BackgroundColor = UIColor.Clear;

            lblCardNumberTitle = new UILabel(new CGRect(0, 0, viewCardNumber.Frame.Width, 12));
            lblCardNumberTitle.TextColor = myTNBColor.SilverChalice();
            lblCardNumberTitle.Font = myTNBFont.MuseoSans9();
            lblCardNumberTitle.TextAlignment = UITextAlignment.Left;
            lblCardNumberTitle.Text = "Card No.";
            lblCardNumberTitle.Hidden = true;
            viewCardNumber.AddSubview(lblCardNumberTitle);

            lblCardNumberError = new UILabel(new CGRect(0, 37, viewCardNumber.Frame.Width, 14));
            lblCardNumberError.TextColor = myTNBColor.Tomato();
            lblCardNumberError.Font = myTNBFont.MuseoSans9();
            lblCardNumberError.TextAlignment = UITextAlignment.Left;
            lblCardNumberError.Text = "Invalid Card No.";
            lblCardNumberError.Hidden = true;
            viewCardNumber.AddSubview(lblCardNumberError);

            txtFieldCardNumber = new UITextField(new CGRect(0, 12, viewCardNumber.Frame.Width - 30, 24));
            txtFieldCardNumber.TextColor = myTNBColor.TunaGrey();
            txtFieldCardNumber.Font = myTNBFont.MuseoSans16();
            txtFieldCardNumber.TextAlignment = UITextAlignment.Left;
            txtFieldCardNumber.Placeholder = "Card No.";
            viewCardNumber.AddSubview(txtFieldCardNumber);

            viewLineCardNumber = new UIView((new CGRect(0, 36, viewCardNumber.Frame.Width, 1)));
            viewLineCardNumber.BackgroundColor = myTNBColor.PlatinumGrey();

            UIView viewScanner = new UIView(new CGRect(viewCardNumber.Frame.Width - 30, 12, 24, 24));
            UIImageView scanner = new UIImageView(new CGRect(0, 0, 24, 24));
            scanner.Image = UIImage.FromBundle("Camera");

            UITapGestureRecognizer tapScan = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("AddCard", null);
                UIViewController viewController =
                    storyBoard.InstantiateViewController("CreditCardScannerViewController") as UIViewController;
                NavigationController.PushViewController(viewController, true);
            });
            viewScanner.AddGestureRecognizer(tapScan);
            viewScanner.AddSubview(scanner);
            viewCardNumber.AddSubview(viewScanner);

            viewCardNumber.AddSubview(viewLineCardNumber);

            //Name 
            UIView viewName = new UIView((new CGRect(18, 128, View.Frame.Width - 36, 51)));
            viewName.BackgroundColor = UIColor.Clear;

            lblNameTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12));
            lblNameTitle.TextColor = myTNBColor.SilverChalice();
            lblNameTitle.Font = myTNBFont.MuseoSans9();
            lblNameTitle.TextAlignment = UITextAlignment.Left;
            lblNameTitle.Text = "Name on Card";
            lblNameTitle.Hidden = true;
            viewName.AddSubview(lblNameTitle);

            lblNameError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14));
            lblNameError.TextColor = myTNBColor.Tomato();
            lblNameError.Font = myTNBFont.MuseoSans9();
            lblNameError.TextAlignment = UITextAlignment.Left;
            lblNameError.Text = "Invalid Name";
            lblNameError.Hidden = true;
            viewName.AddSubview(lblNameError);

            txtFieldName = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24));
            txtFieldName.TextColor = myTNBColor.TunaGrey();
            txtFieldName.Font = myTNBFont.MuseoSans16();
            txtFieldName.TextAlignment = UITextAlignment.Left;
            txtFieldName.Placeholder = "Name on Card";
            viewName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)));
            viewLineName.BackgroundColor = myTNBColor.PlatinumGrey();
            viewName.AddSubview(viewLineName);

            //Card Expiry 
            UIView viewCardExpiry = new UIView((new CGRect(18, 185, View.Frame.Width - 36, 51)));
            viewCardExpiry.BackgroundColor = UIColor.Clear;

            lblCardExpiryTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12));
            lblCardExpiryTitle.TextColor = myTNBColor.SilverChalice();
            lblCardExpiryTitle.Font = myTNBFont.MuseoSans9();
            lblCardExpiryTitle.TextAlignment = UITextAlignment.Left;
            lblCardExpiryTitle.Text = "Card Expiry";
            lblCardExpiryTitle.Hidden = true;
            viewCardExpiry.AddSubview(lblCardExpiryTitle);

            lblCardExpiryError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14));
            lblCardExpiryError.TextColor = myTNBColor.Tomato();
            lblCardExpiryError.Font = myTNBFont.MuseoSans9();
            lblCardExpiryError.TextAlignment = UITextAlignment.Left;
            lblCardExpiryError.Text = "Invalid Card Expiration Date";
            lblCardExpiryError.Hidden = true;
            viewCardExpiry.AddSubview(lblCardExpiryError);

            txtFieldCardExpiry = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24));
            txtFieldCardExpiry.TextColor = myTNBColor.TunaGrey();
            txtFieldCardExpiry.Font = myTNBFont.MuseoSans16();
            txtFieldCardExpiry.TextAlignment = UITextAlignment.Left;
            txtFieldCardExpiry.Placeholder = "Card Expiry (MM/YY)";
            viewCardExpiry.AddSubview(txtFieldCardExpiry);

            txtFieldCardExpiry.AddTarget((sender, e) =>
            {
                var theTextField = (UITextField)sender;
                var textVal = theTextField.Text;

                if (textVal.Length == 2)
                {
                    theTextField.Text = textVal + "/";
                }
                else if (textVal.Length > 5)
                {
                    theTextField.Text = textVal.Substring(5);
                }
            }, UIControlEvent.EditingChanged);

            txtFieldCardExpiry.ShouldChangeCharacters += (textField, range, replacement) =>
            {
                var theTextField = (UITextField)textField;
                var textVal = theTextField.Text;

                if (theTextField.Text.Length == 5 && range.Length == 0)
                {
                    return false;
                }
                else
                {
                    if (replacement == "" && theTextField.Text.Length == 3)
                    {
                        theTextField.Text = textVal.Replace("/", "");
                    }
                    return true;
                }
            };

            viewLineCardExpiry = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)));
            viewLineCardExpiry.BackgroundColor = myTNBColor.PlatinumGrey();
            viewCardExpiry.AddSubview(viewLineCardExpiry);

            //CVV 
            UIView viewCVV = new UIView((new CGRect(18, 242, View.Frame.Width - 36, 51)));
            viewCVV.BackgroundColor = UIColor.Clear;

            lblCVVTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12));
            lblCVVTitle.TextColor = myTNBColor.SilverChalice();
            lblCVVTitle.Font = myTNBFont.MuseoSans9();
            lblCVVTitle.TextAlignment = UITextAlignment.Left;
            lblCVVTitle.Text = "CVV";
            lblCVVTitle.Hidden = true;
            viewCVV.AddSubview(lblCVVTitle);

            lblCVVError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14));
            lblCVVError.TextColor = myTNBColor.Tomato();
            lblCVVError.Font = myTNBFont.MuseoSans9();
            lblCVVError.TextAlignment = UITextAlignment.Left;
            lblCVVError.Text = "Invalid CVV";
            lblCVVError.Hidden = true;
            viewCVV.AddSubview(lblCVVError);

            txtFieldCVV = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24));
            txtFieldCVV.TextColor = myTNBColor.TunaGrey();
            txtFieldCVV.Font = myTNBFont.MuseoSans16();
            txtFieldCVV.TextAlignment = UITextAlignment.Left;
            txtFieldCVV.Placeholder = "CVV";
            viewCVV.AddSubview(txtFieldCVV);

            txtFieldCVV.ShouldChangeCharacters += (textField, range, replacement) =>
            {
                var theTextField = (UITextField)textField;

                if (theTextField.Text.Length == 4 && range.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            };

            viewLineCVV = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)));
            viewLineCVV.BackgroundColor = myTNBColor.PlatinumGrey();
            viewCVV.AddSubview(viewLineCVV);

            btnCheckBox = new UIButton(UIButtonType.Custom);
            btnCheckBox.Frame = new CGRect(18, 320, 24, 24);
            btnCheckBox.SetImage(UIImage.FromBundle("Payment-Checkbox-Inactive"), UIControlState.Normal);
            btnCheckBox.SetImage(UIImage.FromBundle("Payment-Checkbox-Active"), UIControlState.Selected);
            btnCheckBox.SetTitleColor(myTNBColor.FreshGreen(), UIControlState.Normal);
            btnCheckBox.BackgroundColor = UIColor.Clear;
            btnCheckBox.Layer.CornerRadius = 5.0f;
            View.AddSubview(btnCheckBox);
            //btnCheckBox.Hidden = true;

            btnCheckBox.TouchUpInside += (sender, e) =>
            {
                var btn = ((UIButton)sender);
                if (btn.Selected == true)
                {
                    btn.Selected = false;
                    _cardAlreadySaved = false;
                }
                else
                {
                    VerifyCardSaveStatus();
                }
            };

            var lblCheckBoxTitle = new UILabel(new CGRect(51, 324, View.Frame.Width - 69, 18));
            lblCheckBoxTitle.TextColor = myTNBColor.TunaGrey();
            lblCheckBoxTitle.Font = myTNBFont.MuseoSans14();
            lblCheckBoxTitle.TextAlignment = UITextAlignment.Left;
            lblCheckBoxTitle.Text = "Do you want to save this card?";
            View.AddSubview(lblCheckBoxTitle);
            //lblCheckBoxTitle.Hidden = true;

            btnNext = new UIButton(UIButtonType.Custom);
            btnNext.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneX() ? 156 : 132), View.Frame.Width - 36, 48);
            btnNext.SetTitle("Next", UIControlState.Normal);
            btnNext.Font = myTNBFont.MuseoSans16();
            btnNext.Layer.CornerRadius = 5.0f;
            btnNext.BackgroundColor = myTNBColor.SilverChalice();
            btnNext.Enabled = false;
            btnNext.TouchUpInside += (sender, e) =>
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteRequestPayBillCall();
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
            };

            View.AddSubview(viewCardNumber);
            View.AddSubview(viewName);
            View.AddSubview(viewCardExpiry);
            View.AddSubview(viewCVV);
            View.AddSubview(btnNext);

            _textFieldHelper.CreateTextFieldLeftView(txtFieldCardNumber, "Card-Number");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldCardExpiry, "Card-Expiry");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldCVV, "Card-CVV");

            txtFieldCardNumber.KeyboardType = UIKeyboardType.NumberPad;
            txtFieldCardExpiry.KeyboardType = UIKeyboardType.NumberPad;
            txtFieldCVV.KeyboardType = UIKeyboardType.NumberPad;

            _textFieldHelper.CreateDoneButton(txtFieldCardNumber);
            _textFieldHelper.CreateDoneButton(txtFieldCardExpiry);
            _textFieldHelper.CreateDoneButton(txtFieldCVV);

            SetTextFieldEvents(txtFieldCardNumber, lblCardNumberTitle, lblCardNumberError
                               , viewLineCardNumber, string.Empty);
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError
                               , viewLineName, string.Empty);
            SetTextFieldEvents(txtFieldCardExpiry, lblCardExpiryTitle, lblCardExpiryError
                               , viewLineCardExpiry, string.Empty);
            SetTextFieldEvents(txtFieldCVV, lblCVVTitle, lblCVVError
                               , viewLineCVV, string.Empty);
        }

        internal void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine, string pattern)
        {
            SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                if (textField == txtFieldCardNumber)
                {
                    string formattedCardNo = FormatCard(txtFieldCardNumber.Text);
                    textField.Text = formattedCardNo;
                }
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                Console.WriteLine("Editing END");
                lblTitle.Hidden = textField.Text.Length == 0;
                if (textField == txtFieldCardNumber)
                {
                    bool isValid = ValidateCard(textField.Text.Replace(" ", ""));
                    lblError.Hidden = isValid || textField.Text.Length == 0;
                    viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                    textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
                }
                if (textField == txtFieldCardExpiry)
                {
                    bool isValid = IsValidExpiryDate();
                    lblError.Hidden = isValid || textField.Text.Length == 0;
                    viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                    textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
                }

                SetNextButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
        }

        bool IsValidExpiryDate()
        {
            string expiryDateString = txtFieldCardExpiry.Text;
            if (!expiryDateString.Contains("/"))
            {
                return false;
            }
            try
            {
                DateTime dtNow = DateTime.Now;
                string stNowString = dtNow.ToString("MM/yy");
                int currentMonth = int.Parse(stNowString.Split('/')[0]);
                int currentYear = int.Parse(stNowString.Split('/')[1]);

                int expiryMonth = int.Parse(expiryDateString.Split('/')[0]);
                int expiryYear = int.Parse(expiryDateString.Split('/')[1]);

                if (expiryYear < currentYear || expiryMonth > 12)
                {
                    return false;
                }
                if (expiryYear == currentYear)
                {
                    if (expiryMonth >= currentMonth)
                    {
                        return true;
                    }
                    return false;
                }
                if (expiryYear > currentYear)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Expiry Date Error: " + e.Message);
                return false;
            }
        }

        internal void SetNextButtonEnable()
        {
            bool isValid = false;

            bool isCardValid = ValidateCard(txtFieldCardNumber.Text.Replace(" ", ""));
            bool isNameValid = txtFieldName.Text.Length != 0;
            bool isExpiryDateValid = IsValidExpiryDate();
            bool isCVVValid = txtFieldCVV.Text.Length != 0;

            isValid = isCardValid && isNameValid && isExpiryDateValid && isCVVValid;

            btnNext.Enabled = isValid;
            btnNext.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void ExecuteRequestPayBillCall()
        {
            ActivityIndicator.Show();
            RequestMultiPayBill().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_requestPayBill != null && _requestPayBill.d != null
                       && _requestPayBill.d.data != null)
                    {
                        NavigateToVC(_requestPayBill);
                    }
                    else
                    {
                        string errMsg = "There is an error in the server, please try again.";
                        if(_requestPayBill != null && _requestPayBill.d != null && !string.IsNullOrEmpty(_requestPayBill.d.message)){
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

        /*internal Task RequestPayBill()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    customerName = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                    accNum = DataManager.DataManager.SharedInstance.BillingAccountDetails.accNum,
                    amount = _amountDue,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    phoneNo = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo != null ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : "",
                    sspUserId = DataManager.DataManager.SharedInstance.User.UserID,
                    platform = "2",
                    registeredCardId = "",
                    paymentMode = "CC"
                };
                _requestPayBill = serviceManager.RequestPayBill("RequestPayBill", requestParameter);
            });
        }*/

        internal Task RequestMultiPayBill()
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
                    platform = "2",
                    registeredCardId = "",
                    paymentMode = "CC",
                    totalAmount = Double.Parse(TotalAmount),
                    paymentItems = paymentItemList
                };
                _requestPayBill = serviceManager.RequestMultiPayBill("RequestMultiPayBill", requestParameter);
            });
        }

        internal void NavigateToVC(RequestPayBillResponseModel requestPayBillResponseModel)
        {

            UIStoryboard storyBoard = UIStoryboard.FromName("MakePayment", null);
            MakePaymentViewController makePaymentVC =
                storyBoard.InstantiateViewController("MakePaymentViewController") as MakePaymentViewController;

            var card = new CardModel();
            card.CardNo = txtFieldCardNumber.Text.Replace(" ", "");
            card.CardName = txtFieldName.Text;
            card.CardType = GetCardTypeByPreffix(card.CardNo);
            card.CardCVV = txtFieldCVV.Text;

            if (txtFieldCardExpiry.Text.Length == 5)
            {
                card.ExpiryMonth = txtFieldCardExpiry.Text.Substring(0, 2);
                card.ExpiryYear = 20 + txtFieldCardExpiry.Text.Substring(3, 2);
            }

            makePaymentVC._card = card;
            makePaymentVC._requestPayBillResponseModel = requestPayBillResponseModel;
            makePaymentVC._isNewCard = true;
            makePaymentVC._saveCardIsChecked = btnCheckBox.Selected;
            makePaymentVC._paymentMode = "CC";

            var navController = new UINavigationController(makePaymentVC);
            NavigationController.PushViewController(makePaymentVC, true);
        }

        internal string GetCardTypeByPreffix(string cardPreffix)
        {
            string cardType = "M";
            foreach (var item in cardPrefixPattern)
            {
                Regex regex = new Regex(item.Value);
                Match match = regex.Match(cardPreffix);
                if (match.Success)
                {
                    cardType = item.Key;
                }
            }
            return cardType;
        }

        internal bool LuhnVerification(string creditCardNumber)
        {
            bool isValid = false;
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return isValid;
            }
            int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                            .Reverse()
                            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                            .Sum((e) => e / 10 + e % 10);
            isValid = sumOfDigits % 10 == 0;
            Console.WriteLine("isValid: " + isValid);
            return isValid;
        }

        internal bool ValidateCard(string cardNo)
        {
            return LuhnVerification(cardNo);
        }

        internal string FormatCard(string cardNo)
        {
            string cardType = GetCardTypeByPreffix(cardNo);
            Console.WriteLine("cardType: " + cardType);
            int[] format = cardFormatPattern[cardType];
            int start = 0;
            int length = 0;
            string result = string.Empty;

            string cardNoHolder = cardNo.Replace(" ", "");
            for (int i = 0; i < format.Length && cardNoHolder.Length != 0; i++)
            {
                length = format[i];
                if (length > cardNoHolder.Length)
                {
                    length = cardNoHolder.Length;
                }
                string section = cardNoHolder.Substring(start, length);
                if (i > 0)
                {
                    result += " ";
                }
                result += section;
                cardNoHolder = cardNoHolder.Remove(start, length);
            }
            return result;
        }

        internal void VerifyCardSaveStatus()
        {
            bool isValid = ValidateCard(txtFieldCardNumber.Text.Replace(" ", ""));

            if (txtFieldCardNumber.Text == string.Empty)
            {
                Console.WriteLine("Card field is empty!");

                var alert = UIAlertController.Create("Empty card number", "Please enter valid card number and try again.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
                return;
            }

            if (isValid)
            {
                foreach (var card in _registeredCards.d.data)
                {
                    var tmpToBeSavedCardNumber = txtFieldCardNumber.Text.Replace(" ", "");
                    var toBeSavedCardExposedDigits = tmpToBeSavedCardNumber.Substring(0, 6) + tmpToBeSavedCardNumber.Substring(tmpToBeSavedCardNumber.Length - 4);
                    var cardExposeDigits = GetCardExposedDigits(card);

                    if (toBeSavedCardExposedDigits == cardExposeDigits)
                    {
                        _cardAlreadySaved = true;
                    }
                }

                if (_cardAlreadySaved)
                {
                    var alert = UIAlertController.Create("Valid card number", "Seems like you are paying with an already saved Credit / Debit Card. Do you want to continue?", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                    PresentViewController(alert, animated: true, completionHandler: null);
                    btnCheckBox.Selected = true;
                }
                else
                {
                    btnCheckBox.Selected = true;

                }
            }
            else
            {
                var alert = UIAlertController.Create("Invalid card number", "Please enter valid card number and try again.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
        }

        internal string GetCardExposedDigits(RegisteredCardsDataModel card)
        {
            RegisteredCardsDataModel tempCard = card;
            tempCard.ExposedDigits = card.LastDigits.Substring(0, 6) + card.LastDigits.Substring(card.LastDigits.Length - 4);
            Console.WriteLine(tempCard.ExposedDigits);

            return tempCard.ExposedDigits;
        }
    }
}