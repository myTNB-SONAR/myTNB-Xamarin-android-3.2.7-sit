using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryPresenter
    {
        List<ManageBillDeliveryModel> ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        ManageBillDeliveryContract.IView mView;
        public ManageBillDeliveryPresenter(ManageBillDeliveryContract.IView view)
        {
            this.mView = view;
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        }

        public List<ManageBillDeliveryModel> GenerateManageBillDeliveryList(string currentAppNavigation)
        {
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title1"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description1"),
                    Image = "manage_bill_delivery_0"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title2"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description2"),
                    Image = "manage_bill_delivery_1"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title3"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description3"),
                    Image = "manage_bill_delivery_2"
                });
           

            return ManageBillDeliveryList;
        }
    }
}