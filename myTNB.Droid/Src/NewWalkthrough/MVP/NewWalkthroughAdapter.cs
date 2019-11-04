using System.Collections.Generic;
using Android.Support.V4.App;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughAdapter : FragmentStatePagerAdapter
    {
        List<NewWalkthroughModel> newWalkthroughModelList;
        public NewWalkthroughAdapter(FragmentManager fm) : base(fm)
        {
        }

        public void SetData(List<NewWalkthroughModel> dataList)
        {
            newWalkthroughModelList = dataList;
        }
        public override int Count => this.newWalkthroughModelList.Count;

        public override Fragment GetItem(int position)
        {
            NewWalkthroughModel model = this.newWalkthroughModelList[position];
            return NewWalkthroughFragment.Instance(model);
        }
    }
}
