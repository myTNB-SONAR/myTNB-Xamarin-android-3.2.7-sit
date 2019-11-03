using CoreGraphics;
using System;
using UIKit;

namespace myTNB
{
    public partial class ReceiptTableViewCell : UITableViewCell
    {
        public UILabel lblAcctNo, lblAcctNoValue, lblAcctName, lblAcctNameValue
            , lblAmount, lblAmountValue;
        const float TBL_PADDING = 20f;
        const float INNER_PADDING = 20f;
        const float LBL_WIDTH_PADDING = INNER_PADDING * 2;

        public ReceiptTableViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

            lblAcctNo = new UILabel(new CGRect(INNER_PADDING, INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_AccountNo).ToUpper()
            };

            lblAcctNoValue = new UILabel(new CGRect(INNER_PADDING, lblAcctNo.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            lblAcctName = new UILabel(new CGRect(INNER_PADDING, lblAcctNoValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_AccountHolder).ToUpper()
            };

            lblAcctNameValue = new UILabel(new CGRect(INNER_PADDING, lblAcctName.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            lblAmount = new UILabel(new CGRect(INNER_PADDING, lblAcctNameValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_AmountRM).ToUpper()
            };

            lblAmountValue = new UILabel(new CGRect(INNER_PADDING, lblAmount.Frame.GetMaxY(), cellWidth - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            UIView viewLine = GenericLine.GetLine(new CGRect(INNER_PADDING
                , lblAmountValue.Frame.GetMaxY() + INNER_PADDING, cellWidth - LBL_WIDTH_PADDING - TBL_PADDING, 1));

            AddSubviews(new UIView[] { lblAcctNo, lblAcctNoValue, lblAcctName, lblAcctNameValue, lblAmount, lblAmountValue, viewLine });
        }
    }
}