
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Base.Fragments
{
    public class MonthYearPickerDialog : DialogFragment
    {
        private int min_year = 2000;
        private int max_year = 2099;
        private int min_month = 1;
        private int max_month = 12;
        private DateTime mSelectedDateTime;

        private BaseAppCompatActivity mContext;
        private DatePickerDialog.IOnDateSetListener mListener;

        public event EventHandler<int> mCancelHandler;

        private List<string> displayMonthList = new List<string>();
        private List<BaseKeyValueModel> checkMonthList = new List<BaseKeyValueModel>();

        public MonthYearPickerDialog(BaseAppCompatActivity ctx, int minYear, int maxYear, int minMonth, int maxMonth, DateTime selectedDateTime)
        {
            this.mContext = ctx;
            this.min_year = minYear;
            this.max_year = maxYear;
            if (minMonth > 0 && minMonth <= 12)
            {
                this.min_month = minMonth;
            }
            else
            {
                this.min_month = 1;
            }

            if (maxMonth > 0 && maxMonth <= 12)
            {
                this.max_month = maxMonth;
            }
            else
            {
                this.max_month = 12;
            }

            if (this.max_month < this.min_month)
            {
                this.max_month = this.min_month;
            }

            mSelectedDateTime = selectedDateTime;

            List<BaseKeyValueModel> monthList = Utility.GetLocalizedMonthSelectorLabel("months");

            displayMonthList.Clear();
            checkMonthList.Clear();

            for (int i = this.min_month - 1; i < this.max_month; i++)
            {
                displayMonthList.Add(monthList[i].Value);
                checkMonthList.Add(monthList[i]);
            }

        }

        public void SetListener(DatePickerDialog.IOnDateSetListener listener)
        {
            this.mListener = listener;
        }

        public void OnCancel()
        {
            if (mCancelHandler != null)
                mCancelHandler(this, -1);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.mContext);
            LayoutInflater inflater = this.mContext.LayoutInflater;

            DateTime nowDateTime = DateTime.Now;
            View dialog = inflater.Inflate(Resource.Layout.BaseMonthYearPickerDialog, null);
            NumberPicker monthPicker = dialog.FindViewById<NumberPicker>(Resource.Id.picker_month);
            NumberPicker yearPicker = dialog.FindViewById<NumberPicker>(Resource.Id.picker_year);

            if (mSelectedDateTime != null)
            {
                nowDateTime = mSelectedDateTime;
            }

            monthPicker.MinValue = 0;
            monthPicker.MaxValue = displayMonthList.Count - 1;
            monthPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;

            monthPicker.SetDisplayedValues(displayMonthList.ToArray());
            int selectedIndex = 0;
            for (int i = 0; i < checkMonthList.Count - 1; i++)
            {
                if (nowDateTime.Month == int.Parse(checkMonthList[i].Key))
                {
                    selectedIndex = i;
                    break;
                }
            }
            monthPicker.Value = selectedIndex;

            yearPicker.MinValue = this.min_year;
            yearPicker.MaxValue = this.max_year;
            yearPicker.Value = nowDateTime.Year;

            builder.SetView(dialog)
                .SetPositiveButton(Utility.GetLocalizedLabel("Common", "ok"), (senderAlert, args) =>
                {
                    mListener.OnDateSet(null, yearPicker.Value, int.Parse(checkMonthList[monthPicker.Value].Key), 0);
                })
                .SetNegativeButton(Utility.GetLocalizedLabel("Common", "cancel"), (senderAlert, args) =>
                {
                    OnCancel();
                });

            return builder.Create();
        }
    }
}
