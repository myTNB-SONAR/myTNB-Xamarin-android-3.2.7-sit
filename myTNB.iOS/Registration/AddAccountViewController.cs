using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.Registration.CustomerAccounts;
using UIKit;
using myTNB.Extensions;

namespace myTNB.Registration
{
    public partial class AddAccountViewController : UIViewController
    {
        UIButton btnAddAccount;

        UILabel lblAccountNoTitle;
        UILabel lblAccountNoError;

        UILabel lblNicknameTitle;
        UILabel lblNicknameError;
        UILabel lblNicknameHint;

        UILabel lblICNoTitle;
        UILabel lblICNoError;

        //UILabel lblMaidenNameTitle;
        //UILabel lblMaidenNameError;

        UILabel lblAccountTypeTitle;
        UILabel lblAccountTypeError;

        UIView viewAccountNo;
        UIView viewNickname;
        UIView viewICNo;
        //UIView viewMaidenName;
        UIView viewAccountType;

        UITextField txtFieldAccountNo;
        UITextField txtFieldNickname;
        UITextField txtFieldICNo;
        //UITextField txtFieldMaidenName;
        UILabel lblAccountType;

        UIView viewLineAccountNo;
        UIView viewLineNickname;
        UIView viewLineICNo;
        //UIView viewLineMaidenName;
        UIView viewLineAccountType;

        UIView viewInfoContainer;

        public AddAccountViewController(IntPtr handle) : base(handle)
        {
        }

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        AddAccountListModel _addAccountList = new AddAccountListModel();
        public bool isOwner = false;

        const string NAME_PATTERN = @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 \-\\_ _]*$";
        const string IC_NO_PATTERN = @"^[a-zA-Z0-9]+$";
        const string ACCOUNT_NO_PATTERN = @"^[0-9]{12,14}$";
        const string ACCOUNT_TYPE_PATTERN = @"/^[-@./#&+\w\s]*$/";

        string _icNo = string.Empty;
        string _accountNo = string.Empty;
        string _nickName = string.Empty;
        string _scannedBarcode = string.Empty;
        string _maidenName = string.Empty;
        string _accountType = string.Empty;

        List<String> _accountTypeTitleList = new List<String>();
        List<String> _accountTypeValueList = new List<String>();

        ValidateManualAccountLinkingResponseModel _validateManualAccountLinkingResponseModel = new ValidateManualAccountLinkingResponseModel();
        CustomerAccountRecordModel account = new CustomerAccountRecordModel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = "Add Electricity Account";
            InitilizedAccountType();
            InitializedSubviews();
            AddBackButton();
            SetEvents();
            SetWidgetVisibility();
            SetViews();
            btnAddAccount.Enabled = false;
            btnAddAccount.BackgroundColor = myTNBColor.SilverChalice();
            InitilizedAccountType();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetOwnerFieldsVisibility();
            string accountNo = DataManager.DataManager.SharedInstance.AccountNumber;
            if (!string.IsNullOrEmpty(accountNo))
            {
                txtFieldAccountNo.Text = accountNo;
                DataManager.DataManager.SharedInstance.AccountNumber = string.Empty;
            }
            SetAccountType();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        internal void SetAccountType()
        {
            lblAccountType.Text = _accountTypeTitleList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex];
            //Do not delete any maiden name related codes. W

