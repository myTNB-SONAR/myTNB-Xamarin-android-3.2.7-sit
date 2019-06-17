using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.DataManager;
using myTNB.Registration;

namespace myTNB
{
    public partial class UpdateMobileNumberViewController : CustomUIViewController
    {
        public UpdateMobileNumberViewController(IntPtr handle) : base(handle)
        {
        }
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";
        UILabel lblMobileNoTitle;
        UILabel lblMobileNoError;
        UILabel lblMobileNoHint;
        UITextField txtFieldMobileNo;
        UIView viewLineMobileNo;
        UIButton btnSave;

        BaseResponseModel _saveResponse = new BaseResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        string _mobileNo = string.Empty;

        public bool WillHideBackButton = false;
        public bool IsFromLogin = false;
        string _navTitle;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            AddSaveButton();
            SetSubviews();

            if (IsFromLogin)
            {
                DisplayToast("Manage_UpdateMobileNumberToast".Translate());
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.Hidden = true;

            string mobileNo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                                         ? _textFieldHelper.TrimAllSpaces(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo)
                                         : string.Empty;
            txtFieldMobileNo.Text = _textFieldHelper.FormatMobileNo(mobileNo);
            SetVisibility();
            SetSaveButtonEnable();
        }

        internal void SetSubviews()
        {
            nfloat marginY = 16;
            UILabel info;
            UIView viewMobileNumber;
            if (IsFromLogin)
            {
                info = new UILabel
                {
                    Frame = new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 36),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = "Manage_VerifyMobileNumber".Translate()
                };
                View.AddSubview(info);
                viewMobileNumber = new UIView((new CGRect(18, info.Frame.GetMaxY() + marginY, View.Frame.Width - 36, 51)));
            }
            else
            {
                viewMobileNumber = new UIView((new CGRect(18, (DeviceHelper.IsIphoneXUpResolution() ? 104 : 80), View.Frame.Width - 36, 51)));
            }

            viewMobileNumber.BackgroundColor = UIColor.Clear;

            lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMobileNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString("Common_MobileNumber".Translate().ToUpper()
                                                        , font: MyTNBFont.MuseoSans9_300
                                                        , foregroundColor: MyTNBColor.SilverChalice
                                                        , strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoTitle.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoTitle);

            lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid_MobileNumber".Translate()
                                                        , font: MyTNBFont.MuseoSans9_300
                                                        , foregroundColor: MyTNBColor.Tomato
                                                        , strokeWidth: 0
                                                       ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoError.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoError);

            lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Hint_MobileNumberExample".Translate(),
                    font: MyTNBFont.MuseoSans9_300,
                    foregroundColor: MyTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoHint.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoHint);

            txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString("Common_MobileNumber".Translate()
                                                               , font: MyTNBFont.MuseoSans16_300
                                                               , foregroundColor: MyTNBColor.SilverChalice
                                                               , strokeWidth: 0
                                                              ),
                TextColor = MyTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateDoneButton(txtFieldMobileNo);
            txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldMobileNo, "Mobile");
            viewMobileNumber.AddSubview(txtFieldMobileNo);

            viewLineMobileNo = GenericLine.GetLine(new CGRect(0, 36, viewMobileNumber.Frame.Width, 1));
            viewMobileNumber.AddSubview(viewLineMobileNo);
            View.AddSubview(viewMobileNumber);
            SetTextFieldEvents(txtFieldMobileNo, lblMobileNoTitle, lblMobileNoError, viewLineMobileNo, lblMobileNoHint, MOBILE_NO_PATTERN);
        }

        internal void SetVisibility()
        {
            if (txtFieldMobileNo.Text != string.Empty)
            {
                lblMobileNoTitle.Hidden = false;
                txtFieldMobileNo.LeftViewMode = UITextFieldViewMode.Never;
            }
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine, UILabel lblHint, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;

                if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += TNBGlobal.MobileNoPrefix;
                    }
                }
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblHint.Hidden = true;
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text.Replace("+", string.Empty), pattern);

                if (textField == txtFieldMobileNo)
                {
                    isValid = isValid && _textFieldHelper.ValidateMobileNumberLength(textField.Text);
                }

                lblError.Hidden = isValid;
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                SetSaveButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (textField == txtFieldMobileNo)
                {
                    bool isCharValid = _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid)
                        return false;

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                        var count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
                return true;
            };
        }

        internal void SetSaveButtonEnable()
        {
            bool isValidMobileNo;

            var textStr = txtFieldMobileNo.Text?.Trim();
            if (!string.IsNullOrEmpty(textStr))
            {
                if (IsFromLogin)
                {
                    isValidMobileNo = _textFieldHelper.ValidateTextField(textStr.Replace("+", string.Empty), MOBILE_NO_PATTERN);
                }
                else
                {
                    string mobileNo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                                             ? _textFieldHelper.TrimAllSpaces(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo)
                                             : string.Empty;
                    isValidMobileNo = _textFieldHelper.ValidateTextField(textStr.Replace("+", string.Empty), MOBILE_NO_PATTERN)
                                                       && !mobileNo.Equals(_textFieldHelper.TrimAllSpaces(txtFieldMobileNo.Text));
                }
                isValidMobileNo = isValidMobileNo && _textFieldHelper.ValidateMobileNumberLength(textStr);

                btnSave.Enabled = isValidMobileNo;
                btnSave.BackgroundColor = isValidMobileNo ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            }
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            _navTitle = IsFromLogin ? "Manage_MobileNumberNotVerifiedTitle".Translate() : "Manage_MobileNumberVerifiedTitle".Translate();
            titleBarComponent.SetTitle(_navTitle);
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(WillHideBackButton);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom);
            btnSave.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnSave.Layer.CornerRadius = 4;
            btnSave.BackgroundColor = MyTNBColor.SilverChalice;
            btnSave.SetTitle("Common_Next".Translate(), UIControlState.Normal);
            btnSave.Font = MyTNBFont.MuseoSans16_500;
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.TouchUpInside += async (sender, e) =>
            {
                ActivityIndicator.Show();
                _mobileNo = txtFieldMobileNo.Text.Replace(" ", string.Empty);
                BaseResponseModel response = await ServiceCall.SendUpdatePhoneTokenSMS(_mobileNo);

                if (ServiceCall.ValidateBaseResponse(response))
                {
                    DataManager.DataManager.SharedInstance.User.MobileNo = _mobileNo;
                    UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                    var viewController = storyBoard.InstantiateViewController("VerifyPinViewController") as VerifyPinViewController;
                    viewController.IsMobileVerification = true;
                    viewController.IsFromLogin = IsFromLogin;
                    this.NavigationController.PushViewController(viewController, true);
                    ActivityIndicator.Hide();
                }
                else
                {
                    AlertHandler.DisplayServiceError(this, IsFromLogin
                        ? "Error_VerifyDevice".Translate() : "Error_MobileNumberUpdate".Translate());
                    ActivityIndicator.Hide();
                }
            };
            View.AddSubview(btnSave);
        }

        private void SaveMobileNumber()
        {
            ActivityIndicator.Show();

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        Task[] taskList = new Task[] { ServiceCall.UpdatePhoneNumber(_mobileNo, string.Empty, IsFromLogin) };
                        Task.WaitAll(taskList);
                        if (ServiceCall.ValidateBaseResponse(_saveResponse))
                        {
                            DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo = _mobileNo;
                            if (!IsFromLogin)
                            {
                                DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = true;
                            }
                            UserEntity userEntity = new UserEntity();
                            userEntity.Reset();
                            userEntity.InsertItem(DataManager.DataManager.SharedInstance.UserEntity[0]);
                            DataManager.DataManager.SharedInstance.UserEntity = userEntity.GetAllItems();
                            DismissViewController(true, null);
                        }
                        else
                        {
                            AlertHandler.DisplayServiceError(this, string.Empty);
                        }
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }
    }
}