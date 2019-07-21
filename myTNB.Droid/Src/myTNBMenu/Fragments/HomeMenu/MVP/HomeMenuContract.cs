using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;

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
        }

        public interface IUserActionsListener : IBasePresenter
        {
            Task InitiateMyService();

            Task InitiateNewFAQ();

            List<MyService> LoadShimmerServiceList(int count);

            List<NewFAQ> LoadShimmerFAQList(int count);
        }
    }
}
