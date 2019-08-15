using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.Registration;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class SSMRApplicationViewController : CustomUIViewController
    {
        public SSMRApplicationViewController(IntPtr handle) : base(handle) { }
        public bool IsApplication;

        private UIButton _btnSubmit;
        private UIView _viewBottomContainer, _viewContactDetails, _viewTerminate
            , _viewTerminateTitle, _viewTerminateContainer, _viewOthersContainer
            , _viewLineTerminate, _viewLineReason, _viewMainDetails, _viewApplyForTitle
            , _viewContactDetailsTitle, _viewAccountContainer;
        private UIScrollView _scrollContainer;
        private CGRect _scrollViewFrame;
        private CustomTextField _customMobileField;
        private CustomTextField _customEmailField;
        protected List<CustomerAccountRecordModel> _eligibleAccountList;
        protected AccountsSMREligibilityResponseModel _smrEligibleList;
        protected CustomerAccountRecordModel _selectedAccount;
        protected ContactDetailsResponseModel _contactDetails;
        protected SSMRApplicationStatusResponseModel _ssmrApplicationStatus;
        protected TerminationReasonsResponseModel _ssmrTerminationReasons;
        private int _selectedAccountIndex = -1;
        private int _selectedTerminateReasonIndex = 0;
        private UILabel _lblAccountName, _lblAddress, _lblEditInfo, _lblTerminateReason, _lblReason;
        private UITextView _txtViewReason;

        public override void ViewDidLoad()
        {
            PageName = SSMRConstants.Pagename_SSMRApplication;
            NavigationController.NavigationBarHidden = false;
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(SSMRConstants.Notification_SelectSSMRAccount, OnSelectSSMRAccount);
            ConfigureNavigationBar();
            AddTnCSection();
            GetEligibleAccounts();
            AddDetailsSection();
            if (!IsApplication)
            {
                AddTerminateReason();
            }
            ToggleCTA();
            if (!IsApplication)
            {
                OnGetContactInfo();
                OnGetTerminateReasons();
            }
            if (IsApplication && _eligibleAccountList != null && _eligibleAccountList.Count > 0)
            {
                OnGetAccountsSMREligibility();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null)
            {
                NavigationController.NavigationBarHidden = false;
            }
        }

        private void OnSelectSSMRAccount(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SSMRApplicationViewController OnSelectSSMRAccount");
            OnGetContactInfo();
        }

        private void ConfigureNavigationBar()
        {
            UIImage backImg = UIImage.FromBundle(SSMRConstants.IMG_BackIcon);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                ViewHelper.DismissControllersAndSelectTab(this, 0, true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitle);
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
            {
                return;
            }

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
                _scrollContainer.Frame = new CGRect(_scrollContainer.Frame.X, _scrollContainer.Frame.Y
                    , _scrollContainer.Frame.Width, currentViewHeight);
            }
            else
            {
                _scrollContainer.Frame = _scrollViewFrame;
            }
            UIView.CommitAnimations();
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
                Font = TNBFont.MuseoSans_16_500,
                Enabled = true,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnSubmitSMRApplication();
            }));
            _viewBottomContainer.AddSubviews(new UIView[] { viewPadding, txtFieldInfo, _btnSubmit });
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewBottomContainer.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewBottomContainer);
        }

        private UITextView GetInfo()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = new NSAttributedString(GetI18NValue(IsApplication
                 ? SSMRConstants.I18N_TnCSubscribe : SSMRConstants.I18N_TnCUnsubscribe)
                                                                           , new NSAttributedStringDocumentAttributes
                                                                           {
                                                                               DocumentType = NSDocumentType.HTML,
                                                                               StringEncoding = NSStringEncoding.UTF8,
                                                                           }
                                                                           , ref htmlBodyError);
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_12_500,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_300
            }, new NSRange(0, htmlBody.Length));
            UITextView txtFieldInfo = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextAlignment = UITextAlignment.Left
            };
            txtFieldInfo.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                TermsAndConditionViewController viewController =
                    storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
                if (viewController != null)
                {
                    viewController.isPresentedVC = true;
                    UINavigationController navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                }
            }));
            //Resize
            CGSize size = txtFieldInfo.SizeThatFits(new CGSize(BaseMarginedWidth, GetScaledHeight(160)));
            txtFieldInfo.Frame = new CGRect(BaseMargin, GetScaledHeight(17), size.Width, size.Height);
            return txtFieldInfo;
        }
        #endregion

        #region Details Section
        private void AddDetailsSection()
        {
            _scrollContainer = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - _viewBottomContainer.Frame.Height))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            _viewApplyForTitle = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UILabel lblApplyFor = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(IsApplication ? SSMRConstants.I18N_ApplyingFor : SSMRConstants.I18N_TerminateFor)
            };
            _viewApplyForTitle.AddSubview(lblApplyFor);

            _viewMainDetails = new UIView() { BackgroundColor = UIColor.White };

            _viewAccountContainer = new UIView();
            UILabel lblAccountName = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                TextAlignment = UITextAlignment.Left,
                Text = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName ?? TNBGlobal.EMPTY_ADDRESS
            };
            _viewAccountContainer.AddSubview(lblAccountName);
            _viewAccountContainer.Frame = new CGRect(0, 0, ViewWidth, lblAccountName.Frame.GetMaxY() + GetScaledHeight(8));

            _lblAddress = new UILabel(new CGRect(BaseMargin, _viewAccountContainer.Frame.GetMaxY(), BaseMarginedWidth
               , GetScaledHeight(40)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = _selectedAccount != null && !string.IsNullOrEmpty(_selectedAccount.accountStAddress)
                   ? _selectedAccount.accountStAddress : TNBGlobal.EMPTY_ADDRESS
            };

            _viewMainDetails.AddSubviews(new UIView[] { _viewAccountContainer, _lblAddress });
            _viewMainDetails.Frame = new CGRect(0, _viewApplyForTitle.Frame.GetMaxY(), ViewWidth, _lblAddress.Frame.GetMaxY() + GetScaledHeight(16));

            _viewContactDetailsTitle = new UIView(new CGRect(0, _viewMainDetails.Frame.GetMaxY(), ViewWidth, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UILabel lblContactDetails = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(SSMRConstants.I18N_ContactDetails)
            };
            _viewContactDetailsTitle.AddSubview(lblContactDetails);

            _viewContactDetails = new UIView(new CGRect(0, _viewContactDetailsTitle.Frame.GetMaxY(), ViewWidth, GetScaledHeight(142)))
            {
                BackgroundColor = UIColor.White
            };

            _customEmailField = new CustomTextField(_viewContactDetails, new CGPoint(GetScaledWidth(16), GetScaledHeight(16)))
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

            _customMobileField = new CustomTextField(_viewContactDetails, new CGPoint(GetScaledWidth(16), GetScaledHeight(75)))
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

            if (IsApplication)
            {
                _customEmailField.SetEnable = false;
                _customMobileField.SetEnable = false;
            }

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

            _viewContactDetails.AddSubviews(new UIView[] { viewEmail, viewMobile, _lblEditInfo });

            _scrollContainer.AddSubviews(new UIView[] { _viewApplyForTitle, _viewMainDetails, _viewContactDetailsTitle, _viewContactDetails });
            _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewContactDetails.Frame.GetMaxY());
            View.AddSubview(_scrollContainer);
            _scrollViewFrame = _scrollContainer.Frame;
        }
        #endregion

        private void AddTerminateReason()
        {
            UILabel lblReasonTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(120)))
            {
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(SSMRConstants.I18N_TerminateTitle),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            CGSize newLblSize = GetLabelSize(lblReasonTitle, lblReasonTitle.Frame.Width, GetScaledHeight(120));
            CGRect newFrame = lblReasonTitle.Frame;
            newFrame.Height = newLblSize.Height;
            lblReasonTitle.Frame = newFrame;
            _viewTerminateTitle = new UIView(new CGRect(0, _viewContactDetails.Frame.GetMaxY()
               , ViewWidth, lblReasonTitle.Frame.GetMaxY() + GetScaledHeight(8)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };
            _viewTerminateTitle.AddSubview(lblReasonTitle);

            UILabel lblTerminateTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(12)))
            {
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_10_300,
                Text = GetI18NValue(SSMRConstants.I18N_SelectReason).ToUpper()
            };
            _viewTerminate = new UIView(new CGRect(BaseMargin, GetScaledHeight(28), BaseMarginedWidth, GetScaledHeight(24)));

            UIImageView imgDropdown = new UIImageView(new CGRect(_viewTerminate.Frame.Width - GetScaledWidth(30), 0, GetScaledWidth(24), GetScaledWidth(24)))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Dropdow)
            };
            _lblTerminateReason = new UILabel(new CGRect(0, 0, _viewTerminate.Frame.Width - GetScaledWidth(32), GetScaledHeight(24)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_300
            };
            _viewTerminate.AddSubviews(new UIView[] { _lblTerminateReason, imgDropdown });
            _viewLineTerminate = new UIView(new CGRect(BaseMargin, GetYLocationFromFrame(_viewTerminate.Frame, 1), BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkTwo
            };

            #region Reason TextView
            _lblReason = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(12)))
            {
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_10_300,
                Text = GetI18NValue(SSMRConstants.I18N_StateReason).ToUpper(),
                Hidden = true
            };
            _txtViewReason = new UITextView(new CGRect(BaseMargin, GetScaledHeight(12), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = true,
                ScrollEnabled = true,
                AutocorrectionType = UITextAutocorrectionType.No,
                AutocapitalizationType = UITextAutocapitalizationType.None,
                SpellCheckingType = UITextSpellCheckingType.No,
                ReturnKeyType = UIReturnKeyType.Default,
                Text = GetI18NValue(SSMRConstants.I18N_StateReason)
            };
            SetTextViewActions();

            _viewLineReason = new UIView(new CGRect(BaseMargin, GetYLocationFromFrame(_txtViewReason.Frame, 1), BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkTwo
            };

            _viewOthersContainer = new UIView(new CGRect(0, GetYLocationFromFrame(_viewLineTerminate.Frame, 22)
                    , ViewWidth, _viewLineReason.Frame.GetMaxY() + GetScaledHeight(22)))
            { Hidden = true };
            _viewOthersContainer.AddSubviews(new UIView[] { _lblReason, _txtViewReason, _viewLineReason });
            #endregion

            _viewTerminateContainer = new UIView(new CGRect(0, _viewTerminateTitle.Frame.GetMaxY(), ViewWidth, _viewLineTerminate.Frame.GetMaxY() + GetScaledHeight(22)));
            _viewTerminateContainer.AddSubviews(new UIView[] { lblTerminateTitle, _viewTerminate, _viewLineTerminate, _viewOthersContainer });
            _viewTerminateContainer.BackgroundColor = UIColor.White;

            _scrollContainer.AddSubviews(new UIView[] { _viewTerminateTitle, _viewTerminateContainer });
            _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewTerminateContainer.Frame.GetMaxY());
        }

        private void SetTextViewActions()
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIBarButtonItem doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                _lblReason.Hidden = _txtViewReason.Text.Length == GetI18NValue(SSMRConstants.I18N_StateReason).Length;
                _txtViewReason.ResignFirstResponder();
                ToggleCTA();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            _txtViewReason.InputAccessoryView = toolbar;
            _txtViewReason.Changed += (sender, e) =>
            {
                CGRect frame = _txtViewReason.Frame;
                frame.Height = _txtViewReason.ContentSize.Height;
                _txtViewReason.Frame = frame;

                CGRect lineFrame = _viewLineReason.Frame;
                lineFrame.Y = frame.GetMaxY() + GetScaledHeight(1);
                _viewLineReason.Frame = lineFrame;

                CGRect otherContainerFrame = _viewOthersContainer.Frame;
                otherContainerFrame.Height = lineFrame.GetMaxY() + GetScaledHeight(22);
                _viewOthersContainer.Frame = otherContainerFrame;

                CGRect terminateFrame = _viewTerminateContainer.Frame;
                terminateFrame.Height = otherContainerFrame.GetMaxY();
                _viewTerminateContainer.Frame = terminateFrame;

                _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewTerminateContainer.Frame.GetMaxY());
            };

            _txtViewReason.ShouldBeginEditing = (sender) =>
            {
                _lblReason.Hidden = false;
                if (_txtViewReason.Text.Length == GetI18NValue(SSMRConstants.I18N_StateReason).Length)
                {
                    _txtViewReason.Text = string.Empty;
                    _txtViewReason.TextColor = MyTNBColor.CharcoalGrey;
                }
                return true;
            };

            _txtViewReason.ShouldEndEditing = (sender) =>
            {
                if (string.IsNullOrEmpty(_txtViewReason.Text) || string.IsNullOrWhiteSpace(_txtViewReason.Text))
                {
                    _txtViewReason.Text = GetI18NValue(SSMRConstants.I18N_StateReason);
                    _txtViewReason.TextColor = MyTNBColor.SilverChalice;
                }
                _lblReason.Hidden = _txtViewReason.Text.Length == GetI18NValue(SSMRConstants.I18N_StateReason).Length;
                ToggleCTA();
                return true;
            };
        }
        private void SetTextViewDisplay(bool shouldDisplay)
        {
            _viewOthersContainer.Hidden = !shouldDisplay;
            CGRect newFrame = _viewTerminateContainer.Frame;
            newFrame.Height = shouldDisplay ? _viewOthersContainer.Frame.GetMaxY() : _viewLineTerminate.Frame.GetMaxY() + GetScaledHeight(22);
            _viewTerminateContainer.Frame = newFrame;
            _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewTerminateContainer.Frame.GetMaxY());
        }

        private void ToggleCTA()
        {
            bool isValid = _selectedAccount != null && !string.IsNullOrEmpty(_selectedAccount.accountNickName);
            if (isValid && _customEmailField != null && _customMobileField != null)
            {
                isValid = _customEmailField.IsFieldValid && _customMobileField.IsFieldValid;
                if (!IsApplication && _viewOthersContainer != null && !_viewOthersContainer.Hidden)
                {
                    isValid = isValid && (_txtViewReason.Text != GetI18NValue(SSMRConstants.I18N_StateReason));
                }
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

        private void GetEligibleAccounts()
        {
            _eligibleAccountList = SSMRAccounts.GetAccounts();
            _selectedAccount = IsApplication ? SSMRAccounts.GetFirstAccount() : DataManager.DataManager.SharedInstance.SelectedAccount;
        }

        private void OnSelectTerminateReason(int index)
        {
            if (index > -1 && index < _ssmrTerminationReasons.d.data.reasons.Count)
            {
                _selectedTerminateReasonIndex = index;
                _lblTerminateReason.Text = _ssmrTerminationReasons.d.data.reasons[index].ReasonName;
                SetTextViewDisplay(_ssmrTerminationReasons.d.data.reasons[index].ReasonId == SSMRConstants.Service_OthersID);
            }
        }

        private void OnGetContactInfo()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeOnMainThread(async () =>
                    {
                        _contactDetails = await GetContactInfo();
                        if (_contactDetails != null && _contactDetails.d != null
                            && _contactDetails.d.IsSuccess && _contactDetails.d.data != null)
                        {
                            _lblEditInfo.Hidden = true;
                            _customEmailField.SetValue(_contactDetails.d.data.Email);
                            _customMobileField.SetValue(_contactDetails.d.data.Mobile);
                            ToggleCTA();
                            _customEmailField.SetState(true);
                            _customMobileField.SetState(true);
                        }
                        ActivityIndicator.Hide();
                    });
                }
                else
                {
                    DisplayNoDataAlert();
                    ActivityIndicator.Hide();
                }
            });
        }

        private void OnGetAccountsSMREligibility()
        {
            InvokeOnMainThread(async () =>
            {
                _smrEligibleList = await GetAccountsSMREligibility();
                if (_smrEligibleList != null && _smrEligibleList.d != null
                    && _smrEligibleList.d.IsSuccess && _smrEligibleList.d.data != null
                    && _smrEligibleList.d.data.accountEligibilities != null)
                {
                    for (int i = _smrEligibleList.d.data.accountEligibilities.Count - 1; i > -1; i--)
                    {
                        AccountsSMREligibilityModel item = _smrEligibleList.d.data.accountEligibilities[i];
                        if (!item.IsEligible)
                        {
                            _eligibleAccountList.RemoveAt(i);
                        }
                    }
                }
            });
        }

        private bool IsValidTerminateReason()
        {
            return _ssmrTerminationReasons != null && _ssmrTerminationReasons.d != null
                && _ssmrTerminationReasons.d.IsSuccess && _ssmrTerminationReasons.d.data != null
                && _ssmrTerminationReasons.d.data.reasons != null && _ssmrTerminationReasons.d.data.reasons.Count > 0;
        }

        private void OnGetTerminateReasons()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeOnMainThread(async () =>
                    {
                        _ssmrTerminationReasons = await GetSMRTerminationReasons();
                        if (IsValidTerminateReason())
                        {
                            _lblTerminateReason.Text = _ssmrTerminationReasons.d.data.reasons[0].ReasonName;
                            _viewTerminate.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                            {
                                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                                    .InstantiateViewController("GenericSelectorViewController");
                                viewController.Title = GetI18NValue(SSMRConstants.I18N_SelectReason);
                                viewController.Items = _ssmrTerminationReasons.d.data.reasons.Select(x => x.ReasonName).ToList();
                                viewController.OnSelect = OnSelectTerminateReason;
                                viewController.SelectedIndex = _selectedTerminateReasonIndex;
                                viewController.IsRootPage = true;
                                NavigationController.PushViewController(viewController, true);
                            }));
                            SetTextViewDisplay(_ssmrTerminationReasons.d.data.reasons[0].ReasonId == SSMRConstants.Service_OthersID);
                        }
                        ActivityIndicator.Hide();
                    });
                }
                else
                {
                    DisplayNoDataAlert();
                    ActivityIndicator.Hide();
                }

            });
        }

        private void OnSubmitSMRApplication()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
               {
                   if (NetworkUtility.isReachable)
                   {
                       ActivityIndicator.Show();
                       SubmitSMRApplication().ContinueWith(task =>
                       {
                           InvokeOnMainThread(() =>
                          {
                              if (_ssmrApplicationStatus != null && _ssmrApplicationStatus.d != null
                                   && _ssmrApplicationStatus.d.data != null)
                              {
                                  UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                  GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                                  status.StatusDisplayType = IsApplication ? GenericStatusPageViewController.StatusType.SSMRApply
                                  : GenericStatusPageViewController.StatusType.SSMRDiscontinue;
                                  status.IsSuccess = _ssmrApplicationStatus.d.IsSuccess;
                                  status.StatusTitle = _ssmrApplicationStatus.d.DisplayTitle;
                                  status.StatusMessage = _ssmrApplicationStatus.d.DisplayMessage;
                                  status.ReferenceNumber = _ssmrApplicationStatus.d.data.ServiceReqNo;
                                  status.ReferenceDate = _ssmrApplicationStatus.d.data.AppliedOn;
                                  NavigationController.PushViewController(status, true);
                              }
                              else
                              {
                                  DisplayServiceError(_ssmrApplicationStatus.d.ErrorMessage);
                              }
                              ActivityIndicator.Hide();
                          });
                       });
                   }
                   else
                   {
                       DisplayNoDataAlert();
                       ActivityIndicator.Hide();
                   }
               });
            });
        }

        private async Task<ContactDetailsResponseModel> GetContactInfo()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccount = _selectedAccount.accNum,
                isOwnedAccount = _selectedAccount.IsOwnedAccount
            };
            ContactDetailsResponseModel response = serviceManager
                .OnExecuteAPIV6<ContactDetailsResponseModel>(SSMRConstants.Service_GetCARegisteredContact, request);
            return response;
        }

        private async Task<TerminationReasonsResponseModel> GetSMRTerminationReasons()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf
            };
            TerminationReasonsResponseModel response = serviceManager
                .OnExecuteAPIV6<TerminationReasonsResponseModel>(SSMRConstants.Service_GetTerminationReasons, request);
            return response;
        }

        private async Task<AccountsSMREligibilityResponseModel> GetAccountsSMREligibility()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccounts = _eligibleAccountList.Select(x => x.accNum).ToList()
            };
            AccountsSMREligibilityResponseModel response = serviceManager
                .OnExecuteAPIV6<AccountsSMREligibilityResponseModel>(SSMRConstants.Service_GetAccountsSMREligibility, request);
            return response;
        }

        private string GetNewValue(bool isMobilePhone = true)
        {
            string oldValue = string.Empty;
            if (_contactDetails != null && _contactDetails.d != null
                    && _contactDetails.d.IsSuccess && _contactDetails.d.data != null)
            {
                if (isMobilePhone)
                {
                    oldValue = _contactDetails.d.data.Mobile != null ? _contactDetails.d.data.Mobile : string.Empty;
                }
                else
                {
                    oldValue = _contactDetails.d.data.Email != null ? _contactDetails.d.data.Email : string.Empty;
                }
            }
            string newValue = isMobilePhone ? _customMobileField.GetNonFormattedValue() : _customEmailField.GetNonFormattedValue();
            return string.Equals(oldValue, newValue) ? string.Empty : isMobilePhone ? _customMobileField.GetTextFieldValue() : newValue;
        }

        private string GetReason()
        {
            if (IsApplication) { return string.Empty; }
            if (_selectedTerminateReasonIndex > -1 && IsValidTerminateReason())
            {
                if (_ssmrTerminationReasons.d.data.reasons[_selectedTerminateReasonIndex].ReasonId == SSMRConstants.Service_OthersID)
                {
                    return _txtViewReason.Text;
                }
                else
                {
                    return _ssmrTerminationReasons.d.data.reasons[_selectedTerminateReasonIndex].ReasonName;
                }
            }
            return string.Empty;
        }

        private Task SubmitSMRApplication()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccount = _selectedAccount.accNum,
                oldPhone = _contactDetails != null && _contactDetails.d != null
                    && _contactDetails.d.IsSuccess && _contactDetails.d.data != null
                        ? _contactDetails.d.data.Mobile : string.Empty,
                newPhone = GetNewValue(),
                oldEmail = _contactDetails != null && _contactDetails.d != null
                    && _contactDetails.d.IsSuccess && _contactDetails.d.data != null
                        ? _contactDetails.d.data.Email : string.Empty,
                newEmail = GetNewValue(false),
                SMRMode = IsApplication ? SSMRConstants.Service_Register : SSMRConstants.Service_Terminate,
                reason = GetReason(),
            };
            return Task.Factory.StartNew(() =>
            {
                _ssmrApplicationStatus = serviceManager
                    .OnExecuteAPIV6<SSMRApplicationStatusResponseModel>(SSMRConstants.Service_SubmitSSMRApplication, request);
            });
        }
    }
}