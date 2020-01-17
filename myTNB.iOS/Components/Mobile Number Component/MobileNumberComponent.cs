using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class MobileNumberComponent
    {
        private CustomUIView _countryCodeView, _mobileNoView;
        private UIView _parentView, _viewMobileNumber;
        private UILabel _lblTitle;
        private UITextField _txtFieldMobileNo;
        private TextFieldHelper _textFieldHelper;
        private nfloat _yLocation;
        private string _countryCode = string.Empty;

        public Action OnDone { set; private get; }

        public MobileNumberComponent(UIView parentView, nfloat yLocation)
        {
            _textFieldHelper = new TextFieldHelper();
            _parentView = parentView;
            _yLocation = yLocation + GetScaledHeight(16);
        }

        private void CreateUI()
        {
            _viewMobileNumber = new UIView(new CGRect(GetScaledWidth(16), _yLocation
                , _parentView.Frame.Width - GetScaledWidth(32), GetScaledHeight(51)));

            _lblTitle = new UILabel(new CGRect(0, 0, _viewMobileNumber.Frame.Width, GetScaledHeight(12)))
            {
                Font = TNBFont.MuseoSans_10_300,
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_MobileNo).ToUpper()
            };

            AddCountryCodeView();
            AddMobileNumberView();
            _viewMobileNumber.AddSubviews(new UIView[] { _lblTitle, _countryCodeView, _mobileNoView });
        }

        private void AddCountryCodeView()
        {
            _countryCodeView = new CustomUIView(new CGRect(0, _lblTitle.Frame.GetMaxY() + GetScaledHeight(1)
                , GetScaledWidth(82), GetScaledHeight(25)));
            UIView viewLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(24), _countryCodeView.Frame.Width, GetScaledHeight(1)));
            UIImageView imgFlag = new UIImageView(new CGRect(GetScaledWidth(6), GetScaledWidth(6)
                , GetScaledWidth(17), GetScaledWidth(12)))
            {
                BackgroundColor = UIColor.White,
                Image = UIImage.FromBundle("")
            };
            UILabel lblCountryCode = new UILabel(new CGRect(imgFlag.Frame.GetMaxX() + GetScaledWidth(4), 0, GetScaledWidth(31), GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Center,
                Text = _countryCode
            };
            UIImageView imgDropDown = new UIImageView(new CGRect(lblCountryCode.Frame.GetMaxX() + GetScaledWidth(1), 0
                , GetScaledWidth(24), GetScaledWidth(24)))
            {
                BackgroundColor = UIColor.White,
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            _countryCodeView.AddSubviews(new UIView[] { imgFlag, lblCountryCode, imgDropDown, viewLine });
        }

        private void AddMobileNumberView()
        {
            _mobileNoView = new CustomUIView(new CGRect(_countryCodeView.Frame.GetMaxX() + GetScaledWidth(4), _lblTitle.Frame.GetMaxY() + GetScaledHeight(1)
                , _viewMobileNumber.Frame.Width - _countryCodeView.Frame.Width - GetScaledWidth(4), GetScaledHeight(25)));
            UIView viewLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(24), _mobileNoView.Frame.Width, GetScaledHeight(1)));

            _txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 0, _mobileNoView.Frame.Width, GetScaledHeight(24)),
                AttributedPlaceholder = new NSAttributedString(string.Empty
                   , font: TNBFont.MuseoSans_16_300
                   , foregroundColor: MyTNBColor.CharcoalGrey
                   , strokeWidth: 0),
                TextColor = MyTNBColor.CharcoalGrey,
                KeyboardType = UIKeyboardType.NumberPad
            };
            _textFieldHelper.CreateDoneButton(_txtFieldMobileNo);
            _txtFieldMobileNo.ShouldEndEditing = (sender) =>
            {
                if (OnDone != null)
                {
                    OnDone.Invoke();
                }
                return true;
            };
            _txtFieldMobileNo.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                int totalLength = _countryCode.Length + (int)range.Location;
                return !(totalLength == 16);
            };
            _mobileNoView.AddSubviews(new UIView[] { viewLine, _txtFieldMobileNo });
        }

        private nfloat GetScaledHeight(nfloat val)
        {
            return ScaleUtility.GetScaledHeight(val);
        }

        private nfloat GetScaledWidth(nfloat val)
        {
            return ScaleUtility.GetScaledWidth(val);
        }

        public UIView GetUI()
        {
            CreateUI();
            return _viewMobileNumber;
        }

        public string MobileNumber
        {
            set
            {
                if (value.IsValid() && _txtFieldMobileNo != null)
                {
                    _txtFieldMobileNo.Text = value;
                }
            }
            get
            {
                if (_txtFieldMobileNo != null)
                {
                    return _txtFieldMobileNo.Text;
                }
                return string.Empty;
            }
        }

        public string CountryCode
        {
            set
            {
                _countryCode = value;
            }
            get { return _countryCode; }
        }
    }
}
