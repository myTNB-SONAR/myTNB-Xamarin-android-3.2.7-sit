using System.Collections.Generic;
using AndroidX.Fragment.App;
using Java.Lang;

namespace myTNB.AndroidApp.Src.NewWalkthrough.MVP
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
            bool isLastItem = false;
            if (position == this.newWalkthroughModelList.Count - 1)
            {
                isLastItem = true;
            }
            return NewWalkthroughFragment.Instance(model, isLastItem);
        }

        public override int GetItemPosition(Object @object)
        {
            return PositionNone;
        }
    }
}