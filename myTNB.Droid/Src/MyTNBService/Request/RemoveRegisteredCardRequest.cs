using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class RemoveRegisteredCardRequest : BaseRequest
    {
        public string registeredCardId;

        public RemoveRegisteredCardRequest(string registeredCardId)
        {
            this.registeredCardId = registeredCardId;
        }
    }
}
