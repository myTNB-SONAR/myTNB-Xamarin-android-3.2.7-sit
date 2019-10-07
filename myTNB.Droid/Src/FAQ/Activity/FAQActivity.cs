using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;

namespace myTNB_Android.Src.FAQ.Activity
{

    [Activity(Label = "@string/faq_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.FAQ")]
    public class FAQActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.txtFaq1Title)]
        TextView txtFaq1Title;

        [BindView(Resource.Id.txtFaq1Content)]
        TextView txtFaq1Content;

        [BindView(Resource.Id.txtFaq2Title)]
        TextView txtFaq2Title;

        [BindView(Resource.Id.txtFaq2Content)]
        TextView txtFaq2Content;

        [BindView(Resource.Id.txtFaq3Title)]
        TextView txtFaq3Title;

        [BindView(Resource.Id.txtFaq3Content)]
        TextView txtFaq3Content;

        [BindView(Resource.Id.txtFaq4Title)]
        TextView txtFaq4Title;

        [BindView(Resource.Id.txtFaq4Content)]
        TextView txtFaq4Content;

        [BindView(Resource.Id.txtFaq5Title)]
        TextView txtFaq5Title;

        [BindView(Resource.Id.txtFaq5Content)]
        TextView txtFaq5Content;

        [BindView(Resource.Id.txtFaq6Title)]
        TextView txtFaq6Title;

        [BindView(Resource.Id.txtFaq6Content)]
        TextView txtFaq6Content;

        [BindView(Resource.Id.txtFaq7Title)]
        TextView txtFaq7Title;

        [BindView(Resource.Id.txtFaq7Content)]
        TextView txtFaq7Content;


        [BindView(Resource.Id.txtFaq8Title)]
        TextView txtFaq8Title;

        [BindView(Resource.Id.txtFaq8Content)]
        TextView txtFaq8Content;


        [BindView(Resource.Id.txtFaq9Title)]
        TextView txtFaq9Title;

        [BindView(Resource.Id.txtFaq9Content)]
        TextView txtFaq9Content;

        [BindView(Resource.Id.txtFaq10Title)]
        TextView txtFaq10Title;

        [BindView(Resource.Id.txtFaq10Content)]
        TextView txtFaq10Content;

        [BindView(Resource.Id.txtFaq11Title)]
        TextView txtFaq11Title;

        [BindView(Resource.Id.txtFaq11Content)]
        TextView txtFaq11Content;


        [BindView(Resource.Id.txtFaq12Title)]
        TextView txtFaq12Title;

        [BindView(Resource.Id.txtFaq12Content)]
        TextView txtFaq12Content;


        [BindView(Resource.Id.txtFaq13Title)]
        TextView txtFaq13Title;

        [BindView(Resource.Id.txtFaq13Content)]
        TextView txtFaq13Content;


        [BindView(Resource.Id.txtFaq14Title)]
        TextView txtFaq14Title;

        [BindView(Resource.Id.txtFaq14Content)]
        TextView txtFaq14Content;


        [BindView(Resource.Id.txtFaq15Title)]
        TextView txtFaq15Title;

        [BindView(Resource.Id.txtFaq15Content)]
        TextView txtFaq15Content;


        [BindView(Resource.Id.txtFaq16Title)]
        TextView txtFaq16Title;

        [BindView(Resource.Id.txtFaq16Content)]
        TextView txtFaq16Content;


        [BindView(Resource.Id.txtFaq17Title)]
        TextView txtFaq17Title;

        [BindView(Resource.Id.txtFaq17Content)]
        TextView txtFaq17Content;


        [BindView(Resource.Id.txtFaq18Title)]
        TextView txtFaq18Title;

        [BindView(Resource.Id.txtFaq18Content)]
        TextView txtFaq18Content;


        [BindView(Resource.Id.txtFaq19Title)]
        TextView txtFaq19Title;

        [BindView(Resource.Id.txtFaq19Content)]
        TextView txtFaq19Content;


        [BindView(Resource.Id.txtFaq20Title)]
        TextView txtFaq20Title;

        [BindView(Resource.Id.txtFaq20Content)]
        TextView txtFaq20Content;

        public override int ResourceId()
        {
            return Resource.Layout.FAQView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            TextViewUtils.SetMuseoSans300Typeface(txtFaq1Content,
                txtFaq2Content,
                txtFaq3Content,
                txtFaq4Content,
                txtFaq5Content,
                txtFaq6Content,
                txtFaq7Content,
                txtFaq8Content,
                txtFaq9Content,
                txtFaq10Content,
                txtFaq11Content,
                txtFaq12Content,
                txtFaq13Content,
                txtFaq14Content,
                txtFaq15Content,
                txtFaq16Content,
                txtFaq17Content,
                txtFaq18Content,
                txtFaq19Content,
                txtFaq20Content);

            TextViewUtils.SetMuseoSans500Typeface(txtFaq1Title,
                txtFaq2Title,
                txtFaq3Title,
                txtFaq4Title,
                txtFaq5Title,
                txtFaq6Title,
                txtFaq7Title,
                txtFaq8Title,
                txtFaq9Title,
                txtFaq10Title,
                txtFaq11Title,
                txtFaq12Title,
                txtFaq13Title,
                txtFaq14Title,
                txtFaq15Title,
                txtFaq16Title,
                txtFaq17Title,
                txtFaq18Title,
                txtFaq19Title,
                txtFaq20Title);
            // 15 , 18

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
            {
                txtFaq13Content.TextFormatted = Html.FromHtml(GetString(Resource.String.faq_13_content), FromHtmlOptions.ModeLegacy);
                txtFaq16Content.TextFormatted = Html.FromHtml(GetString(Resource.String.faq_16_content), FromHtmlOptions.ModeLegacy);

            }
            else
            {
                txtFaq13Content.TextFormatted = Html.FromHtml(GetString(Resource.String.faq_13_content));
                txtFaq16Content.TextFormatted = Html.FromHtml(GetString(Resource.String.faq_16_content));

            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "FAQs");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}