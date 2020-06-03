using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class SelectBillsTableViewCell : UITableViewCell
    {
        public UILabel _lblName, _lblAmountTitle;
        public UITextView _txtViewAddress;
        public UITextField _txtFieldAmount;
        public UILabel _lblAmountError;
        public UIView _viewLineAmount;
        public UIView _viewCheckBox;
        public UIImageView _imgViewCheckBox;
        public UILabel _lblAccountNo;
        private UIView _viewAmount, _viewSeparator;

        public SelectBillsTableViewCell(IntPtr handle) : base(handle)
        {
            SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            SelectionStyle = UITableViewCellSelectionStyle.None;

            _lblAccountNo = new UILabel();

            _lblName = new UILabel(new CGRect(GetScaledWidth(16), GetScaledWidth(16)
                , Frame.Width - GetScaledWidth(88), GetScaledHeight(24)))
            {
                Text = string.Empty,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_16_500
            };

            _txtViewAddress = new UITextView(new CGRect(GetScaledWidth(16), _lblName.Frame.GetMaxY() + GetScaledWidth(8)
                , Frame.Width - GetScaledWidth(88), GetScaledHeight(32)))
            {
                Text = string.Empty,
                Font = TNBFont.MuseoSans_12_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                BackgroundColor = UIColor.Clear,
                TextContainerInset = new UIEdgeInsets(0, -4, 0, 0),
                UserInteractionEnabled = false,
                Editable = false,
                ScrollEnabled = false
            };

            _viewCheckBox = new UIView(new CGRect(Frame.Width - GetScaledWidth(40)
                , (Frame.Height - GetScaledWidth(40)) / 2, GetScaledWidth(24), GetScaledWidth(24)));
            _imgViewCheckBox = new UIImageView(new CGRect(0, 0, GetScaledWidth(24), GetScaledWidth(24)))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Inactive")
            };
            _viewCheckBox.AddSubview(_imgViewCheckBox);

            _viewAmount = new UIView(new CGRect(GetScaledWidth(16), _txtViewAddress.Frame.GetMaxY() + GetScaledWidth(16)
                , Frame.Width - GetScaledWidth(32), GetScaledHeight(51)))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 0
            };

            _lblAmountTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewAmount.Frame.Width, GetScaledWidth(12)),
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = MyTNBColor.SilverChalice
            };

            _txtFieldAmount = new UITextField
            {
                Frame = new CGRect(0, _lblAmountTitle.Frame.GetMaxY(), _viewAmount.Frame.Width, GetScaledWidth(24)),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(TNBGlobal.DEFAULT_VALUE
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_16_300
            };
            _txtFieldAmount.KeyboardType = UIKeyboardType.DecimalPad;
            new TextFieldHelper().SetKeyboard(_txtFieldAmount);
            new TextFieldHelper().CreateDoneButton(_txtFieldAmount);

            _viewLineAmount = new UIView((new CGRect(0, _txtFieldAmount.Frame.GetMaxY()
                , _viewAmount.Frame.Width - GetScaledWidth(52), GetScaledHeight(1))))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree,
                Tag = 1
            };

            _lblAmountError = new UILabel(new CGRect(0, _viewLineAmount.Frame.GetMaxY(), _viewAmount.Frame.Width, GetScaledWidth(12)))
            {
                TextColor = MyTNBColor.Tomato,
                Font = TNBFont.MuseoSans_9_300,
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _viewAmount.AddSubviews(new UIView[] { _lblAmountTitle, _lblAmountError, _txtFieldAmount, _viewLineAmount });

            _viewSeparator = new UIView(new CGRect(0, _viewAmount.Frame.GetMaxY() + GetScaledWidth(24), Frame.Width, GetScaledWidth(16)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };

            AddSubviews(_lblName, _txtViewAddress, _viewCheckBox, _viewAmount, _viewSeparator);
            UserInteractionEnabled = true;
        }

        public string AmountTitle
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    _lblAmountTitle.AttributedText = new NSAttributedString(value
                        , font: TNBFont.MuseoSans_10_300
                        , foregroundColor: MyTNBColor.SilverChalice
                        , strokeWidth: 0);
                }
            }
        }

        public string AmountError
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    _lblAmountError.Text = value;
                }
            }
        }

        private nfloat GetScaledHeight(nfloat val)
        {
            return ScaleUtility.GetScaledHeight(val);
        }

        private nfloat GetScaledWidth(nfloat val)
        {
            return ScaleUtility.GetScaledWidth(val);
        }
    }
}