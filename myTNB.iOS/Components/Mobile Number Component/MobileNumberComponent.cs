using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace myTNB
{
    public class MobileNumberComponent
    {
        private CustomUIView _countryCodeView, _mobileNoView;
        private UIView _parentView, _viewMobileNumber, _viewCountyCodeLine, _viewMobileNoLine;
        private UILabel _lblTitle, _lblCountryCode;
        private UITextField _txtFieldMobileNo;
        private UIImageView _imgFlag, _imgDropDown;
        private TextFieldHelper _textFieldHelper;
        private nfloat _yLocation;
        private string _countryCode = string.Empty, _countryShortCode = "ML";

        public Action OnDone { set; private get; }
        public Action OnSelect { set; private get; }

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
            _viewCountyCodeLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(24), _countryCodeView.Frame.Width, GetScaledHeight(1)));

            _imgFlag = new UIImageView(new CGRect(GetScaledWidth(6), GetScaledWidth(6)
               , GetScaledWidth(17), GetScaledWidth(12)))
            {
                BackgroundColor = UIColor.White,
                Image = UIImage.FromBundle(CountryShortCode.ToUpper())
            };
            _lblCountryCode = new UILabel(new CGRect(_imgFlag.Frame.GetMaxX() + GetScaledWidth(4), 0, GetScaledWidth(31), GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Center,
                Text = _countryCode
            };
            _imgDropDown = new UIImageView(new CGRect(_lblCountryCode.Frame.GetMaxX(), 0
               , GetScaledWidth(24), GetScaledWidth(24)))
            {
                BackgroundColor = UIColor.White,
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            _countryCodeView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (OnSelect != null)
                {
                    OnSelect.Invoke();
                }
            }));

            _countryCodeView.AddSubviews(new UIView[] { _imgFlag, _lblCountryCode, _imgDropDown, _viewCountyCodeLine });
        }

        private void AddMobileNumberView()
        {
            _mobileNoView = new CustomUIView(new CGRect(_countryCodeView.Frame.GetMaxX() + GetScaledWidth(4)
                , _lblTitle.Frame.GetMaxY() + GetScaledHeight(1)
                , _viewMobileNumber.Frame.Width - _countryCodeView.Frame.Width - GetScaledWidth(4), GetScaledHeight(25)));
            _viewMobileNoLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(24), _mobileNoView.Frame.Width, GetScaledHeight(1)));

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
            _mobileNoView.AddSubviews(new UIView[] { _viewMobileNoLine, _txtFieldMobileNo });
        }

        private nfloat GetScaledHeight(nfloat val)
        {
            return ScaleUtility.GetScaledHeight(val);
        }

        private nfloat GetScaledWidth(nfloat val)
        {
            return ScaleUtility.GetScaledWidth(val);
        }

        private void AdjustView()
        {
            _imgDropDown.Frame = new CGRect(new CGPoint(_lblCountryCode.Frame.GetMaxX(), _imgDropDown.Frame.Y), _imgDropDown.Frame.Size);
            _countryCodeView.Frame = new CGRect(_countryCodeView.Frame.Location, new CGSize(_imgDropDown.Frame.GetMaxX(), _countryCodeView.Frame.Height));
            _viewCountyCodeLine.Frame = new CGRect(_viewCountyCodeLine.Frame.Location, new CGSize(_countryCodeView.Frame.Width, _viewCountyCodeLine.Frame.Height));

            _mobileNoView.Frame = new CGRect(_countryCodeView.Frame.GetMaxX() + GetScaledWidth(4), _mobileNoView.Frame.Y
                , _viewMobileNumber.Frame.Width - _countryCodeView.Frame.Width - GetScaledWidth(4)
                , _mobileNoView.Frame.Height);
            _txtFieldMobileNo.Frame = new CGRect(_txtFieldMobileNo.Frame.Location, new CGSize(_mobileNoView.Frame.Width, _txtFieldMobileNo.Frame.Height));
            _viewMobileNoLine.Frame = new CGRect(_viewMobileNoLine.Frame.Location, new CGSize(_mobileNoView.Frame.Width, _viewMobileNoLine.Frame.Height));
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
                if (_lblCountryCode != null)
                {
                    _lblCountryCode.Text = _countryCode;
                    nfloat newWidth = _lblCountryCode.GetLabelWidth(GetScaledWidth(100));
                    if (newWidth < GetScaledWidth(31))
                    {
                        newWidth = GetScaledWidth(31);
                    }
                    _lblCountryCode.Frame = new CGRect(_lblCountryCode.Frame.Location, new CGSize(newWidth, _lblCountryCode.Frame.Height));
                    AdjustView();
                }
            }
            get { return _countryCode; }
        }

        public string CountryShortCode
        {
            set
            {
                if (value.IsValid())
                {
                    _countryShortCode = value;
                }
                if (_imgFlag != null)
                {
                    _imgFlag.Image = UIImage.FromBundle(_countryShortCode);
                }
            }
            get
            {
                return _countryShortCode;
            }
        }
    }
}
