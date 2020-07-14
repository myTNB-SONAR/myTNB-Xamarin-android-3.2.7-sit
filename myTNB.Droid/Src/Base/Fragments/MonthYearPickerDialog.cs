
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
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

        public MonthYearPickerDialog(BaseAppCompatActivity ctx, int minYear, int maxYear, int minMonth, int maxMonth, DateTime selectedDateTime)
        {
            this.mContext = ctx;
            this.min_year = minYear;
            this.max_year = maxYear;
            this.min_month = minMonth;
            this.max_month = maxMonth;
            mSelectedDateTime = selectedDateTime;
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

            monthPicker.MinValue = this.min_month;
            monthPicker.MaxValue = this.max_month;
            monthPicker.Value = nowDateTime.Month;

            yearPicker.MinValue = this.min_year;
            yearPicker.MaxValue = this.max_year;
            yearPicker.Value = nowDateTime.Year;

            builder.SetView(dialog)
                .SetPositiveButton(Utility.GetLocalizedLabel("Common", "ok"), (senderAlert, args) =>
                {
                    mListener.OnDateSet(null, yearPicker.Value, monthPicker.Value, 0);
                })
                .SetNegativeButton(Utility.GetLocalizedLabel("Common", "cancel"), (senderAlert, args) =>
                {
                    OnCancel();
                });

            return builder.Create();
        }
    }
}
