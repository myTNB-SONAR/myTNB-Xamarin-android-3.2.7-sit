using System;
namespace myTNB.Model
{
    public class PhoneVerificationStatusResponseModel : BaseModel
    {
        public PhoneVerificationStatusResponseModel()
        {
        }

        public PhoneVerificationStatusResponseModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
            StatusCode = model.StatusCode;
        }

        public PhoneVerificationStatusDataModel data { get; set; }
    }
}
