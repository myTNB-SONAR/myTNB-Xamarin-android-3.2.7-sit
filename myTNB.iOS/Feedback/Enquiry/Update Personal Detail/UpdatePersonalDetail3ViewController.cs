using CoreGraphics;
using Foundation;
using myTNB.Feedback.FeedbackImage;
using myTNB.Model;
using myTNB.Registration;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class UpdatePersonalDetail3ViewController : CustomUIViewController
    {
        public UpdatePersonalDetail3ViewController(IntPtr handle) : base(handle)
        {
        }

        public List<ImageDataEnquiryModel> Items;
        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList;
        SubmitFeedbackResponseModel _submitFeedback;
        public bool IsOwner;

        private UIView _viewTitleSection, _viewTitleSection2, _btnSubmitContainer, _viewPersonalDetail;
        private UIView viewCheckBoxTNC;
        private UIScrollView _svContainer;
        private UITextView lblTNC;
        private UIButton _btnSubmit;

        private UITextField txtFieldName, txtFieldICNo, txtFieldEmail
            , txtFieldConfirmEmail, txtFieldPassword, txtFieldConfirmPassword;
        private UITextView txtViewDetails;
        private CustomUIButtonV2 btnRegister;
        private UIView viewLineName, viewLineICNo, viewLineEmail
            , viewLineConfirmEmail, viewLinePassword, viewLineConfirmPassword
            , viewShowConfirmPassword, viewShowPassword, btnRegisterContainer;
        private UIView viewEmail;
        private UILabel lblNameTitle, lblICNoTitle, lblEmailTitle
            , lblConfirmEmailTitle, lblPasswordTitle, lblConfirmPasswordTitle
            , lblNameError, lblICNoError, lblEmailError
            , lblConfirmEmailError, lblPasswordError, lblConfirmPasswordError
            , lblNameHint, lblEmailHint
            , lblConfirmEmailHint, lblPasswordHint, lblICNoHint, lblConfirmPasswordHint;

        private MobileNumberComponent _mobileNumberComponent;

        private string _eMail = string.Empty, _password = string.Empty
            , _fullName = string.Empty, _icNo = string.Empty, _mobileNo = string.Empty;

        //private UIView _viewBottomContainer;
        public bool IsApplication = true;

        //add detail section
        //public bool IsApplication;
        public CustomerAccountRecordModel SelectedAccount;
        public ContactDetailsResponseModel ContactDetails;

        //private CustomUIButtonV2 _btnSubmit;
        private UIView _viewBottomContainer, _viewContactDetails, _viewTerminate
            , _viewTerminateTitle, _viewTerminateContainer, _viewOthersContainer
            , _viewLineTerminate, _viewLineReason, _viewMainDetails, _viewApplyForTitle
            , _viewContactDetailsTitle, _viewAccountContainer;
        private UIView viewName;
        private UIScrollView _scrollContainer;
        private CGRect _scrollViewFrame;
        private CustomTextField _customNameField;
        private CustomTextField _customMobileField;
        private CustomTextField _customEmailField;
        //protected List<CustomerAccountRecordModel> _eligibleAccountList;
        //protected AccountsSMREligibilityResponseModel _smrEligibleList;
        //protected SSMRApplicationStatusResponseModel _ssmrApplicationStatus;
        //protected TerminationReasonsResponseModel _ssmrTerminationReasons;
        private int _selectedTerminateReasonIndex = 0;
        private UILabel _lblAddress, _lblEditInfo, _lblTerminateReason, _lblReason;
        private UITextView _txtViewReason;
        private bool _isAllowEdit;
        private UIView viewMobile;
        private UILabel lblMobileTitle;
        private UILabel lblMobileError;
        private UITextField txtFieldMobile;
        private UIView viewLineMobile;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private UIImageView imgViewCheckBoxTNC;
        private object imgViewCheckBoxIC;
        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public bool IsTNC { get; private set; }

        public override void ViewDidLoad()
        {
            //PageName = FeedbackConstants.Pagename_FeedbackForm;
            base.ViewDidLoad();

            SetHeader();
            AddScrollView();
            AddCTA();
            //AddTnCSection();
            AddSectionTitle();
            AddDetailsSection();
            SetEvents();

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
            Title = "Update Personal Detail";
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

            bool isValid = isValidFieldName && isValidFieldEmail && isValidFieldMobile && IsTNC;

            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;

        }

        //#region TNC Section
        //private void AddTnCSection()
        //{
        //    _viewBottomContainer = new UIView(new CGRect(0, View.Frame.Height - GetScaledHeight(160), View.Frame.Width, GetScaledHeight(160)))
        //    {
        //        BackgroundColor = UIColor.White
        //    };
        //    UIView viewPadding = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(1)))
        //    {
        //        BackgroundColor = MyTNBColor.SectionGrey
        //    };

        //    UITextView txtFieldInfo= GetInfo();
        //    _btnSubmit = new CustomUIButtonV2()
        //    {
        //        Frame = new CGRect(BaseMargin, GetYLocationFromFrame(txtFieldInfo.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
        //        Enabled = true,
        //        BackgroundColor = MyTNBColor.FreshGreen
        //    };
        //    _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
        //    _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() =>
        //    {
        //        ExecuteSubmitUpdatePersonalDetail();
        //    }));
        //    _viewBottomContainer.AddSubviews(new UIView[] { viewPadding, txtFieldInfo, _btnSubmit });
        //    nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
        //    _viewBottomContainer.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
        //    View.AddSubview(_viewBottomContainer);
        //}

        private void GetInfo()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("By submmitting, you are agreeing to the <a href=\"\\\"><strong>TNB Terms & Conditions</strong></a>"
                        , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(12F));
            NSMutableAttributedString mutableHTMLFooter = new NSMutableAttributedString(htmlBody);

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey
            }, new NSRange(0, htmlBody.Length));
            lblTNC = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextAlignment = UITextAlignment.Left
            };
            lblTNC.TextContainerInset = UIEdgeInsets.Zero;

            lblTNC.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                TermsAndConditionViewController viewController =
                    storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
                if (viewController != null)
                {
                    viewController.isPresentedVC = true;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }
            }));
            //Resize
            CGSize size = lblTNC.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));
            lblTNC.Frame = new CGRect(GetXLocationFromFrame(viewCheckBoxTNC.Frame, 8F), 16, size.Width - 24 - 18, size.Height);
            //return txtFieldInfo;
        }
        //#endregion

        //private void AddCTA()
        //{
        //    _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(128))
        //        , View.Frame.Width, DeviceHelper.GetScaledHeight(100)))
        //    {
        //        BackgroundColor = UIColor.White
        //    };

        //    _btnSubmit = new UIButton(UIButtonType.Custom)
        //    {
        //        Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnSubmitContainer.Frame.Width - 36, 48)
        //    };
        //    _btnSubmit.SetTitle("Submit", UIControlState.Normal);
        //    _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
        //    _btnSubmit.Layer.CornerRadius = 5.0f;
        //    //_btnSubmit.Enabled = true; //false;
        //    //_btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;

        //    _btnSubmit.Enabled = true;//Temporary
        //    _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
        //    _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;

        //    _btnSubmit.TouchUpInside += (sender, e) =>
        //    {
        //        //DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage = _feedbackTextView.Text ? string.Empty;
        //        //NavigateToPage();
        //    };
        //    _btnSubmitContainer.AddSubview(_btnSubmit);
        //    View.AddSubview(_btnSubmitContainer);
        //}

        private void AddCTA()
        {
            //_btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145))
            //    , View.Frame.Width, DeviceHelper.GetScaledHeight(100)))
            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(173))
            , View.Frame.Width, DeviceHelper.GetScaledHeight(128)))
            {
                BackgroundColor = UIColor.White
            };

            viewCheckBoxTNC = new UIView(new CGRect(16, 16, 20, 20))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxTNC.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxTNC.Layer.BorderColor = IsTNC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxTNC.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxTNC = new UIImageView(new CGRect(0, 0, 20, 20))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsTNC ? false : true
            };
            viewCheckBoxTNC.AddSubview(imgViewCheckBoxTNC);

            viewCheckBoxTNC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsTNC = !IsTNC;
                imgViewCheckBoxTNC.Hidden = !IsTNC;
                viewCheckBoxTNC.Layer.BorderColor = IsTNC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                SetSubmitButtonEnable();
            }));

            lblTNC = new UITextView(new CGRect(GetXLocationFromFrame(viewCheckBoxTNC.Frame, 18F), 16, View.Frame.Width - 23, 20));
            GetInfo();

            _btnSubmit = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, lblTNC.Frame.GetMaxY() + DeviceHelper.GetScaledHeight(16), _btnSubmitContainer.Frame.Width - 36, 48)
            };
            _btnSubmit.SetTitle("Submit", UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                ExecuteSubmitUpdatePersonalDetail();
            };
            _btnSubmitContainer.AddSubviews(viewCheckBoxTNC, lblTNC, _btnSubmit);
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

        private void ExecuteSubmitUpdatePersonalDetail()
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
                feedbackUpdateDetails = feedbackUpdateDetailsList,//feedbackUpdateDetailsList,//GetSubmitFeedbackWithContact(),
                attachment = Items, //Common
                feedbackCategoryId = "1",
                feedbackTypeId = "",
                accountNum = DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA,
                name = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                phoneNum = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo,
                feedbackMesage = string.Empty, //Common
                stateId = "",
                location = "",
                poleNum = "",
                contactName = _customNameField.TextField.Text,
                contactMobileNo = _customMobileField.TextField.Text,
                contactEmailAddress = _customEmailField.TextField.Text,
                isOwner = IsOwner, // 1=true,0=false , if general enquiry set as false
                relationship = DataManager.DataManager.SharedInstance.Relationship,
                relationshipDesc = DataManager.DataManager.SharedInstance.RelationshipDesc

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