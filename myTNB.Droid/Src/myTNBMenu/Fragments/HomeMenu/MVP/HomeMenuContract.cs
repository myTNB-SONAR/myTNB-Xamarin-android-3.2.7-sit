using System;
namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuContract
    {
        public interface IView
        {
            void OnUpdateAccountListChanged(bool isSearchSubmit);
        }
    }
}
