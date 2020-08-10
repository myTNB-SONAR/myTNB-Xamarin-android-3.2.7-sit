using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Feedback.FeedbackImage;
using myTNB.Home.Feedback;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
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

        public List<ImageDataEnquiryTempModel> Items;
        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList;
        SubmitFeedbackResponseModel _submitFeedback;
        public bool IsOwner;

        private UIView _viewTitleSection, _btnSubmitContainer;
        private UIView viewCheckBoxTNC;
        private UIScrollView _svContainer;
        //private UITextView txtFieldInfo;
        private UITextView lblTNC;
        private UIButton _btnSubmit;

        private UITextField txtFieldName, txtFieldEmail;
        private UIView viewLineName, viewLineEmail;
        private UIView viewEmail;
        private UILabel lblNameTitle, lblEmailTitle
            , lblNameError, lblEmailError;
        private UILabel lblEmailHint;
        private UILabel lblNameHint;
        public bool IsApplication = true;

        //add detail section
        public CustomerAccountRecordModel SelectedAccount;
        public ContactDetailsResponseModel ContactDetails;

        private UIView _viewContactDetails;
        private UIView viewName;

        private UIView viewMobile;
        private UILabel lblMobileTitle;
        private UILabel lblMobileError;
        private UILabel lblMobileHint;
        private UITextField txtFieldMobile;
        private UIView viewLineMobile;

        private CGRect scrollViewFrame;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private UIImageView imgViewCheckBoxTNC;

        UITextField textField;
        private List<ImageDataEnquiryModel> capturedImageList;
        private const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private const string MOBILENUMBER_PATTERN = @"^[0-9 \+]+$";

        public bool IsTNC { get; private set; }


        internal nfloat _navBarHeight;
        private UIView _navbarView;
        private nfloat titleBarHeight = ScaleUtility.GetScaledHeight(24f);
        private CAGradientLayer _gradientLayer;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;
            base.ViewDidLoad();

            //SetHeader();
            SetNavigation();
            AddScrollView();
            //AddCTA();
            AddTnCSection();
            AddSectionTitle();
            AddDetailsSection();
            SetEvents();
            SetSubmitButtonEnable();
            UpdateContentSize();

            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

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
            Title = GetI18NValue(EnquiryConstants.updatePersonalDetTitle);
        }

        private void SetNavigation()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
                _navBarHeight = NavigationController.NavigationBar.Frame.Height;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + _navBarHeight + 38f))
            {
                BackgroundColor = UIColor.Clear
            };

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8f), _navbarView.Frame.Width, titleBarHeight));

            UIView viewBack = new UIView(new CGRect(BaseMarginWidth16, 0, GetScaledWidth(24F), titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), titleBarHeight))
            {
                Image = UIImage.FromBundle(Constants.IMG_Back)
            };
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(56F), 0, _navbarView.Frame.Width - (GetScaledWidth(56F) * 2), titleBarHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(EnquiryConstants.updatePersonalDetTitle)
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            UIView viewStepBar = new UIView(new CGRect(0, viewTitleBar.Frame.GetMaxY() + 4f, _navbarView.Frame.Width, titleBarHeight));
            UILabel lblStep = new UILabel(new CGRect(GetScaledWidth(56F), 0, _navbarView.Frame.Width - (GetScaledWidth(56F) * 2), titleBarHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                Text = GetI18NValue(EnquiryConstants.stepTitle3of3)
            };

            lblStep.TextAlignment = UITextAlignment.Center;
            lblStep.TextColor = UIColor.White;
            viewStepBar.AddSubview(lblStep);

            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationController.PopViewController(true);
                //DismissViewController(true, null);


            }));

            _navbarView.AddSubviews(viewTitleBar, viewStepBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }


        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, _navbarView.Frame.GetMaxY(), View.Frame.Width, View.Frame.Height))
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

            lblNameTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.nameHint).ToUpper());
            lblNameError = GetErrorLabel(GetErrorI18NValue("invalid_fullname"));
            lblNameHint = GetHintLabel(GetI18NValue("nameHintBottom"));

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewName.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.nameHint)
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.displayName ?? string.Empty
            };
            if (txtFieldName.Text != string.Empty)
                lblNameTitle.Hidden = false;

            txtFieldName.ReturnKeyType = UIReturnKeyType.Done;

            viewLineName = GenericLine.GetLine(new CGRect(0, 36, viewName.Frame.Width, 1));

            viewName.AddSubviews(new UIView[] { lblNameTitle, lblNameError, lblNameHint, txtFieldName, viewLineName });
            _viewContactDetails.AddSubview(viewName);

            //Email
            viewEmail = new UIView((new CGRect(18, viewName.Frame.GetMaxY() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblEmailTitle = GetTitleLabel(GetCommonI18NValue("emailAddress").ToUpper());
            lblEmailError = GetErrorLabel(GetErrorI18NValue("invalid_email"));
            lblEmailHint = GetHintLabel("");

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue("emailAddress")
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.email ?? string.Empty
            };
            if (txtFieldEmail.Text != string.Empty)
                lblEmailTitle.Hidden = false;

            txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

            viewLineEmail = GenericLine.GetLine(new CGRect(0, 36, viewEmail.Frame.Width, 1));

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, lblEmailHint, txtFieldEmail, viewLineEmail });
            _viewContactDetails.AddSubview(viewEmail);

            //Mobile Number
            viewMobile = new UIView((new CGRect(18, viewEmail.Frame.GetMaxY() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblMobileTitle = GetTitleLabel(GetCommonI18NValue("mobileNumber").ToUpper());
            lblMobileError = GetErrorLabel(GetErrorI18NValue("invalid_mobileNumber"));
            lblMobileHint = GetHintLabel("");


            txtFieldMobile = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobile.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue("mobileNumber")
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey(),
                Text = userInfo?.mobileNo ?? string.Empty
            };
            txtFieldMobile.Text = GetNewValue();

            if (txtFieldMobile.Text != string.Empty)
                lblMobileTitle.Hidden = false;

            txtFieldMobile.KeyboardType = UIKeyboardType.NumberPad;
            //txtFieldMobile.ReturnKeyType = UIReturnKeyType.Done;

            viewLineMobile = GenericLine.GetLine(new CGRect(0, 36, viewMobile.Frame.Width, 1));

            viewMobile.AddSubviews(new UIView[] { lblMobileTitle, lblMobileError, lblMobileHint, txtFieldMobile, viewLineMobile });
            _viewContactDetails.AddSubview(viewMobile);

            _svContainer.AddSubview(_viewContactDetails);
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewContactDetails.Frame.GetMaxY() + 16f));//+ (_btnNextContainer.Frame.Height + 16f)))
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
            scrollViewFrame = _svContainer.Frame;
        }

        private string GetNewValue()
        {
            string value = "";
            value = txtFieldMobile.Text;
            if (value.Contains("+6") == false && !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
            {
                value = "+6" + value;
                if (!value.Contains("+60"))
                {
                    value = string.Empty;
                }
            }

            return value;
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (visible)
            {
                CGRect r = UIKeyboard.BoundsFromNotification(notification);
                CGRect viewFrame = View.Bounds;
                nfloat currentViewHeight = viewFrame.Height - r.Height;
                _svContainer.Frame = new CGRect(_svContainer.Frame.X, _navbarView.Frame.GetMaxY(), _svContainer.Frame.Width, currentViewHeight - _navbarView.Frame.Height);
                //ScrollView.Frame = new CGRect(0, -DeviceHelper.GetStatusBarHeight(), ScrollView.Frame.Width, currentViewHeight);

            }
            else
            {
                _svContainer.Frame = scrollViewFrame;
            }

            UIView.CommitAnimations();
        }

        private UILabel GetTitleLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 0, View.Frame.Width - 36, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
        }

        private UILabel GetErrorLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 37, View.Frame.Width - 36, 14),
                Font = MyTNBFont.MuseoSans11_300,
                TextColor = MyTNBColor.Tomato,
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
        }

        private UILabel GetHintLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 37, View.Frame.Width - 36, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
        }

        private void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UILabel textFieldHint, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            _textFieldHelper.CreateDoneButton(textField);

            textField.EditingChanged += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;

                string value = textField.Text;
                textFieldHint.Hidden = !IsEmpty(value) && !textFieldError.Hidden || value.Length == 0;

                bool hidden;
                if (textField == txtFieldMobile)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }

                SetSubmitButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
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

                textFieldHint.Hidden = !IsEmpty(value) && !textFieldError.Hidden || value.Length == 0;
                if (sender == txtFieldName)
                {
                    lblNameHint.Hidden = false;
                    lblNameError.Hidden = true;
                }

                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.TextColor = MyTNBColor.TunaGrey();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                textFieldError.Hidden = isValid;

                textFieldHint.Hidden = true;

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
            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, lblNameHint, viewLineName, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(txtFieldEmail, lblEmailTitle, lblEmailError, lblEmailHint, viewLineEmail, EMAIL_PATTERN);
            SetTextFieldEvents(txtFieldMobile, lblMobileTitle, lblMobileError, lblMobileHint, viewLineMobile, MOBILENUMBER_PATTERN);
        }

        private void SetSubmitButtonEnable()
        {
            bool isValidFieldName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern)
               && !string.IsNullOrWhiteSpace(txtFieldName.Text);
            bool isValidFieldEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
               && !string.IsNullOrWhiteSpace(txtFieldEmail.Text);

            bool isValidFieldMobile = _textFieldHelper.ValidateTextField(txtFieldMobile.Text, MOBILENUMBER_PATTERN) && _textFieldHelper.ValidateMobileNumberLength(txtFieldMobile.Text)
               && !string.IsNullOrWhiteSpace(txtFieldMobile.Text);

            //lblTNC.UserInteractionEnabled = isValidFieldEmail;

            bool isValid = isValidFieldName && isValidFieldEmail && isValidFieldMobile && IsTNC;

            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;

        }

        private void AddTnCSection()
        {
            _btnSubmitContainer = new UIView(new CGRect(0, View.Frame.Height - GetScaledHeight(160), View.Frame.Width, GetScaledHeight(160)))
            {
                BackgroundColor = UIColor.White
            };
            UIView viewPadding = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
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

            UITextView txtFieldInfo = GetInfoTNC(); //IsTNC ? GetInfoTNCRead() : GetInfoTNC();

            viewCheckBoxTNC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsTNC = !IsTNC;
                imgViewCheckBoxTNC.Hidden = !IsTNC;
                viewCheckBoxTNC.Layer.BorderColor = IsTNC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                _btnSubmitContainer.RemoveFromSuperview();
                AddTnCSection();
                txtFieldInfo = GetInfoTNC();//IsTNC ? GetInfoTNCRead() : GetInfoTNC();

                _btnSubmitContainer.AddSubviews(txtFieldInfo);
                SetSubmitButtonEnable();
            }));

            _btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(18, GetYLocationFromFrame(txtFieldInfo.Frame, 16), _btnSubmitContainer.Frame.Width - 36, 48),
                Enabled = true,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ExecuteSubmitUpdatePersonalDetail();
                //OnSubmitSMRApplication();
            }));
            _btnSubmitContainer.AddSubviews(new UIView[] { viewPadding, viewCheckBoxTNC, txtFieldInfo, _btnSubmit });
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _btnSubmitContainer.Frame = new CGRect(0, ViewHeight - containerHeight + _navBarHeight + DeviceHelper.GetStatusBarHeight(), ViewWidth, containerHeight + 30);
            View.AddSubview(_btnSubmitContainer);
        }

        private UITextView GetInfoTNC()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = IsTNC ? TextHelper.ConvertToHtmlWithFont(GetI18NValue(EnquiryConstants.enquiryTncRead) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                        , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(12F)) : TextHelper.ConvertToHtmlWithFont(GetI18NValue(EnquiryConstants.enquiryTnc) + "&nbsp;&nbsp;&nbsp;&nbsp;"
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
            UITextView lblTNC = new UITextView
            {
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextAlignment = UITextAlignment.Left,

            };
            lblTNC.TextContainerInset = UIEdgeInsets.Zero;

            lblTNC.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
            {

                UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
                EnquiryTNCViewController viewController =
                    storyBoard.InstantiateViewController("EnquiryTNCViewController") as EnquiryTNCViewController;
                if (viewController != null)
                {
                    viewController.IsOwner = IsOwner;
                    viewController.Name = txtFieldName.Text;
                    viewController.Email = txtFieldEmail.Text;
                    viewController.isPresentedVC = true;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }

            }));
            //Resize
            CGSize size = lblTNC.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));
            lblTNC.Frame = new CGRect(GetXLocationFromFrame(viewCheckBoxTNC.Frame, 8F), 16, View.Frame.Width - GetXLocationFromFrame(viewCheckBoxTNC.Frame, 8F) - 16, size.Height);///size.Height
            return lblTNC;
        }

        /*private UITextView GetInfoTNCRead()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(EnquiryConstants.enquiryTncRead)
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

                UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
                EnquiryTNCViewController viewController =
                    storyBoard.InstantiateViewController("EnquiryTNCViewController") as EnquiryTNCViewController;
                if (viewController != null)
                {
                    viewController.IsOwner = IsOwner;
                    viewController.Name = txtFieldName.Text;
                    viewController.Email = txtFieldEmail.Text;
                    viewController.isPresentedVC = true;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }

            }));
            //Resize
            CGSize size = lblTNC.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));
            lblTNC.Frame = new CGRect(GetXLocationFromFrame(viewCheckBoxTNC.Frame, 8F), 16, View.Frame.Width - GetXLocationFromFrame(viewCheckBoxTNC.Frame, 8F), size.Height);
            return lblTNC;
        }*/

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
                attachment = ConvertImage(),//Items
                feedbackCategoryId = "4",
                feedbackTypeId = "",
                accountNum = DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA,
                name = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].displayName : txtFieldName.Text,
                email = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].email : txtFieldEmail.Text,
                phoneNum = DataManager.DataManager.SharedInstance.IsLoggedIn() ? DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo : txtFieldMobile.Text,
                feedbackMesage = string.Empty, //Common
                stateId = "",
                location = "",
                poleNum = "",
                contactName = txtFieldName.Text,
                contactMobileNo = txtFieldMobile.Text,
                contactEmailAddress = txtFieldEmail.Text,
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

        private List<ImageDataEnquiryModel> ConvertImage()
        {
            capturedImageList = new List<ImageDataEnquiryModel>();
            UIImageHelper _imageHelper = new UIImageHelper();
            ImageDataEnquiryModel imgData;

            for (int i = 0; i < Items.Count; i++)
            {
                imgData = new ImageDataEnquiryModel();

                imgData.fileType = "jpeg";
                imgData.fileHex = _imageHelper.ConvertImageToHex(Items[i].tempImage);
                imgData.fileSize = _imageHelper.GetImageFileSize(Items[i].tempImage).ToString();
                imgData.fileName = Items[i].fileName;
                capturedImageList.Add(imgData);

            }

            return capturedImageList;

        }

    }
}