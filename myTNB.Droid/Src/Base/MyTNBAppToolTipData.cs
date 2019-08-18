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


        public static SMRPhotoPopUpDetailsModel GetTakePhotoToolTipData(bool isSinglePhase, bool isOneMissing, string firstParam, string secondParam)
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
                    if (isOneMissing)
                    {
                        item = list.Find(x => x.Type.Contains("TakePhoto_One_Missing"));
                    }
                    else
                    {
                        item = list.Find(x => x.Type.Contains("TakePhoto"));
                    }
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
                    if (isOneMissing)
                    {
                        item.Description = String.Format("You'll need to submit 1 meter reading value <strong>({0})</strong>.<br><br>Stand close to your meter. You don’t need a photo of the whole meter. Just capture <strong>the box containing the values with your camera directly facing it</strong>. Ensure the numbers are clear and there is no glare, shadow or distortion.", secondParam);
                    }
                    else
                    {
                        item.Description = String.Format("You'll need to submit {0} different meter reading values <strong>({1}).</strong><br/><br/>Stand close to your meter.You don’t need a photo of the whole meter. Just capture <strong>the box containing the values with your camera directly facing it.</strong>Ensure the numbers are clear and there is no glare, shadow or distortion.", firstParam, secondParam);
                    }
                    item.CTA = "Got It!";
                }
            }
            return item;
        }

        public static SMRPhotoPopUpDetailsModel GetUploadPhotoToolTipData(bool isSinglePhase, bool isOneMissing, string firstParam, string secondParam)
        {
            List<SMRPhotoPopUpDetailsModel> list = new List<SMRPhotoPopUpDetailsModel>();
            SMRPhotoPopUpDetailsModel item = new SMRPhotoPopUpDetailsModel();
            if (sMRActivityInfo != null)
            {
                if (isSinglePhase)
                {
                    list.AddRange(sMRActivityInfo.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Single_")));
                    item = list.Find(x => x.Type.Contains("UploadPhoto"));
                }
                else
                {
                    list.AddRange(sMRActivityInfo.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Multi_")));
                    if (isOneMissing)
                    {
                        item = list.Find(x => x.Type.Contains("UploadPhoto_One_Missing"));
                    }
                    else
                    {
                        item = list.Find(x => x.Type.Contains("UploadPhoto"));
                    }
                    item.Description = String.Format(item.Description, firstParam, secondParam);
                }
            }
            else
            {
                if (isSinglePhase)
                {
                    item.Title = "Uploading from your album?";
                    item.Description = "Be sure to upload <strong>a straight-facing photo containing the value</strong>. You don’t need a photo of the whole meter. Ensure the numbers are clear and there is no glare, shadow or distortion.";
                    item.CTA = "Got It!";
                }
                else
                {
                    item.Title = "Uploading from your album?";
                    if (isOneMissing)
                    {
                        item.Description = String.Format("You'll need to submit 1 meter reading value <strong>({0})</strong>.<br><br>Be sure to upload <strong>straight-facing photos containing the values</strong>. You don’t need a photo of the whole meter. Ensure the numbers are clear and there is no glare, shadow or distortion.", secondParam);
                    }
                    else
                    {
                        item.Description = String.Format("You'll need to submit {0} different meter reading values <strong>({1})</strong>.<br><br>Be sure to upload <strong>straight-facing photos containing the values</strong>. You don’t need a photo of the whole meter. Ensure the numbers are clear and there is no glare, shadow or distortion.", firstParam, secondParam);
                    }
                    item.CTA = "Got It!";
                }
            }
            return item;
        }
    }
}
