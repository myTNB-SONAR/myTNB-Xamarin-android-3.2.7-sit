using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using UIKit;

namespace myTNB
{
    public partial class SSMRApplicationViewController : CustomUIViewController
    {
        public SSMRApplicationViewController(IntPtr handle) : base(handle)
        {
        }

        private UIButton _btnSubmit;
        private UIView _viewBottomContainer;
        private UIScrollView _scrollContainer;
        private CGRect _scrollViewFrame;
        private CustomTextField _customMobileField;
        private CustomTextField _customEmailField;

        public override void ViewDidLoad()
        {
            PageName = "SSMRApplication";
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            ConfigureNavigationBar();
            SetFrames();
            AddTnCSection();
            AddDetailsSection();
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

        private void AddDetailsSection()
        {
            _scrollContainer = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - _viewBottomContainer.Frame.Height))
            {
                BackgroundColor = UIColor.White
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

            UIView viewMainDetails = new UIView(new CGRect(0, viewApplyForTitle.Frame.GetMaxY() + 8, ViewWidth, 131))
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

            UIImageView imgDropdown = new UIImageView(new CGRect(ViewWidth - 46, 28, 24, 24))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Dropdow)
            };

            UILabel lblAccountName = new UILabel(new CGRect(16, 28, imgDropdown.Frame.GetMinX() - 20, 24))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans16_300,
                Text = "Saujana Heights"
            };

            UIView viewLine = new UIView(new CGRect(16, lblAccountName.Frame.GetMaxY() + 1, ViewWidth - 32, 1))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkTwo
            };

            UILabel lblAddress = new UILabel(new CGRect(16, viewLine.Frame.GetMaxY() + 22, ViewWidth - 32, 40))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = "No. 3 Jalan Melur, 12 Taman Melur, 68000 Ampang, Selangor"
            };

            viewMainDetails.AddSubviews(new UIView[] { lblAccountTitle, lblAccountName, imgDropdown, viewLine, lblAddress });

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

            UIView viewContactDetails = new UIView(new CGRect(0, viewContactDetailsTitle.Frame.GetMaxY() + 8, ViewWidth, 131))
            {
                BackgroundColor = UIColor.White
            };

            _customEmailField = new CustomTextField(viewContactDetails, new CGPoint(16, 16))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_Email),
                LeftIcon = SSMRConstants.IMG_Email,
                KeyboardType = UIKeyboardType.EmailAddress,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidEmail),
                TypingAction = ToggleCTA,
                TextFieldType = CustomTextField.Type.EmailAddress
            };

            UIView viewEmail = _customEmailField.GetUI();

            _customMobileField = new CustomTextField(viewContactDetails, new CGPoint(16, 75))
            {
                Title = GetCommonI18NValue(SSMRConstants.I18N_MobileNumber),
                LeftIcon = SSMRConstants.IMG_MobileNumber,
                KeyboardType = UIKeyboardType.PhonePad,
                Error = GetErrorI18NValue(SSMRConstants.I18N_InvalidMobileNumber),
                Hint = GetHintI18NValue(SSMRConstants.I18N_HintMobileNumber),
                TypingAction = ToggleCTA,
                TextFieldType = CustomTextField.Type.MobileNumber
            };

            UIView viewMobile = _customMobileField.GetUI();

            viewContactDetails.AddSubviews(new UIView[] { viewEmail, viewMobile });

            _scrollContainer.AddSubviews(new UIView[] { viewApplyForTitle, viewMainDetails, viewContactDetailsTitle, viewContactDetails });
            _scrollContainer.ContentSize = new CGSize(ViewWidth, viewContactDetails.Frame.GetMaxY());
            View.AddSubview(_scrollContainer);
            _scrollViewFrame = _scrollContainer.Frame;
        }

        private void ToggleCTA()
        {
            if (_customEmailField != null && _customMobileField != null)
            {
                bool isValid = _customEmailField.IsFieldValid && _customMobileField.IsFieldValid;
                _btnSubmit.Enabled = isValid;
                _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            }
        }
    }
}