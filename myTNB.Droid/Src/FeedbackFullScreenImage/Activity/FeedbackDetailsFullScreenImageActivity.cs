using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Com.Davemorrissey.Labs.Subscaleview;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Runtime;

namespace myTNB_Android.Src.FeedbackFullScreenImage.Activity
{
    [Activity(Label = "@string/feedback_full_image_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.FullScreenImage")]
    public class FeedbackDetailsFullScreenImageActivity : BaseToolbarAppCompatActivity
    {

        AttachedImage attachedImage;

        [BindView(Resource.Id.imgFeedback)]
        SubsamplingScaleImageView imgFeedback;


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
                imgFeedback.Visibility = ViewStates.Visible;
                // Create your application here
                //Picasso.With(this)
                //    .Load(new Java.IO.File(attachedImage.Path))
                //    .Fit()
                //    .Into(imgFeedback , delegate 
                //    {
                //        if (imgProgress != null)
                //        {
                //            imgProgress.Visibility = ViewStates.Gone;
                //        }
                //        if (imgFeedback != null)
                //        {
                //            imgFeedback.Visibility = ViewStates.Visible;
                //        }

                //    } , delegate { } );

                Java.IO.File temp = new Java.IO.File(attachedImage.Path);

                var source = ImageSource.InvokeUri(temp.Path);

                imgFeedback.SetImage(source);
                imgFeedback.ZoomEnabled = true;
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