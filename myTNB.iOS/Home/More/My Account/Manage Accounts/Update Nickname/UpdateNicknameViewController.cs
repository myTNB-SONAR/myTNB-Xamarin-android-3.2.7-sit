using Foundation;
using System;
using UIKit;
using myTNB.Model;
using myTNB.Dashboard.DashboardComponents;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.DataManager;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB
{
    public partial class UpdateNicknameViewController : UIViewController
    {
        public UpdateNicknameViewController(IntPtr handle) : base(handle)
        {
        }

        BaseResponseModel _saveResponse = new BaseResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UILabel lblNameTitle;
        UILabel lblNameError;
        UITextField txtFieldName;
        UIView viewLineName;
        UIButton btnSave;
        string _newName = string.Empty;

        public CustomerAccountRecordModel CustomerRecord = new CustomerAccountRecordModel();
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            AddSaveButton();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            txtFieldName.Text = CustomerRecord.accDesc;
            SetVisibility();
        }

        internal void SetSubviews()
        {
            //FullName 
            UIView viewFullName = new UIView((new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 51)));
            viewFullName.BackgroundColor = UIColor.Clear;

            lblNameTitle = new UILabel(new CGRect(0, 0, viewFullName.Frame.Width, 12));
            lblNameTitle.Text = "Common_AccountNickname".Translate().ToUpper();
            lblNameTitle.Font = MyTNBFont.MuseoSans9_300;
            lblNameTitle.TextColor = MyTNBColor.SilverChalice;
            lblNameTitle.Hidden = true;

            viewFullName.AddSubview(lblNameTitle);

            lblNameError = new UILabel(new CGRect(0, 37, viewFullName.Frame.Width, 14));
            lblNameError.Font = MyTNBFont.MuseoSans9_300;
            lblNameError.TextAlignment = UITextAlignment.Left;
            lblNameError.TextColor = MyTNBColor.Tomato;
            lblNameError.Text = "Invalid_AccountNickname".Translate();
            lblNameError.Hidden = true;

            viewFullName.AddSubview(lblNameError);

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewFullName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_AccountNickname".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            viewFullName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewFullName.Frame.Width, 1)));
            viewLineName.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewFullName.AddSubview(viewLineName);

            View.AddSubview(viewFullName);
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, viewLineName, TNBGlobal.ACCOUNT_NAME_PATTERN);
        }

        internal void SetVisibility()
        {
            if (txtFieldName.Text != string.Empty)
            {
                lblNameTitle.Hidden = false;
                txtFieldName.LeftViewMode = UITextFieldViewMode.Never;
            }
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;

                if (sender == txtFieldName)
                {
                    // remove auto period on consecutive space
                    int index = txtFieldName.Text.IndexOf(". ", StringComparison.InvariantCulture);
                    if (index >= 0)
                    {
                        System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder(txtFieldName.Text);
                        myStringBuilder.Replace(". ", "  ", index, 2);
                        txtFieldName.Text = myStringBuilder.ToString();
                    }
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
                lblTitle.Hidden = textField.Text?.Length == 0;

                bool isFormatValid = !string.IsNullOrWhiteSpace(textField.Text) && _textFieldHelper.ValidateTextField(textField.Text, pattern);
                bool isUnique = DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(textField.Text);
                bool isValid = isFormatValid && isUnique;
                lblError.Hidden = isValid;

                if (!isValid)
                {
                    lblError.Text = !isFormatValid ? "Invalid_AccountNickname".Translate() : "Invalid_AccountNicknameInUse".Translate();
                }

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

            if (textField == txtFieldName)
            {
                textField.ShouldChangeCharacters = (txtField, range, replacementString) =>
                {
                    if (!string.IsNullOrEmpty(replacementString))
                    {
                        return _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.ACCOUNT_NAME_PATTERN);
                    }

                    return true;
                };
            }
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        internal void SetSaveButtonEnable()
        {
            bool isValid = !string.IsNullOrWhiteSpace(txtFieldName.Text)
                                           && _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                                           && !CustomerRecord.accDesc.Equals(txtFieldName.Text)
                                           && DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(txtFieldName.Text);
            btnSave.Enabled = isValid;
            btnSave.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Manage_UpdateNicknameTitle".Translate());
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
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
            btnSave.SetTitle("Common_Save".Translate(), UIControlState.Normal);
            btnSave.Font = MyTNBFont.MuseoSans16_500;
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Enabled = false;
            btnSave.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _newName = txtFieldName.Text;
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Task[] taskList = new Task[] { Save() };
                            Task.WaitAll(taskList);
                            if (ServiceCall.ValidateBaseResponse(_saveResponse))
                            {
                                DataManager.DataManager.SharedInstance.IsNickNameUpdated = true;
                                int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == CustomerRecord.accNum);
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d[index].accDesc = _newName;
                                UserAccountsEntity uaManager = new UserAccountsEntity();
                                uaManager.DeleteTable();
                                uaManager.CreateTable();
                                uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
                                DataManager.DataManager.SharedInstance.UpdateDueAccountNickname(CustomerRecord.accNum, _newName);

                                // updates the cache when user entity data is updated
                                DataManager.DataManager.SharedInstance.RefreshDataFromAccountUpdate(true);

                                int accountRecordIndex = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == CustomerRecord.accNum);
                                DataManager.DataManager.SharedInstance.AccountRecordIndex = accountRecordIndex;

                                DismissViewController(true, null);
                            }
                            else
                            {
                                AlertHandler.DisplayServiceError(this, "Error_UpdateNickname".Translate());
                            }
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
        }

        internal Task Save()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    sspUserId = DataManager.DataManager.SharedInstance.UserEntity[0].userID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    accountNo = CustomerRecord.accNum,
                    oldAccountNickName = CustomerRecord.accDesc,
                    newAccountNickName = _newName
                };
                _saveResponse = serviceManager.BaseServiceCall("UpdateLinkedAccountNickName", requestParameter);
            });
        }
    }
}