using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB_Android.Src.Enquiry.GSL.MVP.GSLSubmittedDetails
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

                try
                {
                    this.gslRebateModel.IncidentList = new List<GSLRebateIncidentModel>();
                    gslRebateDetails.IncidentInfos.ForEach(incident =>
                    {
                        DateTime incidentDate = new DateTime();
                        DateTime restorationDate = new DateTime();

                        CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());

                        if (incident.IncidentDate.IsValid())
                        {
                            string incidentDateStr = incident.IncidentDate;
                            incidentDate = DateTime.ParseExact(incidentDateStr, GSLRebateConstants.DATE_RESPONSE_PARSE_FORMAT,
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }

                        if (incident.IncidentStartTime.IsValid())
                        {
                            string incidentStartTimeStr = incident.IncidentStartTime;
                            DateTime timeParse = DateTime.ParseExact(incidentStartTimeStr, GSLRebateConstants.TIME_RESPONSE_PARSE_FORMAT,
                                CultureInfo.InvariantCulture, DateTimeStyles.None);

                            TimeSpan incidentTime = new TimeSpan(timeParse.Hour, timeParse.Minute, 0);
                            incidentDate = incidentDate.Date + incidentTime;
                        }

                        if (incident.RestoreDate.IsValid())
                        {
                            string restoreDateStr = incident.RestoreDate;
                            restorationDate = DateTime.ParseExact(restoreDateStr, GSLRebateConstants.DATE_RESPONSE_PARSE_FORMAT,
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }

                        if (incident.RestoreTime.IsValid())
                        {
                            string restoreStartTimeStr = incident.RestoreTime;
                            DateTime timeParse = DateTime.ParseExact(restoreStartTimeStr, GSLRebateConstants.TIME_RESPONSE_PARSE_FORMAT,
                                CultureInfo.InvariantCulture, DateTimeStyles.None);

                            TimeSpan restoreTime = new TimeSpan(timeParse.Hour, timeParse.Minute, 0);
                            restorationDate = restorationDate.Date + restoreTime;
                        }

                        this.gslRebateModel.IncidentList.Add(new GSLRebateIncidentModel
                        {
                            IncidentDateTime = incidentDate.ToString(),
                            RestorationDateTime = restorationDate.ToString()
                        });
                    });
                }
                catch (Exception e)
                {
                    this.view.RenderUIFromModel(this.gslRebateModel);
                    this.view.HideProgressDialog();
                    Utility.LoggingNonFatalError(e);
                }

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
