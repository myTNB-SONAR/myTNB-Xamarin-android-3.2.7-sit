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
        private UIScrollView _svContainer;
        private UIButton _btnSubmit;

        private UITextField txtFieldName, txtFieldICNo, txtFieldEmail
            , txtFieldConfirmEmail, txtFieldPassword, txtFieldConfirmPassword;
        private UITextView txtViewDetails;
        private CustomUIButtonV2 btnRegister;
        private UIView viewLineName, viewLineICNo, viewLineEmail
            , viewLineConfirmEmail, viewLinePassword, viewLineConfirmPassword
            , viewShowConfirmPassword, viewShowPassword, btnRegisterContainer;
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



        public override void ViewDidLoad()
        {
            //PageName = FeedbackConstants.Pagename_FeedbackForm;
            base.ViewDidLoad();

            SetHeader();
            AddScrollView();
            //AddCTA();
            AddTnCSection();
            AddSectionTitle();
            AddDetailsSection();

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

        #region Details Section
        private void AddDetailsSection()
        {
            _scrollContainer = new UIScrollView(new CGRect(0, _viewTitleSection.Frame.GetMaxY(), ViewWidth, ViewHeight));

            _viewContactDetails = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(219)))
            {
                BackgroundColor = UIColor.White
            };

            _customNameField = new CustomTextField(_viewContactDetails, new CGPoint(GetScaledWidth(16), GetScaledHeight(16)))
            {
                Title = "Name",
                LeftIcon = "Name",
                KeyboardType = UIKeyboardType.ASCIICapable,
                //Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidEmail),
                TypingEndAction = ToggleCTA,
                TypingBeginAction = OnEdit,
                TextFieldType = CustomTextField.Type.NameUser,
                //OnCreateValidation = true

            };
            UIView viewName = _customNameField.GetUI();


            _customEmailField = new CustomTextField(_viewContactDetails, new CGPoint(GetScaledWidth(16), GetScaledHeight(75)))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_Email),
                LeftIcon = SSMRConstants.IMG_Email,
                KeyboardType = UIKeyboardType.EmailAddress,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidEmail),
                TypingEndAction = ToggleCTA,
                TypingBeginAction = OnEdit,
                TextFieldType = CustomTextField.Type.EmailAddress,
                OnCreateValidation = true

            };
            UIView viewEmail = _customEmailField.GetUI();


            _customMobileField = new CustomTextField(_viewContactDetails, new CGPoint(GetScaledWidth(16), GetScaledHeight(134)))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_MobileNumber),
                LeftIcon = SSMRConstants.IMG_MobileNumber,
                KeyboardType = UIKeyboardType.PhonePad,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidMobileNumber),
                Hint = GetHintI18NValue(SSMRConstants.I18N_HintMobileNumber),
                TypingEndAction = ToggleCTA,
                TypingBeginAction = OnEdit,
                TextFieldType = CustomTextField.Type.MobileNumber,
                OnCreateValidation = true
            };
            UIView viewMobile = _customMobileField.GetUI();


            _lblEditInfo = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(viewMobile.Frame, 12)
                , _viewContactDetails.Frame.Width - GetScaledWidth(32), GetScaledHeight(32)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_300,
                Text = GetI18NValue(SSMRConstants.I18N_EditInfo),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Hidden = true
            };

            CGSize newLblEditInfoSize = GetLabelSize(_lblEditInfo, _lblEditInfo.Frame.Width, GetScaledHeight(300));
            _lblEditInfo.Frame = new CGRect(_lblEditInfo.Frame.X, _lblEditInfo.Frame.Y, _lblEditInfo.Frame.Width, newLblEditInfoSize.Height);
            _viewContactDetails.AddSubviews(new UIView[] { viewName, viewEmail, viewMobile, _lblEditInfo });

            _scrollContainer.AddSubviews(new UIView[] { _viewContactDetails });
            _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewContactDetails.Frame.GetMaxY());
            _svContainer.AddSubview(_scrollContainer);
            //_scrollViewFrame = _scrollContainer.Frame;
        }
        #endregion

        private void ToggleCTA()
        {
            bool isValid = !_isAllowEdit;
            if (_isAllowEdit && _customEmailField != null && _customMobileField != null)
            {
                isValid = _customEmailField.IsFieldValid && _customMobileField.IsFieldValid;
            }
            if (!IsApplication && _viewOthersContainer != null && !_viewOthersContainer.Hidden)
            {
                isValid = isValid && (_txtViewReason.Text != GetI18NValue(SSMRConstants.I18N_StateReason));
            }
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void OnEdit()
        {
            if (_lblEditInfo.Hidden)
            {
                _lblEditInfo.Hidden = false;
                CGRect detailsFrame = _viewContactDetails.Frame;
                detailsFrame.Height = _lblEditInfo.Frame.GetMaxY() + GetScaledHeight(16);
                _viewContactDetails.Frame = detailsFrame;

                if (!IsApplication)
                {
                    _viewTerminateTitle.Frame = new CGRect(0, _viewContactDetails.Frame.GetMaxY()
                        , ViewWidth, _viewTerminateTitle.Frame.Height);
                    _viewTerminateContainer.Frame = new CGRect(0, _viewTerminateTitle.Frame.GetMaxY()
                        , ViewWidth, _viewTerminateContainer.Frame.Height);
                    _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewTerminateContainer.Frame.GetMaxY());
                }
                else
                {
                    _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewContactDetails.Frame.GetMaxY());
                }
            }
        }

        #region TNC Section
        private void AddTnCSection()
        {
            _viewBottomContainer = new UIView(new CGRect(0, View.Frame.Height - GetScaledHeight(160), View.Frame.Width, GetScaledHeight(160)))
            {
                BackgroundColor = UIColor.White
            };
            UIView viewPadding = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UITextView txtFieldInfo = GetInfo();
            _btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, GetYLocationFromFrame(txtFieldInfo.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                Enabled = true,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ExecuteSubmitUpdatePersonalDetail();
            }));
            _viewBottomContainer.AddSubviews(new UIView[] { viewPadding, txtFieldInfo, _btnSubmit });
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewBottomContainer.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewBottomContainer);
        }

        private UITextView GetInfo()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont("By submmitting, you are agreeing to the <a href=\"\\\"><strong>TNB Terms and Conditions</strong></a>, <a href=\"\\\"><strong>User Agreement</strong></a> and <a href=\"\\\"><strong>Privacy policy</strong></a>."
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
            UITextView txtFieldInfo = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextAlignment = UITextAlignment.Left
            };
            txtFieldInfo.TextContainerInset = UIEdgeInsets.Zero;

            txtFieldInfo.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
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
            CGSize size = txtFieldInfo.SizeThatFits(new CGSize(BaseMarginedWidth, GetScaledHeight(160)));
            txtFieldInfo.Frame = new CGRect(BaseMargin, GetScaledHeight(17), size.Width, size.Height);
            return txtFieldInfo;
        }
        #endregion

        private void AddCTA()
        {
            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(128))
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
            //_btnSubmit.Enabled = true; //false;
            //_btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;

            _btnSubmit.Enabled = true;//Temporary
            _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;

            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                //DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage = _feedbackTextView.Text ? string.Empty;
                //NavigateToPage();
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
                feedbackMesage = DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage, //Common
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