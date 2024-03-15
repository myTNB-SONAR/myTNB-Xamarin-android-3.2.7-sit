using System;
using System.Collections.Generic;


namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SubmitEnquiryRequest : BaseRequest
    {
        public string feedbackCategoryId, feedbackTypeId, accountNum, name, phoneNum, feedbackMesage, stateId, location, poleNum, email , contactName, contactMobileNo, contactEmailAddress  , relationshipDesc;
        public int relationship;
        public bool isOwner;
        public List<FeedbackImage> attachment;
        public DeviceInfoRequest deviceInf;
        public List<FeedbackUpdateDetails> feedbackUpdateDetails;

        public SubmitEnquiryRequest(string feedbackCategoryId, string feedbackTypeId, string accountNum, string name, string phoneNum, string feedbackMesage, string stateId, string location, string poleNum , string ContactMobileNo, string ContactName, string ContactEmailAddress , bool isOwner , int relationship , string relationshipDesc)
        {
            this.feedbackCategoryId = feedbackCategoryId;
            this.feedbackTypeId = feedbackTypeId;
            this.accountNum = accountNum;
            this.name = name;
            this.phoneNum = phoneNum;
            this.feedbackMesage = feedbackMesage;
            this.stateId = stateId;
            this.location = location;
            this.poleNum = poleNum;
            this.attachment = new List<FeedbackImage>();
            this.deviceInf = new DeviceInfoRequest();
            this.contactMobileNo = ContactMobileNo;
            this.contactEmailAddress = ContactEmailAddress;
            this.contactName = ContactName;
            this.feedbackUpdateDetails = new List<FeedbackUpdateDetails>();
            this.isOwner = isOwner;
            this.relationship = relationship;
            this.relationshipDesc = relationshipDesc;

        }

        public void SetFeedbackUpdateDetails(int FeedbackUpdInfoType, string FeedbackUpdInfoTypeDesc, string FeedbackUpdInfoValue)
        {
            this.feedbackUpdateDetails.Add(new FeedbackUpdateDetails( FeedbackUpdInfoType,  FeedbackUpdInfoTypeDesc,  FeedbackUpdInfoValue));
        }

        public void SetFeedbackImage(string fileHex, string fileName, string fileSize , string fileType )
        {
            this.attachment.Add(new FeedbackImage(fileHex, fileName, fileSize, fileType));
        }

        public class FeedbackImage
        {
            public string fileHex, fileName, fileSize , fileType;

            public FeedbackImage(string fileHex, string fileName, string fileSize , string fileType)
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
    }
}