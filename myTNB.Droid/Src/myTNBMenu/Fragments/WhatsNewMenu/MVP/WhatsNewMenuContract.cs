using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Model;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
    public class WhatsNewMenuContract
    {
        public interface IWhatsNewMenuView
        {
            void OnSavedWhatsNewsTimeStamp(string mSavedTimeStamp);

            void CheckWhatsNewsTimeStamp();

            void OnSetResultTabView(List<WhatsNewMenuModel> list);

            void SetEmptyView();

            bool CheckTabVisibility();

            void SetRefreshView(string buttonText, string messageText);

            void OnGetWhatsNewTimestamp();
        }

        public interface IWhatsNewMenuPresenter
        {
            List<WhatsNewMenuModel> InitializeWhatsNewView();

            Task OnGetWhatsNews();

            void OnCancelTask();

            void GetWhatsNewsTimeStamp();

            Task OnGetWhatsNewsTimeStamp();

            void CheckWhatsNewsCache();

            Task OnGetUserWhatsNewList();

            List<NewAppModel> OnGeneraNewAppTutorialList();

            Task OnRecheckWhatsNewsStatus();
        }
    }
}
