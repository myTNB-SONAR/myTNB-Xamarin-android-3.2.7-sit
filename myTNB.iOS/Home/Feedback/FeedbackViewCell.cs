using System; using UIKit; using CoreGraphics;  namespace myTNB {     public partial class FeedbackViewCell : UITableViewCell     {         public UILabel lblTitle;         public UILabel lblSubtTitle;         public UIView viewLine;         public UILabel lblCount;         public UIImageView imgViewIcon;          public FeedbackViewCell(IntPtr handle) : base(handle)         {             nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;             nfloat cellHeight = 80;             Frame = new CGRect(0, 0, cellWidth, cellHeight);              imgViewIcon = new UIImageView(new CGRect(16, 13, 48, 48))
            {
                Image = UIImage.FromBundle("Feedback-Submitted-Others")
            };              lblTitle = new UILabel(new CGRect(80, 16, cellWidth - 96, 16))
            {
                TextColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans16_500()
            };              lblSubtTitle = new UILabel(new CGRect(80, 38, cellWidth - 96, 16))
            {
                TextColor = myTNBColor.TunaGrey(),
                Font = myTNBFont.MuseoSans12_300()
            };              lblCount = new UILabel(new CGRect(cellWidth - 38, 16, 20, 16))
            {
                TextColor = myTNBColor.PowerBlue(),
                TextAlignment = UITextAlignment.Right,
                Font = myTNBFont.MuseoSans12(),
                Hidden = true
            };              viewLine = new UIView(new CGRect(0, cellHeight - 7, cellWidth, 7))
            {
                BackgroundColor = myTNBColor.SectionGrey(),
                Hidden = false
            };              AddSubviews(new UIView[] { imgViewIcon, lblTitle, lblSubtTitle, lblCount, viewLine });         }     } }