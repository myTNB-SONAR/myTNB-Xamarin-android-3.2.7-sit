using System.Collections.Generic;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.WhatsNewDetail.MVP
{
    public class WhatsNewDetailContract
    {
        public interface IWhatsNewDetaillView
        {
            void SetWhatsNewDetail(WhatsNewModel item);

            void SetWhatsNewImage(Bitmap imgSrc);

            void ShowProgressDialog();

            void HideProgressDialog();
        }

        public interface IWhatsNewDetailPresenter
        {
            void FetchWhatsNewImage(WhatsNewModel item);

            void GetActiveWhatsNew(string itemID);

            List<string> ExtractUrls(string text);

            List<WhatsNewDetailImageModel> ExtractImage(string text);

        }
    }
}
