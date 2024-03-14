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

namespace myTNB.Android.Src.FeedbackGeneralEnquiryStepTwo.Model
{
    public class FeedbackUpdateDetailsModel
    {
        public string FeedbackUpdInfoTypeDesc { get; set; }
        public string FeedbackUpdInfoValue { get; set; }
        public int FeedbackUpdInfoType { get; set; }

        //public FeedbackUpdateDetailsModel(int FeedbackUpdInfoType, string FeedbackUpdInfoTypeDesc, string FeedbackUpdInfoValue)
        //{
        //    this.FeedbackUpdInfoType = FeedbackUpdInfoType;
        //    this.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
        //    this.FeedbackUpdInfoValue = FeedbackUpdInfoValue;
        //}
    }
}