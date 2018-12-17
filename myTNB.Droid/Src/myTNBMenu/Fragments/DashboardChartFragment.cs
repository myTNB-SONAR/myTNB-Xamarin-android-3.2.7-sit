using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using MikePhil.Charting.Formatter;
using myTNB_Android.Src.myTNBMenu.Charts.Formatter;
using MikePhil.Charting.Components;
using static MikePhil.Charting.Components.XAxis;
using Android.Graphics;
using MikePhil.Charting.Charts;
using static MikePhil.Charting.Components.YAxis;
using myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView;
using MikePhil.Charting.Data;
using MikePhil.Charting.Interfaces.Datasets;
using myTNB_Android.Src.Utils.Custom.Charts;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.ViewBill.Activity;
using Java.Text;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MakePayment.Activity;
using MikePhil.Charting.Highlight;
using Android.Support.Design.Widget;
using myTNB_Android.Src.Database.Model;
using Android.Support.V4.Content;
using myTNB_Android.Src.Notifications.Activity;
using Java.Util;
using Refit;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.SmartMeterLearnMore.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using Android.Support.V7.App;
using Android.Text;
using myTNB_Android.Src.FAQ.Activity;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartFragment : BaseFragment , DashboardChartContract.IView
    {

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnToggleDay)]
        RadioButton btnToggleDay;

        [BindView(Resource.Id.btnToggleMonth)]
        RadioButton btnToggleMonth;

        UsageHistoryData selectedHistoryData;

        AccountData selectedAccount;

        ChartType ChartType = ChartType.Month;

        bool hasNoInternet;

        [BindView(Resource.Id.bar_chart)]
        BarChart mChart;

        [BindView(Resource.Id.no_data_layout)]
        LinearLayout mNoDataLayout;

        [BindView(Resource.Id.no_internet_layout)]
        LinearLayout mNoInternetLayout;

        [BindView(Resource.Id.txtUsageHIstory)]
        TextView txtUsageHistory;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.txtRange)]
        TextView txtRange;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        [BindView(Resource.Id.btnViewBill)]
        Button btnViewBill;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;


        [BindView(Resource.Id.dashboard_chartview_no_data_title)]
        TextView txtTitleNoData;

        [BindView(Resource.Id.dashboard_chartview_no_data_content)]
        TextView txtContentNoData;

        [BindView(Resource.Id.btnLearnMore)]
        Button btnLearnMore;

        [BindView(Resource.Id.dashboard_chartview_no_internet_title)]
        TextView txtTitleNoInternet;

        [BindView(Resource.Id.dashboard_chartview_no_internet_content)]
        TextView txtContentNoInternet;

        [BindView(Resource.Id.btnTapRefresh)]
        Button btnTapRefresh;

        [BindView(Resource.Id.btnLeft)]
        ImageButton btnLeft;

        [BindView(Resource.Id.btnRight)]
        ImageButton btnRight;

        [BindView(Resource.Id.downtime_layout)]
        LinearLayout mDownTimeLayout;



        private DashboardChartContract.IUserActionsListener userActionsListener;
        private DashboardChartPresenter mPresenter;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");
        IAxisValueFormatter XLabelsFormatter;
        private int currentParentIndex = 0;
        private string errorMSG = null;

        public override int ResourceId()
        {
            return Resource.Layout.DashboardChartView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;


            if (extras.ContainsKey(Constants.NO_INTERNET_CONNECTION))
            {
                hasNoInternet = extras.GetBoolean(Constants.NO_INTERNET_CONNECTION);
            }
            if (!hasNoInternet)
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }

                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE))
                {
                    selectedHistoryData = JsonConvert.DeserializeObject<UsageHistoryData>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE));

                }

                if (extras.ContainsKey(Constants.SELECTED_ERROR_MSG))
                {
                    errorMSG = extras.GetString(Constants.SELECTED_ERROR_MSG);
                }
            }


            SetHasOptionsMenu(true);
            this.mPresenter = new DashboardChartPresenter(this);
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData , AccountData accountData)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE ,data);
            bundle.PutString(Constants.SELECTED_ACCOUNT , JsonConvert.SerializeObject(accountData));
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData, AccountData accountData, string error, string errorMessage)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            bundle.PutString(Constants.SELECTED_ERROR, error);
            bundle.PutString(Constants.SELECTED_ERROR_MSG, errorMessage);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool hasNoInternet)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();

            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION , hasNoInternet);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            if (!hasNoInternet)
            {
                txtTotalPayable.Text = decimalFormat.Format(selectedAccount.AmtCustBal);
                //if(selectedAccount.AmtCustBal <= 0)
                //{
                //    btnPay.Enabled = false;
                //    btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
                //}
            }


            TextViewUtils.SetMuseoSans300Typeface(txtUsageHistory , txtAddress , txtTotalPayable , txtContentNoData , txtContentNoInternet , txtDueDate);
            TextViewUtils.SetMuseoSans300Typeface(btnToggleDay , btnToggleMonth );
            TextViewUtils.SetMuseoSans500Typeface(txtRange , txtTotalPayableTitle , txtTotalPayableCurrency, btnViewBill, btnPay, btnLearnMore, btnTapRefresh, txtTitleNoData, txtTitleNoInternet);

            this.userActionsListener.Start();

            DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
            DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
            DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);

            if (selectedAccount != null)
            {

                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    btnPay.Visibility = ViewStates.Gone;
                    btnViewBill.Text = GetString(Resource.String.dashboard_chart_view_payment_advice);
                    txtUsageHistory.Visibility = ViewStates.Gone;
                    txtTotalPayableTitle.Text = GetString(Resource.String.title_payment_advice_amount);
                }
                else
                {
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewBill.Text = GetString(Resource.String.dashboard_chartview_view_bill);
                }

               
                if (bcrmEntity != null && bcrmEntity.IsDown)
                {
                    DisablePayButton();
                    btnViewBill.Enabled = false;
                    btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                    btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                    txtRange.Visibility = ViewStates.Gone;
                    if (bcrmEntity.IsDown)
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                        {
                            txtAddress.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            txtAddress.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage);
                        }

                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                            bcrmEntity.DowntimeTextMessage,
                            Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        downtimeSnackBar.Show();
                    }
                }
                else
                {
                    if (pgCCEntity.IsDown && pgFPXEntity.IsDown)
                    {
                        DisablePayButton();
                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                                bcrmEntity.DowntimeTextMessage,
                                Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        if (!selectedAccount.AccountCategoryId.Equals("2"))
                        {
                            downtimeSnackBar.Show();
                        }
                    }

                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }

                txtAddress.Click += delegate {
                    if (bcrmEntity.IsDown)
                    {
                        string textMessage = bcrmEntity.DowntimeMessage;
                        if (textMessage != null && textMessage.Contains("http"))
                        {
                            //Launch webview
                            int startIndex = textMessage.LastIndexOf("=") + 2;
                            int lastIndex = textMessage.LastIndexOf("\"");
                            int lengthOfId = (lastIndex - startIndex);
                            if (lengthOfId < textMessage.Length)
                            {
                                string url = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(url));
                                    StartActivity(intent);
                                }
                            }
                        }
                        else if (textMessage != null && textMessage.Contains("faq"))
                        {
                            //Lauch FAQ
                            int startIndex = textMessage.LastIndexOf("=") + 1;
                            int lastIndex = textMessage.LastIndexOf("}");
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < textMessage.Length)
                            {
                                string faqid = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(faqid))
                                {
                                    Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    Activity.StartActivity(faqIntent);
                                }
                            }
                        }
                    }

                };
            }

            if (!string.IsNullOrEmpty(errorMSG))
            {
                ShowUnableToFecthSmartMeterData(errorMSG);
            }
        }

        [OnClick(Resource.Id.btnLeft)]
        internal void OnLeft(object sender , EventArgs eventArgs)
        {
            this.userActionsListener.OnArrowBackClick();
        }

        [OnClick(Resource.Id.btnRight)]
        internal void OnRight(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnArrowForwardClick();
        }


        internal void SetUp()
        {
            if (hasNoInternet)
            {
                return;
            }

            mChart.SetRoundedBarRadius(100f);
            mChart.SetDrawBarShadow(false);
            mChart.SetDrawValueAboveBar(true);
            mChart.Description.Enabled = false;
            mChart.SetMaxVisibleValueCount(7);
            mChart.SetPinchZoom(false);
            mChart.SetDrawGridBackground(false);
            mChart.SetScaleEnabled(false);
            mChart.Legend.Enabled = false;
            mChart.AnimateY(1000);

            if (!hasNoInternet)
            {
                txtAddress.Text = selectedAccount.AddStreet;
            }
            

            if (ChartType == ChartType.Month)
            {
                
                if (!hasNoInternet)
                {
                    txtRange.Text = selectedHistoryData.ByMonth.Range;
                }
                // SETUP XAXIS

                SetUpXAxis();

                // SETUP YAXIS

                SetUpYAxis();

                // SETUP MARKER VIEW

                SetUpMarkerMonthView();

                // ADD DATA

                SetData(selectedHistoryData.ByMonth.Months.Count);


            }
            else
            {

                if (!hasNoInternet)
                {
                    txtRange.Text = selectedHistoryData.ByDay[currentParentIndex].Range;
                }
                // SETUP XAXIS

                SetUpXAxisDay();

                // SETUP YAXIS

                SetUpYAxisDay();

                // SETUP MARKER VIEW

                SetUpMarkerDayView();

                // ADD DATA
                SetDayData(currentParentIndex, 7);
            }

            int graphTopPadding = 20;
            if (selectedAccount.AccountCategoryId.Equals("2"))
            {
                graphTopPadding = 35;
            }
            mChart.SetExtraOffsets(0 , graphTopPadding , 0 , 0);
        }
        #region SETUP AXIS MONTH
        internal void SetUpXAxis()
        {

            XLabelsFormatter = new ChartsMonthFormatter(selectedHistoryData.ByMonth , mChart);

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#77a3ea");

            //xAxis.Typeface = (mTfLight);
            xAxis.SetDrawGridLines(false);
            
            xAxis.Granularity = 1f; // only intervals of 1 day
            xAxis.LabelCount = selectedHistoryData.ByMonth.Months.Count;
            xAxis.ValueFormatter = XLabelsFormatter;


        }
        #endregion 

        #region SETUP AXIS DAY
        internal void SetUpXAxisDay()
        {
            XLabelsFormatter = new ChartsDayFormatter()
            {
                DayData = selectedHistoryData.ByDay,
                Chart = mChart,
                ParentIndex = currentParentIndex
            };

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#77a3ea");

            //xAxis.Typeface = (mTfLight);
            xAxis.SetDrawGridLines(false);

            xAxis.Granularity = 1f; // only intervals of 1 day
            xAxis.LabelCount = 7;
            xAxis.ValueFormatter = XLabelsFormatter;


        }
        #endregion

        #region SETUP Y AXIS BOTH MONTH 
        internal void SetUpYAxis()
        {
            IAxisValueFormatter custom = new MyAxisValueFormatter();

            float maxVal = GetMaxMonthValues();
            float lowestPossibleSpace = (5f / 100f) * -maxVal;
            Console.WriteLine("Space {0}", lowestPossibleSpace);

            YAxis leftAxis = mChart.AxisLeft; ;
            leftAxis.Enabled = false;
            leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
            leftAxis.SetDrawGridLines(false);
            leftAxis.SpaceTop = 10f;
            leftAxis.SpaceBottom = 10f;
            leftAxis.AxisMinimum = lowestPossibleSpace;
            leftAxis.AxisMaximum = maxVal;

            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;
            rightAxis.SetDrawGridLines(false);
            rightAxis.SpaceTop = 10f;
            rightAxis.SpaceBottom = 10f;
            rightAxis.AxisMinimum = lowestPossibleSpace;
            rightAxis.AxisMaximum = maxVal;

        }
        #endregion

        #region SETUP Y AXIS BOTH DAY
        internal void SetUpYAxisDay()
        {
            IAxisValueFormatter custom = new MyAxisValueFormatter();
            float maxVal = GetMaxDaysValues();
            float lowestPossibleSpace = (5f / 100f) * -maxVal;
            Console.WriteLine("Space {0}" , lowestPossibleSpace);

            YAxis leftAxis = mChart.AxisLeft; ;
            leftAxis.Enabled = false;
            leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
            leftAxis.SetDrawGridLines(false);
            leftAxis.SpaceTop = 10f;
            leftAxis.SpaceBottom = 10f;
            leftAxis.AxisMinimum = lowestPossibleSpace;
            leftAxis.AxisMaximum = maxVal;
            //leftAxis.AxisMinimum = 0f;
            //leftAxis.AxisMaximum = 100f;
            //leftAxis.SetLabelCount(5 , true);
            //leftAxis.SetStartAtZero(true);


            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;
            rightAxis.SetDrawGridLines(false);
            rightAxis.SpaceTop = 10f;
            rightAxis.SpaceBottom = 10f;
            rightAxis.AxisMinimum = lowestPossibleSpace;
            rightAxis.AxisMaximum = maxVal;
            //rightAxis.AxisMinimum = 0f;
            //rightAxis.AxisMaximum = 100f;
            //rightAxis.SetLabelCount(5, true);
            //rightAxis.ValueFormatter = custom;
            //rightAxis.SetStartAtZero(true);

        }
        #endregion  

        #region SETUP MARKERVIEW MONTH / HIGHLIGHT TEXT
        internal void SetUpMarkerMonthView()
        {

            SelectedMarkerView markerView = new SelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.Month,
                AccountType = selectedAccount.AccountCategoryId

            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP MARKERVIEW DAY/ HIGHLIGHT TEXT
        internal void SetUpMarkerDayView()
        {

            SelectedMarkerView markerView = new SelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.Day,
                CurrentParentIndex = currentParentIndex

            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP MONTH DATA
        internal void SetData(int barLength)
        {
            List<BarEntry> yVals1 = new List<BarEntry>();
            for (int i = 0; i < barLength ; i++)
            {
                float val = (float)selectedHistoryData.ByMonth.Months[i].Amount;
                if (float.IsPositiveInfinity(val))
                {
                    val = float.PositiveInfinity;
                }

                yVals1.Add(new BarEntry(i, Math.Abs(val)));
            }

            BarDataSet set1;

            if (mChart.Data != null && mChart.Data is BarData)
            {
                var barData = mChart.Data as BarData;

                if (barData.DataSetCount > 0)
                {
                    set1 = barData.GetDataSetByIndex(0) as BarDataSet;
                    set1.Values = yVals1;
                    barData.NotifyDataChanged();
                    mChart.NotifyDataSetChanged();
                }
            }
            else
            {
                set1 = new BarDataSet(yVals1, "");
                set1.SetDrawIcons(false);
                

                set1.HighLightColor = Color.Argb(255, 255, 255, 255);

                //set1.SetColor(Resource.Color.sunGlow , 255);
                int[] color = { Color.Argb(50, 255, 255, 255)};
                set1.SetColors(color);
                List<IBarDataSet> dataSets = new List<IBarDataSet>();
                dataSets.Add(set1);


                BarData data = new BarData(dataSets);

                data.BarWidth = 0.45f;
                
                data.HighlightEnabled = true;
                data.SetValueTextSize(10f);
                data.SetDrawValues(false);
                //data.setValueTypeface(mTfLight);
                
                mChart.Data = data;
            }

            // HIGHLIGHT RIGHT MOST ITEM
            Highlight rightMostBar = new Highlight(barLength - 1, 0 , 0);
            mChart.HighlightValues(new Highlight[] { rightMostBar });


        }
        #endregion

        #region SETUP DAY DATA
        internal void SetDayData(int parentIndex , int barLength)
        {
            List<BarEntry> yVals1 = new List<BarEntry>();
            for (int i = 0; i < barLength; i++)
            {
                float val = (float)selectedHistoryData.ByDay[parentIndex].Days[i].Amount;
                if (float.IsPositiveInfinity(val))
                {
                    val = float.PositiveInfinity;
                }

                yVals1.Add(new BarEntry(i, Math.Abs(val)));
            }

            BarDataSet set1;

            if (mChart.Data != null && mChart.Data is BarData)
            {
                var barData = mChart.Data as BarData;

                if (barData.DataSetCount > 0)
                {
                    set1 = barData.GetDataSetByIndex(0) as BarDataSet;
                    set1.Values = yVals1;
                    barData.NotifyDataChanged();
                    mChart.NotifyDataSetChanged();
                }
            }
            else
            {
                set1 = new BarDataSet(yVals1, "");
                set1.SetDrawIcons(false);


                set1.HighLightColor = Color.Argb(255, 255, 255, 255);

                //set1.SetColor(Resource.Color.sunGlow , 255);
                int[] color = { Color.Argb(50, 255, 255, 255) };
                set1.SetColors(color);
                List<IBarDataSet> dataSets = new List<IBarDataSet>();
                dataSets.Add(set1);


                BarData data = new BarData(dataSets);

                data.BarWidth = 0.4f;

                data.HighlightEnabled = true;
                data.SetValueTextSize(10f);
                data.SetDrawValues(false);
                //data.setValueTypeface(mTfLight);

                mChart.Data = data;
            }
            // HIGHLIGHT RIGHT MOST ITEM
            Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
            mChart.HighlightValues(new Highlight[] { rightMostBar });

        }
        #endregion

        public void ShowByDay()
        {
            ChartType = ChartType.Day;
            mChart.Clear();
            SetUp();
        }

        public void ShowByMonth()
        {
            ChartType = ChartType.Month;

            // TODO : TO FIX STATEFUL CHANGES DURING MONTH/DAY OPTION
            mNoDataLayout.Visibility = ViewStates.Gone;
            mChart.Visibility = ViewStates.Visible;

            mChart.Clear();
            SetUp();
        }

        public void ShowViewBill(BillHistoryV5 selectedBill)
        {
            btnViewBill.Clickable = false;
            btnViewBill.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                btnViewBill.Clickable = true;
                btnViewBill.Enabled = true;
            };
            h.PostDelayed(myAction, 3000);

            if (selectedBill != null && selectedBill.NrBill != null) {
                selectedBill.NrBill = null;
            }

            Intent viewBill = new Intent(this.Activity , typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT , JsonConvert.SerializeObject(selectedAccount));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            StartActivity(viewBill);
        }

        public void ShowPayment()
        {
            //Intent payment_activity = new Intent(this.Activity, typeof(MakePaymentActivity));
            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            //StartActivity(payment_activity);
            StartActivityForResult(payment_activity, DashboardActivity.PAYMENT_RESULT_CODE);
        }

        [OnClick(Resource.Id.btnToggleDay)]
        internal void OnToggleDay(object sender, EventArgs e)
        {
            this.userActionsListener.OnByDay();
        }

        [OnClick(Resource.Id.btnToggleMonth)]
        internal void OnToggleMonth(object sender , EventArgs e)
        {
            this.userActionsListener.OnByMonth();
        }

        [OnClick(Resource.Id.btnViewBill)]
        internal void OnViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
        }

        [OnClick(Resource.Id.btnTapRefresh)]
        internal void OnTapRefresh(object sender , EventArgs e)
        {
            this.userActionsListener.OnTapRefresh();
        }

        [OnClick(Resource.Id.btnPay)]
        internal void OnUserPay(object sender , EventArgs e)
        {
            this.userActionsListener.OnPay();
        }

        [OnClick(Resource.Id.btnLearnMore)]
        internal void OnLearnMore(object sender, EventArgs e)
        {
            this.userActionsListener.OnLearnMore();
        }

        public void SetPresenter(DashboardChartContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public bool IsByDay()
        {
            return ChartType == ChartType.Day;
        }

        public int GetCurrentParentIndex()
        {
            return currentParentIndex;
        }

        public void SetCurrentParentIndex(int newIndex)
        {
            this.currentParentIndex = newIndex;
            mChart.Clear();
            SetUp();


        }

        public int GetMaxParentIndex()
        {
            return selectedHistoryData.ByDay.Count;
        }

        public void EnableLeftArrow(bool show)
        {
            if (show)
            {
                btnLeft.Enabled = true;
                btnLeft.Visibility = ViewStates.Visible;
            }else
            {
                btnLeft.Enabled = false;
                btnLeft.Visibility = ViewStates.Gone;
            }
        }

        public void EnableRightArrow(bool show)
        {
            if (show)
            {
                btnRight.Enabled = true;
                btnRight.Visibility = ViewStates.Visible;
            }
            else
            {
                btnRight.Enabled = false;
                btnRight.Visibility = ViewStates.Gone;
            }
        }


        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == DashboardActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    ((DashboardActivity)Activity).OnTapRefresh();
                }
            }
        }

        public void ShowNotAvailableDayData()
        {
            //txtRange.Text = GetString(Resource.String.dashboard_chartview_no_day_data_available);
            mNoDataLayout.Visibility = ViewStates.Visible;
            mChart.Visibility = ViewStates.Gone;
            mNoInternetLayout.Visibility = ViewStates.Gone;
        }

        public bool IsByDayEmpty()
        {
            return selectedHistoryData.ByDay.Count == 0;
        }

        public void ShowNoInternet()
        {
            DownTimeEntity bcrmEnrity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
            //txtRange.Text = GetString(Resource.String.dashboard_chartview_no_day_data_available);
            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
            if(bcrmEnrity != null && bcrmEnrity.IsDown)
            {
                mNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Gone;
                mNoInternetLayout.Visibility = ViewStates.Gone;
                mDownTimeLayout.Visibility = ViewStates.Visible;
                txtAddress.Text = bcrmEnrity.DowntimeMessage;
                txtAddress.Visibility = ViewStates.Visible;
            }
            else
            {
                mNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Gone;
                mNoInternetLayout.Visibility = ViewStates.Visible;
                mDownTimeLayout.Visibility = ViewStates.Gone;
            }
            
        }

        public bool HasNoInternet()
        {
            return hasNoInternet;
        }

        public void ShowTapRefresh()
        {
            if (Activity is DashboardActivity)
            {
                var dashboardActivity = Activity as DashboardActivity;
                dashboardActivity.OnTapRefresh();
            }

        }

        internal float GetMaxDaysValues()
        {
            float val = 0;
            try {

            foreach (UsageHistoryData.ByDayData ByDay in selectedHistoryData.ByDay)
            {
                foreach (UsageHistoryData.ByDayData.DayData dayData in ByDay.Days)
                {
                    if (Math.Abs(dayData.Amount) > val)
                    {
                        val = Math.Abs((float)dayData.Amount);
                    }
                }
            }
            if (val == 0){
                val = 1;
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
        }

        internal float GetMaxMonthValues()
        {
            float val = 0;
            try {

            foreach (UsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth.Months)
            {
                if (Math.Abs(MonthData.Amount) > val)
                {
                    val = Math.Abs((float)MonthData.Amount);
                }
            }
            if(val == 0){
                val = 1;
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
        }
        private IMenu menu;
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.DashboardToolbarMenu , menu);
            this.menu = menu;
            if (UserNotificationEntity.HasNotifications())
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity  , Resource.Drawable.ic_header_notification_unread));
            }
            else
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification));
            }
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.Activity.InvalidateOptionsMenu();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            //actionBar.SetDisplayHomeAsUpEnabled(true);
            //actionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    this.userActionsListener.OnNotification();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            try {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chartview_data_not_available_no_internet), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate {

                mNoInternetSnackbar.Dismiss();
            }
            );
            mNoInternetSnackbar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNotification()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
        }

        public void ShowAmountProgress()
        {
            try {
            progressBar.Visibility = ViewStates.Visible;
            totalPayableLayout.Visibility = ViewStates.Gone;

        } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideAmountProgress()
        {
            try {
            progressBar.Visibility = ViewStates.Gone;
            totalPayableLayout.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void ShowAmountDue(AccountDueAmount accountDueAmount)
        {
            try {   
            Date d = null;
            try
            {
                d = dateParser.Parse(accountDueAmount.BillDueDate);
            }catch (ParseException e)
            {
                    Utility.LoggingNonFatalError(e);
            }

            if (d != null)
            {
                if(selectedAccount != null)
                {
                    
                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                        double calAmt = selectedAccount.AmtCustBal * -1;
                        if (calAmt <= 0)
                        {
                            calAmt = 0.00;
                        }
                        else
                        {
                            calAmt = Math.Abs(selectedAccount.AmtCustBal);
                        }
                        txtTotalPayable.Text = decimalFormat.Format(calAmt);

                        int incrementDays = int.Parse(accountDueAmount.IncrementREDueDateByDays == null ? "0" : accountDueAmount.IncrementREDueDateByDays);
                        Constants.RE_ACCOUNT_DATE_INCREMENT_DAYS = incrementDays;
                        Calendar c = Calendar.Instance;
                        c.Time = d;
                        c.Add(CalendarField.Date, incrementDays);
                        Date newDate = c.Time;
                        if (calAmt == 0.00)
                        {
                            txtDueDate.Text = "--";
                        }
                        else
                        {
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_by_date_wildcard, dateFormatter.Format(newDate));
                        }
                    }
                    else
                    {
                        txtTotalPayable.Text = decimalFormat.Format(accountDueAmount.AmountDue);
                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                        double calAmt = selectedAccount.AmtCustBal;
                        if (calAmt <= 0.00)
                        {
                            txtDueDate.Text = "--";
                        }
                        else
                        {
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
                        }
                    }
                }
            }
            else
            {
                txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
            }
        } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chart_cancelled_exception_btn_retry), delegate {

                mCancelledExceptionSnackBar.Dismiss();
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
            }
            );
            mCancelledExceptionSnackBar.Show();
        } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_api_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_api_exception_btn_retry), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            try {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chart_unknown_exception_btn_retry), delegate {

                mUknownExceptionSnackBar.Dismiss();
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);

            }
            );
            mUknownExceptionSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mSmartMeterError;
        public void ShowUnableToFecthSmartMeterData(string errorMsg)
        {
            try {
            if (mSmartMeterError != null && mSmartMeterError.IsShown)
            {
                mSmartMeterError.Dismiss();

            }

            mSmartMeterError = Snackbar.Make(rootView, errorMsg, 10000)
            .SetAction(GetString(Resource.String.logout_rate_unknown_exception_btn_close), delegate {
                    mSmartMeterError.Dismiss();
                }
            );
            mSmartMeterError.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnablePayButton()
        {
            btnPay.Enabled = true;
            btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
        }

        public void DisablePayButton()
        {
            btnPay.Enabled = false;
            btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);

        }

        public void ShowLearnMore(Weblink weblink)
        {
            try {
            if (weblink.OpenWith.Equals("APP"))
            {
                Intent smartMeterINtent = new Intent(this.Activity , typeof(SmartMeterLearnMoreActivity));
                smartMeterINtent.PutExtra(Constants.SMART_METER_LINK, JsonConvert.SerializeObject(weblink));
                StartActivity(smartMeterINtent);
            }
            else
            {
                var uri = Android.Net.Uri.Parse(weblink.Url);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}