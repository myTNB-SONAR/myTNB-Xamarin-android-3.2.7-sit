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

namespace myTNB.AndroidApp.Src.UpdatePersonalDetailStepOne.Model
{
  
    public class whoIsRegisteredOwnerResponseModel
    {
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
 
    }

    public class DoINeedOwnerConsentResponseModel
    {
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }

    }
}