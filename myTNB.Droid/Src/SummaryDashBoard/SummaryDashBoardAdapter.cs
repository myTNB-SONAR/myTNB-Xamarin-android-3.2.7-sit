using Android.Graphics.Drawables;


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
using System.Linq;

namespace myTNB_Android.Src.SummaryDashBoard
{
    class SummaryDashBoardAdapter : BaseRecyclerAdapter<SummaryDashBoardDetails>
    {

        List<SummaryDashBoardDetails> reAccount = null;
        List<SummaryDashBoardDetails> normalAccount = null;

        //bool HAS_RE_HEADER = false;
        //bool HAS_NORMAL_HEADER = false;

        private const int BODY = 2;
        private const int HEADER = 1;
        private const int FOOTER = 3;

        //int reAccountCount = 0;
        //int normalAccountCount = 0;
        //int count = 0;

        private ISummaryListener iSummaryListener = null;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");

        public SummaryDashBoardAdapter(List<SummaryDashBoardDetails> itemList, ISummaryListener iSummaryListener) : base(itemList)
        {
            this.iSummaryListener = iSummaryListener;
            reAccount = new List<SummaryDashBoardDetails>();
            normalAccount = new List<SummaryDashBoardDetails>();

            //reAccount = (from item in itemList
            //             where item.AccType == "2"
            //             select item).ToList();


            //normalAccount = (from item in itemList
            //where item.AccType != "2"
            //select item).ToList();


            reAccount = itemList;



            //if (reAccount != null && reAccount.Count() > 0)
            //{
            //    HAS_RE_HEADER = true;
            //    reAccountCount = reAccount.Count();
            //    count = count + reAccountCount + 1;
            //}

            //if (normalAccount != null && normalAccount.Count() > 0)
            //{
            //    HAS_NORMAL_HEADER = true;
            //    normalAccountCount = normalAccount.Count();
            //    count = count + normalAccount.Count() + 1;
            //}

            //Adding footer count...
            //count = count + 1;
        }


