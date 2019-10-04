using System;
using Foundation;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using myTNB.Enums;
using System.Diagnostics;

namespace myTNB.Payment
{
    public class SelectPaymentTableViewSource : UITableViewSource
    {

        private RegisteredCardsResponseModel _registeredCards = new RegisteredCardsResponseModel();
        private SelectPaymentMethodViewController _controller;
        private Action<SystemEnum> OnSelectUnavailablePaymentMethod;

        public SelectPaymentTableViewSource(RegisteredCardsResponseModel registeredCards
            , SelectPaymentMethodViewController controller
            , Action<SystemEnum> onSelectUnavailablePaymentHandler)
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

            _controller = controller;
            OnSelectUnavailablePaymentMethod = onSelectUnavailablePaymentHandler;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                int? lastIndex = _registeredCards?.d?.isError?.ToLower() == "false"
                    && _registeredCards?.d != null ? _registeredCards?.d?.data?.Count : 0;

                if (indexPath.Row == lastIndex)
                {

                    const string CELLIDENTIFIER = "addCardCell";
                    AddCardCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as AddCardCell;

                    UIViewWithDashedLinerBorder backgroundView = new UIViewWithDashedLinerBorder();
                    backgroundView.Frame = new CGRect(18, 5, tableView.Frame.Width - 36, 56);
                    cell.AddSubview(backgroundView);
                    cell.BackgroundColor = MyTNBColor.SectionGrey;
                    cell.Layer.CornerRadius = 4.0f;

                    UIImageView imgAdd = new UIImageView(new CGRect(35, 20, 24, 24));
                    imgAdd.Image = UIImage.FromBundle("IC-Action-Add-Card");
                    cell.AddSubview(imgAdd);

                    UILabel lblAddCard = new UILabel(new CGRect(65, 23, 70, 18));
                    lblAddCard.Font = MyTNBFont.MuseoSans14;
                    lblAddCard.Text = _controller.GetI18NValue(PaymentConstants.I18N_AddCard);
                    lblAddCard.TextColor = UIColor.Gray;
                    cell.AddSubview(lblAddCard);
                    return cell;
                }
                else
                {
                    const string CELLIDENTIFIER = "selectPaymentCell";
                    SelectPaymentCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectPaymentCell;
                    cell.BackgroundColor = MyTNBColor.SectionGrey;

                    RegisteredCardsDataModel card = _registeredCards.d.data[indexPath.Row];
                    UIImageView imgView = new UIImageView(new CGRect(35, 16, 24, 24));

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

                    UILabel lblCardNumber = new UILabel(new CGRect(65, 19, 150, 18));
                    lblCardNumber.Font = MyTNBFont.MuseoSans14;
                    string ccNumber = card.LastDigits;
                    lblCardNumber.Text = "•••• •••• •••• " + ccNumber.Substring(ccNumber.Length - 4);
                    cell.AddSubview(lblCardNumber);
                    return cell;
                }
            }
            else
            {
                const string CELLIDENTIFIER = "fpxCell";

                fpxCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as fpxCell;
                cell.BackgroundColor = MyTNBColor.SectionGrey;

                UILabel lblTitle = new UILabel(new CGRect(65, 19, 150, 18));
                lblTitle.Font = MyTNBFont.MuseoSans14_300;
                lblTitle.TextColor = MyTNBColor.TunaGrey();
                lblTitle.Text = _controller.GetI18NValue(PaymentConstants.I18N_FPXTitle);
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
            }
            return 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            int lastIndex = _registeredCards.d.isError.ToLower() == "false" && _registeredCards.d != null ? _registeredCards.d.data.Count : 0;
            return indexPath.Section == 0 && indexPath.Row == lastIndex ? 78 : 58;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 30));
            view.BackgroundColor = MyTNBColor.SectionGrey;
            UILabel lblSectionTitle = new UILabel(new CGRect(18, 6, tableView.Frame.Width, 24));
            lblSectionTitle.TextColor = MyTNBColor.PowerBlue;
            lblSectionTitle.Font = MyTNBFont.MuseoSans16;
            lblSectionTitle.Text = section == 0 ? _controller.GetCommonI18NValue(Constants.Common_Cards)
                : _controller.GetI18NValue(PaymentConstants.I18N_OtherPaymentMethods);
            view.AddSubview(lblSectionTitle);
            return view;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (NetworkUtility.isReachable)
            {
                int lastIndex = _registeredCards.d != null ? _registeredCards.d.data.Count : 0;
                double amountValue = TextHelper.ParseStringToDouble(_controller.txtFieldAmountValue.Text);
                if (string.IsNullOrWhiteSpace(_controller.txtFieldAmountValue.Text) || amountValue == 0)
                {
                    _controller.DisplayGenericAlert(_controller.GetErrorI18NValue(Constants.Error_AmountTitle)
                        , _controller.GetErrorI18NValue(Constants.Error_AmountMessage));
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
                            _controller.DisplayGenericAlert(string.Empty, _controller.GetI18NValue(PaymentConstants.I18N_MaxCCAmountMessage));
                            return;
                        }

                        if (indexPath.Row == lastIndex)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("AddCard", null);
                            AddCardViewController addCardVC =
                                storyBoard.InstantiateViewController("AddCardViewController") as AddCardViewController;
                            addCardVC._amountDue = amountValue;
                            addCardVC._registeredCards = _registeredCards;
                            addCardVC.AccountsForPayment = _controller.AccountsForPayment;
                            addCardVC.TotalAmount = _controller.TotalAmount;
                            _controller.NavigationController.PushViewController(addCardVC, true);
                        }
                        else
                        {
                            Debug.WriteLine("Existing Credit Card Tapped!");
                            RegisteredCardsDataModel card = _registeredCards.d.data[indexPath.Row];
                            _controller.ShowCVVField(card.CardType, card.Id);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("FPX Tapped!");
                        if (!DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable)
                        {
                            OnSelectUnavailablePaymentMethod?.Invoke(SystemEnum.PaymentFPX);
                            return;
                        }
                        _controller.ExecuteRequestPayBillCall(2, "FPX", "", false, _controller.txtFieldAmountValue.Text);

                    }
                }
            }
            else
            {
                _controller.DisplayNoDataAlert();
            }
        }
    }
}