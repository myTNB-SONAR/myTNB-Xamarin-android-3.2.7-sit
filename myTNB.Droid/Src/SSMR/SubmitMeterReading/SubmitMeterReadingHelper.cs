using System;
namespace myTNB_Android.Src.SSMR.SubmitMeterReading
{
    public class SubmitMeterReadingHelper
    {
        public SubmitMeterReadingHelper()
        {
        }

        //List<CardView> listOfMeter = new List<CardView>();
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);

        //    // Create your application here
        //    SetContentView(Resource.Layout.SubmitMeterReadingLayout);

        //    LinearLayout container = FindViewById(Resource.Id.container) as LinearLayout;

        //    var metrics = Resources.DisplayMetrics;


        //    var widthInDp = (int)metrics.WidthPixels / metrics.Density;
        //    var minus = widthInDp - (32 / metrics.Density);
        //    int divide = (int)(minus / (9 / metrics.Density));
        //    var width = GetPixelSize(divide);


        //    listOfMeter.Add(CreateSubmitReadingCard("kWh", 35));
        //    listOfMeter.Add(CreateSubmitReadingCard("kVARh", 35));
        //    listOfMeter.Add(CreateSubmitReadingCard("kW", 35));

        //    foreach (CardView meterCard in listOfMeter)
        //    {
        //        container.AddView(meterCard);
        //    }
        //}

        //private int ConvertPixelsToDp(float pixelValue)
        //{
        //    var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
        //    return dp;
        //}

        //private int ConvertDpToPixel(int pixelValue)
        //{
        //    var dp = (int)(pixelValue * Resources.DisplayMetrics.Density);
        //    return dp;
        //}

        //public int GetPixelSize(int intVal)
        //{
        //    int inPixel = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, intVal, Resources.DisplayMetrics);
        //    return inPixel;
        //}

        //private CardView CreateSubmitReadingCard(string meterTagValue, int digitWidth)
        //{
        //    CardView meterCard = new CardView(this);
        //    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
        //    int margin = GetPixelSize(16);
        //    layoutParams.SetMargins(margin, GetPixelSize(8), margin, 0);
        //    meterCard.LayoutParameters = layoutParams;
        //    int padding = GetPixelSize(8);
        //    meterCard.SetContentPadding(padding, padding, padding, padding);
        //    meterCard.CardElevation = GetPixelSize(5);
        //    meterCard.Radius = GetPixelSize(5);

        //    //Meter Container
        //    LinearLayout meterContainer = new LinearLayout(this);
        //    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
        //    int margins = GetPixelSize(8);
        //    layoutParams.SetMargins(margins, 0, margins, 0);
        //    meterContainer.LayoutParameters = layoutParams;
        //    meterContainer.SetGravity(GravityFlags.Center);
        //    meterContainer.Orientation = Orientation.Vertical;


        //    int digitLength = 9;

        //    //Current Meter Value
        //    LinearLayout currentMeterContainer = new LinearLayout(this);
        //    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
        //    currentMeterContainer.LayoutParameters = layoutParams;
        //    currentMeterContainer.SetGravity(GravityFlags.Right);


        //    for (int i = 0; i <= digitLength; i++)
        //    {
        //        if ((digitLength - 1) == i)
        //        {
        //            TextView periodValue = new TextView(this);
        //            layoutParams = new LinearLayout.LayoutParams(GetPixelSize(1), LinearLayout.LayoutParams.MatchParent);

        //            layoutParams.SetMargins(0, GetPixelSize(8), 0, 0);
        //            periodValue.LayoutParameters = layoutParams;

        //            periodValue.TextSize = 16;
        //            periodValue.Text = ".";
        //            periodValue.Gravity = GravityFlags.Center;
        //            currentMeterContainer.AddView(periodValue);
        //        }
        //        else
        //        {
        //            TextView currentMeterValue = new TextView(this);
        //            layoutParams = new LinearLayout.LayoutParams(GetPixelSize(digitWidth), LinearLayout.LayoutParams.MatchParent);

