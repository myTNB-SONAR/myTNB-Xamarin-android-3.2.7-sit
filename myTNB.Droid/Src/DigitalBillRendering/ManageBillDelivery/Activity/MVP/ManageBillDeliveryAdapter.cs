using System.Collections.Generic;
using Android.Content;
using AndroidX.Fragment.App;
using Java.Lang;

namespace myTNB.Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryAdapter : FragmentStatePagerAdapter
    {
        private Android.App.Activity mActivity;
        List<ManageBillDeliveryModel> ManageBillDeliveryModelList;

        public ManageBillDeliveryAdapter(FragmentManager fm, Android.App.Activity activity) : base(fm)
        {
            this.mActivity = activity;
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
            return ManageBillDeliveryFragment.Instance(model, isLastItem, this.mActivity);
        }

        public override int GetItemPosition(Object @object)
        {
            return PositionNone;
        }
    }
}