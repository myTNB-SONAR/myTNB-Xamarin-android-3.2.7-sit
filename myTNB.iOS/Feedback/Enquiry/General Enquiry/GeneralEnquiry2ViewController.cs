using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.Registration;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using UIKit;

using myTNB.Home.Feedback;
using System.Threading.Tasks;
using myTNB.Feedback;
using myTNB.Feedback.FeedbackImage;
using myTNB.Feedback.Enquiry.GeneralEnquiry;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB
{
    public partial class GeneralEnquiry2ViewController : CustomUIViewController
    {
        public GeneralEnquiry2ViewController(IntPtr handle) : base(handle) { }

        public List<ImageDataEnquiryModel> Items;
        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList = new List<FeedbackUpdateDetailsModel>();

        private UIView _viewTitleSection, _btnSubmitContainer;
        private UIScrollView _svContainer;
        private UIButton _btnSubmit;

        public bool IsApplication = true;

        //add detail section
        public CustomerAccountRecordModel SelectedAccount;
        public ContactDetailsResponseModel ContactDetails;

        private UIView _viewContactDetails;
        private UIView viewName;
        private UILabel lblNameTitle;
        private UILabel lblNameError;
        private UITextField txtFieldName;

        SubmitFeedbackResponseModel _submitFeedback;
        private UIView viewLineName;
        private UIView viewEmail;
        private UILabel lblEmailTitle;
        private UILabel lblEmailError;
        private UITextField txtFieldEmail;
        private UIView viewLineEmail;
        private UIView viewMobile;
        private UILabel lblMobileTitle;
        private UILabel lblMobileError;
        private UITextField txtFieldMobile;
        private UIView viewLineMobile;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UITextField textField;
        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private const string MOBILENUMBER_PATTERN = @"^[0-9 \+]+$";


        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;
            base.ViewDidLoad();

            SetHeader();
            AddScrollView();
            AddCTA();
            AddSectionTitle();
            AddDetailsSection();
            SetSubmitButtonEnable();
            SetEvents();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }

        private void SetHeader()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (Items.Count > 0)
                    Items.Clear();

                NavigationController?.PopViewController(true);
            });
            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = btnBack;
            }

            Title = GetI18NValue(EnquiryConstants.generalEnquiryTitle);

        }

        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            View.AddSubview(_svContainer);
        }

        private void AddDetailsSection()
        {
            UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
            ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();

            _viewContactDetails = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY() + 8, ViewWidth, GetScaledHeight(219)))
            {
                BackgroundColor = UIColor.White
            };

            //Name
            viewName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNameTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.nameHint).ToUpper()); //(GetI18NValue("nameHint") 
            lblNameError = GetErrorLabel(GetErrorI18NValue("invalid_fullname")); //(GetI18NValue("nameHint") 

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewName.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.displayName ?? string.Empty
            };

            txtFieldName.ReturnKeyType = UIReturnKeyType.Done;

            viewLineName = GenericLine.GetLine(new CGRect(0, 36, viewName.Frame.Width, 1));

            viewName.AddSubviews(new UIView[] { lblNameTitle, lblNameError, txtFieldName, viewLineName });
            _viewContactDetails.AddSubview(viewName);


            //Email
            viewEmail = new UIView((new CGRect(18, viewName.Frame.GetMaxY() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblEmailTitle = GetTitleLabel(GetCommonI18NValue("emailAddress").ToUpper()); //(GetI18NValue("name") 
            lblEmailError = GetErrorLabel(GetErrorI18NValue("invalid_email")); //(GetI18NValue("name")

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.email ?? string.Empty
            };

            txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

            viewLineEmail = GenericLine.GetLine(new CGRect(0, 36, viewEmail.Frame.Width, 1));

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, txtFieldEmail, viewLineEmail });
            _viewContactDetails.AddSubview(viewEmail);


            //Mobile Number
            viewMobile = new UIView((new CGRect(18, viewEmail.Frame.GetMaxY() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblMobileTitle = GetTitleLabel(GetCommonI18NValue("mobileNumber").ToUpper()); //(GetI18NValue("name") 
            lblMobileError = GetErrorLabel(GetErrorI18NValue("invalid_mobileNumber")); //(GetI18NValue("name")

            txtFieldMobile = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobile.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.mobileNo ?? string.Empty
            };

            txtFieldMobile.ReturnKeyType = UIReturnKeyType.Done;

            viewLineMobile = GenericLine.GetLine(new CGRect(0, 36, viewMobile.Frame.Width, 1));

            viewMobile.AddSubviews(new UIView[] { lblMobileTitle, lblMobileError, txtFieldMobile, viewLineMobile });
            _viewContactDetails.AddSubview(viewMobile);


            _svContainer.AddSubview(_viewContactDetails);
        }

        private void AddCTA()
        {
            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145))
                , View.Frame.Width, DeviceHelper.GetScaledHeight(100)))
            {
                BackgroundColor = UIColor.White
            };

            _btnSubmit = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnSubmitContainer.Frame.Width - 36, 48)
            };
            _btnSubmit.SetTitle(GetCommonI18NValue("submit"), UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                ExecuteSubmitGeneralEnquiry();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);

        }

        private void AddSectionTitle()
        {
            if (_viewTitleSection != null) { return; }
            _viewTitleSection = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.contactEnquiryTitle),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };

            _viewTitleSection.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection);
        }

        private void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            _textFieldHelper.CreateDoneButton(textField);

            textField.EditingChanged += (sender, e) =>
            {
                //textFieldTitle.Hidden = textField.Text.Length == 0;
                SetSubmitButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                //textFieldTitle.Hidden = textField.Text.Length == 0;
                if (textField == txtFieldMobile && textField.Text.Length == 0)
                {
                    textField.Text += TNBGlobal.MobileNoPrefix;
                }
                string value = textField.Text;
                bool hidden;
                if (textField == txtFieldMobile)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }
                //LblTitle.Hidden = hidden;
                //LblHint.Hidden = !IsEmpty(value) && !LblError.Hidden || value.Length == 0;

                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.TextColor = MyTNBColor.TunaGrey();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                textFieldError.Hidden = isValid;

                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters = (txtField, range, replacementString) =>
            {
                if (txtField == txtFieldMobile)
                {
                    bool isCharValid = _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid) { return false; }

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _textFieldHelper.TrimAllSpaces(textField.Text);
                        nint count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }

                return true;
            };

            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }


        private void SetEvents()
        {
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, viewLineName, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(txtFieldEmail, lblEmailTitle, lblEmailError, viewLineEmail, EMAIL_PATTERN);
            SetTextFieldEvents(txtFieldMobile, lblMobileTitle, lblMobileError, viewLineMobile, MOBILENUMBER_PATTERN);
        }

        private void SetSubmitButtonEnable()
        {
            bool isValidFieldName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern)
               && !string.IsNullOrWhiteSpace(txtFieldName.Text);

            bool isValidFieldEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
               && !string.IsNullOrWhiteSpace(txtFieldEmail.Text);

            bool isValidFieldMobile = _textFieldHelper.ValidateTextField(txtFieldMobile.Text, MOBILENUMBER_PATTERN) && _textFieldHelper.ValidateMobileNumberLength(txtFieldMobile.Text)
               && !string.IsNullOrWhiteSpace(txtFieldMobile.Text);

            bool isValid = isValidFieldName && isValidFieldEmail && isValidFieldMobile;

            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;

        }

        private UILabel GetTitleLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 0, View.Frame.Width - 36, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
        }

        private UILabel GetErrorLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 37, View.Frame.Width - 36, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
        }

        private bool IsEmpty(string value)
        {
            if (textField == txtFieldMobile)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) { return true; }
                int countryCodeIndex = value.IndexOf(@"+60", 0, StringComparison.CurrentCulture);
                return countryCodeIndex > -1 && value.Length == 3;
            }
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }


        private void ExecuteSubmitGeneralEnquiry()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        _submitFeedback = new SubmitFeedbackResponseModel();
                        SubmitFeedback().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_submitFeedback != null && _submitFeedback?.d != null
                                   && _submitFeedback.d.IsSuccess && _submitFeedback?.d?.data != null)
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
                                    EnquiryStatusPageViewController status = storyBoard.InstantiateViewController("EnquiryStatusPageViewController") as EnquiryStatusPageViewController;
                                    status.IsSuccess = true;
                                    status.ReferenceNumber = _submitFeedback?.d?.data?.ServiceReqNo;
                                    status.ReferenceDate = _submitFeedback?.d?.data?.DateCreated;
                                    status.StatusDisplayType = EnquiryStatusPageViewController.StatusType.Enquiry;
                                    NavigationController.PushViewController(status, true);
                                }
                                else
                                {
                                    DisplayServiceError(_submitFeedback?.d?.DisplayMessage ?? string.Empty);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private object GetRequestParameters()
        {
            ServiceManager serviceManager = new ServiceManager();
            return new
            {
                serviceManager.usrInf,
                serviceManager.deviceInf,
                feedbackUpdateDetails = feedbackUpdateDetailsList,
                attachment = Items, //Common
                feedbackCategoryId = "1",
                feedbackTypeId = "",
                accountNum = DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA,
                name = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].displayName : txtFieldName.Text,
                email = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].email : txtFieldEmail.Text,
                phoneNum = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : txtFieldMobile.Text,
                feedbackMesage = DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage, 
                stateId = "",
                location = "",
                poleNum = "",
                contactName = txtFieldName.Text,
                contactMobileNo = txtFieldMobile.Text,
                contactEmailAddress = txtFieldEmail.Text,
                isOwner = false, // 1=true,0=false , if general enquiry set as false
                relationship = 0,
                relationshipDesc = string.Empty
            };
        }

        private Task SubmitFeedback()
        {
            object requestParameter = GetRequestParameters();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                 _submitFeedback = serviceManager.OnExecuteAPIV6<SubmitFeedbackResponseModel>("SubmitFeedbackWithContactDetails", requestParameter);
            });
        }

    }
}