            if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "1")
            {
                //viewMaidenName.Hidden = false;
                lblICNoTitle.Text = "OWNER'S IC NO.";
                lblICNoError.Text = "Invalid IC No.";
                lblNicknameHint.AttributedText = new NSAttributedString(
                    "AddAcctResNickNameHint".Translate(),
                    font: myTNBFont.MuseoSans11_300(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0);
                txtFieldICNo.AttributedPlaceholder = new NSAttributedString(
                    "Owner's IC No."
                    , font: myTNBFont.MuseoSans18_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                );
            }
            else
            {
                //viewMaidenName.Hidden = true;
                lblICNoTitle.Text = "ROC NO.";
                lblICNoError.Text = "Invalid ROC No.";
                lblNicknameHint.AttributedText = new NSAttributedString(
                    "AddAcctComNickNameHint".Translate(),
                    font: myTNBFont.MuseoSans11_300(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0);
                txtFieldICNo.AttributedPlaceholder = new NSAttributedString(
                    "ROC No."
                    , font: myTNBFont.MuseoSans18_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                );
            }
            //viewMaidenName.Hidden = true;
        }

        internal void SetOwnerFieldsVisibility()
        {
            viewICNo.Hidden = !isOwner;
            //viewMaidenName.Hidden = !isOwner;
        }

        internal void SetWidgetVisibility()
        {
            lblAccountNoTitle.Hidden = String.IsNullOrEmpty(txtFieldAccountNo.Text);
            lblICNoTitle.Hidden = String.IsNullOrEmpty(txtFieldICNo.Text);
            lblNicknameTitle.Hidden = String.IsNullOrEmpty(txtFieldNickname.Text);
            //lblMaidenNameTitle.Hidden = String.IsNullOrEmpty(txtFieldMaidenName.Text);
            lblAccountTypeTitle.Hidden = String.IsNullOrEmpty(lblAccountType.Text);

            lblAccountNoError.Hidden = true;
            lblICNoError.Hidden = true;
            lblNicknameError.Hidden = true;
            lblNicknameHint.Hidden = true;
            //lblMaidenNameError.Hidden = true;
            lblAccountTypeError.Hidden = true;
        }

        internal void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldAccountNo, "IC");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldNickname, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");
            //_textFieldHelper.CreateTextFieldLeftView(txtFieldMaidenName, "Name");
        }

        internal void SetEvents()
        {
            btnAddAccount.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _accountNo = txtFieldAccountNo.Text;
                _icNo = txtFieldICNo.Text;
                _nickName = txtFieldNickname.Text;
                _maidenName = "";//txtFieldMaidenName.Text;
                _accountType = _accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex];
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            string accountEntry = txtFieldAccountNo.Text;
                            int index = -1;
                            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
                            {
                                index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == accountEntry);
                            }
                            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                                && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null
                                && index == -1)
                            {
                                index = DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.FindIndex(x => x.accNum == accountEntry);
                            }
                            if (index == -1)
                            {
                                ExecuteValidateManualAccountLinking();
                            }
                            else
                            {
                                var alert = UIAlertController.Create("Duplicate Account", "This account number is already added on the list", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };

            SetTextFieldEvents(txtFieldAccountNo, lblAccountNoTitle, lblAccountNoError, viewLineAccountNo, ACCOUNT_NO_PATTERN);
            SetTextFieldEvents(txtFieldNickname, lblNicknameTitle, lblNicknameError, viewLineNickname, TNBGlobal.ACCOUNT_NAME_PATTERN);
            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError, viewLineICNo, IC_NO_PATTERN);
            //SetTextFieldEvents(txtFieldMaidenName, lblMaidenNameTitle, lblMaidenNameError, viewLineMaidenName, NAME_PATTERN);
        }

        internal void ExecuteValidateManualAccountLinking()
        {
            ValidateAccountLinking().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_validateManualAccountLinkingResponseModel.d != null
                        && _validateManualAccountLinkingResponseModel.d.isError == "false"
                        && _validateManualAccountLinkingResponseModel.d.status == "success")
                    {
                        account = new CustomerAccountRecordModel();
                        account.accNum = txtFieldAccountNo.Text;
                        account.accDesc = txtFieldNickname.Text;
                        account.accountNickName = txtFieldNickname.Text;
                        account.accountTypeId = _accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex];
                        account.icNum = txtFieldICNo.Text;
                        account.accountStAddress = _validateManualAccountLinkingResponseModel.d.data.accountStAddress;
                        account.accountCategoryId = _validateManualAccountLinkingResponseModel.d.data.accountCategoryId;
                        account.isOwned = isOwner ? "TRUE" : "FALSE";
                        account.isLocal = true;

                        if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList == null
                              || DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d == null)
                        {
                            DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d = new List<CustomerAccountRecordModel>();
                        }
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.Add(account);

                        ActivityIndicator.Hide();
                        foreach (var vc in this.NavigationController.ViewControllers)
                        {
                            if (vc is AccountsViewController)
                            {
                                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
                                this.NavigationController.PopToViewController(vc, false);
                            }
                        }
                    }
                    else
                    {
                        if (_validateManualAccountLinkingResponseModel.d != null
                             && _validateManualAccountLinkingResponseModel.d.message != null
                           )
                        {
                            //var message = "Unable to add your account. Please try again"
                            var alert = UIAlertController.Create("Unable to add account", _validateManualAccountLinkingResponseModel.d.message, UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                            ActivityIndicator.Hide();
                        }

                    }
                });
            });
        }

        internal Task ValidateAccountLinking()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                //Temporary workaround
                string icNumber = isOwner ? _icNo : "";
                string maidenName = "";//isOwner ? _maidenName : "";
                if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "2")
                {
                    maidenName = "";
                }
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accountNum = _accountNo,
                    accountType = _accountType,
                    userIdentificationNum = icNumber,
                    suppliedMotherName = maidenName,
                    isOwner = isOwner ? "TRUE" : "FALSE"
                };
                _validateManualAccountLinkingResponseModel = serviceManager.ValidateManualAccountLinking("ValidateManualAccountLinking", requestParameter);
            });
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UIView viewLine, string pattern)
        {
            textField.EditingChanged += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;

                if (sender == txtFieldNickname)
                {
                    // remove auto period on consecutive space
                    int index = txtFieldNickname.Text.IndexOf(". ", StringComparison.InvariantCulture);
                    if (index >= 0)
                    {
                        System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder(txtFieldNickname.Text);
                        myStringBuilder.Replace(". ", "  ", index, 2);
                        txtFieldNickname.Text = myStringBuilder.ToString();
                        
                    }
                }
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
                textField.TextColor = myTNBColor.TunaGrey();
                if (sender == txtFieldNickname)
                {
                    lblNicknameHint.Hidden = false;
                    lblNicknameError.Hidden = true;
                }
            };
            textField.ShouldEndEditing = (sender) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                if (textField == txtFieldAccountNo)
                {
                    isValid = isValid && _textFieldHelper.ValidateTextFieldWithLength(textField.Text, TNBGlobal.AccountNumberLowCharLimit);
                }
                else if (textField == txtFieldNickname)
                {
                    bool isFormatValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                    bool isUnique = DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(textField.Text);

                    isValid = isFormatValid && isUnique;

                    if(!isValid)
                    {
                        var errText = !isFormatValid ? "AddAcctNicknameInvalidError".Translate() : "AcctNicknameInUseError".Translate();
                        textFieldError.AttributedText = new NSAttributedString(                                         errText,                                            font: myTNBFont.MuseoSans11_300(),                                         foregroundColor: myTNBColor.Tomato(),                                         strokeWidth: 0                                         );
                    }
                }
                else if (isOwner && textField == txtFieldICNo)
                {
                    isValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                }

                textFieldError.Hidden = isValid;
                if (!textFieldError.Hidden)
                {
                    lblNicknameHint.Hidden = true;
                }

                viewLine.BackgroundColor = isValid ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
                SetAddAccountButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters = (txtField, range, replacementString) =>
            {
                if (textField == txtFieldNickname)
                {
                    if (!string.IsNullOrEmpty(replacementString))
                    {
                        return _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.ACCOUNT_NAME_PATTERN);
                    }
                }
                else if (textField == txtFieldAccountNo)
                {
                    string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                    var count = content.Length + replacementString.Length - range.Length;
                    return count <= TNBGlobal.AccountNumberLowCharLimit;
                }
                else if (isOwner && textField == txtFieldICNo)
                {
                    if (!string.IsNullOrEmpty(replacementString))
                    {
                        return _textFieldHelper.ValidateTextField(replacementString, IC_NO_PATTERN);
                    }
                }

                return true;
            };

            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;

                if (sender == txtFieldNickname)
                    lblNicknameHint.Hidden = true;
            };
        }

        internal void SetAddAccountButtonEnable()
        {
            bool isAccountValid = _textFieldHelper.ValidateTextField(txtFieldAccountNo.Text, ACCOUNT_NO_PATTERN) && _textFieldHelper.ValidateTextFieldWithLength(txtFieldAccountNo.Text, TNBGlobal.AccountNumberLowCharLimit);
            bool isAccountNameValid = !string.IsNullOrWhiteSpace(txtFieldNickname.Text) 
                                             && _textFieldHelper.ValidateTextField(txtFieldNickname.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                                             && DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(txtFieldNickname.Text);
            bool isICNoValid = true;
            bool isMaidenNameValid = true;

            if (isOwner)
            {
                isICNoValid = !string.IsNullOrWhiteSpace(txtFieldICNo.Text) && _textFieldHelper.ValidateTextField(txtFieldICNo.Text, IC_NO_PATTERN);
                //isMaidenNameValid = _textFieldHelper.ValidateTextField(txtFieldMaidenName.Text, NAME_PATTERN);
            }
            /*if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "2")
            {
                isMaidenNameValid = true;
            }*/

            bool isValid = isAccountValid && isAccountNameValid && isICNoValid && isMaidenNameValid;

            btnAddAccount.Enabled = isValid;
            btnAddAccount.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.NavigationController.PopViewController(true);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void InitializedSubviews()
        {
            //Account No. 
            viewAccountNo = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 66)));
            viewAccountNo.BackgroundColor = UIColor.Clear;

            lblAccountNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAccountNo.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "ACCOUNT NO.",
                    font: myTNBFont.MuseoSans11_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountNo.AddSubview(lblAccountNoTitle);

            lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewAccountNo.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "ErrInvalidAcct".Translate(),
                                           font: myTNBFont.MuseoSans11_300(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountNo.AddSubview(lblAccountNoError);

            txtFieldAccountNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewAccountNo.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Account No.",
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewAccountNo.AddSubview(txtFieldAccountNo);

            viewLineAccountNo = new UIView((new CGRect(0, 36, viewAccountNo.Frame.Width, 1)));
            viewLineAccountNo.BackgroundColor = myTNBColor.PlatinumGrey();
            viewAccountNo.AddSubview(viewLineAccountNo);

            UIView viewScanner = new UIView(new CGRect(viewAccountNo.Frame.Width - 30, 12, 24, 24));
            UIImageView scanner = new UIImageView(new CGRect(0, 0, 24, 24));
            scanner.Image = UIImage.FromBundle("Scan");

            UITapGestureRecognizer tapScan = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                BarcodeScannerViewController viewController =
                    storyBoard.InstantiateViewController("BarcodeScannerViewController") as BarcodeScannerViewController;
                this.NavigationController.PushViewController(viewController, true);
            });
            viewScanner.AddGestureRecognizer(tapScan);
            viewScanner.AddSubview(scanner);
            viewAccountNo.AddSubview(viewScanner);

            UIView viewAccountInfo = new UIView(new CGRect((viewAccountNo.Frame.Width / 4)
                                                           , 52
                                                           , (viewAccountNo.Frame.Width / 4) * 3
                                                           , 16));

            UILabel lblAccountNoInfo = new UILabel
            {
                Frame = new CGRect(0, 0, (viewAccountNo.Frame.Width / 4) * 3, 16),
                AttributedText = new NSAttributedString(
                    "Where is my account no.?",
                    font: myTNBFont.MuseoSans14_500(),
                    foregroundColor: myTNBColor.PowerBlue(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Right
            };

            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                if (viewInfoContainer == null)
                {
                    viewInfoContainer = new UIView(UIScreen.MainScreen.Bounds);
                    viewInfoContainer.BackgroundColor = new UIColor(0, .75F);
                    viewInfoContainer.Hidden = true;

                    UIView viewInfo = new UIView(new CGRect(18
                                                            , (viewInfoContainer.Frame.Height / 2) - 147
                                                            , viewInfoContainer.Frame.Width - 36
                                                            , 294));
                    viewInfo.BackgroundColor = UIColor.White;
                    viewInfo.Alpha = 1F;
                    viewInfo.Layer.CornerRadius = 4;

                    UIImageView imgInfo = new UIImageView(new CGRect(0, 0, viewInfo.Frame.Width, 120));
                    imgInfo.Image = UIImage.FromBundle("Find_Account_Number");

                    UILabel lblTitle = new UILabel(new CGRect(16, 137, viewInfo.Frame.Width - 32, 20));
                    lblTitle.TextAlignment = UITextAlignment.Left;
                    lblTitle.TextColor = myTNBColor.TunaGrey();
                    lblTitle.Font = myTNBFont.MuseoSans14_500();
                    lblTitle.Text = "Where’s my account no.?";

                    UILabel lblDetails = new UILabel(new CGRect(16, 162, viewInfo.Frame.Width - 32, 54));
                    lblDetails.TextAlignment = UITextAlignment.Left;
                    lblDetails.TextColor = myTNBColor.TunaGrey();
                    lblDetails.Font = myTNBFont.MuseoSans14_500();
                    lblDetails.Lines = 0;
                    lblDetails.LineBreakMode = UILineBreakMode.WordWrap;
                    lblDetails.Text = "Your 12-digit account no. can be found on the top left corner of your monthly paper bill.";

                    UIButton btnDismiss = new UIButton(UIButtonType.Custom);
                    btnDismiss.Frame = new CGRect(0, viewInfo.Frame.Height - 30, viewInfo.Frame.Width, 20);
                    btnDismiss.SetTitle("Got It!", UIControlState.Normal);
                    btnDismiss.SetTitleColor(myTNBColor.PowerBlue(), UIControlState.Normal);
                    btnDismiss.Font = myTNBFont.MuseoSans16_500();
                    btnDismiss.TouchUpInside += (sender, e) =>
                    {
                        viewInfoContainer.Hidden = true;
                    };
                    viewInfo.AddSubviews(new UIView[] { imgInfo, lblTitle, lblDetails, btnDismiss });

                    viewInfoContainer.AddSubview(viewInfo);
                    UIApplication.SharedApplication.KeyWindow.AddSubview(viewInfoContainer);
                }
                viewInfoContainer.Hidden = false;
            });

            viewAccountInfo.AddSubview(lblAccountNoInfo);
            viewAccountInfo.AddGestureRecognizer(tapInfo);
            viewAccountNo.AddSubview(viewAccountInfo);

            //Nickname
            viewNickname = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)));
            viewNickname.BackgroundColor = UIColor.Clear;

            lblNicknameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewNickname.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "ACCOUNT NICKNAME",
                                                       font: myTNBFont.MuseoSans11_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameTitle);

            lblNicknameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewNickname.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "AddAcctNicknameInvalidError".Translate(),
                                           font: myTNBFont.MuseoSans11_300(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameError);

            lblNicknameHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewNickname.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "AddAcctComNickNameHint".Translate(),
                                           font: myTNBFont.MuseoSans11_300(),
                                            foregroundColor: myTNBColor.TunaGrey(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameHint);

            txtFieldNickname = new UITextField
            {
                Frame = new CGRect(0, 12, viewNickname.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Account Nickname",
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewNickname.AddSubview(txtFieldNickname);

            viewLineNickname = new UIView((new CGRect(0, 36, viewNickname.Frame.Width, 1)));
            viewLineNickname.BackgroundColor = myTNBColor.PlatinumGrey();
            viewNickname.AddSubview(viewLineNickname);

            //AccountType
            viewAccountType = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)));
            viewAccountType.BackgroundColor = UIColor.Clear;

            lblAccountTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAccountType.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "ACCOUNT TYPE",
                                                       font: myTNBFont.MuseoSans11_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountType.AddSubview(lblAccountTypeTitle);

            lblAccountTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, viewAccountType.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                            "Invalid Account Type",
                                               font: myTNBFont.MuseoSans11_300(),
                                            foregroundColor: myTNBColor.Tomato(),
                                            strokeWidth: 0
                                           ),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountType.AddSubview(lblAccountTypeError);


            lblAccountType = new UILabel(new CGRect(0, 12, viewAccountType.Frame.Width, 24));
            lblAccountType.Text = _accountTypeTitleList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex];
            lblAccountType.Font = myTNBFont.MuseoSans18_300();
            lblAccountType.TextColor = myTNBColor.TunaGrey();

            viewAccountType.AddSubview(lblAccountType);

            UIImageView imgDropDown = new UIImageView(new CGRect(viewAccountType.Frame.Width - 30, 12, 24, 24));
            imgDropDown.Image = UIImage.FromBundle("IC-Action-Dropdown");
            viewAccountType.AddSubview(imgDropDown);

            viewLineAccountType = new UIView((new CGRect(0, 36, viewAccountType.Frame.Width, 1)));
            viewLineAccountType.BackgroundColor = myTNBColor.PlatinumGrey();
            viewAccountType.AddSubview(viewLineAccountType);

            UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                SelectAccountTypeViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTypeViewController") as SelectAccountTypeViewController;
                this.NavigationController.PushViewController(viewController, true);
            });
            viewAccountType.AddGestureRecognizer(tapAccounType);

            //IC No.
            viewICNo = new UIView((new CGRect(18, 217, View.Frame.Width - 36, 51)));
            viewICNo.BackgroundColor = UIColor.Clear;

            lblICNoTitle = new UILabel(new CGRect(0, 0, viewICNo.Frame.Width, 12));
            lblICNoTitle.Font = myTNBFont.MuseoSans11_300();
            lblICNoTitle.TextColor = myTNBColor.SilverChalice();
            lblICNoTitle.Text = "OWNER'S IC NO.";
            lblICNoTitle.TextAlignment = UITextAlignment.Left;

            viewICNo.AddSubview(lblICNoTitle);

            lblICNoError = new UILabel(new CGRect(0, 37, viewICNo.Frame.Width, 14));
            lblICNoError.Font = myTNBFont.MuseoSans11_300();
            lblICNoError.TextColor = myTNBColor.Tomato();
            lblICNoError.Text = "Invalid IC No.";
            lblICNoError.TextAlignment = UITextAlignment.Left;

            viewICNo.AddSubview(lblICNoError);

            txtFieldICNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewICNo.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Owner's IC No.",
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewICNo.AddSubview(txtFieldICNo);

            viewLineICNo = new UIView((new CGRect(0, 36, viewICNo.Frame.Width, 1)));
            viewLineICNo.BackgroundColor = myTNBColor.PlatinumGrey();
            viewICNo.AddSubview(viewLineICNo);

            /*
            //Maiden's name
            viewMaidenName = new UIView((new CGRect(18, 284, View.Frame.Width - 36, 51)));
            viewMaidenName.BackgroundColor = UIColor.Clear;

            lblMaidenNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMaidenName.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "OWNER'S MOTHER'S NAME",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            viewMaidenName.AddSubview(lblMaidenNameTitle);

            lblMaidenNameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMaidenName.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid owner's mother's name",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            viewMaidenName.AddSubview(lblMaidenNameError);

            txtFieldMaidenName = new UITextField
            {
                Frame = new CGRect(0, 12, viewMaidenName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Owner’s mother’s name",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewMaidenName.AddSubview(txtFieldMaidenName);

            viewLineMaidenName = new UIView((new CGRect(0, 36, viewMaidenName.Frame.Width, 1)));
            viewLineMaidenName.BackgroundColor = myTNBColor.PlatinumGrey();
            viewMaidenName.AddSubview(viewLineMaidenName);
            */
            btnAddAccount = new UIButton(UIButtonType.Custom);
            btnAddAccount.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 159 : 135), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnAddAccount.SetTitle("Add Account", UIControlState.Normal);
            btnAddAccount.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddAccount.BackgroundColor = myTNBColor.FreshGreen();
            btnAddAccount.Layer.CornerRadius = 4.0f;

            View.AddSubview(viewAccountNo);
            View.AddSubview(viewNickname);
            View.AddSubview(viewICNo);
            //View.AddSubview(viewMaidenName);
            View.AddSubview(viewAccountType);
            View.AddSubview(btnAddAccount);

            SetKeyboard(txtFieldICNo);
            SetKeyboard(txtFieldNickname);
            SetKeyboard(txtFieldAccountNo);
            txtFieldAccountNo.KeyboardType = UIKeyboardType.NumberPad;
            //SetKeyboard(txtFieldMaidenName);
            _textFieldHelper.CreateDoneButton(txtFieldAccountNo);
        }

        internal void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        internal void InitilizedAccountType()
        {
            _accountTypeTitleList.Add("Residential");
            _accountTypeTitleList.Add("Commercial");

            _accountTypeValueList.Add("1");
            _accountTypeValueList.Add("2");
        }
    }
}