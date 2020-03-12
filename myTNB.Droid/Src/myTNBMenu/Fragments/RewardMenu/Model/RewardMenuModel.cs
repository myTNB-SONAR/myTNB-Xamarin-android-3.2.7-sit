using Android.App;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model
{
    public class RewardMenuModel
    {
        public RewardItemFragment Fragment { set; get; }
        public string TabTitle { set; get; }
        public REWARDSITEMLISTMODE FragmentListMode { set; get; }
        public string FragmentSearchString { set; get; }
    }
}