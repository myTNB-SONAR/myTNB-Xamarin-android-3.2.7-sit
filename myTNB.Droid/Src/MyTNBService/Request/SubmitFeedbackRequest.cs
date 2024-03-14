using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class SubmitFeedbackRequest : BaseRequest
    {
        public string feedbackCategoryId, feedbackTypeId, accountNum, name, phoneNum, feedbackMesage, stateId, location, poleNum;
        public List<FeedbackImage> images;
        public DeviceInfoRequest deviceInf;

        public SubmitFeedbackRequest(string feedbackCategoryId, string feedbackTypeId, string accountNum, string name, string phoneNum, string feedbackMesage, string stateId, string location, string poleNum)
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
            this.images = new List<FeedbackImage>();
            deviceInf = new DeviceInfoRequest();
        }

        public void SetFeedbackImage(string imageHex, string fileName, string fileSize)
        {
            this.images.Add(new FeedbackImage(imageHex, fileName, fileSize));
        }

        public class FeedbackImage
        {
            public string imageHex, fileName, fileSize;

            public FeedbackImage(string imageHex, string fileName, string fileSize)
            {
                this.imageHex = imageHex;
                this.fileName = fileName;
                this.fileSize = fileSize;
            }
        }
    }
}
