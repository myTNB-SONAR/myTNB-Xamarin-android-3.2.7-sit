using myTNB.Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using static myTNB.Android.Src.Utils.Constants;

namespace myTNB.Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Model
{
	public class WhatsNewMenuModel
    {
		public WhatsNewItemFragment Fragment { set; get; }
		public string TabTitle { set; get; }
		public WHATSNEWITEMLISTMODE FragmentListMode { set; get; }
		public string FragmentSearchString { set; get; }
	}
}