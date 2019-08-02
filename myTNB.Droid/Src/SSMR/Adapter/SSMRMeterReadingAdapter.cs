using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SSMR.MVP;

namespace myTNB_Android.Src.SSMR.Adapter
{
    public class SSMRMeterReadingAdapter : FragmentStatePagerAdapter
    {
        List<SSMRMeterReadingModel> SSMRMeterReadingModel = new List<SSMRMeterReadingModel>();
        public SSMRMeterReadingAdapter(FragmentManager fm) : base(fm)
        {
        }

        public void SetData(List<SSMRMeterReadingModel> dataList)
        {
            SSMRMeterReadingModel = dataList;
        }
        public override int Count => this.SSMRMeterReadingModel.Count;

        public override Fragment GetItem(int position)
        {
            SSMRMeterReadingModel model = this.SSMRMeterReadingModel[position];
            return SSMRMeterReadingFragment.Instance(model);
        }
    }
}
