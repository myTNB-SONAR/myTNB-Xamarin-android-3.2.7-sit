using System;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Widget;
using Android.Views;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class ManualMeterEditView : LinearLayout
    {
        Context mContext;
        EditText meterEditText1, meterEditText2, meterEditText3, meterEditText4, meterEditText5, meterEditText6, meterEditText7, meterEditText8;
        public ManualMeterEditView(Context context) : base(context)
        {
            mContext = context;

            //< LinearLayout
            //    android: layout_width = "match_parent"
            //    android: layout_height = "60dp"
            //    android: orientation = "horizontal"
            //    android: weightSum = "8"
            //    android: gravity = "right" >
            LayoutParams layoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            this.Orientation = Orientation.Horizontal;
            this.WeightSum = 8;
            this.SetGravity(GravityFlags.Right);
            this.LayoutParameters = layoutParams;

            meterEditText1 = CreateMeterEditText();
            this.AddView(meterEditText1);
            meterEditText2 = CreateMeterEditText();
            this.AddView(meterEditText2);
            meterEditText3 = CreateMeterEditText();
            this.AddView(meterEditText3);
            meterEditText4 = CreateMeterEditText();
            this.AddView(meterEditText4);
            meterEditText5 = CreateMeterEditText();
            this.AddView(meterEditText5);
            meterEditText6 = CreateMeterEditText();
            this.AddView(meterEditText6);
            meterEditText7 = CreateMeterEditText();
            this.AddView(meterEditText7);
            meterEditText8 = CreateMeterEditText();
            this.AddView(meterEditText8);
        }

        private EditText CreateMeterEditText()
        {
             //< EditText
             //       android: id = "@+id/new_reading_2"
             //       android: layout_width = "0dp"
             //       android: layout_height = "match_parent"
             //       android: textSize = "16dp"
             //       android: layout_weight = "1"
             //       android: text = ""
             //       android: maxLength = "2"
             //       android: layout_marginLeft = "1dp"
             //       android: layout_marginTop = "8dp"
             //       android: background = "#f3f3f3"
             //       android: gravity = "center"
             //       android: inputType = "number"
             //       android: digits = "0123456789" />
            EditText editText = new EditText(mContext);
            LinearLayout.LayoutParams editTextParams = new LinearLayout.LayoutParams(0,LinearLayout.LayoutParams.MatchParent);
            editTextParams.Weight = 1;
            editText.Gravity = Android.Views.GravityFlags.Center;
            editText.InputType = Android.Text.InputTypes.ClassNumber;
            editText.SetFilters(new IInputFilter[] { new Android.Text.InputFilterLengthFilter(1)});
            editText.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            return editText;
        }
    }
}
