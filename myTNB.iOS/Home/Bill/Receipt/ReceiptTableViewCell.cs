using CoreGraphics;
using Foundation;
using System;
using UIKit;
using myTNB.Extensions;

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
            lblAcctNo.Font = myTNBFont.MuseoSans10_500();
            lblAcctNo.TextColor = myTNBColor.SilverChalice();
            lblAcctNo.Text = "PDFAcctNumber".Translate();

            lblAcctNoValue = new UILabel(new CGRect(INNER_PADDING, lblAcctNo.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctNoValue.TextAlignment = UITextAlignment.Left;
            lblAcctNoValue.Font = myTNBFont.MuseoSans14_500();
            lblAcctNoValue.TextColor = myTNBColor.TunaGrey();

            lblAcctName = new UILabel(new CGRect(INNER_PADDING, lblAcctNoValue.Frame.GetMaxY()  + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctName.TextAlignment = UITextAlignment.Left;
            lblAcctName.Font = myTNBFont.MuseoSans10_500();
            lblAcctName.TextColor = myTNBColor.SilverChalice();
            lblAcctName.Text = "PDFAcctName".Translate();

            lblAcctNameValue = new UILabel(new CGRect(INNER_PADDING, lblAcctName.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAcctNameValue.TextAlignment = UITextAlignment.Left;
            lblAcctNameValue.Font = myTNBFont.MuseoSans14_500();
            lblAcctNameValue.TextColor = myTNBColor.TunaGrey();

            lblAmount = new UILabel(new CGRect(INNER_PADDING, lblAcctNameValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16));
            lblAmount.TextAlignment = UITextAlignment.Left;
            lblAmount.Font = myTNBFont.MuseoSans10_500();
            lblAmount.TextColor = myTNBColor.SilverChalice();
            lblAmount.Text = "PDFAmnt".Translate();

            lblAmountValue = new UILabel(new CGRect(INNER_PADDING, lblAmount.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16));
            lblAmountValue.TextAlignment = UITextAlignment.Left;
            lblAmountValue.Font = myTNBFont.MuseoSans14_500();
            lblAmountValue.TextColor = myTNBColor.TunaGrey();

            UIView viewLine = new UIView(new CGRect(INNER_PADDING, lblAmountValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING - TBL_PADDING, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();

            AddSubviews(new UIView[] { lblAcctNo, lblAcctNoValue, lblAcctName, lblAcctNameValue, lblAmount, lblAmountValue, viewLine });
        }
    }
}