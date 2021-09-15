using System.Collections.Generic;
using AndroidX.Fragment.App;
using Java.Lang;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryAdapter : FragmentStatePagerAdapter
    {
        List<ManageBillDeliveryModel> ManageBillDeliveryModelList;
        public ManageBillDeliveryAdapter(FragmentManager fm) : base(fm)
        {

        }

        public void SetData(List<ManageBillDeliveryModel> dataList)
        {
            ManageBillDeliveryModelList = dataList;
        }

        public override int Count => this.ManageBillDeliveryModelList.Count;

        public override Fragment GetItem(int position)
        {
            ManageBillDeliveryModel model = this.ManageBillDeliveryModelList[position];
            bool isLastItem = false;
            if (position == this.ManageBillDeliveryModelList.Count - 1)
            {
                isLastItem = true;
            }
            return ManageBillDeliveryFragment.Instance(model, isLastItem);
        }

        public override int GetItemPosition(Object @object)
        {
            return PositionNone;
        }
    }
}