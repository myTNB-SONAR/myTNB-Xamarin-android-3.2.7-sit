using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
