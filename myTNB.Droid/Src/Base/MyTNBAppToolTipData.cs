using System;
using System.Collections.Generic;
using System.Linq;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.myTNBMenu.Models.SMRActivityInfoResponse;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.GetAccountsSMREligibilityResponse;
using myTNB_Android.Src.myTNBMenu.Models;
using Android.Graphics;
using Android.Util;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAppToolTipData
    {

        static SMRActivityInfo sMRActivityInfo;
        private List<SMREligibiltyPopUpDetails> mSMREligibilityPopupDetailList = new List<SMREligibiltyPopUpDetails>();
        private List<BillMandatoryChargesTooltipModel> mBillMandatoryChargesTooltipModelList = new List<BillMandatoryChargesTooltipModel>();
        private List<EppToolTipModel> mEppToolTipModelList = new List<EppToolTipModel>();
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
                List<PopupSelectorModel> popupSelectorModels = Utility.GetTooltipSelectorModel("SSMRCaptureMeter", "SMRPhotoPopUpDetails");
                if (isSinglePhase)
                {
                    PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Single_TakePhoto"; });
                    item.Title = popupSelectorModel.Title;
                    item.Description = popupSelectorModel.Description;
                    item.CTA = popupSelectorModel.CTA;
                }
                else
                {
                    if (isOneMissing)
                    {
                        PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Multi_TakePhoto_One_Missing"; });
                        item.Title = popupSelectorModel.Title;
                        item.Description = popupSelectorModel.Description;
                        item.CTA = popupSelectorModel.CTA;
                    }
                    else
                    {
                        PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Multi_TakePhoto"; });
                        item.Title = popupSelectorModel.Title;
                        item.Description = String.Format(popupSelectorModel.Description, firstParam, secondParam);
                        item.CTA = popupSelectorModel.CTA;
                    }
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
                List<PopupSelectorModel> popupSelectorModels = Utility.GetTooltipSelectorModel("SSMRCaptureMeter", "SMRPhotoPopUpDetails");
                if (isSinglePhase)
                {
                    PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Single_UploadPhoto"; });
                    item.Title = popupSelectorModel.Title;
                    item.Description = popupSelectorModel.Description;
                    item.CTA = popupSelectorModel.CTA;
                }
                else
                {
                    if (isOneMissing)
                    {
                        PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Multi_UploadPhoto_One_Missing"; });
                        item.Title = popupSelectorModel.Title;
                        item.Description = popupSelectorModel.Description;
                        item.CTA = popupSelectorModel.CTA;
                    }
                    else
                    {
                        PopupSelectorModel popupSelectorModel = popupSelectorModels.Find(model => { return model.Type == "Multi_UploadPhoto"; });
                        item.Title = popupSelectorModel.Title;
                        item.Description = String.Format(popupSelectorModel.Description, firstParam, secondParam);
                        item.CTA = popupSelectorModel.CTA;
                    }
                }
            }
            return item;
        }

        public SMREligibiltyPopUpDetailData GetSMREligibiltyPopUpDetails()
        {
            SMREligibiltyPopUpDetailData eligibiltyPopUpDetails = null;
            if (mSMREligibilityPopupDetailList.Count > 0)
            {
                SMREligibiltyPopUpDetails details = mSMREligibilityPopupDetailList.Find(x => {
                    return x.Type == "Not_SMR_CA";
                });
                if (details != null)
                {
                    eligibiltyPopUpDetails = new SMREligibiltyPopUpDetailData();
                    eligibiltyPopUpDetails.title = details.Title;
                    eligibiltyPopUpDetails.description = details.Description;
                    eligibiltyPopUpDetails.cta = details.CTA;
                }
            }
            else
            {
                Utility.GetTooltipSelectorModel("SSMRReadingHistory", "SMREligibiltyPopUpDetails").ForEach(tooltipModel =>
                {
                    if (tooltipModel.Type == "Not_SMR_CA")
                    {
                        eligibiltyPopUpDetails = new SMREligibiltyPopUpDetailData();
                        eligibiltyPopUpDetails.title = tooltipModel.Title;
                        eligibiltyPopUpDetails.description = tooltipModel.Description;
                        eligibiltyPopUpDetails.cta = tooltipModel.CTA;
                    }
                });
            }
            return eligibiltyPopUpDetails;
        }

        public static List<UnderstandTooltipModel> GetUnderstandBillTooltipData(BaseActivityCustom baseActivity)
        {
            List<UnderstandTooltipModel> tooltipModelDataList = new List<UnderstandTooltipModel>();
            UnderstandTooltipModel tooltipModel;
            try
            {
                string jsonData = SitecoreCmsEntity.GetItemById(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP);
                if (jsonData != null)
                {
                    List<BillsTooltipModelEntity> billTooltipDataList = JsonConvert.DeserializeObject<List<BillsTooltipModelEntity>>(jsonData);
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
                    tooltipModel = new UnderstandTooltipModel();
                    tooltipModel.TooltipImage = null;
                    tooltipModel.Title = baseActivity.GetLabelByLanguage("tooltiptitle1");
                    List<string> itemList = baseActivity.GetLabelByLanguage("tooltipdesc1").Split('|').ToList();
                    tooltipModel.ItemList = itemList;
                    tooltipModelDataList.Add(tooltipModel);

                    tooltipModel = new UnderstandTooltipModel();
                    tooltipModel.TooltipImage = null;
                    tooltipModel.Title = baseActivity.GetLabelByLanguage("tooltiptitle2");
                    itemList = baseActivity.GetLabelByLanguage("tooltipdesc2").Split('|').ToList();
                    tooltipModel.ItemList = itemList;
                    tooltipModelDataList.Add(tooltipModel);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return tooltipModelDataList;
        }

        public static  List<EPPTooltipResponse> GetEppToolTipData()
        {
            List<EPPTooltipResponse> tooltipModelDataList = new List<EPPTooltipResponse>();
            EPPTooltipResponse tooltipModel;
            string jsonData = SitecoreCmsEntity.GetItemById(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP);
            //syahmi modified
            if (jsonData != null && jsonData!="null")
            {
                List<EPPToolTipEntity> EPPTooltipDataList = JsonConvert.DeserializeObject<List<EPPToolTipEntity>>(jsonData);
                
                EPPTooltipDataList.ForEach(data =>
                {
                    tooltipModel = new EPPTooltipResponse();
                    tooltipModel.Title = data.Title;
                    tooltipModel.PopUpTitle = data.PopUpTitle;
                    tooltipModel.PopUpBody = data.PopUpBody;
                    tooltipModel.ImageBitmap = Base64ToBitmap(data.ImageBase64);
                    tooltipModelDataList.Add(tooltipModel);
                });
            }
    

    
            return tooltipModelDataList;
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        public BillMandatoryChargesTooltipModel GetMandatoryChargesTooltipData()
        {
            BillMandatoryChargesTooltipModel tooltipModel = mBillMandatoryChargesTooltipModelList.Find(model =>
            {
                return model.Type == "MandatoryCharges";
            });

            if (tooltipModel == null)
            {
                tooltipModel = new BillMandatoryChargesTooltipModel();
                tooltipModel.CTA = "Got It!";
                tooltipModel.Title = "We strongly advise clearing your one-time charges first.";
                tooltipModel.Description = "Your <b>one-time charges</b> like Security Deposit, Processing Fee, Stamp Duty and Meter Cost should be cleared first to ensure the start/continuation of your electricity.";
            }
            return tooltipModel;
        }

        public BillMandatoryChargesTooltipModel GetMandatoryPaymentTooltipData()
        {
            BillMandatoryChargesTooltipModel tooltipModel = mBillMandatoryChargesTooltipModelList.Find(model =>
            {
                return model.Type == "MandatoryPayment";
            });

            if (tooltipModel == null)
            {
                tooltipModel = new BillMandatoryChargesTooltipModel();
                tooltipModel.CTA = "View Details,Got It!";
                tooltipModel.Title = "We strongly advise clearing your one-time charges first.";
                tooltipModel.Description = "Clearing the <b>{0}</b> from your one-time charges for <b>{1}</b> will ensure the start/continuation of your electricity.<br><br>You may view a breakdown in your bill details.";
            }
            return tooltipModel;
        }

        public class SMREligibiltyPopUpDetailData
        {
            public string title { set; get; }
            public string description { set; get; }
            public string cta { set; get; }
        }
    }
}
