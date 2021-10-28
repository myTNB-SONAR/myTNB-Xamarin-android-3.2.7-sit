using System.Collections.Generic;
using myTNB_Android.Src.Enquiry.GSL.MVP;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SubmitGSLEnquiryRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public Feedback feedback;

        public SubmitGSLEnquiryRequest(GSLRebateModel gSLRebateModel)
        {
            this.feedback = new Feedback(gSLRebateModel);
            this.deviceInf = new DeviceInfoRequest();
        }

        public class Feedback
        {
            public string feedbackCategoryId, feedbackTypeId, accountNum, name, email, phoneNum, feedbackMessage, stateId, location, poleNum, contactName, contactMobileNo, contactEmailAddress;
            public string EnquiryId, EnquiryName, relationshipDesc;
            public string TenantFullName, TenantEmail, TenantMobileNumber, RebateId;
            public int relationship;
            public bool isOwner;
            public List<FeedbackUpdateDetails> feedbackUpdateDetails;
            public List<IncidentInfoDetails> IncidentInfos;
            public List<EnquiryImage> AttachedFiles;

            public Feedback(GSLRebateModel gSLRebateModel)
            {
                this.feedbackCategoryId = gSLRebateModel.FeedbackCategoryId;
                this.feedbackTypeId = string.Empty;
                this.accountNum = gSLRebateModel.AccountNum;
                this.name = gSLRebateModel.ContactInfo.FullName;
                this.email = gSLRebateModel.ContactInfo.Email;
                this.phoneNum = gSLRebateModel.ContactInfo.MobileNumber;
                this.feedbackMessage = string.Empty;
                this.stateId = string.Empty;
                this.location = string.Empty;
                this.poleNum = string.Empty;
                this.AttachedFiles = new List<EnquiryImage>();
                this.contactName = gSLRebateModel.ContactInfo.FullName;
                this.contactMobileNo = gSLRebateModel.ContactInfo.MobileNumber;
                this.contactEmailAddress = gSLRebateModel.ContactInfo.Email;
                this.feedbackUpdateDetails = new List<FeedbackUpdateDetails>();
                this.IncidentInfos = new List<IncidentInfoDetails>();
                this.isOwner = gSLRebateModel.IsOwner;
                this.relationship = 0;
                this.relationshipDesc = "";
                this.EnquiryId = string.Empty;
                this.EnquiryName = string.Empty;
                this.TenantFullName = gSLRebateModel.TenantInfo.FullName;
                this.TenantEmail = gSLRebateModel.TenantInfo.Email;
                this.TenantMobileNumber = gSLRebateModel.TenantInfo.MobileNumber;
                this.RebateId = gSLRebateModel.RebateTypeKey;
            }

            public void SetEnquiryImage(string fileHex, string fileName, string fileSize, string fileType)
            {
                this.AttachedFiles.Add(new EnquiryImage(fileHex, fileName, fileSize, fileType));
            }

            public void SetIncidentInfos(string incidentDateTime, string restoreDateTime)
            {
                this.IncidentInfos.Add(new IncidentInfoDetails(incidentDateTime, restoreDateTime));
            }
        }

        public class EnquiryImage
        {
            public string fileHex, fileName, fileSize, fileType;

            public EnquiryImage(string fileHex, string fileName, string fileSize, string fileType)
            {
                this.fileHex = fileHex;
                this.fileName = fileName;
                this.fileSize = fileSize;
                this.fileType = fileType;
            }
        }

        public class FeedbackUpdateDetails
        {
            public string FeedbackUpdInfoTypeDesc, FeedbackUpdInfoValue;
            public int FeedbackUpdInfoType;

            public FeedbackUpdateDetails(int FeedbackUpdInfoType, string FeedbackUpdInfoTypeDesc, string FeedbackUpdInfoValue)
            {
                this.FeedbackUpdInfoType = FeedbackUpdInfoType;
                this.FeedbackUpdInfoTypeDesc = FeedbackUpdInfoTypeDesc;
                this.FeedbackUpdInfoValue = FeedbackUpdInfoValue;
            }
        }

        public class IncidentInfoDetails
        {
            public string IncidentDateTime, RestoreDateTime;

            public IncidentInfoDetails(string incidentDateTime, string restoreDateTime)
            {
                this.IncidentDateTime = incidentDateTime;
                this.RestoreDateTime = restoreDateTime;
            }
        }
    }
}
