using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateIdentificationNo : BaseRequestId
    {
        public string IdNo, IdType;

        public UpdateIdentificationNo(string idType, string idNo)
        {
            this.IdType = idType;
            this.IdNo = idNo;
        }
    }
}
