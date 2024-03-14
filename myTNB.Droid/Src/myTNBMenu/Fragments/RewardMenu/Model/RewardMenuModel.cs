using Android.App;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using static myTNB.Android.Src.Utils.Constants;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model
{
    public class RewardMenuModel
    {
        public RewardItemFragment Fragment { set; get; }
        public string TabTitle { set; get; }
        public REWARDSITEMLISTMODE FragmentListMode { set; get; }
        public string FragmentSearchString { set; get; }
    }
}