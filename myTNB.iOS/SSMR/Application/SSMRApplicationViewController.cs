using CoreGraphics;
using Foundation;
using myTNB.Home.Feedback.FeedbackEntry;
using myTNB.Model;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace myTNB
{
    public partial class SSMRApplicationViewController : CustomUIViewController
    {
        public SSMRApplicationViewController(IntPtr handle) : base(handle)
        {
        }

        private UIButton _btnSubmit;
        private UIView _viewBottomContainer, _viewContactDetails;
        private UIScrollView _scrollContainer;
        private CGRect _scrollViewFrame;
        private CustomTextField _customMobileField;
        private CustomTextField _customEmailField;
        protected List<CustomerAccountRecordModel> _eligibleAccountList;
        private int _selectedAccountIndex = 0;
        private UILabel _lblAccountName, _lblAddress, _lblEditInfo;

        public override void ViewDidLoad()
        {
            PageName = "SSMRApplication";
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            ConfigureNavigationBar();
            SetFrames();
            AddTnCSection();
            PrepareEligibleAccounts();
            AddDetailsSection();
            ToggleCTA();
        }

        private void ConfigureNavigationBar()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                //DismissViewController(true, null);
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
            _viewBottomContainer = new UIView(new CGRect(0, View.Frame.Height - 160, View.Frame.Width, 160))
            {
                BackgroundColor = UIColor.White
            };
            UIView viewPadding = new UIView(new CGRect(0, 0, ViewWidth, 16))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UITextView txtFieldInfo = GetInfo();
            _btnSubmit = CustomUIButton.GetUIButton(new CGRect(16, txtFieldInfo.Frame.GetMaxY() + 16, View.Frame.Width - 32, 48)
                , GetCommonI18NValue(SSMRConstants.I18N_Submit));
            _btnSubmit.Enabled = true;
            _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                //Execute Service Call
                UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                GenericStatusPageViewController feedbackStatusVS = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                feedbackStatusVS.IsSuccess = true;
                feedbackStatusVS.ServiceRequestNumber = "123";//_submitFeedback?.d?.data?.ServiceReqNo;
                //feedbackStatusVS.DateCreated = //_submitFeedback?.d?.data?.DateCreated;
                NavigationController.PushViewController(feedbackStatusVS, true);
            };
            _viewBottomContainer.AddSubviews(new UIView[] { viewPadding, txtFieldInfo, _btnSubmit });
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewBottomContainer.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewBottomContainer);
        }

        private UITextView GetInfo()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(SSMRConstants.I18N_TnC)
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, 12f);
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = MyTNBFont.MuseoSans12_500,
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
            //Resize
            CGSize size = txtFieldInfo.SizeThatFits(new CGSize(ViewWidth - 32, 160));
            txtFieldInfo.Frame = new CGRect(16, 32, size.Width, size.Height);
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

            UIView viewApplyForTitle = new UIView(new CGRect(0, 0, ViewWidth, 48))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UILabel lblApplyFor = new UILabel(new CGRect(16, 16, ViewWidth - 32, 24))
            {
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetI18NValue(SSMRConstants.I18N_ApplyingFor)
            };
            viewApplyForTitle.AddSubview(lblApplyFor);

            UIView viewMainDetails = new UIView(new CGRect(0, viewApplyForTitle.Frame.GetMaxY(), ViewWidth, 131))
            {
                BackgroundColor = UIColor.White
            };

            UILabel lblAccountTitle = new UILabel(new CGRect(16, 16, ViewWidth - 32, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans9_300,
                Text = GetCommonI18NValue(SSMRConstants.I18N_Account).ToUpper()
            };
            UIView viewAccountName = new UIView(new CGRect(16, 28, ViewWidth - 32, 24));
            viewAccountName.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_eligibleAccountList != null && _eligibleAccountList.Count > 0)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                    GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                        .InstantiateViewController("GenericSelectorViewController");
                    viewController.Title = GetCommonI18NValue(SSMRConstants.I18N_SelectAccounts);
                    viewController.Items = _eligibleAccountList.Select(x => x.accountNickName).ToList();
                    viewController.OnSelect = OnSelectAccount;
                    viewController.SelectedIndex = _selectedAccountIndex;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                }
            }));
            UIImageView imgDropdown = new UIImageView(new CGRect(viewAccountName.Frame.Width - 30, 0, 24, 24))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Dropdow)
            };
            CustomerAccountRecordModel initialAccount = GetFirstAccount();
            _lblAccountName = new UILabel(new CGRect(0, 0, viewAccountName.Frame.Width - 32, 24))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans16_300,
                Text = initialAccount != null && !string.IsNullOrEmpty(initialAccount.accountNickName)
                   ? initialAccount.accountNickName : string.Empty
            };
            viewAccountName.AddSubviews(new UIView[] { _lblAccountName, imgDropdown });

            UIView viewLine = new UIView(new CGRect(16, viewAccountName.Frame.GetMaxY() + 1, ViewWidth - 32, 1))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkTwo
            };

            _lblAddress = new UILabel(new CGRect(16, viewLine.Frame.GetMaxY() + 22, ViewWidth - 32, 40))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = initialAccount != null && !string.IsNullOrEmpty(initialAccount.accountStAddress)
                   ? initialAccount.accountStAddress : string.Empty
            };

            viewMainDetails.AddSubviews(new UIView[] { lblAccountTitle, viewAccountName, viewLine, _lblAddress });

            UIView viewContactDetailsTitle = new UIView(new CGRect(0, viewMainDetails.Frame.GetMaxY(), ViewWidth, 48))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            UILabel lblContactDetails = new UILabel(new CGRect(16, 16, ViewWidth - 32, 24))
            {
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetI18NValue(SSMRConstants.I18N_ContactDetails)
            };
            viewContactDetailsTitle.AddSubview(lblContactDetails);

            _viewContactDetails = new UIView(new CGRect(0, viewContactDetailsTitle.Frame.GetMaxY(), ViewWidth, 142))
            {
                BackgroundColor = UIColor.White
            };

            _customEmailField = new CustomTextField(_viewContactDetails, new CGPoint(16, 16))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_Email),
                LeftIcon = SSMRConstants.IMG_Email,
                KeyboardType = UIKeyboardType.EmailAddress,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidEmail),
                TypingEndAction = ToggleCTA,
                TypingBeginAction = OnEdit,
                TextFieldType = CustomTextField.Type.EmailAddress,
                Value = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                OnCreateValidation = true
            };
            UIView viewEmail = _customEmailField.GetUI();

            _customMobileField = new CustomTextField(_viewContactDetails, new CGPoint(16, 75))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_MobileNumber),
                LeftIcon = SSMRConstants.IMG_MobileNumber,
                KeyboardType = UIKeyboardType.PhonePad,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidMobileNumber),
                Hint = GetHintI18NValue(SSMRConstants.I18N_HintMobileNumber),
                TypingEndAction = ToggleCTA,
                TypingBeginAction = OnEdit,
                TextFieldType = CustomTextField.Type.MobileNumber,
                Value = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo,
                OnCreateValidation = true
            };
            UIView viewMobile = _customMobileField.GetUI();

            _lblEditInfo = new UILabel(new CGRect(16, viewMobile.Frame.GetMaxY() + 12, _viewContactDetails.Frame.Width - 32, 32))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans12_300,
                Text = GetI18NValue(SSMRConstants.I18N_EditInfo),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Hidden = true
            };

            CGSize newLblEditInfoSize = GetLabelSize(_lblEditInfo, _lblEditInfo.Frame.Width, 300);
            _lblEditInfo.Frame = new CGRect(_lblEditInfo.Frame.X, _lblEditInfo.Frame.Y, _lblEditInfo.Frame.Width, newLblEditInfoSize.Height);

            _viewContactDetails.AddSubviews(new UIView[] { viewEmail, viewMobile, _lblEditInfo });

            _scrollContainer.AddSubviews(new UIView[] { viewApplyForTitle, viewMainDetails, viewContactDetailsTitle, _viewContactDetails });
            _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewContactDetails.Frame.GetMaxY());
            View.AddSubview(_scrollContainer);
            _scrollViewFrame = _scrollContainer.Frame;
        }
        #endregion

        private void ToggleCTA()
        {
            if (_customEmailField != null && _customMobileField != null)
            {
                bool isValid = _customEmailField.IsFieldValid && _customMobileField.IsFieldValid;
                _btnSubmit.Enabled = isValid;
                _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            }
        }

        private void OnEdit()
        {
            if (_lblEditInfo.Hidden)
            {
                _lblEditInfo.Hidden = false;
                CGRect detailsFrame = _viewContactDetails.Frame;
                detailsFrame.Height = _lblEditInfo.Frame.GetMaxY() + 16;
                _viewContactDetails.Frame = detailsFrame;
                _scrollContainer.ContentSize = new CGSize(ViewWidth, _viewContactDetails.Frame.GetMaxY());
            }
        }

        private void PrepareEligibleAccounts()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                _eligibleAccountList
                    = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll(x => !x.IsREAccount && (x.IsNormalMeter || x.IsOwnedAccount));
            }
        }

        private CustomerAccountRecordModel GetFirstAccount()
        {
            if (_eligibleAccountList != null && _eligibleAccountList.Count > 0)
            {
                return _eligibleAccountList[0];
            }
            return null;
        }

        private void OnSelectAccount(int index)
        {
            if (index > -1)
            {
                _selectedAccountIndex = index;
                _lblAccountName.Text = _eligibleAccountList[index].accountNickName;
                _lblAddress.Text = _eligibleAccountList[index].accountStAddress;
            }
        }
    }
}