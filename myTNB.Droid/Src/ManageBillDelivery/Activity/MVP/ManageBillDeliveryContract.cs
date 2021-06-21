using System.Collections.Generic;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryContract
    {
        public interface IView
        {
            string GetAppString(int id);
        }

        public interface IPresenter
        {
            List<ManageBillDeliveryModel> GenerateNewWalkthroughList(string currentAppNavigation);
        }
    }
}