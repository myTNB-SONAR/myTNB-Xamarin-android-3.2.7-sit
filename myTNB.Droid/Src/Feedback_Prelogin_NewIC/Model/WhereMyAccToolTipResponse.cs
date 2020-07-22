using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.Model
{
   public class WhereMyAccToolTipResponse
    {
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public Bitmap ImageBitmap { set; get; }
    }
}