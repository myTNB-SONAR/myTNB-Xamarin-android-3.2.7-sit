using System;
using System.Collections.Generic;


namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SubmitEnquiryTypeRequest : BaseRequest
    {



        public DeviceInfoRequest deviceInf;

        public Feedback feedback;

        public SubmitEnquiryTypeRequest(string feedbackCategoryId, string feedbackTypeId, string accountNum, string name, string phoneNum, string feedbackMesage, string stateId, string location, string poleNum, string ContactMobileNo, string ContactName, string ContactEmailAddress, bool isOwner, int relationship, string relationshipDesc, string EnquiryId, string EnquiryName, List<FeedbackUpdateDetails> feedbackUpdateDetails, List<FeedbackImage> attachment)
        {
            this.feedback = new Feedback(feedbackCategoryId, feedbackTypeId, accountNum, name, phoneNum, feedbackMesage, stateId, location, poleNum, ContactMobileNo, ContactName, ContactEmailAddress, isOwner, relationship, relationshipDesc, EnquiryId, EnquiryName, feedbackUpdateDetails, attachment);
            this.deviceInf = new DeviceInfoRequest();
        }


        public class Feedback
        {
            public string feedbackCategoryId, feedbackTypeId, accountNum, name, phoneNum, feedbackMesage, stateId, location, poleNum, email, contactName, contactMobileNo, contactEmailAddress, relationshipDesc, EnquiryId, EnquiryName, deviceId, feedbackMessage, relationshipwithcadesc;
            public int relationshipwithca;
            public bool isOwner;
            public List<FeedbackUpdateDetails> feedbackUpdateDetails;
            public List<FeedbackImage> AttachedFiles;
            public Feedback(string feedbackCategoryId, string feedbackTypeId, string accountNum, string name, string phoneNum, string feedbackMesage, string stateId, string location, string poleNum, string ContactMobileNo, string ContactName, string ContactEmailAddress, bool isOwner, int relationship, string relationshipDesc, string EnquiryId, string EnquiryName, List<FeedbackUpdateDetails> feedbackUpdateDetails, List<FeedbackImage> attachment)
            {
                this.feedbackCategoryId = feedbackCategoryId;
                this.feedbackTypeId = feedbackTypeId;
                this.accountNum = accountNum;
                this.name = name;
                this.email = ContactEmailAddress;
                this.phoneNum = phoneNum;
                this.feedbackMessage = feedbackMesage;
                this.stateId = stateId;
                this.location = location;
                this.poleNum = poleNum;
                this.contactName = ContactName;
                this.contactMobileNo = ContactMobileNo;
                this.contactEmailAddress = ContactEmailAddress;
                this.isOwner = isOwner;
                this.relationshipwithca = relationship;
                this.relationshipwithcadesc = relationshipDesc;
                this.EnquiryId = EnquiryId;
                this.EnquiryName = EnquiryName;
                this.feedbackUpdateDetails = feedbackUpdateDetails;
                this.AttachedFiles = attachment;
            }
            public void SetFeedbackUpdateDetails(int FeedbackUpdInfoType, string FeedbackUpdInfoTypeDesc, string FeedbackUpdInfoValue)
            {
                this.feedbackUpdateDetails.Add(new FeedbackUpdateDetails(FeedbackUpdInfoType, FeedbackUpdInfoTypeDesc, FeedbackUpdInfoValue));
            }

            public void SetFeedbackImage(string fileHex, string fileName, string fileSize, string fileType)
            {
                this.AttachedFiles.Add(new FeedbackImage(fileHex, fileName, fileSize, fileType));
            }
        }
        public class FeedbackImage
        {
            public string fileHex, fileName, fileSize, fileType;

            public FeedbackImage(string fileHex, string fileName, string fileSize, string fileType)
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