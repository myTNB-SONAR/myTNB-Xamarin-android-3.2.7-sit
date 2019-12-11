using System;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Foundation;
using System.Globalization;
using System.Diagnostics;
using myTNB.Payment;
using myTNB.Payment.AddCard;

namespace myTNB
{
    public partial class AddCardViewController : CustomUIViewController
    {
        public AddCardViewController(IntPtr handle) : base(handle) { }

        private UIScrollView ScrollView;
        private CGRect scrollViewFrame;

        private UILabel lblCardNumberTitle, lblNameTitle, lblCardExpiryTitle, lblCVVTitle
            , lblCardNumberError, lblNameError, lblCardExpiryError, lblCVVError;
        private UITextField txtFieldCardNumber, txtFieldName, txtFieldCardExpiry, txtFieldCVV;
        private UIView viewLineCardNumber, viewLineName, viewLineCardExpiry, viewLineCVV;
        private UIButton btnNext, btnCheckBox;
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private bool _cardAlreadySaved = false;

        public double _amountDue = 0;
        public RegisteredCardsResponseModel _registeredCards;
        public List<CustomerAccountRecordModel> AccountsForPayment = new List<CustomerAccountRecordModel>();
        public double TotalAmount = 0.00;

        private Dictionary<string, int[]> cardFormatPattern = new Dictionary<string, int[]>
        {
            {"V", new int[] { 4, 4, 4, 4 }},
            {"M", new int[] { 4, 4, 4, 4 }},
            {"A", new int[] { 4, 6, 5}}
        };

        private Dictionary<string, string> cardPrefixPattern = new Dictionary<string, string>
        {
            {"V", @"^$|^4.*"},
            {"M", @"^$|^5.*"},
            {"A", @"^$|^3$|^3[47].*"}
        };

        private Dictionary<string, string> cardValidationPattern = new Dictionary<string, string>
        {
            {"V", @"^4[0-9]{15}$"},
            {"M", @"^5[0-9]{15}$"},
            {"A", @"^3[47][0-9]{13}$"}
        };

        private const int MinCvvLength = 3;
        private const int MaxCvvLength = 4;