        //            int marginLeft = GetPixelSize(1);
        //            if (digitLength == i)
        //            {
        //                marginLeft = 0;
        //            }
        //            int marginTop = GetPixelSize(8);
        //            layoutParams.SetMargins(marginLeft, marginTop, 0, 0);
        //            currentMeterValue.LayoutParameters = layoutParams;

        //            currentMeterValue.Text = "0";
        //            currentMeterValue.TextSize = 16;
        //            currentMeterValue.Gravity = GravityFlags.Center;
        //            currentMeterValue.SetTextColor(Color.ParseColor("#8b8b8b"));
        //            currentMeterContainer.AddView(currentMeterValue);
        //        }

        //    }
        //    meterContainer.AddView(currentMeterContainer);

        //    //Manual Input Meter Value
        //    LinearLayout manualInputMeterContainer = new LinearLayout(this);
        //    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
        //    manualInputMeterContainer.LayoutParameters = layoutParams;
        //    manualInputMeterContainer.SetGravity(GravityFlags.Right);

        //    for (int i = 0; i <= digitLength; i++)
        //    {
        //        if ((digitLength - 1) == i)
        //        {
        //            TextView periodValue = new TextView(this);
        //            layoutParams = new LinearLayout.LayoutParams(GetPixelSize(1), LinearLayout.LayoutParams.MatchParent);

        //            layoutParams.SetMargins(0, GetPixelSize(8), 0, 0);
        //            periodValue.LayoutParameters = layoutParams;

        //            periodValue.TextSize = 16;
        //            periodValue.Text = ".";
        //            periodValue.Gravity = GravityFlags.Center;
        //            manualInputMeterContainer.AddView(periodValue);
        //        }
        //        else
        //        {
        //            EditText manualInputMeterValue = new EditText(this);
        //            layoutParams = new LinearLayout.LayoutParams(GetPixelSize(digitWidth), LinearLayout.LayoutParams.MatchParent);
        //            int marginLeft = GetPixelSize(1);
        //            if (digitLength == i)
        //            {
        //                marginLeft = 0;
        //            }
        //            int marginTop = GetPixelSize(8);
        //            layoutParams.SetMargins(marginLeft, marginTop, 0, 0);
        //            manualInputMeterValue.LayoutParameters = layoutParams;
        //            manualInputMeterValue.Text = "0";
        //            manualInputMeterValue.TextSize = 16;
        //            manualInputMeterValue.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(1) });
        //            manualInputMeterValue.Gravity = GravityFlags.Center;
        //            manualInputMeterValue.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
        //            manualInputMeterContainer.AddView(manualInputMeterValue);
        //        }
        //    }
        //    meterContainer.AddView(manualInputMeterContainer);


        //    //Meter Validate Message
        //    TextView validateMessage = new TextView(this);
        //    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
        //    layoutParams.SetMargins(0, GetPixelSize(4), 0, 0);
        //    layoutParams.Gravity = GravityFlags.Right;
        //    validateMessage.LayoutParameters = layoutParams;
        //    validateMessage.TextSize = 10;
        //    validateMessage.Text = "This value is too high!";
        //    validateMessage.SetTextColor(Color.ParseColor("#e44b21"));
        //    meterContainer.AddView(validateMessage);

        //    //Meter Tag
        //    TextView meterTag = new TextView(this);
        //    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
        //    layoutParams.SetMargins(0, GetPixelSize(8), 0, 0);
        //    layoutParams.Gravity = GravityFlags.Right;
        //    meterTag.LayoutParameters = layoutParams;

        //    meterTag.SetPadding(GetPixelSize(11), 0, GetPixelSize(11), 0);
        //    meterTag.TextSize = 14;
        //    meterTag.Text = meterTagValue;
        //    meterTag.SetTextColor(Color.ParseColor("#ffffff"));
        //    meterTag.SetBackgroundResource(Resource.Drawable.meter_reading_label_background);
        //    meterContainer.AddView(meterTag);

        //    meterCard.AddView(meterContainer);
        //    return meterCard;
        //}
    }
}
