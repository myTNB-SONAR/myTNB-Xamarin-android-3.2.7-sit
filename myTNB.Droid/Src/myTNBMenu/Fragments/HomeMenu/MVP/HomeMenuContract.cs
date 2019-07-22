using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void OnUpdateAccountListChanged(bool isSearchSubmit);

            void SetMyServiceRecycleView();

            void SetNewFAQRecycleView();

            void SetMyServiceResult(List<MyService> list);

            void SetNewFAQResult(List<NewFAQ> list);

            string GetDeviceId();

            void ShowMyServiceRetryOptions(string msg);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            Task InitiateMyService();

            Task InitiateNewFAQ();

            Task RetryMyService();

            List<MyService> LoadShimmerServiceList(int count);

            List<NewFAQ> LoadShimmerFAQList(int count);
        }
    }
}