        public override void ViewDidLoad()
        {
            PageName = AddCardConstants.Pagename_AddCard;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            SetNavigationItems();
            SetSubviews();

            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
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

        private void SetNavigationItems()
        {
            NavigationItem.HidesBackButton = true;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(false);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(AddCardConstants.I18N_Title);
        }

        private void SetSubviews()
        {
            // setup scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(ScrollView);

            //Scrollview content size
            ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, UIScreen.MainScreen.Bounds.Height).Size;

            scrollViewFrame = ScrollView.Frame;

            UILabel lblDescription = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 36))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = GetI18NValue(AddCardConstants.I18N_AcceptedCardsMessage),
                TextAlignment = UITextAlignment.Left
            };
            ScrollView.AddSubview(lblDescription);

            //Credit Card Number 
            UIView viewCardNumber = new UIView((new CGRect(18, 68, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblCardNumberTitle = new UILabel(new CGRect(0, 0, viewCardNumber.Frame.Width, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_CardNumber),
                Hidden = true
            };
            viewCardNumber.AddSubview(lblCardNumberTitle);

            lblCardNumberError = new UILabel(new CGRect(0, 37, viewCardNumber.Frame.Width, 14))
            {
                TextColor = MyTNBColor.Tomato,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_InvalidCardNumber),
                Hidden = true
            };
            viewCardNumber.AddSubview(lblCardNumberError);

            txtFieldCardNumber = new UITextField(new CGRect(0, 12, viewCardNumber.Frame.Width - 30, 24))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300,
                TextAlignment = UITextAlignment.Left,
                Placeholder = GetI18NValue(AddCardConstants.I18N_CardNumber)
            };
            viewCardNumber.AddSubview(txtFieldCardNumber);

            viewLineCardNumber = new UIView((new CGRect(0, 36, viewCardNumber.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            UIView viewScanner = new UIView(new CGRect(viewCardNumber.Frame.Width - 30, 12, 24, 24));
            UIImageView scanner = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Camera")
            };

            UITapGestureRecognizer tapScan = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("AddCard", null);
                CreditCardScannerViewController viewController =
                    storyBoard.InstantiateViewController("CreditCardScannerViewController") as CreditCardScannerViewController;
                NavigationController.PushViewController(viewController, true);
            });
            viewScanner.AddGestureRecognizer(tapScan);
            viewScanner.AddSubview(scanner);
            viewCardNumber.AddSubview(viewScanner);

            viewCardNumber.AddSubview(viewLineCardNumber);

            //Name 
            UIView viewName = new UIView((new CGRect(18, 128, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNameTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_NameOnCard).ToUpper(),
                Hidden = true
            };
            viewName.AddSubview(lblNameTitle);

            lblNameError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14))
            {
                TextColor = MyTNBColor.Tomato,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_InvalidName),
                Hidden = true
            };
            viewName.AddSubview(lblNameError);

            txtFieldName = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300,
                TextAlignment = UITextAlignment.Left,
                Placeholder = GetI18NValue(AddCardConstants.I18N_NameOnCard)
            };
            viewName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            viewName.AddSubview(viewLineName);

            //Card Expiry 
            UIView viewCardExpiry = new UIView((new CGRect(18, 185, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblCardExpiryTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_CardExpiry).ToUpper(),
                Hidden = true
            };
            viewCardExpiry.AddSubview(lblCardExpiryTitle);

            lblCardExpiryError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14))
            {
                TextColor = MyTNBColor.Tomato,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_InvalidCardExpiry),
                Hidden = true
            };
            viewCardExpiry.AddSubview(lblCardExpiryError);

            txtFieldCardExpiry = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300,
                TextAlignment = UITextAlignment.Left,
                Placeholder = GetI18NValue(AddCardConstants.I18N_HintCardExpiry)
            };
            viewCardExpiry.AddSubview(txtFieldCardExpiry);

            txtFieldCardExpiry.AddTarget((sender, e) =>
            {
                UITextField theTextField = (UITextField)sender;
                string textVal = theTextField.Text;

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
                UITextField theTextField = (UITextField)textField;
                string textVal = theTextField.Text;

                if (theTextField.Text.Length == 5 && range.Length == 0)
                {
                    return false;
                }
                else
                {
                    if (replacement == string.Empty && theTextField.Text.Length == 3)
                    {
                        theTextField.Text = textVal.Replace("/", string.Empty);
                    }
                    return true;
                }
            };

            viewLineCardExpiry = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            viewCardExpiry.AddSubview(viewLineCardExpiry);

            //CVV 
            UIView viewCVV = new UIView((new CGRect(18, 242, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblCVVTitle = new UILabel(new CGRect(0, 0, viewName.Frame.Width, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_CVV).ToUpper(),
                Hidden = true
            };
            viewCVV.AddSubview(lblCVVTitle);

            lblCVVError = new UILabel(new CGRect(0, 37, viewName.Frame.Width, 14))
            {
                TextColor = MyTNBColor.Tomato,
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(AddCardConstants.I18N_InvalidCVV),
                Hidden = true
            };
            viewCVV.AddSubview(lblCVVError);

            txtFieldCVV = new UITextField(new CGRect(0, 12, viewName.Frame.Width, 24))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300,
                TextAlignment = UITextAlignment.Left,
                Placeholder = GetI18NValue(AddCardConstants.I18N_CVV)
            };
            viewCVV.AddSubview(txtFieldCVV);

            txtFieldCVV.ShouldChangeCharacters += (textField, range, replacement) =>
            {
                UITextField theTextField = (UITextField)textField;
                return !(theTextField.Text.Length == MaxCvvLength && range.Length == 0);
            };

            viewLineCVV = new UIView((new CGRect(0, 36, viewName.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            viewCVV.AddSubview(viewLineCVV);

            btnCheckBox = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, 320, 24, 24),
                BackgroundColor = UIColor.Clear
            };
            btnCheckBox.SetImage(UIImage.FromBundle("Payment-Checkbox-Inactive"), UIControlState.Normal);
            btnCheckBox.SetImage(UIImage.FromBundle("Payment-Checkbox-Active"), UIControlState.Selected);
            btnCheckBox.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            btnCheckBox.Layer.CornerRadius = 5.0f;
            ScrollView.AddSubview(btnCheckBox);
            //btnCheckBox.Hidden = true;

            btnCheckBox.TouchUpInside += (sender, e) =>
            {
                UIButton btn = ((UIButton)sender);
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

            UILabel lblCheckBoxTitle = new UILabel(new CGRect(51, 324, View.Frame.Width - 69, 18));
            lblCheckBoxTitle.TextColor = MyTNBColor.TunaGrey();
            lblCheckBoxTitle.Font = MyTNBFont.MuseoSans14;
            lblCheckBoxTitle.TextAlignment = UITextAlignment.Left;
            lblCheckBoxTitle.Text = GetI18NValue(AddCardConstants.I18N_SaveCardMessage);
            ScrollView.AddSubview(lblCheckBoxTitle);
            //lblCheckBoxTitle.Hidden = true;

            btnNext = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(133)
                    , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                Font = MyTNBFont.MuseoSans16_500,
                BackgroundColor = MyTNBColor.SilverChalice,
                Enabled = false
            };
            btnNext.SetTitle(GetCommonI18NValue(Constants.Common_Next), UIControlState.Normal);
            btnNext.Layer.CornerRadius = 5.0f;
            btnNext.TouchUpInside += (sender, e) =>
            {
                RemoveCachedAccountRecords();
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
                            DisplayNoDataAlert();
                        }
                    });
                });
            };

            ScrollView.AddSubview(viewCardNumber);
            ScrollView.AddSubview(viewName);
            ScrollView.AddSubview(viewCardExpiry);
            ScrollView.AddSubview(viewCVV);
            ScrollView.AddSubview(btnNext);

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

        private void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle
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
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = textField.Text?.Length > 0;
                if (textField == txtFieldCardNumber)
                {
                    isValid = isValid && ValidateCard(textField.Text.Replace(" ", string.Empty));
                }
                else if (textField == txtFieldName)
                {
                    isValid = isValid && IsValidName(textField.Text);
                }
                else if (textField == txtFieldCardExpiry)
                {
                    isValid = isValid && IsValidExpiryDate();
                }
                else if (textField == txtFieldCVV)
                {
                    isValid = isValid && IsValidCVV(textField.Text);
                }

                lblError.Hidden = isValid;
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

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
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
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
                Debug.WriteLine("Expiry Date Error: " + e.Message);
                return false;
            }
        }

        private void SetNextButtonEnable()
        {
            bool isCardValid = ValidateCard(txtFieldCardNumber.Text.Replace(" ", string.Empty));
            bool isNameValid = IsValidName(txtFieldName.Text);
            bool isExpiryDateValid = IsValidExpiryDate();
            bool isCVVValid = IsValidCVV(txtFieldCVV.Text);
            bool isValid = isCardValid && isNameValid && isExpiryDateValid && isCVVValid;
            btnNext.Enabled = isValid;
            btnNext.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void ExecuteRequestPayBillCall()
        {
            ActivityIndicator.Show();
            InvokeOnMainThread(async () =>
            {
                GetPaymentTransactionIdResponseModel _paymentTransaction = await GetPaymentTransactionId();
                if (_paymentTransaction != null && _paymentTransaction.d != null && _paymentTransaction.d.IsSuccess
                    && _paymentTransaction.d.data != null)
                {
                    Debug.WriteLine("Success");
                    NavigateToVC(_paymentTransaction);
                }
                else
                {
                    DisplayServiceError(_paymentTransaction?.d?.DisplayMessage ?? string.Empty);
                }
                ActivityIndicator.Hide();
            });
        }

        private void NavigateToVC(GetPaymentTransactionIdResponseModel paymentTransactionIDResponseModel)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("MakePayment", null);
            MakePaymentViewController makePaymentVC =
                storyBoard.InstantiateViewController("MakePaymentViewController") as MakePaymentViewController;

            CardModel card = new CardModel
            {
                CardNo = txtFieldCardNumber.Text.Replace(" ", string.Empty),
                CardName = txtFieldName.Text
            };
            card.CardType = GetCardTypeByPreffix(card.CardNo);
            card.CardCVV = txtFieldCVV.Text;

            if (txtFieldCardExpiry.Text.Length == 5)
            {
                card.ExpiryMonth = txtFieldCardExpiry.Text.Substring(0, 2);
                card.ExpiryYear = 20 + txtFieldCardExpiry.Text.Substring(3, 2);
            }
            if (makePaymentVC != null)
            {
                makePaymentVC._card = card;
                makePaymentVC._paymentTransactionIDResponseModel = paymentTransactionIDResponseModel;
                makePaymentVC._isNewCard = true;
                makePaymentVC._saveCardIsChecked = btnCheckBox.Selected;
                makePaymentVC._paymentMode = "CC";
                NavigationController.PushViewController(makePaymentVC, true);
            }
        }

        private string GetCardTypeByPreffix(string cardPreffix)
        {
            string cardType = "M";
            foreach (KeyValuePair<string, string> item in cardPrefixPattern)
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

        private bool LuhnVerification(string creditCardNumber)
        {
            bool isValid = false;
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return isValid;
            }
            int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                .Reverse().Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                .Sum((e) => e / 10 + e % 10);
            isValid = sumOfDigits % 10 == 0;
            Debug.WriteLine("Luhn Validation isValid: " + isValid);
            return isValid;
        }

        private bool ValidateCard(string cardNo)
        {
            return LuhnVerification(cardNo);
        }

        /// <summary>
        /// Checks if CVV is valid
        /// </summary>
        /// <returns><c>true</c>, if valid cvv was ised, <c>false</c> otherwise.</returns>
        /// <param name="cvv">Cvv.</param>
        private bool IsValidCVV(string cvv)
        {
            if (!string.IsNullOrWhiteSpace(cvv))
            {
                return _textFieldHelper.ValidateTextField(cvv
                    , TNBGlobal.NumbersOnlyPattern) && cvv.Length >= MinCvvLength;
            }
            return false;
        }

        /// <summary>
        /// Checks if name is valid
        /// </summary>
        /// <returns><c>true</c>, if valid name was ised, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        private bool IsValidName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name.Length > 0;
            }
            return false;
        }

        private string FormatCard(string cardNo)
        {
            string cardType = GetCardTypeByPreffix(cardNo);
            Debug.WriteLine("cardType: " + cardType);
            int[] format = cardFormatPattern[cardType];
            int start = 0;
            int length = 0;
            string result = string.Empty;

            string cardNoHolder = cardNo.Replace(" ", string.Empty);
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

        private void VerifyCardSaveStatus()
        {
            bool isValid = ValidateCard(txtFieldCardNumber.Text.Replace(" ", string.Empty));

            if (txtFieldCardNumber.Text == string.Empty)
            {
                DisplayGenericAlert(GetI18NValue(AddCardConstants.I18N_EmptyCardNumberTitle)
                    , GetI18NValue(AddCardConstants.I18N_EmptyCardNumberMessage));
                return;
            }

            if (isValid)
            {
                foreach (RegisteredCardsDataModel card in _registeredCards.d.data)
                {
                    string tmpToBeSavedCardNumber = txtFieldCardNumber.Text.Replace(" ", string.Empty);
                    string toBeSavedCardExposedDigits = tmpToBeSavedCardNumber.Substring(0, 6)
                        + tmpToBeSavedCardNumber.Substring(tmpToBeSavedCardNumber.Length - 4);
                    string cardExposeDigits = GetCardExposedDigits(card);

                    if (toBeSavedCardExposedDigits == cardExposeDigits)
                    {
                        _cardAlreadySaved = true;
                    }
                }

                if (_cardAlreadySaved)
                {
                    DisplayGenericAlert(GetI18NValue(AddCardConstants.I18N_SavedCardTitle)
                        , GetI18NValue(AddCardConstants.I18N_SavedCardMessage));
                }
                btnCheckBox.Selected = true;
            }
            else
            {
                DisplayGenericAlert(GetI18NValue(AddCardConstants.I18N_EmptyCardNumberTitle)
                    , GetI18NValue(AddCardConstants.I18N_EmptyCardNumberMessage));
            }
        }

        private string GetCardExposedDigits(RegisteredCardsDataModel card)
        {
            RegisteredCardsDataModel tempCard = card;
            tempCard.ExposedDigits = card.LastDigits.Substring(0, 6) + card.LastDigits.Substring(card.LastDigits.Length - 4);
            return tempCard.ExposedDigits;
        }

        /// <summary>
        /// Handles the keyboard notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded) { return; }

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
                ScrollView.Frame = new CGRect(ScrollView.Frame.X, ScrollView.Frame.Y, ScrollView.Frame.Width, currentViewHeight);
            }
            else
            {
                ScrollView.Frame = scrollViewFrame;
            }
            UIView.CommitAnimations();
        }

        private async Task<GetPaymentTransactionIdResponseModel> GetPaymentTransactionId(string platform = "2", string paymentMode = "CC")
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
                registeredCardId = string.Empty,
                paymentMode,
                totalAmount = TotalAmount,
                paymentItems
            };
            GetPaymentTransactionIdResponseModel response = serviceManager.OnExecuteAPIV6<GetPaymentTransactionIdResponseModel>(AddCardConstants.Service_GetPaymentTransactionId, request);
            return response;
        }
    }
}