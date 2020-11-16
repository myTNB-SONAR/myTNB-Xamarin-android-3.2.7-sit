using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateIdentificationNo : BaseRequest
    {
        public string IdNo, IdType, usrId;

        public UpdateIdentificationNo(string email, string idType, string idNo)
        {
            this.usrId = email;
            this.IdType = idType;
            this.IdNo = idNo;
        }
    }
}
