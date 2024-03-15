using System;
using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.AndroidApp.Src.UpdatePersonalDetailStepTwo.Model
{
  
    public class HowDoesCopyofIdentificationResponseModel
    {
        public string Title { set; get; }
        public string PopUpTitle { set; get; }

        public string PopUpBody { set; get; }
        public Bitmap ImageBitmap { set; get; }
  
    }

    public class HowDoesProofOfConsentResponseBitmapModel
    {
        public string Title { set; get; }
        public string PopUpTitle { set; get; }

        public string PopUpBody { set; get; }
        public Bitmap ImageBitmap { set; get; }

    }
}