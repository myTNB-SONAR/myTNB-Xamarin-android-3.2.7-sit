using Android.App;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model
{
    public class RewardMenuModel
    {
        public Fragment Fragment { set; get; }
        public string TabTitle { set; get; }
        public REWARDSITEMLISTMODE FragmentListMode { set; get; }
        public string FragmentSearchString { set; get; }
    }
}