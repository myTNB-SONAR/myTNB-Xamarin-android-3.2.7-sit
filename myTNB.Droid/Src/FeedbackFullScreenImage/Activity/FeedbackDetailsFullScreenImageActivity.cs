using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Com.Davemorrissey.Labs.Subscaleview;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Utils.ZoomImageView;
using Square.Picasso;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.FeedbackFullScreenImage.Activity
{
    [Activity(Label = "@string/feedback_full_image_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.FullScreenImage")]
    public class FeedbackDetailsFullScreenImageActivity : BaseToolbarAppCompatActivity
    {

        AttachedImage attachedImage;

        [BindView(Resource.Id.imgFeedback)]
        ZoomImageView imgFeedback;


        public override int ResourceId()
        {
            return Resource.Layout.FeedbackDetailsFullscreenImageView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return attachedImage.Name;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE))
                    {
                        //attachedImage = JsonConvert.DeserializeObject<AttachedImage>(Intent.Extras.GetString(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE));
                        attachedImage = DeSerialze<AttachedImage>(extras.GetString(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE));
                    }
                }

                base.OnCreate(savedInstanceState);

                Java.IO.File temp = new Java.IO.File(attachedImage.Path);

                imgFeedback
                    .FromFile(temp.Path)
                    .Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Feedback Details View Image");
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