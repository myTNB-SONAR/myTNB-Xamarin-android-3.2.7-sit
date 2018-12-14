using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.DataManager;

namespace myTNB
{
    public partial class UpdateMobileNumberViewController : UIViewController
    {
        public UpdateMobileNumberViewController(IntPtr handle) : base(handle)
        {
        }
        const string MOBILE_NO_PATTERN = @"^[0-9]+$";
        UILabel lblMobileNoTitle;
        UILabel lblMobileNoError;
        UITextField txtFieldMobileNo;
        UIView viewLineMobileNo;
        UIButton btnSave;

        BaseResponseModel _saveResponse = new BaseResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        string _mobileNo = string.Empty;

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
            txtFieldMobileNo.Text = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo;
        }

        internal void SetSubviews()
        {
            UIView viewMobileNumber = new UIView((new CGRect(18, DeviceHelper.IsIphoneX() ? 104 : 80, View.Frame.Width - 36, 51)));
            viewMobileNumber.BackgroundColor = UIColor.Clear;

            lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMobileNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString("MOBILE NO."
                                                        , font: myTNBFont.MuseoSans9()
                                                        , foregroundColor: myTNBColor.SilverChalice()
                                                        , strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoTitle.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoTitle);

            lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid mobile no."
                                                        , font: myTNBFont.MuseoSans9()
                                                        , foregroundColor: myTNBColor.Tomato()
                                                        , strokeWidth: 0
                                                       ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoError.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoError);

            txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString("Mobile No."
                                                               , font: myTNBFont.MuseoSans16()
                                                               , foregroundColor: myTNBColor.SilverChalice()
                                                               , strokeWidth: 0
                                                              ),
                TextColor = myTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateDoneButton(txtFieldMobileNo);
            txtFieldMobileNo.KeyboardType = UIKeyboardType.PhonePad;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldMobileNo, "Mobile");
            viewMobileNumber.AddSubview(txtFieldMobileNo);

            viewLineMobileNo = new UIView((new CGRect(0, 36, viewMobileNumber.Frame.Width, 1)));
            viewLineMobileNo.BackgroundColor = myTNBColor.PlatinumGrey();
            viewMobileNumber.AddSubview(viewLineMobileNo);
            View.AddSubview(viewMobileNumber);
            SetTextFieldEvents(txtFieldMobileNo, lblMobileNoTitle, lblMobileNoError, viewLineMobileNo, MOBILE_NO_PATTERN);
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text.Replace("+", string.Empty), pattern);
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
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = false;
            };
        }

        internal void SetSaveButtonEnable()
        {
            bool isValidMobileNo = _textFieldHelper.ValidateTextField(txtFieldMobileNo.Text.Replace("+", string.Empty), MOBILE_NO_PATTERN)
                                                   && !DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo.Equals(txtFieldMobileNo.Text);
            btnSave.Enabled = isValidMobileNo;
            btnSave.BackgroundColor = isValidMobileNo ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Update Mobile Number");
            titleBarComponent.SetNotificationVisibility(true);
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
                _mobileNo = txtFieldMobileNo.Text;
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
                                DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo = _mobileNo;
                                DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = true;
                                UserEntity userEntity = new UserEntity();
                                userEntity.Reset();
                                userEntity.InsertItem(DataManager.DataManager.SharedInstance.UserEntity[0]);
                                DataManager.DataManager.SharedInstance.UserEntity = userEntity.GetAllItems();
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
                    oldPhoneNumber = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo,
                    newPhoneNumber = _mobileNo
                };
                _saveResponse = serviceManager.BaseServiceCall("UpdatePhoneNumber", requestParameter);
            });
        }
    }
}