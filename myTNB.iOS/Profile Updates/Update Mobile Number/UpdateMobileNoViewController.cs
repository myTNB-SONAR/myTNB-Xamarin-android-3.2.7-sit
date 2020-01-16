using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.DataManager;
using myTNB.Model;
using myTNB.MyAccount;
using myTNB.Registration;
using UIKit;

namespace myTNB
{
    public class UpdateMobileNoViewController : CustomUIViewController
    {
        public bool IsFromLogin { set; private get; }
        public bool WillHideBackButton { set; private get; }

        private CustomUIButtonV2 _btnNext;
        private CustomUIView _infoView;
        private MobileNumberComponent _mobileNumberComponent;
        private string _mobileNo;

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_UpdateMobileNumber;
            base.ViewDidLoad();
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            SetNavigationBar();
            SetViews();
            SetCTA();

            if (IsFromLogin)
            {
                DisplayToast(GetI18NValue(MyAccountConstants.I18N_VerifyDeviceMessage));
            }
        }

        private void SetNavigationBar()
        {
            Title = GetI18NValue(IsFromLogin ? MyAccountConstants.I18N_VerifyDeviceTitle : MyAccountConstants.I18N_UpdateMobileTitle);
            if (!WillHideBackButton)
            {
                UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    DismissViewController(true, null);
                });
                NavigationItem.LeftBarButtonItem = btnBack;
            }
        }

        private void SetViews()
        {
            UIView cardView = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(123))) { BackgroundColor = UIColor.White };
            _mobileNumberComponent = new MobileNumberComponent(cardView)
            {
                OnDone = OnDone
            };
            UIView viewMobileNumber = _mobileNumberComponent.GetUI();
            AddInfoView();
            cardView.AddSubviews(new UIView[] { viewMobileNumber, _infoView });
            View.AddSubview(cardView);
        }

        private void SetCTA()
        {
            nfloat containerHeight = GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;
            nfloat yLoc = View.Frame.Height - DeviceHelper.TopSafeAreaInset - NavigationController.NavigationBar.Frame.Height - containerHeight;

            UIView cardView = new UIView(new CGRect(0, yLoc, View.Frame.Width, containerHeight)) { BackgroundColor = UIColor.White };

            _btnNext = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, BaseMargin, BaseMarginedWidth, GetScaledHeight(48))
            };
            _btnNext.SetTitle(GetCommonI18NValue(Constants.Common_Next), UIControlState.Normal);
            _btnNext.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnNext.AddGestureRecognizer(new UITapGestureRecognizer(async () =>
            {
                ActivityIndicator.Show();
                _mobileNo = "";// txtFieldMobileNo.Text.Replace(" ", string.Empty);
                BaseResponseModelV2 response = await ServiceCall.SendUpdatePhoneTokenSMS(_mobileNo);

                if (ServiceCall.ValidateBaseResponse(response))
                {
                    DataManager.DataManager.SharedInstance.User.MobileNo = _mobileNo;
                    UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                    VerifyPinViewController viewController = storyBoard.InstantiateViewController("VerifyPinViewController") as VerifyPinViewController;
                    viewController.IsMobileVerification = true;
                    viewController.IsFromLogin = IsFromLogin;
                    NavigationController.PushViewController(viewController, true);
                    ActivityIndicator.Hide();
                }
                else
                {
                    DisplayServiceError(response?.d?.DisplayMessage ?? string.Empty);
                    ActivityIndicator.Hide();
                }
            }));
            IsCTAEnabled = false;
            cardView.AddSubview(_btnNext);
            View.AddSubview(cardView);
        }

        private void AddInfoView()
        {
            _infoView = new CustomUIView(new CGRect(BaseMargin, GetScaledHeight(83), BaseMarginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            _infoView.Layer.CornerRadius = _infoView.Frame.Height / 2;
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle("IC-Info-Blue")
            };
            UILabel lblDescription = new UILabel(new CGRect(imgView.Frame.GetMaxX() + GetScaledWidth(8)
                , GetScaledHeight(4), _infoView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "What is it for?"
            };
            _infoView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert("This is just for the purpose of registering on the myTNB app."
                    , "Updating your phone number here will not change the existing phone number registered with TNB."
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , false);
            }));
            _infoView.AddSubviews(new UIView[] { imgView, lblDescription });
        }

        private void OnDone()
        {
            _mobileNo = _mobileNumberComponent.MobileNumber;
            IsCTAEnabled = _mobileNo.IsValid();
        }

        private bool IsCTAEnabled
        {
            set
            {
                if (_btnNext != null)
                {
                    _btnNext.Enabled = value;
                    _btnNext.BackgroundColor = value ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
                }
            }
        }
    }
}