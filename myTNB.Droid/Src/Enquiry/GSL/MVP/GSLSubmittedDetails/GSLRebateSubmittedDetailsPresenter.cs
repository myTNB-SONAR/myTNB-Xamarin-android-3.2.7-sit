using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using static myTNB.AndroidApp.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB.AndroidApp.Src.Enquiry.GSL.MVP.GSLSubmittedDetails
{
    public class GSLRebateSubmittedDetailsPresenter : GSLRebateSubmittedDetailsContract.IUserActionsListener
    {
        private readonly GSLRebateSubmittedDetailsContract.IView view;

        SubmittedFeedbackDetails gslRebateDetails;
        GSLRebateModel gslRebateModel;
        private Context mContext;

        public GSLRebateSubmittedDetailsPresenter(GSLRebateSubmittedDetailsContract.IView view, Context mContext)
        {
            this.view = view;
            this.mContext = mContext;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize(SubmittedFeedbackDetails details)
        {
            this.gslRebateModel = new GSLRebateModel();
            this.gslRebateDetails = details;
            this.view?.SetUpViews();
        }

        public void Start() { }

        public async void PrepareDataAsync()
        {
            try
            {
                this.view.ShowProgressDialog();

                List<AttachedImage> attachImageList = new List<AttachedImage>();

                if (this.gslRebateDetails == null)
                {
                    return;
                }

                foreach (ImageResponse image in gslRebateDetails.ImageList)
                {
                    if (image.FileName.ToLower().Contains("pdf"))
                    {
                        try
                        {
                            byte[] byteOfPdf = FileUtils.StringToByteArray(image.ImageHex);
                            string filePath = await FileUtils.SaveAsyncPDF(this.mContext, byteOfPdf, FileUtils.IMAGE_FOLDER, image.FileName);
                            var attachImage = new AttachedImage()
                            {
                                Path = filePath,
                                Name = image.FileName
                            };
                            attachImageList.Add(attachImage);
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        Bitmap bitmap = await FileUtils.GetImageFromHexAsync(image.ImageHex, image.FileSize);
                        string filePath = await FileUtils.SaveAsync(this.mContext, bitmap, FileUtils.IMAGE_FOLDER, image.FileName);
                        var attachImage = new AttachedImage()
                        {
                            Path = filePath,
                            Name = image.FileName
                        };
                        attachImageList.Add(attachImage);
                    }
                }

                this.view.RenderAttachments(attachImageList);

                this.gslRebateDetails.IsOwner = gslRebateDetails.IsOwner;
                this.gslRebateModel.ServiceReqNo = gslRebateDetails.ServiceReqNo;
                this.gslRebateDetails.StatusCode = gslRebateDetails.StatusCode;
                this.gslRebateModel.StatusDesc = gslRebateDetails.StatusDesc;
                this.gslRebateModel.RebateTypeKey = gslRebateDetails.RebateId;

                this.gslRebateModel.ContactInfo = new GSLRebateAccountInfoModel
                {
                    FullName = gslRebateDetails.ContactName,
                    Email = gslRebateDetails.ContactEmailAddress,
                    MobileNumber = gslRebateDetails.ContactMobileNo
                };

                CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(gslRebateDetails.AccountNum);
                if (account != null)
                {
                    this.gslRebateModel.ContactInfo.Address = account.AccountStAddress;
                }

                this.gslRebateModel.TenantInfo = new GSLRebateTenantModel
                {
                    FullName = gslRebateDetails.TenantFullName,
                    Email = gslRebateDetails.TenantEmail,
                    MobileNumber = gslRebateDetails.TenantMobileNumber
                };

                this.gslRebateModel.IncidentDisplayList = new List<GSLRebateIncidentDisplayModel>();
                gslRebateDetails.IncidentInfos.ForEach(incident =>
                {
                    this.gslRebateModel.IncidentDisplayList.Add(new GSLRebateIncidentDisplayModel
                    {
                        IncidentDate = incident.IncidentDate,
                        IncidentTime = incident.IncidentStartTime,
                        RestorationDate = incident.RestoreDate,
                        RestorationTime = incident.RestoreTime
                    });
                });

                this.view.RenderUIFromModel(this.gslRebateModel);
                this.view.HideProgressDialog();
            }
            catch (Exception e)
            {
                this.view.HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
