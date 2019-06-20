using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu
{
    public class REPaymentListAdapter : BaseCustomAdapter<PaymentHistoryRE>
    {

        public REPaymentListAdapter(Context context) : base(context)
        {
        }

        public REPaymentListAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public REPaymentListAdapter(Context context, List<PaymentHistoryRE> itemList) : base(context, itemList)
        {
        }

        public REPaymentListAdapter(Context context, List<PaymentHistoryRE> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
                SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM");
                SimpleDateFormat billDateFormatter = new SimpleDateFormat("MMM yyyy");
                SimpleDateFormat rangeFormatter = new SimpleDateFormat("dd MMM yyyy");
                DecimalFormat payableFormatter = new DecimalFormat("#,###,###,###,###,##0.00");
                REPaymentListViewHolder vh;
                PaymentHistoryRE PaymentHistoryRE = GetItemObject(position);
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(context).Inflate(Resource.Layout.PaymentRowView, parent, false);
                    vh = new REPaymentListViewHolder(convertView);
                    convertView.Tag = vh;
                }
                else
                {
                    vh = convertView.Tag as REPaymentListViewHolder;
                }

                Date d = null;
                try
                {
                    d = dateParser.Parse(PaymentHistoryRE.PaidDate);
                    vh.txtDate.Text = dateFormatter.Format(d);

                }
                catch (Java.Text.ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                //vh.txtPayment.Text = "RM " + payableFormatter.Format(PaymentHistoryV5.AmtPaid);
                vh.txtAmtPayable.Text = "RM " + payableFormatter.Format(PaymentHistoryRE.Amount);
                vh.txtPaymentFrom.Text = "from TNB";

                vh.btnExpand.Visibility = ViewStates.Gone;

                TextViewUtils.SetMuseoSans500Typeface(vh.txtPayment, vh.txtDate, vh.txtAmtPayable);
                TextViewUtils.SetMuseoSans300Typeface(vh.txtPaymentFrom);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

        class REPaymentListViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtDate)]
            internal TextView txtDate;

            [BindView(Resource.Id.txtPayment)]
            internal TextView txtPayment;

            [BindView(Resource.Id.txtAmtPayable)]
            internal TextView txtAmtPayable;

            [BindView(Resource.Id.txtPaymentFrom)]
            internal TextView txtPaymentFrom;

            [BindView(Resource.Id.btnExpand)]
            internal ImageView btnExpand;

            public REPaymentListViewHolder(View itemView) : base(itemView)
            {
            }
        }
    }
}