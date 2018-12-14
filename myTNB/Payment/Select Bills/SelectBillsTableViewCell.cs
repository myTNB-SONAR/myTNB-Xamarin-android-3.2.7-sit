using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Extensions;

namespace myTNB
{
    public partial class SelectBillsTableViewCell : UITableViewCell
    {
        public UILabel _lblName;
        public UILabel _lblAccountNo;
        public UITextView _txtViewAddress;
        public UITextField _txtFieldAmount;
        public UILabel _lblAmountError;
        public UIView _viewLineAmount;
        public UIView _viewCheckBox;
        public UIImageView _imgViewCheckBox;

        public SelectBillsTableViewCell(IntPtr handle) : base(handle)
        {
            this.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            this.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            this.SelectionStyle = UITableViewCellSelectionStyle.None;

            _lblName = new UILabel(new CGRect(18, 16, this.ContentView.Frame.Width - 36 - 40, 18));
            _lblName.Text = string.Empty;
            _lblName.TextColor = myTNBColor.TunaGrey();
            _lblName.Font = myTNBFont.MuseoSans14_500();

            _lblAccountNo = new UILabel(new CGRect(18, 34, this.ContentView.Frame.Width - 36 - 40, 16));
            _lblAccountNo.Text = string.Empty;
            _lblAccountNo.TextColor = myTNBColor.TunaGrey();
            _lblAccountNo.Font = myTNBFont.MuseoSans12_300();

            _txtViewAddress = new UITextView(new CGRect(18, 66, this.Frame.Width - 36 - 40, 32));
            _txtViewAddress.Text = "No. 3 Jalan Melur, 12 Taman Melur, 68000 Ampang, Selangor"; //For Testing
            _txtViewAddress.Font = myTNBFont.MuseoSans12_300();
            _txtViewAddress.TextAlignment = UITextAlignment.Left;
            _txtViewAddress.TextColor = myTNBColor.TunaGrey();
            _txtViewAddress.BackgroundColor = UIColor.Clear;
            _txtViewAddress.TextContainerInset = new UIEdgeInsets(0, -4, 0, 0);
            _txtViewAddress.UserInteractionEnabled = false;
            _txtViewAddress.Editable = false;
            _txtViewAddress.ScrollEnabled = false;

            _viewCheckBox = new UIView(new CGRect(this.Frame.Width - 42, 83, 24, 24));
            _imgViewCheckBox = new UIImageView(new CGRect(0, 0, 24, 24));
            _imgViewCheckBox.Image = UIImage.FromBundle("Payment-Checkbox-Inactive");
            _viewCheckBox.AddSubview(_imgViewCheckBox);

            //Amount
            UIView viewAmount = new UIView((new CGRect(18, 114, this.Frame.Width - 36, 51)));
            viewAmount.BackgroundColor = UIColor.Clear;
            viewAmount.Tag = 0;

            UILabel lblAmountTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewAmount.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "AMOUNT (RM)",
                                                       font: myTNBFont.MuseoSans9_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            viewAmount.AddSubview(lblAmountTitle);

            _lblAmountError = new UILabel(new CGRect(0, 37, viewAmount.Frame.Width, 14));
            _lblAmountError.TextColor = myTNBColor.Tomato();
            _lblAmountError.Font = myTNBFont.MuseoSans9_300();
            _lblAmountError.Text = "SelectBillInvalidAmount".Translate();
            _lblAmountError.TextAlignment = UITextAlignment.Left;
            _lblAmountError.Hidden = true;
            viewAmount.AddSubview(_lblAmountError);

            _txtFieldAmount = new UITextField
            {
                Frame = new CGRect(0, 12, viewAmount.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "0.00",
                    font: myTNBFont.MuseoSans16_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _txtFieldAmount.KeyboardType = UIKeyboardType.DecimalPad;
            new TextFieldHelper().SetKeyboard(_txtFieldAmount);
            new TextFieldHelper().CreateDoneButton(_txtFieldAmount);
            viewAmount.AddSubview(_txtFieldAmount);

            _viewLineAmount = new UIView((new CGRect(0, 36, viewAmount.Frame.Width - 62, 1)));
            _viewLineAmount.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewLineAmount.Tag = 1;
            viewAmount.AddSubview(_viewLineAmount);

            UIView viewSeparator = new UIView((new CGRect(0, 181, this.Frame.Width, 24)));
            viewSeparator.BackgroundColor = myTNBColor.LightGrayBG();

            this.AddSubviews(_lblName, _lblAccountNo, _txtViewAddress, _viewCheckBox, viewAmount, viewSeparator);
        }
    }
}