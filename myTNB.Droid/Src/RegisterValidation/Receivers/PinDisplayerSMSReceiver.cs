
using Android.App;
using Android.Content;
using Android.Widget;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.RegisterValidation.Receivers
{
    //[BroadcastReceiver(Enabled = true)]
    //[IntentFilter(new[] { "com.myTNB.smsReceiver" })]
    public class PinDisplayerSMSReceiver : BroadcastReceiver
    {
        private EditText[] pin;

        public PinDisplayerSMSReceiver() : base()
        {

        }

        public PinDisplayerSMSReceiver(params EditText[] pin)
        {
            this.pin = pin;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (pin == null)
            {
                return;
            }
            string StringPin = intent.Extras.GetString(Constants.RETRIEVE_PIN_FROM_SMS).Trim();
            for (int i = 0; i < pin.Length; i++)
            {
                this.pin[i].Text = StringPin[i].ToString();
            }
        }
    }
}