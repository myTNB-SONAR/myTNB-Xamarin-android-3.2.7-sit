using CoreGraphics;
using Foundation;
using myTNB.Home.Bill;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
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
        private UIView _btnSubmitContainer;
        private UIButton _btnSubmit;
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
        private UIView viewLineIC;

        private UIView _viewCheckBoxContainerAccount;
        private UIView viewCheckBoxAccount;
        private UIImageView imgViewCheckBoxAccount;
        private UILabel lblAccount;
        private UIView viewLineAccount;

        private UIView _viewCheckBoxContainerMobileNumber;
        private UIView viewCheckBoxMobileNumber;
        private UIImageView imgViewCheckBoxMobileNumber;
        private UILabel lblMobileNumber;
        private UIView viewLineMobileNumber;

        private UIView _viewCheckBoxContainerEmail;
        private UIView viewCheckBoxEmail;
        private UIImageView imgViewCheckBoxEmail;
        private UILabel lblEmail;
        private UIView viewLineEmail;

        private UIView _viewCheckBoxContainerMailing;
        private UIView viewCheckBoxMaling;
        private UIImageView imgViewCheckBoxMaling;
        private UILabel lblMailing;
        private UIView viewLineMailing;

        private UIView _viewCheckBoxContainerPremise;
        private UIView viewCheckBoxPremise;
        private UIImageView imgViewCheckBoxPremise;
        private UILabel lblPremise;
        private UIView viewLinePremise;

        private UIView viewAccountRelationTypeContainer;
        private UIView viewAccountRelation;
        private UILabel lblAccountRelationTitle;
        private UILabel lblRelationError;
        private UILabel lblRelation;
        private UIView viewLineRelation;
        private UIView _viewTitleICSection;
        private UIView _viewUpdateICContainer;
        private UILabel _lblTitle;
        private UILabel _lblValue;
        private UIView viewICNumber;
        private UILabel lblICNoTitle;
        private UILabel lblICNoError;
        private UITextField txtFieldICNo;
        private UIView viewLineICNo;

        private UILabel lblICNoHint;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private List<string> _typeRelationshipNameList = new List<string>();

        List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList;

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

        public UpdatePersonalDetailViewController (IntPtr handle) : base (handle){ }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex = 0;

            InitilizeRelationshipType();
            SetHeader();
            AddScrollView();
            AddCTA();
            AddYesNo();

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
                Text = "Is this electricity account registered under your name?",
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
            _btnNo.SetTitle("No", UIControlState.Normal);
            _btnNo.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnNo.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnYes = new CustomUIButtonV2
            {
                Frame = new CGRect(_btnNo.Frame.GetMaxX() + GetScaledWidth(4), GetScaledHeight(16), btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnYes.SetTitle("Yes", UIControlState.Normal);
            _btnYes.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnYes.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnNo.TouchUpInside += (sender, e) =>
            {
                _btnNo.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnNo.BackgroundColor = MyTNBColor.FreshGreen;

                IsOwner = false;

                removeRelationshipBelow();

                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                //temporary
                _btnSubmit.Enabled = true;
                _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;

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

                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                //SetVisibility();
                //SetViews();

                //temporary
                _btnSubmit.Enabled = true;
                _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;


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
                Text = "What is your relationship with the owner?",
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2
            };
            _viewTitleSectionRelationship.AddSubview(lblSectionTitle);

            _viewContainerRelationship = new UIView(new CGRect(0, _viewYesNoContainer.Frame.GetMaxY(), View.Frame.Width, IsSpecifyOther? GetScaledHeight(222) : GetScaledHeight(155)));

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
                    AttributedText = AttributedStringUtility.GetAttributedString("RELATIONSHIP WITH OWNER"
                        , AttributedStringUtility.AttributedStringType.Title),
                    TextAlignment = UITextAlignment.Left
                };
                viewAccountRelation.AddSubview(lblAccountRelationTitle);

                lblRelationError = new UILabel
                {
                    Frame = new CGRect(0, 37, viewAccountRelation.Frame.Width, 14),
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
                    viewController.Title = "Relationship with Owner";
                    viewController.Items = new List<string>()
                    {
                        "Child",
                        "Tenant", 
                        "Guardian",
                        "Parent",
                        "Spouse",
                        "Others"
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

                lblSpecifyOtherTitle = GetTitleLabel("PLEASE SPECIFY YOUR RELATIONSHIP");
                lblSpecifyOtherError = GetErrorLabel("");

                txtFieldSpecifyOther = new UITextField
                {
                    Frame = new CGRect(0, 12, viewSpecifyOther.Frame.Width, 24),
                    TextColor = MyTNBColor.TunaGrey()
                };
                txtFieldSpecifyOther.ReturnKeyType = UIReturnKeyType.Done;

                viewLineSpecifyOther = GenericLine.GetLine(new CGRect(0, 36, viewSpecifyOther.Frame.Width, 1));
                viewSpecifyOther.AddSubviews(new UIView[] { lblSpecifyOtherTitle, lblSpecifyOtherError, txtFieldSpecifyOther, viewLineSpecifyOther });


                SetTextFieldEvents(txtFieldSpecifyOther, lblSpecifyOtherTitle, lblSpecifyOtherError
                   , viewLineSpecifyOther, lblICNoHint, TNBGlobal.CustomerNamePattern);

                //Other reason

                viewAccountRelationTypeContainer.AddSubviews(viewAccountRelation, viewSpecifyOther);//viewSpecifyOther

                _viewContainerRelationship.AddSubviews(_viewTitleSectionRelationship, viewAccountRelationTypeContainer);
                _svContainer.AddSubviews(_viewContainerRelationship);

            }

        private void AddUpdateIC()
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

            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError
               , viewLineICNo, lblICNoHint, TNBGlobal.IC_NO_PATTERN);
        }

        private void checkBoxList()
        {

            _viewTitleSection2 = new UIView(new CGRect(0, IsOwner ? _viewYesNoContainer.Frame.GetMaxY() : _viewContainerRelationship.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(72)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            //}
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = IsOwner ? "Which information you would like to update? (Optional)" : "Which information would you like to update on your owner's behalf?",
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
                Hidden = IsIC? false : true
            };
            viewCheckBoxIC.AddSubview(imgViewCheckBoxIC);

            viewCheckBoxIC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsIC = !IsIC;
                imgViewCheckBoxIC.Hidden = !IsIC;
                viewCheckBoxIC.Layer.BorderColor = IsIC ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

                GetTextUIField();

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();

            }));

            lblIC = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxIC.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerIC.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Identification Number (IC / Passport)"
            };
            _viewCheckBoxContainerIC.AddSubviews(new UIView[] { viewCheckBoxIC, lblIC});

            //IC Number
            if (IsIC)
            {
                viewICNumber = new UIView((new CGRect(18, lblIC.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblICNoTitle = GetTitleLabel("ENTER NEW IDENTIFICATION NUMBER");
                lblICNoError = GetErrorLabel("");

                txtFieldICNo = new UITextField
                {
                    Frame = new CGRect(0, 12, viewICNumber.Frame.Width, 24),
                    AttributedPlaceholder = AttributedStringUtility.GetAttributedString(""
                    , AttributedStringUtility.AttributedStringType.Value),
                    TextColor = MyTNBColor.TunaGrey()
                };

                txtFieldICNo.ReturnKeyType = UIReturnKeyType.Done;

                viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNumber.Frame.Width, 1));

                viewICNumber.AddSubviews(new UIView[] { lblICNoTitle, lblICNoError, txtFieldICNo, viewLineICNo });
                _viewCheckBoxContainerIC.AddSubview(viewICNumber);

                SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError
                , viewLineICNo, lblICNoHint, TNBGlobal.CustomerNamePattern);
            }
        }
        private void checkBoxAccountOwnerName()
        {
            _viewCheckBoxContainerAccount = new UIView(new CGRect(0, _viewCheckBoxContainerIC.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsAccount ? GetScaledHeight(119F) : GetScaledHeight(56F))) //60f
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

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
            }));

            lblAccount = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxAccount.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerAccount.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Account's Owner Name"
            };

            _viewCheckBoxContainerAccount.AddSubviews(new UIView[] { viewCheckBoxAccount, lblAccount});

            if (IsAccount)
            {
                //Account's Owner Name
                viewAccOwnerName = new UIView((new CGRect(18, lblAccount.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblAccOwnerNameTitle = GetTitleLabel(IsOwner ? "ENTER NEW ACCOUNT" : "ENTER NEW ACCOUNT'S OWNER NAME");
                lblAccOwnerNameError = GetErrorLabel("");

                txtFieldAccOwnerName = new UITextField
                {
                    Frame = new CGRect(0, 12, viewAccOwnerName.Frame.Width, 24),
                    TextColor = MyTNBColor.TunaGrey()
                };
                txtFieldAccOwnerName.ReturnKeyType = UIReturnKeyType.Done;

                viewLineAccOwnerName = GenericLine.GetLine(new CGRect(0, 36, viewAccOwnerName.Frame.Width, 1));

                viewAccOwnerName.AddSubviews(new UIView[] { lblAccOwnerNameTitle, lblAccOwnerNameError, txtFieldAccOwnerName, viewLineAccOwnerName });
                _viewCheckBoxContainerAccount.AddSubview(viewAccOwnerName);
            }
        }
        private void checkBoxMobileNumber()
        {
            _viewCheckBoxContainerMobileNumber = new UIView(new CGRect(0, _viewCheckBoxContainerAccount.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsMobileNumber ? GetScaledHeight(119F) : GetScaledHeight(56F))) //60F
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

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();

            }));

            lblMobileNumber = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxMobileNumber.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerMobileNumber.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Mobile Number"
            };

            _viewCheckBoxContainerMobileNumber.AddSubviews(new UIView[] { viewCheckBoxMobileNumber, lblMobileNumber});

            if (IsMobileNumber)
            { 
            //Mobile Number
            viewMobileNumber = new UIView((new CGRect(18, lblMobileNumber.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblMobileNumberTitle = GetTitleLabel("ENTER NEW MOBILE NUMBER");
            lblMobileNumberError = GetErrorLabel("");

            txtFieldMobileNumber = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldMobileNumber.ReturnKeyType = UIReturnKeyType.Done;

            viewLineMobile = GenericLine.GetLine(new CGRect(0, 36, viewMobileNumber.Frame.Width, 1));

            viewMobileNumber.AddSubviews(new UIView[] { lblMobileNumberTitle, lblMobileNumberError, txtFieldMobileNumber, viewLineMobile });
            _viewCheckBoxContainerMobileNumber.AddSubview(viewMobileNumber);
            }
        }
        private void checkBoxEmail()
        {
            _viewCheckBoxContainerEmail = new UIView(new CGRect(0, _viewCheckBoxContainerMobileNumber.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsEmail ? GetScaledHeight(119F) : GetScaledHeight(56F))) //60F
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

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();
            }));

            lblEmail = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxEmail.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerEmail.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Email Address"
            };

            _viewCheckBoxContainerEmail.AddSubviews(new UIView[] { viewCheckBoxEmail, lblEmail});

            if (IsEmail)
            { 
            //Email
            viewEmail = new UIView((new CGRect(18, lblEmail.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblEmailTitle = GetTitleLabel("ENTER NEW EMAIL ADDRESS");
            lblEmailError = GetErrorLabel("");

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                TextColor = MyTNBColor.TunaGrey()
            };

            txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

            viewLineEmailLine = GenericLine.GetLine(new CGRect(0, 36, viewEmail.Frame.Width, 1));

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, txtFieldEmail, viewLineEmailLine });
            _viewCheckBoxContainerEmail.AddSubview(viewEmail);
            }
        }
        private void checkBoxAccountMailing()
        {
            _viewCheckBoxContainerMailing = new UIView(new CGRect(0, _viewCheckBoxContainerEmail.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsMailing ? GetScaledHeight(119F) : GetScaledHeight(56F))) //60F
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

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();

            }));

            lblMailing = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxMaling.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerMailing.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Mailing Address"
            };
            _viewCheckBoxContainerMailing.AddSubviews(new UIView[] { viewCheckBoxMaling, lblMailing });

            if (IsMailing)
            {
                //Mailing
                viewMailing = new UIView((new CGRect(18, lblMailing.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblMailingTitle = GetTitleLabel("ENTER NEW MAILING ADDRESS");
                lblMailingError = GetErrorLabel("");

                txtFieldMailing = new UITextField
                {
                    Frame = new CGRect(0, 12, viewMailing.Frame.Width, 24),
                    TextColor = MyTNBColor.TunaGrey()
                };
                txtFieldMailing.ReturnKeyType = UIReturnKeyType.Done;

                viewLineMailinglLine = GenericLine.GetLine(new CGRect(0, 36, viewMailing.Frame.Width, 1));

                viewMailing.AddSubviews(new UIView[] { lblMailingTitle, lblMailingError, txtFieldMailing, viewLineMailinglLine });
                _viewCheckBoxContainerMailing.AddSubview(viewMailing);
            }
        }
        private void checkBoxAccountPremise()
        {
            _viewCheckBoxContainerPremise = new UIView(new CGRect(0, _viewCheckBoxContainerMailing.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, IsPremise ? GetScaledHeight(119F) : GetScaledHeight(56F))) //60
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

                removeCheckBoxBelow();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

                SetTextUIField();

            }));

            lblPremise = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBoxPremise.Frame, 18F), GetScaledHeight(16F), _viewCheckBoxContainerPremise.Frame.Width - 23, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Premise Address"
            };

            _viewCheckBoxContainerPremise.AddSubviews(new UIView[] { viewCheckBoxPremise, lblPremise});

            if (IsPremise)
            {
                //Premise
                viewPremise = new UIView((new CGRect(18, lblPremise.Frame.GetMaxY() + 14, View.Frame.Width - 36, 51)))
                {
                    BackgroundColor = UIColor.Clear
                };

                lblPremiseTitle = GetTitleLabel("ENTER NEW PERMISE ADDRESS");
                lblPremiseError = GetErrorLabel("");

                txtFieldPremise = new UITextField
                {
                    Frame = new CGRect(0, 12, viewPremise.Frame.Width, 24),
                    TextColor = MyTNBColor.TunaGrey()
                };
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
            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145F))
                , View.Frame.Width, DeviceHelper.GetScaledHeight(100F)))
            {
                BackgroundColor = UIColor.White
            };

            _btnSubmit = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnSubmitContainer.Frame.Width - 36, 48)
            };
            _btnSubmit.SetTitle("Next", UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                NavigateToPage();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
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
                Text = "Who is registered owner?"

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert("Who is registered owner?"
                    , "This electricity account must be registered under your name with your IC or Passport."
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
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "Do I need owner's consent?"

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert("Do I need owner's consent?"
                    , "You as a non-owner or an authorised person are required to provide the owner’s proof of consent to update the personal details on their behalf as it will permanently update the owner’s TNB electricity account details. You will also consent to update your own contact information as it is still the owner’s property."
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
            _viewCheckBoxListContainer.Frame = new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), ViewWidth, GetScaledHeight(_ownerConsentToolTipsView.Frame.GetMaxY() + GetScaledHeight(24)));
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewCheckBoxListContainer.Frame.GetMaxY())); //+ (_btnSubmitContainer.Frame.Height + 50f)));
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight() + GetScaledHeight(80F)).Size;
        }


        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle
            , UILabel lblError, UIView viewLine, UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                //DisplayEyeIcon(textField);
                //SetRegisterButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                //DisplayEyeIcon(textField);
                textField.LeftViewMode = UITextFieldViewMode.Never;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                /*//Handling for Confirm Email
                if (textField == txtFieldConfirmEmail)
                {
                    bool isMatch = txtFieldEmail.Text.Equals(txtFieldConfirmEmail.Text);
                    lblError.Text = isValid ? GetErrorI18NValue(Constants.Error_MismatchedEmail)
                        : GetErrorI18NValue(Constants.Error_InvalidEmailAddress);
                    isValid = isValid && isMatch;
                }
                //Handling for Confirm Password
                else if (textField == txtFieldConfirmPassword)
                {
                    bool isMatch = txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
                    lblError.Text = isValid ? GetErrorI18NValue(Constants.Error_MismatchedPassword)
                        : GetHintI18NValue(Constants.Hint_Password);
                    isValid = isValid && isMatch;
                }
                else*/
                //if (textField == txtFieldSpecifyOther)
                //{
                //    isValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                //}
                //DisplayEyeIcon(textField);*/
                lblError.Hidden = isValid;
                lblHint.Hidden = true;
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
                if (textField == txtFieldSpecifyOther)
                {
                    bool isCharValid = string.IsNullOrEmpty(replacementString)
                        || _textFieldHelper.ValidateTextField(replacementString, pattern);
                    if (!isCharValid)
                    {
                        return false;
                    }
                }
                return true;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        private void SetRegisterButtonEnable()
        {
            bool isValidFieldSpecifyOther = _textFieldHelper.ValidateTextField(txtFieldSpecifyOther.Text, TNBGlobal.CustomerNamePattern)
                && !string.IsNullOrWhiteSpace(txtFieldSpecifyOther.Text);
            //bool isValidFieldICNo = _textFieldHelper.ValidateTextField(txtFieldICNo.Text, TNBGlobal.IC_NO_PATTERN);
            //bool isValidMobileNo = _mobileNumberComponent.MobileNumber.IsValid();
            //bool isValidEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
            //    && _textFieldHelper.ValidateTextField(txtFieldConfirmEmail.Text, EMAIL_PATTERN)
            //    && txtFieldEmail.Text.Equals(txtFieldConfirmEmail.Text);
            //bool isValidPassword = _textFieldHelper.ValidateTextField(txtFieldPassword.Text, PASSWORD_PATTERN)
            //    && _textFieldHelper.ValidateTextField(txtFieldConfirmPassword.Text, PASSWORD_PATTERN)
            //    && txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
            //bool isValid = isValidName && isValidICNo && isValidMobileNo
            //    && isValidEmail && isValidPassword;
            bool isValid = isValidFieldSpecifyOther;  //&& isValidFieldICNo;

            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void SetViews()
        {
            //_textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");

            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
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
                TextAlignment = UITextAlignment.Left
            };
        }

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex = index;
            lblRelation.Text = _typeRelationshipNameList[DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex];

            if (index == 5)
            {
                removeRelationshipBelow();
                IsSpecifyOther = true;
                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

            }
            else
            {
                removeRelationshipBelow();
                IsSpecifyOther = false;
                AddSectionTitleRelationship();
                checkBoxList();
                UpdateCheckBoxListHeight();
                UpdateContentSize();

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
            if(_viewContainerRelationship != null)
                _viewContainerRelationship.RemoveFromSuperview();
            if(_viewTitleSection2 != null)
                _viewTitleSection2.RemoveFromSuperview();
            if(_viewCheckBoxListContainer != null)
                _viewCheckBoxListContainer.RemoveFromSuperview();
        }

        private void InitilizeRelationshipType()
        {
            _typeRelationshipNameList.Add("Child");
            _typeRelationshipNameList.Add("Tenant"); //Friend
            _typeRelationshipNameList.Add("Guardian");
            _typeRelationshipNameList.Add("Parent");
            _typeRelationshipNameList.Add("Spouse");
            _typeRelationshipNameList.Add("Others");
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

        private void NavigateToPage()
        {
            feedbackUpdateDetailsList = new List<FeedbackUpdateDetailsModel>();

            int relationship = DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex + 1;
            string relationshipDesc = _typeRelationshipNameList[DataManager.DataManager.SharedInstance.CurrentSelectedRelationshipTypeNoIndex];

            if (IsSpecifyOther && relationship ==6 )
            {
                DataManager.DataManager.SharedInstance.Relationship = relationship;
                DataManager.DataManager.SharedInstance.RelationshipDesc = txtFieldSpecifyOther.Text;
            }
            else
            {
                DataManager.DataManager.SharedInstance.Relationship = relationship;
                DataManager.DataManager.SharedInstance.RelationshipDesc = relationshipDesc;
            }

            if (IsIC)
            {
                int FeedbackUpdInfoType = 1;
                string FeedbackUpdInfoTypeDesc = "Identification number (IC/Passport)";
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
                string FeedbackUpdInfoTypeDesc = "Account Name";
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
                string FeedbackUpdInfoTypeDesc = "Mobile Number";
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
                string FeedbackUpdInfoTypeDesc = "Email Address";
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
                string FeedbackUpdInfoTypeDesc = "Mailing Address";
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
                string FeedbackUpdInfoTypeDesc = "Premises Address";
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
            viewController.feedbackUpdateDetailsList = feedbackUpdateDetailsList;
            NavigationController?.PushViewController(viewController, true);

     
        }
    }
}