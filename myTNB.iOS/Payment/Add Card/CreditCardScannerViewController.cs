using System;
using Card.IO;
using CoreGraphics;
using myTNB.Payment.AddCard;
using UIKit;

namespace myTNB.Payment
{
    public partial class CreditCardScannerViewController : CustomUIViewController
    {
        public CreditCardScannerViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = AddCardConstants.Pagename_CardScanner;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            SetNavigationItems();
            SetSubviews();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void SetNavigationItems()
        {
            NavigationItem.HidesBackButton = true;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(AddCardConstants.I18N_Title);
        }

        private void SetSubviews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 60))
            {
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = GetI18NValue(AddCardConstants.I18N_ScanMessage),
                TextAlignment = UITextAlignment.Left
            };
            View.AddSubview(lblDescription);

            CardIOView cardIOView = new CardIOView
            {
                Frame = new CGRect(0, lblDescription.Frame.GetMaxY() + 10, View.Frame.Width, View.Frame.Height - 130),
                HideCardIOLogo = true,
                Delegate = new CardIODelegate(this),
                ScanInstructions = GetI18NValue(AddCardConstants.I18N_ScanInstructions)
            };
            View.AddSubview(cardIOView);
        }

        private class CardIODelegate : CardIOViewDelegate
        {
            private UIViewController _controller;
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