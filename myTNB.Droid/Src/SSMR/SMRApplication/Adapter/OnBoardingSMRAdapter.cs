using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;

namespace myTNB_Android.Src.SSMR.SMRApplication.Adapter
{
    public class OnBoardingSMRAdapter : FragmentStatePagerAdapter
    {
        List<OnBoardingDataModel> onBoardingDataModelList;
        public OnBoardingSMRAdapter(FragmentManager fm) : base(fm)
        {
        }

        public void SetData(List<OnBoardingDataModel> dataList)
        {
            onBoardingDataModelList = dataList;
        }
        public override int Count => this.onBoardingDataModelList.Count;

        public override Fragment GetItem(int position)
        {
            OnBoardingDataModel model = this.onBoardingDataModelList[position];
            return OnBoardingSMRFragment.Instance(model);
        }
    }
}
