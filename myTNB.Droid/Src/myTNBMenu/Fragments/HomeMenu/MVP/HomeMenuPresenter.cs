using Android.Content;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
	public class HomeMenuPresenter : HomeMenuContract.IUserActionsListener
	{
		internal readonly string TAG = typeof(HomeMenuPresenter).Name;

		CancellationTokenSource cts;



		private HomeMenuContract.IView mView;

		internal int currentBottomNavigationMenu = Resource.Id.menu_dashboard;


		public HomeMenuPresenter(HomeMenuContract.IView mView)
		{
			this.mView = mView;
			this.mView?.SetPresenter(this);
		}

        public void Start()
        {

        }
    }

}