        public override int ItemCount
        {
            get
            {
                return reAccount.Count() + 1;
            }
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

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            if (position == 0)
            {
                var viewHolder1 = holder as SummaryDashBoardHeaderViewHolder;

                viewHolder1.SetHeaderText("Renewable Energy");
                viewHolder1.SetRightText("My Earnings");

            }
            else
            {
                var viewHolder3 = holder as SummaryDashBoardViewHolder;
                viewHolder3.PopulateData(reAccount[position - 1]);
                //        viewHolder3.SetAcountName(reAccount[position - 1].AccName);
                //        viewHolder3.SetAcountNumber(reAccount[position - 1].AccNumber);
                //viewHolder3.SetDueDate(reAccount[position - 1].BillDueDate, reAccount[position - 1].AmountDue);
                //viewHolder3.SetAmountText(reAccount[position - 1].AmountDue);
            }

            //if (position == 0 || (position) == (reAccountCount != 0 ? (reAccountCount + 1) : reAccountCount )) {
            //    var viewHolder1 = holder as SummaryDashBoardHeaderViewHolder;
            //    if (HAS_RE_HEADER && position == 0) {
            //        viewHolder1.SetHeaderText("Renewable Energy");
            //        viewHolder1.SetRightText("Receivable");
            //    } else if ((position) == (reAccountCount != 0 ? (reAccountCount + 1) : reAccountCount) && HAS_NORMAL_HEADER)
            //    {
            //        viewHolder1.SetHeaderText("Accounts"); 
            //        viewHolder1.SetRightText("Due");
            //    }
            //} 
            //else if (position > (count -1))

            //{
            //    var viewHolder2 = holder as SummaryDashBoardFooterViewHolder;
            //    //viewHolder2.SetAcountName();
            //}
            //else {
            //    var viewHolder3 = holder as SummaryDashBoardViewHolder;
            //    if (position > (reAccountCount != 0 ? (reAccountCount + 1) : reAccountCount))
            //    {
            //        int normalPosition = 0;
            //        if (HAS_RE_HEADER) {
            //            normalPosition = position - (count - 2);
            //        } else {
            //            normalPosition = position - (count - 1);
            //        }

            //        viewHolder3.SetAcountName(normalAccount[normalPosition - 1].AccName);
            //        viewHolder3.SetAcountNumber(normalAccount[normalPosition - 1].AccNumber);
            //        viewHolder3.SetDueDate(normalAccount[normalPosition - 1].BillDueDate);
            //        viewHolder3.SetAmountText(normalAccount[normalPosition - 1].AmountDue);
            //    } else {
            //        viewHolder3.SetAcountName(reAccount[position - 1].AccName);
            //        viewHolder3.SetAcountNumber(reAccount[position - 1].AccNumber);
            //        viewHolder3.SetDueDate(reAccount[position - 1].BillDueDate);
            //        viewHolder3.SetAmountText(reAccount[position - 1].AmountDue);
            //    }

            //}

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

            DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");


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

                Drawable leafIcon = ContextCompat.GetDrawable(itemView.Context, Resource.Drawable.ic_display_RE_Dashboard);
                leafIcon.Alpha = 255;

                accountNameText.SetCompoundDrawablesWithIntrinsicBounds(null, null, leafIcon, null);
                accountNameText.CompoundDrawablePadding = 10;


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
                    SetAmountText(summaryDashBoardDetails.AmountDue, summaryDashBoardDetails.AccType);
                    SetAcountNumber(summaryDashBoardDetails.AccNumber);
                    SetDueDate(summaryDashBoardDetails.BillDueDate, summaryDashBoardDetails.AmountDue, summaryDashBoardDetails);
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

            public void SetAmountText(String amount, String accType)
            {
                try
                {
                    if (!string.IsNullOrEmpty(amount))
                    {
                        if(amount != "--")
                        {
                            if (accType.Equals("2"))
                            {
                                double amt = Convert.ToDouble(amount);
                                amt = amt * -1;
                                if (amt <= 0)
                                {
                                    amt = 0.00;
                                }
                                else
                                {
                                    amt = Math.Abs(amt);
                                }
                                amountText.Text = " " + decimalFormat.Format(amt);
                            }
                            else
                            {
                                double amt = Convert.ToDouble(amount);
                                amountText.Text = " " + decimalFormat.Format(amt);
                            }
                        }
                        else
                        {
                            rmCurrencyText.Text = "";
                            amountText.Text = "--";
                        }
                    }
                    else
                    {
                        amountText.Text = " 0.00";
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public void SetDueDate(String dueDateValue, String amount, SummaryDashBoardDetails summaryDashBoardDetails)
            {
                try
                {
                    if(amount != "--")
                    {
                        double amt = Convert.ToDouble(amount);
                        if (summaryDashBoardDetails.AccType.Equals("2"))
                        {
                            amt = amt * -1;
                            if (amt <= 0)
                            {
                                amt = 0.00;
                            }
                            else
                            {
                                amt = Math.Abs(amt);
                            }
                        }


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

                            if (summaryDashBoardDetails.AccType.Equals("2"))
                            {
                                int incrementDays = int.Parse(summaryDashBoardDetails.IncrementREDueDateByDays == null ? "0" : summaryDashBoardDetails.IncrementREDueDateByDays);
                                Calendar c = Calendar.Instance;
                                c.Time = d;
                                c.Add(CalendarField.Date, incrementDays);
                                Date newDate = c.Time;
                                if (amt == 0.00)
                                {
                                    dueDate.Text = "--";
                                }
                                else
                                {
                                    string dtnewDate = dateFormatter.Format(newDate);
                                    dueDate.Text = dtnewDate;
                                }
                            }
                            else
                            {
                                dueDate.Text = dt;
                            }

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

        //class SummaryDashBoardFooterViewHolder : BaseRecyclerViewHolder
        //{
        //    [BindView(Resource.Id.summaryFooter)]
        //    TextView summaryFooterValue;

        //    public SummaryDashBoardFooterViewHolder(View itemView) : base(itemView)
        //    {
        //    }
        //}
    }
}