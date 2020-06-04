﻿using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MultipleAccountPayment.Adapter
{
    class MPAddCardAdapter : BaseAdapter
    {
        List<CreditCard> cardList;
        Android.App.Activity activity;
        public event EventHandler<int> OnItemClick;

        public MPAddCardAdapter(Android.App.Activity activity, List<CreditCard> cards)
        {
            this.activity = activity;
            cardList = cards;
        }

        public override int Count
        {
            get { return cardList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public CreditCard GetCardDetailsAt(int position)
        {
            return cardList[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        void OnClick(int position)
        {
            if (OnItemClick != null)
                OnItemClick(this, position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(
            Resource.Layout.CardItemView, parent, false);
            try
            {
                Button cardView = view.FindViewById<Button>(Resource.Id.btnCard);
                cardView.Click += delegate
                {
                    OnClick(position);
                };
                TextViewUtils.SetMuseoSans500Typeface(cardView);
                string lastDigit = cardList[position].LastDigits.Substring(cardList[position].LastDigits.Length - 4);
                string html = "<![CDATA[" + activity.GetString(Resource.String.credit_card_masked) + lastDigit + "]]>";
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                {
                    cardView.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    cardView.TextFormatted = Html.FromHtml(html);
                }

                if (cardList[position].CardType.Equals("VISA") || cardList[position].CardType.Equals("V"))
                {
                    cardView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.visa,
                        0, 0, 0);
                }
                else if (cardList[position].CardType.Equals("MASTERCARD") || cardList[position].CardType.Equals("M"))
                {
                    cardView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.master,
                        0, 0, 0);
                }
                else if (cardList[position].CardType.Equals("AMEX") || cardList[position].CardType.Equals("A"))
                {
                    cardView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_amex,
                        0, 0, 0);
                }
                else if (cardList[position].CardType.Equals("JCB") || cardList[position].CardType.Equals("J"))
                {
                    cardView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_payment_card_jcb,
                        0, 0, 0);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return view;
        }
    }
}