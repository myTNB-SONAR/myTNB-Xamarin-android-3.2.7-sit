using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.Registration.CustomerAccounts;
using UIKit;

namespace myTNB.Registration
{
    public partial class AddAccountViewController : CustomUIViewController
    {
        private UIButton btnAddAccount;

        private UILabel lblAccountNoTitle, lblAccountNoError, lblNicknameTitle, lblNicknameError
            , lblNicknameHint, lblICNoTitle, lblICNoError, lblAccountTypeTitle
            , lblAccountTypeError, lblAccountType;

        //UILabel lblMaidenNameTitle, lblMaidenNameError;

        private UIView viewAccountNo, viewNickname, viewICNo, viewAccountType, viewLineAccountNo
            , viewLineNickname, viewLineICNo, viewLineAccountType;
        //UIView viewMaidenName, viewLineMaidenName;

        private UITextField txtFieldAccountNo, txtFieldNickname, txtFieldICNo;
        //UITextField txtFieldMaidenName;

        public AddAccountViewController(IntPtr handle) : base(handle) { }

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        public bool isOwner = false;

        private string _icNo, _accountNo, _nickName, _scannedBarcode, _maidenName, _accountType;

        private List<string> _accountTypeTitleList = new List<string>();
        private List<string> _accountTypeValueList = new List<string>();

        private ValidateManualAccountLinkingResponseModel _validateManualAccountLinkingResponse
            = new ValidateManualAccountLinkingResponseModel();
        private CustomerAccountRecordModel account = new CustomerAccountRecordModel();

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            InitilizedAccountType();
            InitializedSubviews();
            AddBackButton();
            SetEvents();
            SetWidgetVisibility();
            SetViews();
            btnAddAccount.Enabled = false;
            btnAddAccount.BackgroundColor = MyTNBColor.SilverChalice;
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

        private void SetAccountType()
        {
            lblAccountType.Text = _accountTypeTitleList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex];
            //Do not delete any maiden name related codes.

