
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ManageCards.Adapter
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
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    viewHolder.txtCardNumber.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    viewHolder.txtCardNumber.TextFormatted = Html.FromHtml(html);
                }

                if (item.CardType.Equals("VISA") || item.CardType.Equals("V"))
                {
                    viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.visa,
                        0, 0, 0);
                }
                else if (item.CardType.Equals("MASTERCARD") || item.CardType.Equals("M"))
                {
                    viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.master,
                        0, 0, 0);
                }
                else if (item.CardType.Equals("AMEX") || item.CardType.Equals("A"))
                {
                    viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_amex,
                        0, 0, 0);
                }
                else if (item.CardType.Equals("JCB") || item.CardType.Equals("J"))
                {
                    viewHolder.txtCardNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_jcb,
                        0, 0, 0);
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

            public ManageCardsViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                imgCardNumberDelete.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}