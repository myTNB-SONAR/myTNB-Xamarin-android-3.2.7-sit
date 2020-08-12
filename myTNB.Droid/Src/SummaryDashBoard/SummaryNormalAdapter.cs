
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.SummaryDashBoard.SummaryListener;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SummaryDashBoard
{
    public class SummaryNormalAdapter : BaseRecyclerAdapter<SummaryDashBoardDetails>
    {

        List<SummaryDashBoardDetails> normalAccount = null;

        private const int BODY = 2;
        private const int HEADER = 1;

        private ISummaryListener iSummaryListener = null;



        public SummaryNormalAdapter(List<SummaryDashBoardDetails> itemList, ISummaryListener iSummaryListener) : base(itemList)
        {
            this.iSummaryListener = iSummaryListener;

            normalAccount = itemList;
        }


        public override int GetItemViewType(int position)
        {
            if (position == 0)
            {
                return HEADER;
            }

            //else if (position > (count - 1)) {
            //    return FOOTER;
            //}

            return BODY;
        }

        public override int ItemCount
        {
            get
            {
                return (normalAccount.Count + 1);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (position == 0)
            {
                var viewHolder1 = holder as SummaryDashBoardHeaderViewHolder;

                viewHolder1.SetHeaderText("Accounts");
                viewHolder1.SetRightText("Due");

            }
            else
            {
                var viewHolder3 = holder as SummaryDashBoardViewHolder;
                viewHolder3.PopulateData(normalAccount[position - 1]);
                //viewHolder3.SetAcountName(normalAccount[position - 1].AccName);
                //viewHolder3.SetAcountNumber(normalAccount[position - 1].AccNumber);
                //viewHolder3.SetDueDate(normalAccount[position - 1].BillDueDate, normalAccount[position - 1].AmountDue);
                //viewHolder3.SetAmountText(normalAccount[position - 1].AmountDue);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case HEADER:
                    return new SummaryDashBoardHeaderViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SummaryDashBoardHeader, parent, false));

                //case FOOTER:
                //return new SummaryDashBoardFooterViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SummaryDashBoardFooter, parent, false));

                default:
                    return new SummaryDashBoardViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SummaryDashboardAdapter, parent, false), iSummaryListener);

            }
        }




        class SummaryDashBoardViewHolder : BaseRecyclerViewHolder, View.IOnClickListener
        {

            [BindView(Resource.Id.accountNameTxt)]
            TextView accountNameText;

            [BindView(Resource.Id.amount)]
            TextView amountText;

            [BindView(Resource.Id.due_date)]
            TextView dueDate;

            [BindView(Resource.Id.accountNumber)]
            TextView accountNumberText;


            [BindView(Resource.Id.rmText)]
            TextView rmCurrencyText;

            [BindView(Resource.Id.adapterLayout)]
            LinearLayout mAdapterLayout;

            private ISummaryListener iSummaryListener = null;
            private SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();

            Java.Text.DecimalFormat decimalFormat = new Java.Text.DecimalFormat("#,###,###,###,##0.00");

            Java.Text.SimpleDateFormat dateParser = new Java.Text.SimpleDateFormat("dd/MM/yyyy");
            Java.Text.SimpleDateFormat dateFormatter = new Java.Text.SimpleDateFormat("dd MMM");

            public SummaryDashBoardViewHolder(View itemView, ISummaryListener iSummaryListener) : base(itemView)
            {
                this.iSummaryListener = iSummaryListener;
                TextViewUtils.SetMuseoSans500Typeface(rmCurrencyText);
                TextViewUtils.SetMuseoSans500Typeface(accountNameText);
                TextViewUtils.SetMuseoSans500Typeface(amountText);
                TextViewUtils.SetMuseoSans300Typeface(dueDate);
                TextViewUtils.SetMuseoSans300Typeface(accountNumberText);

                mAdapterLayout.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.adapterLayout:
                        try
                        {
                            if (summaryDashBoardDetails != null)
                            {
                                iSummaryListener.OnClick(summaryDashBoardDetails);
                            }
                            else
                            {
                                iSummaryListener.OnClick(null);
                            }
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        break;
                }
            }


            public void PopulateData(SummaryDashBoardDetails summaryDashBoardDetails)
            {
                try
                {
                    this.summaryDashBoardDetails = summaryDashBoardDetails;
                    SetAcountName(summaryDashBoardDetails.AccName);
                    SetAmountText(summaryDashBoardDetails.AmountDue);
                    SetAcountNumber(summaryDashBoardDetails.AccNumber);
                    SetDueDate(summaryDashBoardDetails.BillDueDate, summaryDashBoardDetails.AmountDue);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            public void SetAcountName(String accountName)
            {
                try
                {
                    accountNameText.Text = "";

                    if (!string.IsNullOrEmpty(accountName))
                    {
                        accountNameText.Text = accountName;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }


            public void SetAcountNumber(String accountNumberValue)
            {
                try
                {
                    accountNumberText.Text = "";

                    if (!string.IsNullOrEmpty(accountNumberValue))
                    {
                        accountNumberText.Text = accountNumberValue;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public void SetAmountText(String amount)
            {
                try
                {
                    amountText.Text = "";

                    if (!string.IsNullOrEmpty(amount))
                    {
                        if (amount != "--")
                        {
                            double amt = Convert.ToDouble(amount);
                            amountText.Text = " " + decimalFormat.Format(amt);
                        }
                        else
                        {
                            rmCurrencyText.Text = "";
                            amountText.Text = "--";
                        }

                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public void SetDueDate(String dueDateValue, String amount)
            {
                try
                {
                    if (amount != "--")
                    {
                        double amt = Convert.ToDouble(amount);

                        dueDate.Text = "--";

                        if (!string.IsNullOrEmpty(dueDateValue) && amt > 0)
                        {

                            Date d = null;
                            try
                            {
                                d = dateParser.Parse(dueDateValue);
                            }
                            catch (ParseException e)
                            {

                                Utility.LoggingNonFatalError(e);

                            }
                            string dt = dateFormatter.Format(d);

                            //DateTime dt = Convert.ToDateTime(dueDateValue);
                            //dueDate.Text = dt.ToString("dd MMM");

                            dueDate.Text = dt;
                        }
                    }
                    else
                    {
                        dueDate.Text = "-";
                    }

                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

            }
        }


        class SummaryDashBoardHeaderViewHolder : BaseRecyclerViewHolder
        {
            [BindView(Resource.Id.headerText)]
            TextView headerTextValue;


            [BindView(Resource.Id.rightText)]
            TextView rightTextValue;


            public SummaryDashBoardHeaderViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans500Typeface(rightTextValue);
                TextViewUtils.SetMuseoSans500Typeface(headerTextValue);
            }


            public void SetHeaderText(String headertext)
            {
                try
                {
                    headerTextValue.Text = "";

                    if (!string.IsNullOrEmpty(headertext))
                    {
                        headerTextValue.Text = headertext;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

            }


            public void SetRightText(String righttext)
            {
                try
                {
                    rightTextValue.Text = "";

                    if (!string.IsNullOrEmpty(righttext))
                    {
                        rightTextValue.Text = righttext;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

            }
        }

    }
}
