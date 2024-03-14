using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using myTNB.Android.Src.Utils;
using System.Text;

namespace myTNB.Android.Src.RegisterValidation.Receivers
{
    //[BroadcastReceiver(Enabled = true, Label = "SMSReceiver")]
    //[IntentFilter(new[] { "android.fileprovider.Telephony.SMS_RECEIVED" })]
    public class SMSReceiver : BroadcastReceiver
    {
        private const string TAG = "SMSBroadcastReceiver";
        private const string IntentAction = "android.fileprovider.Telephony.SMS_RECEIVED";
        private const string TNBTOKEN_SUBJECT = "Your myTnB token code is:";
        private const string TNBTOKEN_SUBJECT_V2 = "Your myTNB OTP is:";
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "Intent received: " + intent?.Action);

            if (intent?.Action != IntentAction) return;

            var bundle = intent?.Extras;

            if (bundle == null) return;

            var pdus = bundle?.Get("pdus");
            var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);

            var msgs = new SmsMessage[castedPdus.Length];

            var sb = new StringBuilder();

            for (var i = 0; i < msgs.Length; i++)
            {
                var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
                JNIEnv.CopyArray(castedPdus[i].Handle, bytes);

                msgs[i] = SmsMessage.CreateFromPdu(bytes);



                if (msgs[i] != null && !string.IsNullOrEmpty(msgs[i].MessageBody))
                {

                    sb.Append(string.Format("SMS From: {0}{1}Body: {2}{1}", msgs[i].OriginatingAddress,
                                        System.Environment.NewLine, msgs[i].MessageBody));

                    if (msgs[i].MessageBody.ToLower().Contains(TNBTOKEN_SUBJECT.ToLower()) || msgs[i].MessageBody.ToLower().Contains(TNBTOKEN_SUBJECT_V2.ToLower()))
                    {
                        string[] splitMessage = msgs[i].MessageBody.Split(':');
                        if (splitMessage != null && splitMessage.Length > 1)
                        {
                            Intent pIntent = new Intent("com.myTNB.smsReceiver");
                            pIntent.PutExtra(Constants.RETRIEVE_PIN_FROM_SMS, splitMessage[1]);
                            context.SendBroadcast(pIntent);
                            break;
                        }

                    }
                }

                //if (msgs[i].OriginatingAddress.Contains("6600"))
                //{
                //    // TODO: SENT FROM SMS GATEWAY
                //}
                //if (msgs[i].MessageBody.StartsWith("Your myTnB token code is:"))
                //{
                //    string[] splitMessage = msgs[i].MessageBody.Split(':');
                //    if (splitMessage.Length > 1)
                //    {
                //        Intent pinIntent = new Intent("com.myTNB.smsReceiver");
                //        pinIntent.PutExtra(Constants.RETRIEVE_PIN_FROM_SMS , splitMessage[1]);
                //        context.SendBroadcast(pinIntent);
                //        break;
                //    }

                //}


            }

            //Toast.MakeText(context, sb.ToString(), ToastLength.Long).Show();
        }
    }
}