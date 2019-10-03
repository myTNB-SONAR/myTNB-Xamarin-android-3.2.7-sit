﻿using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.myTNBMenu.Models.SMRActivityInfoResponse;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.GetAccountsSMREligibilityResponse;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAppToolTipData
    {
        
        static SMRActivityInfo sMRActivityInfo;
        private List<SMREligibiltyPopUpDetails> mSMREligibilityPopupDetailList = new List<SMREligibiltyPopUpDetails>();
        private List<BillMandatoryChargesTooltipModel> mBillMandatoryChargesTooltipModelList = new List<BillMandatoryChargesTooltipModel>();
        private static MyTNBAppToolTipData Instance;

        private MyTNBAppToolTipData(){}

        public static MyTNBAppToolTipData GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MyTNBAppToolTipData();
            }
            return Instance;
        }

        public static void SetSMRActivityInfo(SMRActivityInfo smrActivityInfo)
        {
            sMRActivityInfo = smrActivityInfo;
        }

        public void SetSMREligibiltyPopUpDetailList(List<SMREligibiltyPopUpDetails> smrEligibilityPopupDetailList)
        {
            mSMREligibilityPopupDetailList = smrEligibilityPopupDetailList;
        }

        public void SetBillMandatoryChargesTooltipModelList(List<BillMandatoryChargesTooltipModel> billMandatoryChargesTooltipModelList)
        {
            mBillMandatoryChargesTooltipModelList = billMandatoryChargesTooltipModelList;
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
                        item.Description = String.Format(item.Description, secondParam);
                    }
                    else
                    {
                        item = list.Find(x => x.Type.Contains("TakePhoto"));
                        item.Description = String.Format(item.Description, firstParam, secondParam);
                    }
                }
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
                        item.Description = String.Format(item.Description, secondParam);
                    }
                    else
                    {
                        item = list.Find(x => x.Type.Contains("UploadPhoto"));
                        item.Description = String.Format(item.Description, firstParam, secondParam);
                    }
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

        public SMREligibiltyPopUpDetailData GetSMREligibiltyPopUpDetails()
        {
            SMREligibiltyPopUpDetailData eligibiltyPopUpDetails = new SMREligibiltyPopUpDetailData();
            if (mSMREligibilityPopupDetailList.Count > 0)
            {
                SMREligibiltyPopUpDetails details = mSMREligibilityPopupDetailList.Find(x => {
                    return x.Type == "Not_SMR_CA";
                });
                eligibiltyPopUpDetails.title = details.Title;
                eligibiltyPopUpDetails.description = details.Description;
                eligibiltyPopUpDetails.cta = details.CTA;
            }
            else
            {
                eligibiltyPopUpDetails.title = "Why are some of my accounts not here?";
                eligibiltyPopUpDetails.description = "We took the liberty to only show you electricity accounts that are eligble for the Self Meter Reading service. <a style=\"text-decoration:none\" href =\"faqid={B8EBBADE-0918-43B7-8093-BB2B19614033}\">Click here</a> to learn more about its eligibility.";
                eligibiltyPopUpDetails.cta = "Got It!";
            }
            return eligibiltyPopUpDetails;
        }

        public static List<UnderstandTooltipModel> GetUnderstandBillTooltipData()
        {
            List<UnderstandTooltipModel> tooltipModelDataList = new List<UnderstandTooltipModel>();
            List<SitecoreCmsEntity> entryList = SitecoreCmsEntity.GetItemById("BILL_TOOLTIP");
            if (entryList.Count > 0)
            {
                List<BillsTooltipModelEntity> billTooltipDataList = JsonConvert.DeserializeObject<List<BillsTooltipModelEntity>>(entryList[0].jsonStringData);
                UnderstandTooltipModel tooltipModel;
                billTooltipDataList.ForEach(data =>
                {
                    tooltipModel = new UnderstandTooltipModel();
                    tooltipModel.Title = data.Title;
                    tooltipModel.ItemList = data.Description.Split('|').ToList();
                    tooltipModel.TooltipImage = ImageUtils.GetImageBitmapFromUrl(data.Image);
                    tooltipModelDataList.Add(tooltipModel);
                });
            }
            else
            {
                UnderstandTooltipModel newModel = new UnderstandTooltipModel();
                newModel.TooltipImage = null;
                newModel.Title = "Title 1";
                List<string> itemList = "Security Deposit|Stamp Duty|Processing Fee|Meter Fee".Split('|').ToList();
                newModel.ItemList = itemList;
                tooltipModelDataList.Add(newModel);

                newModel = new UnderstandTooltipModel();
                newModel.TooltipImage = null;
                newModel.Title = "Title 2";
                itemList = "Security Deposit|Stamp Duty|Processing Fee|Meter Fee".Split('|').ToList();
                newModel.ItemList = itemList;
                tooltipModelDataList.Add(newModel);
            }
            return tooltipModelDataList;
        }

        public BillMandatoryChargesTooltipModel GetMandatoryChargesTooltipData()
        {
            return mBillMandatoryChargesTooltipModelList.Find(model =>
            {
                return model.Type == "MandatoryCharges";
            });
        }

        public BillMandatoryChargesTooltipModel GetMandatoryPaymentTooltipData()
        {
            return mBillMandatoryChargesTooltipModelList.Find(model =>
            {
                return model.Type == "MandatoryPayment";
            });
        }


        public class SMREligibiltyPopUpDetailData
        {
            public string title { set; get; }
            public string description { set; get; }
            public string cta { set; get; }
        }
    }
}
