using System;
using System.Collections.Generic;

using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;

namespace myTNB_Android.Src.SSMR.SMRApplication.Adapter
{
    public class OnBoardingSMRAdapter : FragmentStatePagerAdapter
    {
        List<ApplySSMRModel> onBoardingDataModelList;
        public OnBoardingSMRAdapter(FragmentManager fm) : base(fm)
        {
        }

        public void SetData(List<ApplySSMRModel> dataList)
        {
            onBoardingDataModelList = dataList;
        }
        public override int Count => this.onBoardingDataModelList.Count;

        public override Fragment GetItem(int position)
        {
            ApplySSMRModel model = this.onBoardingDataModelList[position];
            return OnBoardingSMRFragment.Instance(model);
        }
    }
}
