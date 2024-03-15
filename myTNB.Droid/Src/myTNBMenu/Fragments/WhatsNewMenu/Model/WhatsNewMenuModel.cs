using myTNB.AndroidApp.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using static myTNB.AndroidApp.Src.Utils.Constants;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.WhatsNewMenu.Model
{
	public class WhatsNewMenuModel
    {
		public WhatsNewItemFragment Fragment { set; get; }
		public string TabTitle { set; get; }
		public WHATSNEWITEMLISTMODE FragmentListMode { set; get; }
		public string FragmentSearchString { set; get; }
	}
}