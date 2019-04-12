using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.More.MyAccount.ManageCards
{
    public class ManageCardsDataSource : UITableViewSource
    {
        ManageCardViewController _controller;
        List<RegisteredCardsDataModel> _registeredCards = new List<RegisteredCardsDataModel>();

        Dictionary<string, int[]> cardFormatPattern = new Dictionary<string, int[]>
        {
            {"V", new int[] { 4, 4, 4, 4 }},
            {"M", new int[] { 4, 4, 4, 4 }},
            {"A", new int[] { 4, 6, 5}},
            {"J", new int[] { 4, 4, 4, 4 }}
        };

        public ManageCardsDataSource(ManageCardViewController controller)
        {
            _controller = controller;
            if(DataManager.DataManager.SharedInstance.RegisteredCards != null
               && DataManager.DataManager.SharedInstance.RegisteredCards.d != null
               && DataManager.DataManager.SharedInstance.RegisteredCards.d.data != null){
                _registeredCards = DataManager.DataManager.SharedInstance.RegisteredCards.d.data;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _registeredCards.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string cardNo = _registeredCards[indexPath.Row].LastDigits;
            string maskedCardNo = cardNo.Substring(cardNo.Length - 4).PadLeft(cardNo.Length, '•');
            string cardType = _registeredCards[indexPath.Row].CardType;
            string formattedCardNo = FormatCard(maskedCardNo, cardType);

            var cell = tableView.DequeueReusableCell("ManageCardViewCell", indexPath) as ManageCardViewCell;
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 58);
            cell.lblCardNo.Text = formattedCardNo;
            cell.imgViewCC.Image = UIImage.FromBundle(GetCardIcon(cardType));
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 58;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _controller.HandleDeleteCardEvent(indexPath.Row);
        }

        internal string GetCardIcon(string cardType)
        {
            string cardIcon = string.Empty;
            switch (cardType)
            {
                case "M":
                    {
                        cardIcon = "IC-Payment-Card-Mastercard";
                        break;
                    }
                case "V":
                    {
                        cardIcon = "IC-Payment-Card-Visa";
                        break;
                    }
                case "A":
                    {
                        cardIcon = "IC-Payment-Card-Amex";
                        break;
                    }
                case "J":
                    {
                        cardIcon = string.Empty;//image not evailable yet
                        break;
                    }
                default:
                    {
                        cardIcon = string.Empty;
                        break;
                    }
            };
            return cardIcon;
        }

        internal string FormatCard(string cardNo, string cardType)
        {
            Debug.WriteLine("cardType: " + cardType);
            if(string.IsNullOrEmpty(cardType)){
                return cardNo;
            }
            int[] format = cardFormatPattern[cardType];
            int start = 0;
            int length = 0;
            string result = string.Empty;

            string cardNoHolder = cardNo.Replace(" ", "");
            for (int i = 0; i < format.Length && cardNoHolder.Length != 0; i++)
            {
                length = format[i];
                if (length > cardNoHolder.Length)
                {
                    length = cardNoHolder.Length;
                }
                string section = cardNoHolder.Substring(start, length);
                if (i > 0)
                {
                    result += " ";
                }
                result += section;
                cardNoHolder = cardNoHolder.Remove(start, length);
            }
            return result;
        }
    }
}