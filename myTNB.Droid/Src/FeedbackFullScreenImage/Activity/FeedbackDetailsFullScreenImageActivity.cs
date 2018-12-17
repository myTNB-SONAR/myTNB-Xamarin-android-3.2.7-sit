using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;
using CheeseBind;
using Square.Picasso;
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
        ImageView imgFeedback;


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
                attachedImage = JsonConvert.DeserializeObject<AttachedImage>(Intent.Extras.GetString(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE));

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

                Picasso.With(this)
                    .Load(new Java.IO.File(attachedImage.Path))
                    .Fit()
                    .Into(imgFeedback);
            } catch(Exception e) {
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