using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
    public class WhatsNewItemContract
    {
        public interface IWhatsNewItemView
        {

        }

        public interface IWhatsNewItemPresenter
        {
            List<WhatsNewModel> InitializeWhatsNewList();

            List<WhatsNewModel> GetActiveWhatsNewList();

            List<WhatsNewModel> GetActiveWhatsNewList(string categoryID);

            void UpdateWhatsNewRead(string itemID, bool flag);
        }
    }
}