            if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "1")
            {
                //viewMaidenName.Hidden = false;
                lblICNoTitle.Text = GetI18NValue(AddAccountConstants.I18N_OwnerICNumber).ToUpper();
                lblICNoError.Text = GetErrorI18NValue(Constants.Error_InvalidICNumber);
                lblNicknameHint.AttributedText = AttributedStringUtility.GetAttributedString(GetHintI18NValue(Constants.Hint_Nickname)
                    , AttributedStringUtility.AttributedStringType.Hint);
                txtFieldICNo.AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(AddAccountConstants.I18N_OwnerICNumber)
                    , AttributedStringUtility.AttributedStringType.Value);
            }
            else
            {
                //viewMaidenName.Hidden = true;
                lblICNoTitle.Text = GetI18NValue(AddAccountConstants.I18N_ROCNumber).ToUpper();
                lblICNoError.Text = GetErrorI18NValue(Constants.Error_InvalidROCNumber);
                lblNicknameHint.AttributedText = AttributedStringUtility.GetAttributedString(GetHintI18NValue(Constants.Hint_BusinessNickname)
                    , AttributedStringUtility.AttributedStringType.Hint);
                txtFieldICNo.AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(AddAccountConstants.I18N_ROCNumber)
                    , AttributedStringUtility.AttributedStringType.Value);
            }
            //viewMaidenName.Hidden = true;
        }

        private void SetOwnerFieldsVisibility()
        {
            viewICNo.Hidden = !isOwner;
            //viewMaidenName.Hidden = !isOwner;
        }

        private void SetWidgetVisibility()
        {
            lblAccountNoTitle.Hidden = string.IsNullOrEmpty(txtFieldAccountNo.Text);
            lblICNoTitle.Hidden = string.IsNullOrEmpty(txtFieldICNo.Text);
            lblNicknameTitle.Hidden = string.IsNullOrEmpty(txtFieldNickname.Text);
            //lblMaidenNameTitle.Hidden = string.IsNullOrEmpty(txtFieldMaidenName.Text);
            lblAccountTypeTitle.Hidden = string.IsNullOrEmpty(lblAccountType.Text);

            lblAccountNoError.Hidden = true;
            lblICNoError.Hidden = true;
            lblNicknameError.Hidden = true;
            lblNicknameHint.Hidden = true;
            //lblMaidenNameError.Hidden = true;
            lblAccountTypeError.Hidden = true;
        }

        private void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldAccountNo, "Account-Number");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldNickname, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");
            //_textFieldHelper.CreateTextFieldLeftView(txtFieldMaidenName, "Name");
        }

        private void SetEvents()
        {
            btnAddAccount.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _accountNo = txtFieldAccountNo.Text;
                _icNo = txtFieldICNo.Text;
                _nickName = txtFieldNickname.Text;
                _maidenName = string.Empty;//txtFieldMaidenName.Text;
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
                                DisplayGenericAlert(GetErrorI18NValue(Constants.Error_DuplicateAccountTitle)
                                    , GetErrorI18NValue(Constants.Error_DuplicateAccountMessage));
                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };

            SetTextFieldEvents(txtFieldAccountNo, lblAccountNoTitle, lblAccountNoError
                , viewLineAccountNo, TNBGlobal.ACCOUNT_NO_PATTERN);
            SetTextFieldEvents(txtFieldNickname, lblNicknameTitle, lblNicknameError
                , viewLineNickname, TNBGlobal.ACCOUNT_NAME_PATTERN);
            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError
                , viewLineICNo, TNBGlobal.IC_NO_PATTERN);
            //SetTextFieldEvents(txtFieldMaidenName, lblMaidenNameTitle
            //    , lblMaidenNameError, viewLineMaidenName, NAME_PATTERN);
        }

        private void ExecuteValidateManualAccountLinking()
        {
            ValidateAccountLinking().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_validateManualAccountLinkingResponse != null
                        && _validateManualAccountLinkingResponse.d != null
                        && _validateManualAccountLinkingResponse.d.IsSuccess)
                    {
                        account = new CustomerAccountRecordModel
                        {
                            accNum = txtFieldAccountNo.Text,
                            accDesc = txtFieldNickname.Text,
                            accountNickName = txtFieldNickname.Text,
                            accountTypeId = _accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex],
                            icNum = txtFieldICNo.Text,
                            accountStAddress = _validateManualAccountLinkingResponse.d.data.accountStAddress,
                            accountCategoryId = _validateManualAccountLinkingResponse.d.data.accountCategoryId,
                            isOwned = isOwner ? "TRUE" : "FALSE",
                            isLocal = true
                        };

                        if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList == null
                              || DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d == null)
                        {
                            DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d = new List<CustomerAccountRecordModel>();
                        }
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.Add(account);

                        ActivityIndicator.Hide();
                        foreach (UIViewController vc in NavigationController.ViewControllers)
                        {
                            if (vc is AccountsViewController)
                            {
                                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
                                NavigationController?.PopToViewController(vc, false);
                            }
                        }
                    }
                    else
                    {
                        DisplayServiceError(_validateManualAccountLinkingResponse?.d?.ErrorMessage ?? string.Empty);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private Task ValidateAccountLinking()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                //Workaround
                string icNumber = isOwner ? _icNo : string.Empty;
                string maidenName = string.Empty;//isOwner ? _maidenName : string.Empty;
                if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "2")
                {
                    maidenName = string.Empty;
                }
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    accountNum = _accountNo,
                    accountType = _accountType,
                    userIdentificationNum = icNumber,
                    suppliedMotherName = maidenName,
                    isOwner = isOwner ? "TRUE" : "FALSE"
                };
                _validateManualAccountLinkingResponse = serviceManager.OnExecuteAPIV6<ValidateManualAccountLinkingResponseModel>
                    (AddAccountConstants.Service_ValidateManualAccountLinking, requestParameter);
            });
        }

        private void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle
            , UILabel textFieldError, UIView viewLine, string pattern)
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
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.TextColor = MyTNBColor.TunaGrey();
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

                    if (!isValid)
                    {
                        string errText = !isFormatValid ? GetErrorI18NValue(Constants.Error_InvalidNickname)
                            : GetErrorI18NValue(Constants.Error_DuplicateNickname);
                        textFieldError.AttributedText = AttributedStringUtility.GetAttributedString(errText
                            , AttributedStringUtility.AttributedStringType.Error);
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
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
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
                        return _textFieldHelper.ValidateTextField(replacementString
                            , TNBGlobal.ACCOUNT_NAME_PATTERN);
                    }
                }
                else if (textField == txtFieldAccountNo)
                {
                    string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                    nint count = content.Length + replacementString.Length - range.Length;
                    return count <= TNBGlobal.AccountNumberLowCharLimit;
                }
                else if (isOwner && textField == txtFieldICNo)
                {
                    if (!string.IsNullOrEmpty(replacementString))
                    {
                        return _textFieldHelper.ValidateTextField(replacementString
                            , TNBGlobal.IC_NO_PATTERN);
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

        private void SetAddAccountButtonEnable()
        {
            bool isAccountValid = _textFieldHelper.ValidateTextField(txtFieldAccountNo.Text, TNBGlobal.ACCOUNT_NO_PATTERN)
                && _textFieldHelper.ValidateTextFieldWithLength(txtFieldAccountNo.Text, TNBGlobal.AccountNumberLowCharLimit);
            bool isAccountNameValid = !string.IsNullOrWhiteSpace(txtFieldNickname.Text)
                && _textFieldHelper.ValidateTextField(txtFieldNickname.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                && DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(txtFieldNickname.Text);
            bool isICNoValid = true;
            bool isMaidenNameValid = true;

            if (isOwner)
            {
                isICNoValid = !string.IsNullOrWhiteSpace(txtFieldICNo.Text)
                    && _textFieldHelper.ValidateTextField(txtFieldICNo.Text, TNBGlobal.IC_NO_PATTERN);
                //isMaidenNameValid = _textFieldHelper.ValidateTextField(txtFieldMaidenName.Text, NAME_PATTERN);
            }
            /*if (_accountTypeValueList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex] == "2")
            {
                isMaidenNameValid = true;
            }*/

            bool isValid = isAccountValid && isAccountNameValid && isICNoValid && isMaidenNameValid;

            btnAddAccount.Enabled = isValid;
            btnAddAccount.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController?.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void InitializedSubviews()
        {
            //Account No. 
            viewAccountNo = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 66)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblAccountNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAccountNo.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_AccountNo)
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountNo.AddSubview(lblAccountNoTitle);

            lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewAccountNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_AccountLength)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountNo.AddSubview(lblAccountNoError);

            txtFieldAccountNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewAccountNo.Frame.Width - 30, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_AccountNo)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            viewAccountNo.AddSubview(txtFieldAccountNo);

            viewLineAccountNo = GenericLine.GetLine(new CGRect(0, 36, viewAccountNo.Frame.Width, 1));
            viewAccountNo.AddSubview(viewLineAccountNo);

            UIView viewScanner = new UIView(new CGRect(viewAccountNo.Frame.Width - 30, 12, 24, 24));
            UIImageView scanner = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Scan")
            };

            UITapGestureRecognizer tapScan = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                BarcodeScannerViewController viewController =
                    storyBoard.InstantiateViewController("BarcodeScannerViewController") as BarcodeScannerViewController;
                NavigationController.PushViewController(viewController, true);
            });
            viewScanner.AddGestureRecognizer(tapScan);
            viewScanner.AddSubview(scanner);
            viewAccountNo.AddSubview(viewScanner);

            UIView viewAccountInfo = new UIView(new CGRect((viewAccountNo.Frame.Width / 4)
                , 52, (viewAccountNo.Frame.Width / 4) * 3, 16));

            UILabel lblAccountNoInfo = new UILabel
            {
                Frame = new CGRect(0, 0, (viewAccountNo.Frame.Width / 4) * 3, 16),
                AttributedText = new NSAttributedString(
                    GetI18NValue(AddAccountConstants.I18N_WhereIsMyAccountTitle),
                    font: MyTNBFont.MuseoSans14_500,
                    foregroundColor: MyTNBColor.PowerBlue,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Right
            };

            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert(GetI18NValue(AddAccountConstants.I18N_WhereIsMyAccountTitle)
                    , GetI18NValue(AddAccountConstants.I18N_WhereIsMyAccountDetails)
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , UIImage.FromBundle("Find_Account_Number"));
            });

            viewAccountInfo.AddSubview(lblAccountNoInfo);
            viewAccountInfo.AddGestureRecognizer(tapInfo);
            viewAccountNo.AddSubview(viewAccountInfo);

            //Nickname
            viewNickname = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNicknameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewNickname.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_AccountNickname)
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameTitle);

            lblNicknameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewNickname.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_InvalidNickname)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameError);

            lblNicknameHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewNickname.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetHintI18NValue(Constants.Hint_Nickname)
                    , AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left
            };
            viewNickname.AddSubview(lblNicknameHint);

            txtFieldNickname = new UITextField
            {
                Frame = new CGRect(0, 12, viewNickname.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_AccountNickname)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            viewNickname.AddSubview(txtFieldNickname);

            viewLineNickname = GenericLine.GetLine(new CGRect(0, 36, viewNickname.Frame.Width, 1));
            viewNickname.AddSubview(viewLineNickname);

            //AccountType
            viewAccountType = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblAccountTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAccountType.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_AccountType)
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountType.AddSubview(lblAccountTypeTitle);

            lblAccountTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, viewAccountType.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_InvalidAccountType)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountType.AddSubview(lblAccountTypeError);


            lblAccountType = new UILabel(new CGRect(0, 12, viewAccountType.Frame.Width, 24))
            {
                Text = _accountTypeTitleList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex],
                Font = MyTNBFont.MuseoSans18_300,
                TextColor = MyTNBColor.TunaGrey()
            };

            viewAccountType.AddSubview(lblAccountType);

            UIImageView imgDropDown = new UIImageView(new CGRect(viewAccountType.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };
            viewAccountType.AddSubview(imgDropDown);

            viewLineAccountType = GenericLine.GetLine(new CGRect(0, 36, viewAccountType.Frame.Width, 1));
            viewAccountType.AddSubview(viewLineAccountType);

            UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = GetI18NValue(AddAccountConstants.I18N_SelectAccountType);
                viewController.Items = new List<string>()
                {
                   GetI18NValue(AddAccountConstants.I18N_Residential)
                    , GetI18NValue(AddAccountConstants.I18N_Commercial)
                };
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            });
            viewAccountType.AddGestureRecognizer(tapAccounType);

            //IC No.
            viewICNo = new UIView((new CGRect(18, 217, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblICNoTitle = new UILabel(new CGRect(0, 0, viewICNo.Frame.Width, 12))
            {
                Font = MyTNBFont.MuseoSans11_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetI18NValue(AddAccountConstants.I18N_OwnerICNumber).ToUpper(),
                TextAlignment = UITextAlignment.Left
            };

            viewICNo.AddSubview(lblICNoTitle);

            lblICNoError = new UILabel(new CGRect(0, 37, viewICNo.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans11_300,
                TextColor = MyTNBColor.Tomato,
                Text = GetErrorI18NValue(Constants.Error_InvalidICNumber),
                TextAlignment = UITextAlignment.Left
            };

            viewICNo.AddSubview(lblICNoError);

            txtFieldICNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewICNo.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(AddAccountConstants.I18N_OwnerICNumber)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            viewICNo.AddSubview(txtFieldICNo);

            viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNo.Frame.Width, 1));
            viewICNo.AddSubview(viewLineICNo);

            /*
            //Maiden's name
            viewMaidenName = new UIView((new CGRect(18, 284, View.Frame.Width - 36, 51)));
            viewMaidenName.BackgroundColor = UIColor.Clear;

            lblMaidenNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMaidenName.Frame.Width, 12),
                AttributedText = new NSAttributedString("OWNER'S MOTHER'S NAME"
                    , font: myTNBFont.MuseoSans9
                    , foregroundColor: myTNBColor.SilverChalice
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left
            };
            viewMaidenName.AddSubview(lblMaidenNameTitle);

            lblMaidenNameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMaidenName.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid owner's mother's name"
                    , font: myTNBFont.MuseoSans9
                    , foregroundColor: myTNBColor.Tomato
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left
            };
            viewMaidenName.AddSubview(lblMaidenNameError);

            txtFieldMaidenName = new UITextField
            {
                Frame = new CGRect(0, 12, viewMaidenName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString("Owner’s mother’s name"
                    , font: myTNBFont.MuseoSans16
                    , foregroundColor: myTNBColor.SilverChalice
                    , strokeWidth: 0),
                TextColor = myTNBColor.TunaGrey()
            };
            viewMaidenName.AddSubview(txtFieldMaidenName);

            viewLineMaidenName = new UIView((new CGRect(0, 36, viewMaidenName.Frame.Width, 1)));
            viewLineMaidenName.BackgroundColor = myTNBColor.PlatinumGrey;
            viewMaidenName.AddSubview(viewLineMaidenName);
            */
            btnAddAccount = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 159 : 135)
                , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48))
            };
            btnAddAccount.SetTitle(GetI18NValue(AddAccountConstants.I18N_AddAccountCTATitle), UIControlState.Normal);
            btnAddAccount.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddAccount.BackgroundColor = MyTNBColor.FreshGreen;
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

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = index;
        }

        private void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        private void InitilizedAccountType()
        {
            _accountTypeTitleList.Add(GetI18NValue(AddAccountConstants.I18N_Residential));
            _accountTypeTitleList.Add(GetI18NValue(AddAccountConstants.I18N_Commercial));

            _accountTypeValueList.Add("1");
            _accountTypeValueList.Add("2");
        }
    }
}