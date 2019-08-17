using System;
using System.Collections.Generic;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using static myTNB_Android.Src.myTNBMenu.Models.SMRActivityInfoResponse;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAppToolTipData
    {
        static SMRActivityInfo sMRActivityInfo;

        public static void SetSMRActivityInfo(SMRActivityInfo smrActivityInfo)
        {
            sMRActivityInfo = smrActivityInfo;
        }


        public static SMRPhotoPopUpDetailsModel GetTakePhotoToolTipData(bool isSinglePhase, string firstParam, string secondParam)
        {
            List<SMRPhotoPopUpDetailsModel> list = new List<SMRPhotoPopUpDetailsModel>();
            SMRPhotoPopUpDetailsModel item = new SMRPhotoPopUpDetailsModel();
            if (sMRActivityInfo != null)
            {
                if (isSinglePhase)
                {
                    list.AddRange(sMRActivityInfo.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Single_")));
                    item = list.Find(x => x.Type.Contains("TakePhoto"));
                }
                else
                {
                    list.AddRange(sMRActivityInfo.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Multi_")));
                    item = list.Find(x => x.Type.Contains("TakePhoto"));
                    item.Description = String.Format(item.Description, firstParam, secondParam);
                }
                //item = list.Find(x => x.Type.Contains("UploadPhoto"));
            }
            else
            {
                if (isSinglePhase)
                {
                    item.Title = "How do I take this photo?";
                    item.Description = "Stand close to your meter. You don’t need a photo of the whole meter. Just capture <strong>the box containing the value with your camera directly facing it.</strong> Ensure the numbers are clear and there is no glare, shadow or distortion.";
                    item.CTA = "Got It!";
                }
                else
                {
                    item.Title = "How do I take these photos?";
                    item.Description = String.Format("You'll need to submit {0} different meter reading values <strong>({1}).</strong><br/><br/>Stand close to your meter.You don’t need a photo of the whole meter. Just capture <strong>the box containing the values with your camera directly facing it.</strong>Ensure the numbers are clear and there is no glare, shadow or distortion.", firstParam, secondParam);
                    item.CTA = "Got It!";
                }
            }
            return item;
        }
    }
}
