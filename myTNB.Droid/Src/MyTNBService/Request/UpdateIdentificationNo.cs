using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class UpdateIdentificationNo : BaseRequest
    {
        public string IdNo, IdType;

        public UpdateIdentificationNo(string idType, string idNo)
        {
            this.IdType = idType;
            this.IdNo = idNo;
        }
    }
}
