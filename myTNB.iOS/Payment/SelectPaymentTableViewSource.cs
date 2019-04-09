using System;
using Foundation;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Globalization;
using myTNB.Enums;


namespace myTNB.Payment.AddCard
{
    public class SelectPaymentTableViewSource : UITableViewSource
    {

        RegisteredCardsResponseModel _registeredCards = new RegisteredCardsResponseModel();
        RequestPayBillResponseModel _requestPayBill = new RequestPayBillResponseModel();
        SelectPaymentMethodViewController _selectPaymentMethodVC;
        Action<SystemEnum> OnSelectUnavailablePaymentMethod;

        public SelectPaymentTableViewSource(RegisteredCardsResponseModel registeredCards, RequestPayBillResponseModel requestPayBill, SelectPaymentMethodViewController selectPaymentMethodVC,
                                            Action<SystemEnum> onSelectUnavailablePaymentHandler)
        {
            if (registeredCards != null && registeredCards.d != null && registeredCards.d.data != null)
            {
                _registeredCards = registeredCards;
            }
            else
            {
                _registeredCards.d = new RegisteredCardsModel();
                _registeredCards.d.data = new System.Collections.Generic.List<RegisteredCardsDataModel>();
            }

            if (requestPayBill != null && requestPayBill.d != null && requestPayBill.d.data != null)
            {
                _requestPayBill = requestPayBill;
            }
            else
            {
                _requestPayBill.d = new RequestPayBillModel();
                _requestPayBill.d.data = new RequestPayBillDataModel();
            }

            _selectPaymentMethodVC = selectPaymentMethodVC;
            OnSelectUnavailablePaymentMethod = onSelectUnavailablePaymentHandler;

        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                var lastIndex = _registeredCards?.d?.isError?.ToLower() == "false" && _registeredCards?.d != null ? _registeredCards?.d?.data?.Count : 0;

                if (indexPath.Row == lastIndex)
                {

                    const string CELLIDENTIFIER = "addCardCell";
                    var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as AddCardCell;

                    var backgroundView = new UIViewWithDashedLinerBorder();
                    backgroundView.Frame = new CGRect(18, 5, tableView.Frame.Width - 36, 56);
                    //backgroundView.Layer.BorderColor = UIColor.Yellow.CGColor;
                    cell.AddSubview(backgroundView);
                    cell.BackgroundColor = myTNBColor.SectionGrey();
                    cell.Layer.CornerRadius = 4.0f;

                    var imgAdd = new UIImageView(new CGRect(35, 20, 24, 24));
                    imgAdd.Image = UIImage.FromBundle("IC-Action-Add-Card");
                    cell.AddSubview(imgAdd);

                    var lblAddCard = new UILabel(new CGRect(65, 23, 70, 18));
                    lblAddCard.Font = myTNBFont.MuseoSans14();
                    lblAddCard.Text = "Add Card";
                    lblAddCard.TextColor = UIColor.Gray;
                    cell.AddSubview(lblAddCard);

                    return cell;
                }
                else
                {
                    const string CELLIDENTIFIER = "selectPaymentCell";
                    var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectPaymentCell;
                    cell.BackgroundColor = myTNBColor.SectionGrey();

                    var card = _registeredCards.d.data[indexPath.Row];

                    var imgView = new UIImageView(new CGRect(35, 16, 24, 24));

                    cell.AddSubview(imgView);
                    if (card.CardType == "V")
                    {
                        imgView.Image = UIImage.FromBundle("IC-Payment-Card-Visa");
                    }
                    else if (card.CardType == "M")
                    {
                        imgView.Image = UIImage.FromBundle("IC-Payment-Card-Mastercard");
                    }
                    else if (card.CardType == "A")
                    {
                        imgView.Image = UIImage.FromBundle("IC-Payment-Card-Amex");
                    }
                    else
                    {
                    }

                    var lblCardNumber = new UILabel(new CGRect(65, 19, 150, 18));
                    lblCardNumber.Font = myTNBFont.MuseoSans14();
                    var ccNumber = card.LastDigits;
                    lblCardNumber.Text = "•••• •••• •••• " + ccNumber.Substring(ccNumber.Length - 4);
                    cell.AddSubview(lblCardNumber);

                    return cell;
                }
            }
            else
            {
                const string CELLIDENTIFIER = "fpxCell";

                var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as fpxCell;
                cell.BackgroundColor = myTNBColor.SectionGrey();

                var lblTitle = new UILabel(new CGRect(65, 19, 150, 18));
                lblTitle.Font = myTNBFont.MuseoSans14_300();
                lblTitle.TextColor = myTNBColor.TunaGrey();
                lblTitle.Text = "Online Banking via FPX";

                cell.AddSubview(lblTitle);

                return cell;
            }

        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                if (_registeredCards != null && _registeredCards.d != null
                   && _registeredCards.d.isError.ToLower().Equals("false")
                   && _registeredCards.d.data != null)
                {
                    return _registeredCards.d.data.Count + 1;
                }
                return 1;
            }
            else
            {
                return 1;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var lastIndex = _registeredCards.d.isError.ToLower() == "false" && _registeredCards.d != null ? _registeredCards.d.data.Count : 0;

            if (indexPath.Section == 0 && indexPath.Row == lastIndex)
            {
                return 78;
            }

            return 58;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 30));
            view.BackgroundColor = myTNBColor.SectionGrey();

            var lblSectionTitle = new UILabel(new CGRect(18, 6, tableView.Frame.Width, 24));

            lblSectionTitle.TextColor = myTNBColor.PowerBlue();
            lblSectionTitle.Font = myTNBFont.MuseoSans16();
            if (section == 0)
            {
                lblSectionTitle.Text = "Credit / Debit Card";
            }
            else
            {
                lblSectionTitle.Text = "Other Payment Methods";
            }
            view.AddSubview(lblSectionTitle);
            return view;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (NetworkUtility.isReachable)
            {
                var lastIndex = _registeredCards.d != null ? _registeredCards.d.data.Count : 0;

                double amountValue = TextHelper.ParseStringToDouble(_selectPaymentMethodVC.txtFieldAmountValue.Text);

                if (string.IsNullOrWhiteSpace(_selectPaymentMethodVC.txtFieldAmountValue.Text) || amountValue == 0)
                {
                    var alert = UIAlertController.Create("Invalid Amount", "Kindly enter a non-zero/positive amount", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                    _selectPaymentMethodVC.PresentViewController(alert, animated: true, completionHandler: null);
                }
                else
                {
                    if (indexPath.Section == 0)
                    {
                        if (!DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable)
                        {
                            OnSelectUnavailablePaymentMethod?.Invoke(SystemEnum.PaymentCreditCard);
                            return;
                        }

                        if (amountValue > 5000)
                        {
                            _selectPaymentMethodVC.DisplayPaymentThreshold();
                            return;
                        }

                        if (indexPath.Row == lastIndex)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("AddCard", null);
                            AddCardViewController addCardVC =
                                storyBoard.InstantiateViewController("AddCardViewController") as AddCardViewController;
                            addCardVC._amountDue = amountValue;
                            addCardVC._registeredCards = _registeredCards;
                            addCardVC.AccountsForPayment = _selectPaymentMethodVC.AccountsForPayment;
                            addCardVC.TotalAmount = _selectPaymentMethodVC.TotalAmount;
                            var navController = new UINavigationController(addCardVC);
                            _selectPaymentMethodVC.NavigationController.PushViewController(addCardVC, true);
                        }
                        else
                        {
                            Console.WriteLine("Existing Credit Card Tapped!");
                            var card = _registeredCards.d.data[indexPath.Row];
                            _selectPaymentMethodVC.ShowCVVField(card.CardType, card.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("FPX Tapped!");

                        if (!DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable)
                        {
                            OnSelectUnavailablePaymentMethod?.Invoke(SystemEnum.PaymentFPX);
                            return;
                        }

                        _selectPaymentMethodVC.ExecuteRequestPayBillCall(2, "FPX", "", false, _selectPaymentMethodVC.txtFieldAmountValue.Text);

                    }
                }
            }
            else
            {
                var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                _selectPaymentMethodVC.PresentViewController(alert, animated: true, completionHandler: null);
            }
        }
    }
}