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

        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";


        public override void ViewDidLoad()
        {
            //PageName = FeedbackConstants.Pagename_FeedbackForm;
            base.ViewDidLoad();

            SetHeader();
            AddScrollView();
            AddCTA();
            AddSectionTitle();
            AddDetailsSection();
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
                NavigationController?.PopViewController(true);
            });
            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = btnBack;
            }

            //NavigationItem.LeftBarButtonItem = btnBack;
            //Title = DataManager.DataManager.SharedInstance.FeedbackCategory?.Find(x => x?.FeedbackCategoryId == FeedbackID)?.FeedbackCategoryName;
            Title = "General Enquiry";
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
            _viewContactDetails = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY() + 8, ViewWidth, GetScaledHeight(219)))
            {
                BackgroundColor = UIColor.White
            };

            //Name
            viewName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNameTitle = GetTitleLabel("NAME");
            lblNameError = GetErrorLabel("Name is required");

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewName.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
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

            lblEmailTitle = GetTitleLabel("EMAIL ADDRESS");
            lblEmailError = GetErrorLabel("Invalid email address");

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
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

            lblMobileTitle = GetTitleLabel("MOBILE NUMBER");
            lblMobileError = GetErrorLabel("Invalid mobile number");

            txtFieldMobile = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobile.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
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
            _btnSubmit.SetTitle("Submit", UIControlState.Normal);
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
                Text = "Who should contact regarding this enquiry?",
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
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.TextColor = MyTNBColor.TunaGrey();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                //textFieldTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                //if (textField == _txtAccountNumber)
                //{
                //    isValid = isValid && _textFieldHelper.ValidateTextFieldWithLength(textField.Text, TNBGlobal.AccountNumberLowCharLimit);
                //}

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
                //if (textField == _txtAccountNumber)
                //{
                //    string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                //    nint count = content.Length + replacementString.Length - range.Length;
                //    return count <= TNBGlobal.AccountNumberLowCharLimit;
                //}

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
            SetTextFieldEvents(txtFieldMobile, lblMobileTitle, lblMobileError, viewLineMobile, TNBGlobal.MobileNoPattern);
        }

        private void SetSubmitButtonEnable()
        {
            bool isValidFieldName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern)
               && !string.IsNullOrWhiteSpace(txtFieldName.Text);

            bool isValidFieldEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
               && !string.IsNullOrWhiteSpace(txtFieldEmail.Text);

            bool isValidFieldMobile = _textFieldHelper.ValidateTextField(txtFieldMobile.Text, TNBGlobal.MobileNoPattern)
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
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                                    status.IsSuccess = true;
                                    status.ReferenceNumber = _submitFeedback?.d?.data?.ServiceReqNo;
                                    status.ReferenceDate = _submitFeedback?.d?.data?.DateCreated;
                                    status.StatusDisplayType = GenericStatusPageViewController.StatusType.Enquiry;
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
                name = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                phoneNum = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo,
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