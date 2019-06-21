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
    public class BillsListAdapter : BaseCustomAdapter<BillHistoryV5>
    {
        AccountData accountData;

        public BillsListAdapter(Context context) : base(context)
        {
        }

        public BillsListAdapter(Context context, bool notify, AccountData accountData) : base(context, notify)
        {
            this.accountData = accountData;
        }

        public BillsListAdapter(Context context, bool notify) : base(context, notify)
        {

        }

        public BillsListAdapter(Context context, List<BillHistoryV5> itemList) : base(context, itemList)
        {
        }

        public BillsListAdapter(Context context, List<BillHistoryV5> itemList, bool notify) : base(context, itemList, notify)
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
                BillsListViewHolder vh = null;
                BillHistoryV5 BillHistoryV5 = GetItemObject(position);
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(context).Inflate(Resource.Layout.BillsRowView, parent, false);
                    vh = new BillsListViewHolder(convertView);
                    convertView.Tag = vh;
                }
                else
                {
                    vh = convertView.Tag as BillsListViewHolder;
                }
                Date d = null;
                try
                {
                    d = dateParser.Parse(BillHistoryV5.DtBill);
                }
                catch (Java.Text.ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                if (d != null)
                {
                    vh.txtDate.Text = dateFormatter.Format(d);
                    if (accountData != null && accountData.AccountCategoryId.Equals("2"))
                    {
                        vh.txtBillDate.Text = billDateFormatter.Format(d);// + " Advice";
                    }
                    else
                    {
                        vh.txtBillDate.Text = billDateFormatter.Format(d);// + " Bill";
                    }

                    ///<summary>
                    /// Revert non owner CR changes
                    ///</summary>
                    //if(accountData != null && !accountData.IsOwner)
                    //{
                    //    vh.btnExpand.Visibility = ViewStates.Gone;
                    //}
                    //else
                    //{
                    //    vh.btnExpand.Visibility = ViewStates.Visible;
                    //}

                    Calendar nextMonth = Calendar.GetInstance(Locale.Default);
                    nextMonth.TimeInMillis = d.Time;
                    nextMonth.Add(CalendarField.Month, 1);
                    vh.txtRange.Text = rangeFormatter.Format(d) + " - " + rangeFormatter.Format(nextMonth.Time);

                }

                if (accountData != null && accountData.AccountCategoryId.Equals("2"))
                {
                    double calAmt = BillHistoryV5.AmPayable * -1;
                    if (calAmt <= 0)
                    {
                        calAmt = 0.00;
                    }
                    else
                    {
                        calAmt = System.Math.Abs(BillHistoryV5.AmPayable);
                    }
                    vh.txtAmtPayable.Text = "RM " + payableFormatter.Format(calAmt);
                }
                else
                {
                    vh.txtAmtPayable.Text = "RM " + payableFormatter.Format(BillHistoryV5.AmPayable);
                }


                TextViewUtils.SetMuseoSans500Typeface(vh.txtBillDate, vh.txtDate, vh.txtAmtPayable);
                TextViewUtils.SetMuseoSans300Typeface(vh.txtRange);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }


        class BillsListViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtDate)]
            internal TextView txtDate;

            [BindView(Resource.Id.txtBillDate)]
            internal TextView txtBillDate;

            [BindView(Resource.Id.txtAmtPayable)]
            internal TextView txtAmtPayable;

            [BindView(Resource.Id.txtRange)]
            internal TextView txtRange;

            [BindView(Resource.Id.btnExpand)]
            internal ImageView btnExpand;


            public BillsListViewHolder(View itemView) : base(itemView)
            {

            }
        }
    }
}