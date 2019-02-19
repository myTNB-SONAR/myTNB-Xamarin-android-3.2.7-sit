using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using Java.Util;
using Android.Net;
using myTNB_Android.Src.Utils;
using Java.Text;
using Android.Text;

namespace myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu
{
    public class PaymentListAdapter : BaseCustomAdapter<PaymentHistoryV5>
    {

        public PaymentListAdapter(Context context) : base(context)
        {
        }

        public PaymentListAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public PaymentListAdapter(Context context, List<PaymentHistoryV5> itemList) : base(context, itemList)
        {
        }

        public PaymentListAdapter(Context context, List<PaymentHistoryV5> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
            SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM");
            SimpleDateFormat billDateFormatter = new SimpleDateFormat("MMM yyyy");
            SimpleDateFormat rangeFormatter = new SimpleDateFormat("dd MMM yyyy");
            DecimalFormat payableFormatter = new DecimalFormat("#,###,###,###,###,##0.00");
            PaymentListViewHolder vh;
            PaymentHistoryV5 PaymentHistoryV5 = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.PaymentRowView, parent, false);
                vh = new PaymentListViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as PaymentListViewHolder;
            }

            Date d = null;
            try
            {
                d = dateParser.Parse(PaymentHistoryV5.DtEvent);
                vh.txtDate.Text = dateFormatter.Format(d);
                
            }
            catch (Java.Text.ParseException e)
            {

            }

            //vh.txtPayment.Text = "RM " + payableFormatter.Format(PaymentHistoryV5.AmtPaid);
            vh.txtAmtPayable.Text = "RM " + payableFormatter.Format(PaymentHistoryV5.AmtPaid);
            vh.txtPaymentFrom.Text = PaymentHistoryV5.CdPBranch;

            if (!TextUtils.IsEmpty(PaymentHistoryV5.NmPBranch))
            {
                vh.txtPaymentFrom.Text = "via " + PaymentHistoryV5.NmPBranch;

            }
            else
            {
                vh.txtPaymentFrom.Text = "";
            }

            if(String.IsNullOrEmpty(PaymentHistoryV5.MechantTransId))
            {
                vh.btnExpand.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.btnExpand.Visibility = ViewStates.Visible;
            }

            TextViewUtils.SetMuseoSans500Typeface(vh.txtPayment , vh.txtDate , vh.txtAmtPayable);
            TextViewUtils.SetMuseoSans300Typeface(vh.txtPaymentFrom);

            return convertView;
        }

        class PaymentListViewHolder : BaseAdapterViewHolder
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

            public PaymentListViewHolder(View itemView) : base(itemView)
            {
            }
        }
    }
}