using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
	public class HomeMenuContract
	{
		public interface IView : IBaseView<IUserActionsListener>
		{


		}

		public interface IUserActionsListener : IBasePresenter
		{

		}
	}
}