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

            _lblName = new UILabel(new CGRect(16, 16, ContentView.Frame.Width - 32 - 40, 18))
            {
                Text = string.Empty,
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_500
            };

            _txtViewAddress = new UITextView(new CGRect(16, _lblName.Frame.GetMaxY() + 8, Frame.Width - 36 - 40, 32))
            {
                Text = string.Empty,
                Font = MyTNBFont.MuseoSans12_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.TunaGrey(),
                BackgroundColor = UIColor.Clear,
                TextContainerInset = new UIEdgeInsets(0, -4, 0, 0),
                UserInteractionEnabled = false,
                Editable = false,
                ScrollEnabled = false
            };

            _viewCheckBox = new UIView(new CGRect(Frame.Width - 42, (Frame.Height - 48) / 2, 24, 24));
            _imgViewCheckBox = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Inactive")
            };
            _viewCheckBox.AddSubview(_imgViewCheckBox);

            //Amount
            _viewAmount = new UIView(new CGRect(16, _txtViewAddress.Frame.GetMaxY() + 16, Frame.Width - 32, 51))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 0
            };

            _lblAmountTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewAmount.Frame.Width, 12),
                AttributedText = new NSAttributedString("SelectBill_IAmPaying".Translate().ToUpper(),
                   font: MyTNBFont.MuseoSans9_300,
                   foregroundColor: MyTNBColor.SilverChalice,
                   strokeWidth: 0),
                TextAlignment = UITextAlignment.Left
            };

            _lblAmountError = new UILabel(new CGRect(0, 37, _viewAmount.Frame.Width, 14))
            {
                TextColor = MyTNBColor.Tomato,
                Font = MyTNBFont.MuseoSans9_300,
                Text = "Invalid_PayAmount".Translate(),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldAmount = new UITextField
            {
                Frame = new CGRect(0, 12, _viewAmount.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(TNBGlobal.DEFAULT_VALUE
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16_300
            };
            _txtFieldAmount.KeyboardType = UIKeyboardType.DecimalPad;
            new TextFieldHelper().SetKeyboard(_txtFieldAmount);
            new TextFieldHelper().CreateDoneButton(_txtFieldAmount);

            _viewLineAmount = new UIView((new CGRect(0, 36, _viewAmount.Frame.Width - 62, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey,
                Tag = 1
            };

            _viewAmount.AddSubviews(new UIView[] { _lblAmountTitle, _lblAmountError, _txtFieldAmount, _viewLineAmount });

            _viewSeparator = new UIView(new CGRect(0, 163, Frame.Width, 24))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };

            AddSubviews(_lblName, _txtViewAddress, _viewCheckBox, _viewAmount, _viewSeparator);
            UserInteractionEnabled = true;
        }
    }
}