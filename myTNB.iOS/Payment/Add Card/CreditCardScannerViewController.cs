using System;
using Card.IO;
using CoreGraphics;
using UIKit;

namespace myTNB.Payment.AddCard
{
    public partial class CreditCardScannerViewController : UIViewController
    {
        public CreditCardScannerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            SetNavigationItems();
            SetSubviews();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        internal void SetNavigationItems()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetSubviews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 60));
            lblDescription.Font = myTNBFont.MuseoSans16_300();
            lblDescription.TextColor = myTNBColor.TunaGrey();
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Lines = 0;
            lblDescription.Text = "Payment_ScanMessage".Translate();
            lblDescription.TextAlignment = UITextAlignment.Left;
            View.AddSubview(lblDescription);

            CardIOView cardIOView = new CardIOView();
            cardIOView.Frame = new CGRect(0, lblDescription.Frame.GetMaxY() + 10, View.Frame.Width, View.Frame.Height - 130);
            cardIOView.HideCardIOLogo = true;
            cardIOView.Delegate = new CardIODelegate(this);
            View.AddSubview(cardIOView);

        }

        class CardIODelegate : CardIOViewDelegate
        {
            UIViewController _controller;
            public CardIODelegate(UIViewController controller)
            {
                _controller = controller;
            }
            public override void DidScanCard(CardIOView cardIOView, CreditCardInfo cardInfo)
            {
                if (cardInfo != null)
                {
                    DataManager.DataManager.SharedInstance.CreditCardInfo.CreditCardNumber = cardInfo.CardNumber;
                    DataManager.DataManager.SharedInstance.CreditCardInfo.CardHolderName = cardInfo.CardholderName;
                    DataManager.DataManager.SharedInstance.CreditCardInfo.ExpiryDate = cardInfo.ExpiryMonth + "/" + cardInfo.ExpiryYear;
                    DataManager.DataManager.SharedInstance.CreditCardInfo.CVV = cardInfo.Cvv;
                    DataManager.DataManager.SharedInstance.CreditCardInfo.CardType = cardInfo.CardType.ToString();
                    _controller?.NavigationController?.PopViewController(true);
                }

            }
        }
    }
}