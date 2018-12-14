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
        public UpdateNicknameViewController (IntPtr handle) : base (handle)
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
        }

        internal void SetSubviews()
        {
            //FullName 
            UIView viewFullName = new UIView((new CGRect(18, DeviceHelper.IsIphoneX() ? 104 : 80, View.Frame.Width - 36, 51)));
            viewFullName.BackgroundColor = UIColor.Clear;

            lblNameTitle = new UILabel(new CGRect(0, 0, viewFullName.Frame.Width, 12));
            lblNameTitle.Text = "ACCOUNT NICKNAME";
            lblNameTitle.Font = myTNBFont.MuseoSans9_300();
            lblNameTitle.TextColor = myTNBColor.SilverChalice();
            lblNameTitle.Hidden = true;

            viewFullName.AddSubview(lblNameTitle);

            lblNameError = new UILabel(new CGRect(0, 37, viewFullName.Frame.Width, 14));
            lblNameError.Font = myTNBFont.MuseoSans9();
            lblNameError.TextAlignment = UITextAlignment.Left;
            lblNameError.TextColor = myTNBColor.Tomato();
            lblNameError.Text = "Invalid account nickname";
            lblNameError.Hidden = true;

            viewFullName.AddSubview(lblNameError);

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewFullName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Account Nickname"
                    , font: myTNBFont.MuseoSans16_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            viewFullName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewFullName.Frame.Width, 1)));
            viewLineName.BackgroundColor = myTNBColor.PlatinumGrey();
            viewFullName.AddSubview(viewLineName);

            View.AddSubview(viewFullName);
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, viewLineName, TNBGlobal.ACCOUNT_NAME_PATTERN);
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) => {
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
            textField.EditingDidBegin += (sender, e) => {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.ShouldEndEditing = (sender) => {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
                SetSaveButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) => {
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
        }

        internal void SetSaveButtonEnable()
        {
            bool isValid = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                                           && !CustomerRecord.accDesc.Equals(txtFieldName.Text);
            btnSave.Enabled = isValid;
            btnSave.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Update Account Nickname");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() => {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom);
            btnSave.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneX() ? 96 : 72), View.Frame.Width - 36, 48);
            btnSave.Layer.CornerRadius = 4;
            btnSave.BackgroundColor = myTNBColor.SilverChalice();
            btnSave.SetTitle("Save", UIControlState.Normal);
            btnSave.Font = myTNBFont.MuseoSans16();
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
                                DismissViewController(true, null);
                            }
                            else
                            {
                                DisplayAlertMessage("Error", "Cannot update mobile number. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("No Data Connection", "Please check your data connection and try again.");
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
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