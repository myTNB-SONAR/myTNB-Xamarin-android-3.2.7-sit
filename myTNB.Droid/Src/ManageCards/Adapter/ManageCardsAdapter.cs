
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ManageCards.Adapter
{
    public class ManageCardsAdapter : BaseRecyclerAdapter<CreditCardData>
    {

        public event EventHandler<int> RemoveClick;

        public ManageCardsAdapter(bool notify) : base(notify)
        {
        }

        public ManageCardsAdapter(List<CreditCardData> itemList) : base(itemList)
        {
        }

        public ManageCardsAdapter(List<CreditCardData> itemList, bool notify) : base(itemList, notify)
        {
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ManageCardsViewHolder viewHolder = holder as ManageCardsViewHolder;
            try
            {
                CreditCardData item = GetItemObject(position);

                string lastDigit = item.LastDigits.Substring(item.LastDigits.Length - 4);
                string html = "<![CDATA[" + viewHolder.ItemView.Context.GetString(Resource.String.credit_card_masked) + lastDigit + "]]>";
                viewHolder.txtExpiredCard.Text = Utility.GetLocalizedLabel("ManageCards", "CCExpired");
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    viewHolder.txtCardNumber.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                    TextViewUtils.SetTextSize16(viewHolder.txtCardNumber);
                    TextViewUtils.SetTextSize12(viewHolder.txtExpiredCard);
                }
                else
                {
                    viewHolder.txtCardNumber.TextFormatted = Html.FromHtml(html);
                    TextViewUtils.SetTextSize16(viewHolder.txtCardNumber);
                    TextViewUtils.SetTextSize12(viewHolder.txtExpiredCard);
                }

                if (item.IsExpired)
                {
                    viewHolder.txtExpiredCard.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.txtCardNumber.SetPadding(0, 0, 0, 8);
                    viewHolder.txtExpiredCard.Visibility = ViewStates.Gone;
                }

                if (item.CardType.Equals("VISA") || item.CardType.Equals("V"))
                {
                    //viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.visa,
                    //    0, 0, 0);
                    viewHolder.imgCard.SetImageResource(Resource.Drawable.visa);
                }
                else if (item.CardType.Equals("MASTERCARD") || item.CardType.Equals("M"))
                {
                    //viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.master,
                    //    0, 0, 0);
                    viewHolder.imgCard.SetImageResource(Resource.Drawable.master);
                }
                else if (item.CardType.Equals("AMEX") || item.CardType.Equals("A"))
                {
                    //viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_amex,
                    //    0, 0, 0);
                    viewHolder.imgCard.SetImageResource(Resource.Drawable.ic_payment_card_amex);
                }
                else if (item.CardType.Equals("JCB") || item.CardType.Equals("J"))
                {
                    //viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_jcb,
                    //    0, 0, 0);
                    viewHolder.imgCard.SetImageResource(Resource.Drawable.ic_payment_card_jcb);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
           
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new ManageCardsViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ManageCardsRow, parent, false), OnClick);
        }

        void OnClick(int position)
        {
            if (RemoveClick != null)
                RemoveClick(this, position);
        }

        class ManageCardsViewHolder : BaseRecyclerViewHolder
        {
            [BindView(Resource.Id.txtCardNumber)]
            public TextView txtCardNumber;

            [BindView(Resource.Id.imgCardNumberDelete)]
            public ImageView imgCardNumberDelete;

            [BindView(Resource.Id.imgCard)]
            public ImageView imgCard;

            [BindView(Resource.Id.txtExpiredCard)]
            public TextView txtExpiredCard;
           

            public ManageCardsViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                imgCardNumberDelete.Click += (sender, e) => listener(base.LayoutPosition);
            }

            
        }
    }
}