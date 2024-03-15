using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetVerifyRequest : BaseRequest
    {
        public string IdNo, IdType;

        public GetVerifyRequest(string idType, string idNo)
        {
            this.IdType = idType;
            this.IdNo = idNo;
        }
    }
}
