using CoreGraphics;
using System;
using UIKit;


namespace myTNB
{
    public partial class ReceiptTableViewCell : UITableViewCell
    {
        public UILabel lblAcctNo;
        public UILabel lblAcctNoValue;
        public UILabel lblAcctName;
        public UILabel lblAcctNameValue;
        public UILabel lblAmount;
        public UILabel lblAmountValue;
        const float TBL_PADDING = 20f;
        const float INNER_PADDING = 20f;
        const float LBL_WIDTH_PADDING = INNER_PADDING * 2;

        public ReceiptTableViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblAcctNo = new UILabel(new CGRect(INNER_PADDING, INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctNo.TextAlignment = UITextAlignment.Left;
            lblAcctNo.Font = MyTNBFont.MuseoSans10_500;
            lblAcctNo.TextColor = MyTNBColor.SilverChalice;
            lblAcctNo.Text = "Common_AccountNumber".Translate().ToUpper();

            lblAcctNoValue = new UILabel(new CGRect(INNER_PADDING, lblAcctNo.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctNoValue.TextAlignment = UITextAlignment.Left;
            lblAcctNoValue.Font = MyTNBFont.MuseoSans14_500;
            lblAcctNoValue.TextColor = MyTNBColor.TunaGrey();

            lblAcctName = new UILabel(new CGRect(INNER_PADDING, lblAcctNoValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctName.TextAlignment = UITextAlignment.Left;
            lblAcctName.Font = MyTNBFont.MuseoSans10_500;
            lblAcctName.TextColor = MyTNBColor.SilverChalice;
            lblAcctName.Text = "Receipt_AccountName".Translate().ToUpper();

            lblAcctNameValue = new UILabel(new CGRect(INNER_PADDING, lblAcctName.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctNameValue.TextAlignment = UITextAlignment.Left;
            lblAcctNameValue.Font = MyTNBFont.MuseoSans14_500;
            lblAcctNameValue.TextColor = MyTNBColor.TunaGrey();

            lblAmount = new UILabel(new CGRect(INNER_PADDING, lblAcctNameValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16));
            lblAmount.TextAlignment = UITextAlignment.Left;
            lblAmount.Font = MyTNBFont.MuseoSans10_500;
            lblAmount.TextColor = MyTNBColor.SilverChalice;
            lblAmount.Text = "Common_Amount(RM)".Translate().ToUpper();

            lblAmountValue = new UILabel(new CGRect(INNER_PADDING, lblAmount.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAmountValue.TextAlignment = UITextAlignment.Left;
            lblAmountValue.Font = MyTNBFont.MuseoSans14_500;
            lblAmountValue.TextColor = MyTNBColor.TunaGrey();

            UIView viewLine = new UIView(new CGRect(INNER_PADDING, lblAmountValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING - TBL_PADDING, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;

            AddSubviews(new UIView[] { lblAcctNo, lblAcctNoValue, lblAcctName, lblAcctNameValue, lblAmount, lblAmountValue, viewLine });
        }
    }
}