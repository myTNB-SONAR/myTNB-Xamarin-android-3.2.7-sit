using Foundation;
using System;
using UIKit;
using myTNB.Model;
using myTNB.Dashboard.DashboardComponents;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.DataManager;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.MyAccount;

namespace myTNB
{
    public partial class UpdateNicknameViewController : CustomUIViewController
    {
        public UpdateNicknameViewController(IntPtr handle) : base(handle) { }

        private BaseResponseModelV2 _saveResponse = new BaseResponseModelV2();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private UILabel lblNameTitle, lblNameError;
        private UITextField txtFieldName;
        private UIView viewLineName;
        private UIButton btnSave;
        private string _newName = string.Empty;

        public CustomerAccountRecordModel CustomerRecord = new CustomerAccountRecordModel();
        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_UpdateNickname;
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

        private void SetSubviews()
        {
            //FullName 
            UIView viewFullName = new UIView((new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNameTitle = new UILabel(new CGRect(0, 0, viewFullName.Frame.Width, 12))
            {
                Text = GetCommonI18NValue(Constants.Common_AccountNickname).ToUpper(),
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Hidden = true
            };

            viewFullName.AddSubview(lblNameTitle);

            lblNameError = new UILabel(new CGRect(0, 37, viewFullName.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.Tomato,
                Text = GetErrorI18NValue(Constants.Error_InvalidNickname),
                Hidden = true
            };

            viewFullName.AddSubview(lblNameError);

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewFullName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    GetCommonI18NValue(Constants.Common_AccountNickname)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            viewFullName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewFullName.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            viewFullName.AddSubview(viewLineName);

            View.AddSubview(viewFullName);
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, viewLineName, TNBGlobal.ACCOUNT_NAME_PATTERN);
        }

        private void SetVisibility()
        {
            if (txtFieldName.Text != string.Empty)
            {
                lblNameTitle.Hidden = false;
                txtFieldName.LeftViewMode = UITextFieldViewMode.Never;
            }
        }

        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
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
                    lblError.Text = GetErrorI18NValue(isFormatValid ? Constants.Error_DuplicateNickname : Constants.Error_InvalidNickname);
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

        private void SetSaveButtonEnable()
        {
            bool isValid = !string.IsNullOrWhiteSpace(txtFieldName.Text)
                && _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                && !CustomerRecord.accDesc.Equals(txtFieldName.Text)
                && DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(txtFieldName.Text);
            btnSave.Enabled = isValid;
            btnSave.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(MyAccountConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                    ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.SilverChalice,
                Enabled = false,
                Font = MyTNBFont.MuseoSans16_500
            };

            btnSave.SetTitle(GetCommonI18NValue(Constants.Common_Save), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 4;
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
                            Task[] taskList = { Save() };
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

                                // update the new nickname from the rearrange list if it exists
                                DataManager.DataManager.SharedInstance.UpdateNicknameFromArrangedList(CustomerRecord.accNum, _newName);

                                DismissViewController(true, null);
                            }
                            else
                            {
                                DisplayServiceError(_saveResponse?.d?.ErrorMessage ?? string.Empty);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
        }

        private Task Save()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    accountNo = CustomerRecord.accNum,
                    oldAccountNickName = CustomerRecord.accDesc,
                    newAccountNickName = _newName
                };
                _saveResponse = serviceManager.BaseServiceCallV6(MyAccountConstants.Service_UpdateNickname, requestParameter);
            });
        }
    }
}