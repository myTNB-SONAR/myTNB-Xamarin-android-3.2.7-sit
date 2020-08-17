using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Home.Bill;
using myTNB.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class UpdatePersonalDetailViewController : CustomUIViewController
    {
        private UIScrollView _svContainer;
        private UIView _viewContainerRelationship;
        private UIView _viewTitleSection, _viewTitleSection2, _viewYesNoContainer, _viewTitleSectionRelationship;
        private UIView _btnNextContainer;
        private UIButton _btnNext;
        private CustomUIButtonV2 _btnNo, _btnYes;
        private CustomUIView _yesnoToolTipsView;
        private CustomUIView _ownerConsentToolTipsView;

        private bool IsOwner = true;

        private bool IsIC = false;
        private bool IsAccount = false;
        private bool IsMobileNumber = false;
        private bool IsEmail = false;
        private bool IsMailing = false;
        private bool IsPremise = false;

        private bool IsSpecifyOther = false;

        private string IC = string.Empty;
        private string Account = string.Empty;
        private string MobileNumber = string.Empty;
        private string Email = string.Empty;
        private string Mailing = string.Empty;
        private string Premise = string.Empty;

        private UIView _viewCheckBoxListContainer;

        private UIView _viewCheckBoxContainerIC;
        private UIView viewCheckBoxIC;
        private UIImageView imgViewCheckBoxIC;
        private UILabel lblIC;

        private UIView _viewCheckBoxContainerAccount;
        private UIView viewCheckBoxAccount;
        private UIImageView imgViewCheckBoxAccount;
        private UILabel lblAccount;

        private UIView _viewCheckBoxContainerMobileNumber;
        private UIView viewCheckBoxMobileNumber;
        private UIImageView imgViewCheckBoxMobileNumber;
        private UILabel lblMobileNumber;

        private UIView _viewCheckBoxContainerEmail;
        private UIView viewCheckBoxEmail;
        private UIImageView imgViewCheckBoxEmail;
        private UILabel lblEmail;

        private UIView _viewCheckBoxContainerMailing;
        private UIView viewCheckBoxMaling;
        private UIImageView imgViewCheckBoxMaling;
        private UILabel lblMailing;

        private UIView _viewCheckBoxContainerPremise;
        private UIView viewCheckBoxPremise;
        private UIImageView imgViewCheckBoxPremise;
        private UILabel lblPremise;

        private UIView viewAccountRelationTypeContainer;
        private UIView viewAccountRelation;
        private UILabel lblAccountRelationTitle;
        private UILabel lblRelationError;
        private UILabel lblRelation;
        private UIView viewLineRelation;

        private UIView viewICNumber;
        private UILabel lblICNoTitle;
        private UILabel lblICNoError;
        private UITextField txtFieldICNo;
        private UIView viewLineICNo;

        private UILabel lblICNoHint;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private List<string> _typeRelationshipNameList = new List<string>();

        List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList;

        private const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private const string MOBILENUMBER_PATTERN = @"^[0-9 \+]+$";
        private const string ADDRESS_PATTERN = @"^.*$"; //@"^[A-Za-z0-9 ]*$" @"^.{5,100}$" @"^.*$"
        UITextField textField;

        //Specify Other
        private UIView viewSpecifyOther;
        private UILabel lblSpecifyOtherTitle;
        private UILabel lblSpecifyOtherError;
        private UITextField txtFieldSpecifyOther;
        private UIView viewLineSpecifyOther;
        //Owner account name
        private UIView viewAccOwnerName;
        private UILabel lblAccOwnerNameTitle;
        private UILabel lblAccOwnerNameError;
        private UITextField txtFieldAccOwnerName;
        private UIView viewLineAccOwnerName;
        //Mobile Number
        private UIView viewMobileNumber;
        private UILabel lblMobileNumberTitle;
        private UILabel lblMobileNumberError;
        private UITextField txtFieldMobileNumber;
        private UIView viewLineMobile;
        //Email
        private UIView viewEmail;
        private UILabel lblEmailTitle;
        private UILabel lblEmailError;
        private UITextField txtFieldEmail;
        private UIView viewLineEmailLine;
        //Mailing
        private UIView viewMailing;
        private UILabel lblMailingTitle;
        private UILabel lblMailingError;
        private UITextField txtFieldMailing;
        private UIView viewLineMailinglLine;
        //Premise
        private UIView viewPremise;
        private UILabel lblPremiseTitle;
        private UILabel lblPremiseError;
        private UITextField txtFieldPremise;
        private UIView viewLinePremiselLine;

        internal nfloat _navBarHeight;
        private UIView _navbarView;
        private nfloat titleBarHeight = ScaleUtility.GetScaledHeight(24f);
        private CAGradientLayer _gradientLayer;

        private CGRect scrollViewFrame;
        private CGRect keyboardRectangle;


        public UpdatePersonalDetailViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();

            DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex = 0;

            InitilizeRelationshipType();
            //SetHeader();
            SetNavigation();
            AddScrollView();
            AddCTA();
            AddYesNo();

            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
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
                Text = GetI18NValue(EnquiryConstants.stepTitle1of3)
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

        private void AddYesNo()
        {
            _viewTitleSection = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.registeredTitle),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };

            _viewTitleSection.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection);

            _viewYesNoContainer = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(120)))
            {
                BackgroundColor = UIColor.White
            };

            nfloat btnWidth = (BaseMarginedWidth - GetScaledWidth(4)) / 2;
            _btnNo = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, GetScaledHeight(16), btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnNo.SetTitle(GetCommonI18NValue("no"), UIControlState.Normal);
            _btnNo.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnNo.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnYes = new CustomUIButtonV2
            {
                Frame = new CGRect(_btnNo.Frame.GetMaxX() + GetScaledWidth(4), GetScaledHeight(16), btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnYes.SetTitle(GetCommonI18NValue("yes"), UIControlState.Normal);
            _btnYes.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnYes.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnNo.TouchUpInside += (sender, e) =>
            {
                _btnNo.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnNo.BackgroundColor = MyTNBColor.FreshGreen;

                IsOwner = false;

                removeRelationshipBelow();
                ResetCheckbox();
                //GetTextUIField();

                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();
                SetEvents();
                DisableButton();

                //SetTextUIField();

                _btnYes.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
                _btnYes.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                _btnYes.BackgroundColor = UIColor.White;

            };
            _btnYes.TouchUpInside += (sender, e) =>
            {
                _btnYes.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnYes.BackgroundColor = MyTNBColor.FreshGreen;

                IsOwner = true;

                removeRelationshipBelow();
                ResetCheckbox();
                //GetTextUIField();

                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();
                SetEvents();
                DisableButton();

                //SetTextUIField();

                _btnNo.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
                _btnNo.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                _btnNo.BackgroundColor = UIColor.White;

            };

            _viewYesNoContainer.AddSubviews(new UIView[] { _btnNo, _btnYes });
            _viewYesNoContainer.AddSubview(GetYesNoTooltipView(GetYLocationFromFrame(_btnNo.Frame, 16)));

            _svContainer.AddSubview(_viewYesNoContainer);
        }

        private void AddSectionTitleRelationship()
        {
            _viewTitleSectionRelationship = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.ownerTitle),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };
            _viewTitleSectionRelationship.AddSubview(lblSectionTitle);

            _viewContainerRelationship = new UIView(new CGRect(0, _viewYesNoContainer.Frame.GetMaxY(), View.Frame.Width, IsSpecifyOther ? GetScaledHeight(222) : GetScaledHeight(155)));

            viewAccountRelationTypeContainer = new UIView((new CGRect(0, _viewTitleSectionRelationship.Frame.GetMaxY(), View.Frame.Width, IsSpecifyOther ? GetScaledHeight(150) : GetScaledHeight(83))))
            {
                BackgroundColor = UIColor.White
            };


            viewAccountRelation = new UIView((new CGRect(18, 18, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.White
            };

            lblAccountRelationTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAccountRelation.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.relationshipTitle) //relationshipTitle
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
            viewAccountRelation.AddSubview(lblAccountRelationTitle);

            lblRelationError = new UILabel
            {
                Frame = new CGRect(0, 37, viewAccountRelation.Frame.Width, 14),
                Font = MyTNBFont.MuseoSans11_300,
                TextColor = MyTNBColor.Tomato,
                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_InvalidAccountType)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            viewAccountRelation.AddSubview(lblRelationError);


            lblRelation = new UILabel(new CGRect(0, 12, viewAccountRelation.Frame.Width, 24))
            {

                Text = _typeRelationshipNameList[DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex],
                TextColor = MyTNBColor.TunaGrey()
            };

            viewAccountRelation.AddSubview(lblRelation);

            UIImageView imgDropDown = new UIImageView(new CGRect(viewAccountRelation.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };
            viewAccountRelation.AddSubview(imgDropDown);

            viewLineRelation = GenericLine.GetLine(new CGRect(0, 36, viewAccountRelation.Frame.Width, 1));
            viewAccountRelation.AddSubview(viewLineRelation);

            UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = GetI18NValue(EnquiryConstants.relationshipTitle);
                viewController.Items = new List<string>()
                    {
                        GetI18NValue(EnquiryConstants.childTitle),
                        GetI18NValue(EnquiryConstants.tenantTitle),
                        GetI18NValue(EnquiryConstants.guardianTitle),
                        GetI18NValue(EnquiryConstants.parentTitle),
                        GetI18NValue(EnquiryConstants.spouseTitle),
                        GetI18NValue(EnquiryConstants.othersTitle)
                    };
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);

            });
            viewAccountRelation.AddGestureRecognizer(tapAccounType);

            //Specify Other reason
            viewSpecifyOther = new UIView((new CGRect(18, viewAccountRelation.Frame.GetMaxY() + 18, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear,
                Hidden = !IsSpecifyOther
            };

            lblSpecifyOtherTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.otherRelationshipHint));
            lblSpecifyOtherError = GetErrorLabel("");

            txtFieldSpecifyOther = new UITextField
            {
                Frame = new CGRect(0, 12, viewSpecifyOther.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.otherRelationshipHint)
                , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldSpecifyOther.ReturnKeyType = UIReturnKeyType.Done;

            viewLineSpecifyOther = GenericLine.GetLine(new CGRect(0, 36, viewSpecifyOther.Frame.Width, 1));
            viewSpecifyOther.AddSubviews(new UIView[] { lblSpecifyOtherTitle, lblSpecifyOtherError, txtFieldSpecifyOther, viewLineSpecifyOther });

            //Other reason

            viewAccountRelationTypeContainer.AddSubviews(viewAccountRelation, viewSpecifyOther);//viewSpecifyOther

            _viewContainerRelationship.AddSubviews(_viewTitleSectionRelationship, viewAccountRelationTypeContainer);
            _svContainer.AddSubviews(_viewContainerRelationship);

        }

        /*private void AddUpdateIC()
        {
            _viewTitleICSection = new UIView(new CGRect(0, _viewYesNoContainer.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitleIC = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "Update your identification number below:",
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };

            _viewTitleICSection.AddSubview(lblSectionTitleIC);
            _svContainer.AddSubview(_viewTitleICSection); //add title label

            _viewUpdateICContainer = new UIView(new CGRect(0, _viewTitleICSection.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(120)))
            {
                BackgroundColor = UIColor.White
            };

            _lblTitle = new UILabel(new CGRect(GetScaledWidth(16), GetScaledHeight(16), View.Frame.Width - GetScaledWidth(32), GetScaledHeight(12)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_10_300,
                TextAlignment = UITextAlignment.Left,
                Text = "Current Identification Number"

            };
            _lblValue = new UILabel(new CGRect(GetScaledWidth(16), _lblTitle.Frame.GetMaxY(), View.Frame.Width - GetScaledWidth(32), GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_300,
                TextAlignment = UITextAlignment.Left
            };

            UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
            ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            string value = string.Empty;
            string action = string.Empty;

            string icNo = userInfo?.identificationNo;
            if (!string.IsNullOrEmpty(icNo) && icNo.Length > 4)
            {
                string lastDigit = icNo.Substring(icNo.Length - 4);
                icNo = "******-**-" + lastDigit;
            }
            _lblValue.Text = icNo;

            _viewUpdateICContainer.AddSubviews(new UIView[] { _lblTitle, _lblValue});

            //IC Number
            UIView viewICNumber = new UIView((new CGRect(18, _lblValue.Frame.GetMaxY() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblICNoTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_IDNumber));
            lblICNoError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidIDNumber));

            txtFieldICNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewICNumber.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_IDNumber)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };

            txtFieldICNo.KeyboardType = UIKeyboardType.Default;
            txtFieldICNo.ReturnKeyType = UIReturnKeyType.Done;

            viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNumber.Frame.Width, 1));
            viewICNumber.AddSubviews(new UIView[] { lblICNoTitle, lblICNoError, txtFieldICNo, viewLineICNo });
            _viewUpdateICContainer.AddSubview(viewICNumber);

            _svContainer.AddSubview(_viewUpdateICContainer);

        }*/

        private void checkBoxList()
        {

            _viewTitleSection2 = new UIView(new CGRect(0, IsOwner ? _viewYesNoContainer.Frame.GetMaxY() : _viewContainerRelationship.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };

            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = IsOwner ? GetI18NValue(EnquiryConstants.updateNoOwnerTitle) : GetI18NValue(EnquiryConstants.updateOwnerTitle),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };

            _viewTitleSection2.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection2);

            _viewCheckBoxListContainer = new UIView(new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), ViewWidth, GetScaledHeight(116F)));

            checkBoxIC();
            checkBoxAccountOwnerName();
            checkBoxMobileNumber();
            checkBoxEmail();
            checkBoxAccountMailing();
            checkBoxAccountPremise();
            OwnerConsentShow();

            _viewCheckBoxListContainer.AddSubviews(_viewCheckBoxContainerIC, _viewCheckBoxContainerAccount, _viewCheckBoxContainerMobileNumber, _viewCheckBoxContainerEmail, _viewCheckBoxContainerMailing, _viewCheckBoxContainerPremise);
            _svContainer.AddSubviews(_viewCheckBoxListContainer);

        }

        private void checkBoxIC()
        {
            _viewCheckBoxContainerIC = new UIView(new CGRect(0, 0, ViewWidth, IsIC ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxIC = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxIC.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxIC.Layer.BorderColor = IsIC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxIC.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxIC = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsIC ? false : true
            };
            viewCheckBoxIC.AddSubview(imgViewCheckBoxIC);

            viewCheckBoxIC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsIC = !IsIC;
                imgViewCheckBoxIC.Hidden = !IsIC;
                viewCheckBoxIC.Layer.BorderColor = IsIC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (IC != null && txtFieldICNo != null)
                    IC = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblIC = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxIC.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerIC.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.icTitle)
            };
            _viewCheckBoxContainerIC.AddSubviews(new UIView[] { viewCheckBoxIC, lblIC });

            //IC Number
            if (IsIC)
            {
                viewICNumber = new UIView((new CGRect(18, lblIC.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblICNoTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.icHint).ToUpper());
                lblICNoError = GetErrorLabel(GetI18NValue(EnquiryConstants.icReq));

                txtFieldICNo = new UITextField
                {
                    Frame = new CGRect(0, 12, viewICNumber.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.icHint)
                    , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (IC != string.Empty)
                    lblICNoTitle.Hidden = false;

                txtFieldICNo.ReturnKeyType = UIReturnKeyType.Done;

                viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNumber.Frame.Width, 1));

                viewICNumber.AddSubviews(new UIView[] { lblICNoTitle, lblICNoError, txtFieldICNo, viewLineICNo });
                _viewCheckBoxContainerIC.AddSubview(viewICNumber);

            }
        }
        private void checkBoxAccountOwnerName()
        {
            _viewCheckBoxContainerAccount = new UIView(new CGRect(0, _viewCheckBoxContainerIC.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsAccount ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxAccount = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxAccount.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxAccount.Layer.BorderColor = IsAccount ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxAccount.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxAccount = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsAccount ? false : true
            };
            viewCheckBoxAccount.AddSubview(imgViewCheckBoxAccount);

            viewCheckBoxAccount.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsAccount = !IsAccount;
                imgViewCheckBoxAccount.Hidden = !IsAccount;
                viewCheckBoxAccount.Layer.BorderColor = IsAccount ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (Account != null && txtFieldAccOwnerName != null)
                    Account = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblAccount = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxAccount.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerAccount.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.accNametitle)
            };

            _viewCheckBoxContainerAccount.AddSubviews(new UIView[] { viewCheckBoxAccount, lblAccount });

            if (IsAccount)
            {
                //Account's Owner Name
                viewAccOwnerName = new UIView((new CGRect(18, lblAccount.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblAccOwnerNameTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.accNameHint).ToUpper());
                lblAccOwnerNameError = GetErrorLabel(GetI18NValue(EnquiryConstants.ownerReq));

                txtFieldAccOwnerName = new UITextField
                {
                    Frame = new CGRect(0, 12, viewAccOwnerName.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.accNameHint)
                    , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (Account != string.Empty)
                    lblAccOwnerNameTitle.Hidden = false;

                txtFieldAccOwnerName.ReturnKeyType = UIReturnKeyType.Done;

                viewLineAccOwnerName = GenericLine.GetLine(new CGRect(0, 36, viewAccOwnerName.Frame.Width, 1));

                viewAccOwnerName.AddSubviews(new UIView[] { lblAccOwnerNameTitle, lblAccOwnerNameError, txtFieldAccOwnerName, viewLineAccOwnerName });
                _viewCheckBoxContainerAccount.AddSubview(viewAccOwnerName);
            }
        }
        private void checkBoxMobileNumber()
        {
            _viewCheckBoxContainerMobileNumber = new UIView(new CGRect(0, _viewCheckBoxContainerAccount.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsMobileNumber ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxMobileNumber = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxMobileNumber.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxMobileNumber.Layer.BorderColor = IsMobileNumber ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxMobileNumber.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxMobileNumber = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsMobileNumber ? false : true
            };
            viewCheckBoxMobileNumber.AddSubview(imgViewCheckBoxMobileNumber);

            viewCheckBoxMobileNumber.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsMobileNumber = !IsMobileNumber;
                imgViewCheckBoxMobileNumber.Hidden = !IsMobileNumber;
                viewCheckBoxMobileNumber.Layer.BorderColor = IsMobileNumber ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (MobileNumber != null && txtFieldMobileNumber != null)
                    MobileNumber = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblMobileNumber = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxMobileNumber.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerMobileNumber.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.mobileNumberTitle)
            };

            _viewCheckBoxContainerMobileNumber.AddSubviews(new UIView[] { viewCheckBoxMobileNumber, lblMobileNumber });

            if (IsMobileNumber)
            {
                //Mobile Number
                viewMobileNumber = new UIView((new CGRect(18, lblMobileNumber.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblMobileNumberTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.mobileNumberHint));
                lblMobileNumberError = GetErrorLabel(GetI18NValue(EnquiryConstants.mobileReq));

                txtFieldMobileNumber = new UITextField
                {
                    Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.mobileNumberHint)
                        , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (MobileNumber != string.Empty)
                    lblMobileNumberTitle.Hidden = false;

                txtFieldMobileNumber.KeyboardType = UIKeyboardType.NumberPad;
                //txtFieldMobileNumber.ReturnKeyType = UIReturnKeyType.Done;

                viewLineMobile = GenericLine.GetLine(new CGRect(0, 36, viewMobileNumber.Frame.Width, 1));

                viewMobileNumber.AddSubviews(new UIView[] { lblMobileNumberTitle, lblMobileNumberError, txtFieldMobileNumber, viewLineMobile });
                _viewCheckBoxContainerMobileNumber.AddSubview(viewMobileNumber);
            }
        }
        private void checkBoxEmail()
        {
            _viewCheckBoxContainerEmail = new UIView(new CGRect(0, _viewCheckBoxContainerMobileNumber.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsEmail ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxEmail = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxEmail.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxEmail.Layer.BorderColor = IsEmail ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxEmail.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxEmail = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsEmail ? false : true
            };
            viewCheckBoxEmail.AddSubview(imgViewCheckBoxEmail);

            viewCheckBoxEmail.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsEmail = !IsEmail;
                imgViewCheckBoxEmail.Hidden = !IsEmail;
                viewCheckBoxEmail.Layer.BorderColor = IsEmail ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (Email != null && txtFieldEmail != null)
                    Email = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblEmail = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxEmail.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerEmail.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.emailAddressTitle)
            };

            _viewCheckBoxContainerEmail.AddSubviews(new UIView[] { viewCheckBoxEmail, lblEmail });

            if (IsEmail)
            {
                //Email
                viewEmail = new UIView((new CGRect(18, lblEmail.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblEmailTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.emailAddressHint));
                lblEmailError = GetErrorLabel(GetI18NValue(EnquiryConstants.emailReq));

                txtFieldEmail = new UITextField
                {
                    Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.emailAddressHint)
                        , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (Email != string.Empty)
                    lblEmailTitle.Hidden = false;

                txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

                viewLineEmailLine = GenericLine.GetLine(new CGRect(0, 36, viewEmail.Frame.Width, 1));

                viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, txtFieldEmail, viewLineEmailLine });
                _viewCheckBoxContainerEmail.AddSubview(viewEmail);
            }
        }
        private void checkBoxAccountMailing()
        {
            _viewCheckBoxContainerMailing = new UIView(new CGRect(0, _viewCheckBoxContainerEmail.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsMailing ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxMaling = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxMaling.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxMaling.Layer.BorderColor = IsMailing ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxMaling.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxMaling = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsMailing ? false : true
            };
            viewCheckBoxMaling.AddSubview(imgViewCheckBoxMaling);

            viewCheckBoxMaling.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsMailing = !IsMailing;
                imgViewCheckBoxMaling.Hidden = !IsMailing;
                viewCheckBoxMaling.Layer.BorderColor = IsMailing ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (Mailing != null && txtFieldMailing != null)
                    Mailing = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblMailing = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxMaling.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerMailing.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.mailingAddressTitle)

            };
            _viewCheckBoxContainerMailing.AddSubviews(new UIView[] { viewCheckBoxMaling, lblMailing });

            if (IsMailing)
            {
                //Mailing
                viewMailing = new UIView((new CGRect(18, lblMailing.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblMailingTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.mailingAddressHint));
                lblMailingError = GetErrorLabel(GetI18NValue(EnquiryConstants.mailingReq));

                txtFieldMailing = new UITextField
                {
                    Frame = new CGRect(0, 12, viewMailing.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.mailingAddressHint)
                    , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (Mailing != string.Empty)
                    lblMailingTitle.Hidden = false;

                txtFieldMailing.ReturnKeyType = UIReturnKeyType.Done;

                viewLineMailinglLine = GenericLine.GetLine(new CGRect(0, 36, viewMailing.Frame.Width, 1));

                viewMailing.AddSubviews(new UIView[] { lblMailingTitle, lblMailingError, txtFieldMailing, viewLineMailinglLine });
                _viewCheckBoxContainerMailing.AddSubview(viewMailing);
            }
        }
        private void checkBoxAccountPremise()
        {
            _viewCheckBoxContainerPremise = new UIView(new CGRect(0, _viewCheckBoxContainerMailing.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsPremise ? GetScaledHeight(119F) : GetScaledHeight(56F)))
            {
                BackgroundColor = UIColor.White
            };
            viewCheckBoxPremise = new UIView(new CGRect(GetScaledWidth(16F), GetScaledHeight(16F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBoxPremise.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBoxPremise.Layer.BorderColor = IsPremise ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            viewCheckBoxPremise.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBoxPremise = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear,
                Hidden = IsPremise ? false : true
            };
            viewCheckBoxPremise.AddSubview(imgViewCheckBoxPremise);

            viewCheckBoxPremise.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsPremise = !IsPremise;
                imgViewCheckBoxPremise.Hidden = !IsPremise;
                viewCheckBoxPremise.Layer.BorderColor = IsPremise ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                if (Premise != null && txtFieldPremise != null)
                    Premise = string.Empty;

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
                SetEvents();
                //DisableButton();
                SetNextButtonEnable();

            }));

            lblPremise = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxPremise.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerPremise.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(EnquiryConstants.premiseAddressTitle)
            };

            _viewCheckBoxContainerPremise.AddSubviews(new UIView[] { viewCheckBoxPremise, lblPremise });

            if (IsPremise)
            {
                //Premise
                viewPremise = new UIView((new CGRect(18, lblPremise.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblPremiseTitle = GetTitleLabel(GetI18NValue(EnquiryConstants.premiseAddressHint));
                lblPremiseError = GetErrorLabel(GetI18NValue(EnquiryConstants.permisesReq));

                txtFieldPremise = new UITextField
                {
                    Frame = new CGRect(0, 12, viewPremise.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.premiseAddressHint)
                    , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };
                if (Premise != string.Empty)
                    lblPremiseTitle.Hidden = false;

                txtFieldPremise.ReturnKeyType = UIReturnKeyType.Done;

                viewLinePremiselLine = GenericLine.GetLine(new CGRect(0, 36, viewPremise.Frame.Width, 1));

                viewPremise.AddSubviews(new UIView[] { lblPremiseTitle, lblPremiseError, txtFieldPremise, viewLinePremiselLine });
                _viewCheckBoxContainerPremise.AddSubview(viewPremise);
            }

        }

        private void OwnerConsentShow()
        {
            _viewCheckBoxListContainer.AddSubview(GetOwnerConsentTooltipView(GetYLocationFromFrame(_viewCheckBoxContainerPremise.Frame, 16)));
            _ownerConsentToolTipsView.Hidden = IsOwner ? true : false;
        }

        private void AddCTA()
        {
            nfloat containerHeight = GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;
            nfloat yLoc = View.Frame.Height - DeviceHelper.TopSafeAreaInset - NavigationController.NavigationBar.Frame.Height - containerHeight;
            if (DeviceHelper.IsIOS10AndBelow)
            {
                yLoc = ViewHeight - containerHeight;
            }

            _btnNextContainer = new UIView(new CGRect(0, yLoc + _navBarHeight + DeviceHelper.GetStatusBarHeight()
                , View.Frame.Width, DeviceHelper.GetScaledHeight(100F) + 30))
            {
                BackgroundColor = UIColor.White
            };

            _btnNext = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnNextContainer.Frame.Width - 36, 48)
            };
            _btnNext.SetTitle(GetCommonI18NValue("next"), UIControlState.Normal);
            _btnNext.Font = MyTNBFont.MuseoSans18_300;
            _btnNext.Layer.CornerRadius = 5.0f;
            _btnNext.Enabled = false;
            _btnNext.BackgroundColor = MyTNBColor.SilverChalice;
            _btnNext.TouchUpInside += (sender, e) =>
            {
                NavigateToPage();
            };
            _btnNextContainer.AddSubview(_btnNext);
            View.AddSubview(_btnNextContainer);
        }

        public CustomUIView GetYesNoTooltipView(nfloat yLoc)
        {
            _yesnoToolTipsView = new CustomUIView(new CGRect(0, yLoc, View.Frame.Width, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMarginWidth16, 0, View.Frame.Width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _yesnoToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.registeredInfo)

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert(GetI18NValue(EnquiryConstants.registeredInfo) //registeredInfo
                    , GetI18NValue(EnquiryConstants.registeredInfoDetail)
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , null);
            });
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            _yesnoToolTipsView.AddGestureRecognizer(tapInfo);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _yesnoToolTipsView.AddSubview(viewInfo);

            return _yesnoToolTipsView;
        }

        public CustomUIView GetOwnerConsentTooltipView(nfloat yLoc)
        {
            _ownerConsentToolTipsView = new CustomUIView(new CGRect(0, yLoc, View.Frame.Width, GetScaledHeight(80)))
            {
                BackgroundColor = UIColor.Clear
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMarginWidth16, 0, View.Frame.Width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _ownerConsentToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_11_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.ownerConsentInfo)

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert(GetI18NValue(EnquiryConstants.ownerConsentTitle)
                    , GetI18NValue(EnquiryConstants.ownerConsentDescription)
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , null);
            });
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            _ownerConsentToolTipsView.AddGestureRecognizer(tapInfo);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _ownerConsentToolTipsView.AddSubview(viewInfo);

            return _ownerConsentToolTipsView;
        }


        private void UpdateCheckBoxListHeight()
        {
            _viewCheckBoxListContainer.Frame = new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), ViewWidth, GetScaledHeight(_ownerConsentToolTipsView.Frame.GetMaxY()) + 32f);
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewCheckBoxListContainer.Frame.GetMaxY() + 16f));//+ (_btnNextContainer.Frame.Height + 16f)))
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
            scrollViewFrame = _svContainer.Frame;
        }


        private void SetTextFieldEvents(UILabel lblTitle, UITextField textField
               , UILabel lblError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            _textFieldHelper.CreateDoneButton(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                SetNextButtonEnable();
                //if (textField == txtFieldSpecifyOther && textField.Text.Length == 0 && IsSpecifyOther)
                //{
                //    DisableButton();
                //}
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                if (textField == txtFieldMobileNumber && textField.Text.Length == 0)
                {
                    textField.Text += TNBGlobal.MobileNoPrefix;
                }
                string value = textField.Text;
                bool hidden;
                if (textField == txtFieldMobileNumber)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }

                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.LeftViewMode = UITextFieldViewMode.Never;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                //lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                //Handling for Confirm Email
                if (textField == txtFieldEmail)
                {
                    bool isMatch = txtFieldEmail.Text.Equals(txtFieldEmail.Text);
                    lblError.Text = isValid ? GetErrorI18NValue(Constants.Error_MismatchedEmail)
                        : GetErrorI18NValue(Constants.Error_InvalidEmailAddress);
                    isValid = isValid && isMatch;
                }
                //Handling for Account Name
                else if (textField == txtFieldAccOwnerName)
                {
                    isValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                }
                lblError.Hidden = isValid;
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (textField == txtFieldAccOwnerName)
                {
                    bool isCharValid = string.IsNullOrEmpty(replacementString)
                        || _textFieldHelper.ValidateTextField(replacementString, pattern);
                    if (!isCharValid)
                    {
                        return false;
                    }
                }
                else if (txtField == txtFieldMobileNumber)
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

        private void SetNextButtonEnable()
        {
            bool isValidFieldIC, isValidFieldAccOwnerName,
                isValidFieldMobile, isValidFieldEmail, isValidFieldMailing,
                isValidFieldPremise, isValidFieldOther;
            bool isValid = true;

            DisableButton();

            if (IsSpecifyOther)
            {
                bool specifyValid = IsIC || IsAccount || IsMobileNumber || IsEmail || IsMailing || IsPremise;
                isValidFieldOther = _textFieldHelper.ValidateTextField(txtFieldSpecifyOther.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldSpecifyOther.Text) && specifyValid != false && txtFieldSpecifyOther.Text.Length <= 50;

                if (!isValidFieldOther)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }

            if (IsIC)
            {
                isValidFieldIC = _textFieldHelper.ValidateTextField(txtFieldICNo.Text, TNBGlobal.IC_NO_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldICNo.Text);

                if (!isValidFieldIC)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }
            if (IsAccount)
            {
                isValidFieldAccOwnerName = _textFieldHelper.ValidateTextField(txtFieldAccOwnerName.Text, TNBGlobal.ACCOUNT_NAME_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldAccOwnerName.Text);

                if (!isValidFieldAccOwnerName)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }
            if (IsMobileNumber)
            {
                isValidFieldMobile = _textFieldHelper.ValidateTextField(txtFieldMobileNumber.Text, MOBILENUMBER_PATTERN) && _textFieldHelper.ValidateMobileNumberLength(txtFieldMobileNumber.Text)
                   && !string.IsNullOrWhiteSpace(txtFieldMobileNumber.Text);

                if (!isValidFieldMobile)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }
            if (IsEmail)
            {
                isValidFieldEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldEmail.Text);

                if (!isValidFieldEmail)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }
            if (IsMailing)
            {
                isValidFieldMailing = _textFieldHelper.ValidateTextField(txtFieldMailing.Text, ADDRESS_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldMailing.Text);

                if (!isValidFieldMailing)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }
            if (IsPremise)
            {
                isValidFieldPremise = _textFieldHelper.ValidateTextField(txtFieldPremise.Text, ADDRESS_PATTERN)
                   && !string.IsNullOrWhiteSpace(txtFieldPremise.Text);

                if (!isValidFieldPremise)
                {
                    DisableButton();
                    isValid = false;
                    //return;
                }
            }

            if (isValid)
            {
                if (IsIC || IsAccount || IsMobileNumber || IsEmail || IsMailing || IsPremise)
                {
                    _btnNext.Enabled = true;// isValid;
                    _btnNext.BackgroundColor = MyTNBColor.FreshGreen;// isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
                }
            }

        }


        private void DisableButton()
        {
            _btnNext.Enabled = false;
            _btnNext.BackgroundColor = MyTNBColor.SilverChalice;

        }

        private void SetEvents()
        {
            if (IsSpecifyOther)
            {
                SetTextFieldEvents(lblSpecifyOtherTitle, txtFieldSpecifyOther, lblSpecifyOtherError, viewLineSpecifyOther, TNBGlobal.CustomerNamePattern);
            }
            if (IsIC)
            {
                SetTextFieldEvents(lblICNoTitle, txtFieldICNo, lblICNoError, viewLineICNo, TNBGlobal.IC_NO_PATTERN);
            }
            if (IsAccount)
            {
                SetTextFieldEvents(lblAccOwnerNameTitle, txtFieldAccOwnerName, lblAccOwnerNameError, viewLineAccOwnerName, TNBGlobal.CustomerNamePattern);
            }
            if (IsMobileNumber)
            {
                SetTextFieldEvents(lblMobileNumberTitle, txtFieldMobileNumber, lblMobileNumberError, viewLineMobile, MOBILENUMBER_PATTERN);
            }
            if (IsEmail)
            {
                SetTextFieldEvents(lblEmailTitle, txtFieldEmail, lblEmailError, viewLineEmailLine, EMAIL_PATTERN);
            }
            if (IsMailing)
            {
                SetTextFieldEvents(lblMailingTitle, txtFieldMailing, lblMailingError, viewLineMailinglLine, ADDRESS_PATTERN);
            }
            if (IsPremise)
            {
                SetTextFieldEvents(lblPremiseTitle, txtFieldPremise, lblPremiseError, viewLinePremiselLine, ADDRESS_PATTERN);
            }
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

        private bool IsEmpty(string value)
        {
            if (textField == txtFieldMobileNumber)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) { return true; }
                int countryCodeIndex = value.IndexOf(@"+60", 0, StringComparison.CurrentCulture);
                return countryCodeIndex > -1 && value.Length == 3;
            }
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex = index;
            lblRelation.Text = _typeRelationshipNameList[DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex];

            if (index == 5)
            {
                GetTextUIField();

                removeRelationshipBelow();
                IsSpecifyOther = true;
                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();
                SetEvents();
                DisableButton();
                //SetNextButtonEnable();

                SetTextUIField();
            }
            else
            {
                GetTextUIField();

                removeRelationshipBelow();
                IsSpecifyOther = false;
                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();
                SetEvents();
                DisableButton();
                //SetNextButtonEnable();

                SetTextUIField();
            }
        }

        private void removeCheckBoxBelow()
        {
            if (_viewTitleSection2 != null)
                _viewTitleSection2.RemoveFromSuperview();
            if (_viewCheckBoxListContainer != null)
                _viewCheckBoxListContainer.RemoveFromSuperview();
        }

        private void removeRelationshipBelow()
        {
            if (_viewContainerRelationship != null)
                _viewContainerRelationship.RemoveFromSuperview();
            if (_viewTitleSection2 != null)
                _viewTitleSection2.RemoveFromSuperview();
            if (_viewCheckBoxListContainer != null)
                _viewCheckBoxListContainer.RemoveFromSuperview();
        }

        private void InitilizeRelationshipType()
        {
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.childTitle));
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.tenantTitle)); //Friend
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.guardianTitle));
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.parentTitle));
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.spouseTitle));
            _typeRelationshipNameList.Add(GetI18NValue(EnquiryConstants.othersTitle));

        }

        private void ResetCheckbox()
        {
            IsIC = false;
            IsAccount = false;
            IsMobileNumber = false;
            IsEmail = false;
            IsMailing = false;
            IsPremise = false;

            IsSpecifyOther = false;
        }

        private void GetTextUIField()
        {
            if (IC != null && txtFieldICNo != null)
                IC = txtFieldICNo.Text;

            if (Account != null && txtFieldAccOwnerName != null)
                Account = txtFieldAccOwnerName.Text;

            if (MobileNumber != null && txtFieldMobileNumber != null)
                MobileNumber = txtFieldMobileNumber.Text;

            if (Email != null && txtFieldEmail != null)
                Email = txtFieldEmail.Text;

            if (Mailing != null && txtFieldMailing != null)
                Mailing = txtFieldMailing.Text;

            if (Premise != null && txtFieldPremise != null)
                Premise = txtFieldPremise.Text;
        }

        private void SetTextUIField()
        {
            if (IC != null && txtFieldICNo != null)
                txtFieldICNo.Text = IC;

            if (Account != null && txtFieldAccOwnerName != null)
                txtFieldAccOwnerName.Text = Account;

            if (MobileNumber != null && txtFieldMobileNumber != null)
                txtFieldMobileNumber.Text = MobileNumber;

            if (Email != null && txtFieldEmail != null)
                txtFieldEmail.Text = Email;

            if (Mailing != null && txtFieldMailing != null)
                txtFieldMailing.Text = Mailing;

            if (Premise != null && txtFieldPremise != null)
                txtFieldPremise.Text = Premise;
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
                _svContainer.Frame = new CGRect(_svContainer.Frame.X, _navbarView.Frame.GetMaxY() , _svContainer.Frame.Width, currentViewHeight -_navbarView.Frame.Height);
                //ScrollView.Frame = new CGRect(0, -DeviceHelper.GetStatusBarHeight(), ScrollView.Frame.Width, currentViewHeight);

            }
            else
            {
                _svContainer.Frame = scrollViewFrame;
            }

            UIView.CommitAnimations();
        }

        private void NavigateToPage()
        {

            feedbackUpdateDetailsList = new List<FeedbackUpdateDetailsModel>();

            if (IsOwner == false)
            {
                int relationship = DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex + 1;
                string relationshipDesc = _typeRelationshipNameList[DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex];

                if (IsSpecifyOther && relationship == 6)
                {
                    if (txtFieldSpecifyOther.Text != string.Empty)
                    {
                        DataManager.DataManager.SharedInstance.Relationship = relationship;
                        DataManager.DataManager.SharedInstance.RelationshipDesc = txtFieldSpecifyOther.Text ?? relationshipDesc;
                    }
                    else
                    {
                        DisableButton(); return;
                    }
                }
                else
                {
                    DataManager.DataManager.SharedInstance.Relationship = relationship;
                    DataManager.DataManager.SharedInstance.RelationshipDesc = relationshipDesc;
                }
            }
            else
            {
                int relationship = 0;
                string relationshipDesc = null;

                DataManager.DataManager.SharedInstance.Relationship = relationship;
                DataManager.DataManager.SharedInstance.RelationshipDesc = relationshipDesc;
            }

            if (IsIC)
            {
                int FeedbackUpdInfoType = 1;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.icTitle);//"Identification number (IC/Passport)";
                string FeedbackUpdInfoValue = txtFieldICNo.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }
            if (IsAccount)
            {
                int FeedbackUpdInfoType = 2;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.accNametitle);//"Account Name";
                string FeedbackUpdInfoValue = txtFieldAccOwnerName.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }
            if (IsMobileNumber)
            {
                int FeedbackUpdInfoType = 3;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.mobileNumberTitle);//"Mobile Number";
                string FeedbackUpdInfoValue = txtFieldMobileNumber.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }
            if (IsEmail)
            {
                int FeedbackUpdInfoType = 4;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.emailAddressTitle);//"Email Address";
                string FeedbackUpdInfoValue = txtFieldEmail.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }
            if (IsMailing)
            {
                int FeedbackUpdInfoType = 5;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.mailingAddressTitle); //"Mailing Address";
                string FeedbackUpdInfoValue = txtFieldMailing.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }
            if (IsPremise)
            {
                int FeedbackUpdInfoType = 6;
                string FeedbackUpdInfoTypeDesc = GetI18NValue(EnquiryConstants.premiseAddressTitle);// "Premises Address";
                string FeedbackUpdInfoValue = txtFieldPremise.Text;

                FeedbackUpdateDetailsModel feedbackUpdateDetailsModel = new FeedbackUpdateDetailsModel();
                feedbackUpdateDetailsModel.FeedbackUpdInfoType = FeedbackUpdInfoType;
                feedbackUpdateDetailsModel.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                feedbackUpdateDetailsModel.FeedbackUpdInfoValue = FeedbackUpdInfoValue;

                feedbackUpdateDetailsList.Add(feedbackUpdateDetailsModel);
            }

            UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
            UpdatePersonalDetail2ViewController viewController =
                storyBoard.InstantiateViewController("UpdatePersonalDetail2ViewController") as UpdatePersonalDetail2ViewController;
            viewController.IsOwner = IsOwner;
            viewController.IsPremise = IsPremise;
            viewController.feedbackUpdateDetailsList = feedbackUpdateDetailsList;
            NavigationController?.PushViewController(viewController, true);


        }
    }
}