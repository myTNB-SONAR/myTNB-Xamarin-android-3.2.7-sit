using Android.App;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using static myTNB.AndroidApp.Src.Utils.Constants;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model
{
    public class RewardMenuModel
    {
        public RewardItemFragment Fragment { set; get; }
        public string TabTitle { set; get; }
        public REWARDSITEMLISTMODE FragmentListMode { set; get; }
        public string FragmentSearchString { set; get; }
    }
}