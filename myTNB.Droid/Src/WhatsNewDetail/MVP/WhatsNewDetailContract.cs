using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.WhatsNewDetail.MVP
{
    public class WhatsNewDetailContract
    {
        public interface IWhatsNewDetaillView
        {
            void SetWhatsNewDetail(WhatsNewModel item);

            void SetWhatsNewImage(Bitmap imgSrc);

            void ShowProgressDialog();

            void HideProgressDialog();

            void SetWhatsNewDetailImage(List<WhatsNewDetailImageModel> containedImage);

            void HideWhatsNewDetailImage();

            string GetLocalItemID();

            void SetupFullScreenShimmer();

            string GenerateTmpFilePath();

            void OnUpdateFullScreenImage(Bitmap fullBitmap);

            void OnUpdateFullScreenPdf(string path);

            void UpdateWhatsNewDetail(WhatsNewModel item);

            void dynaAction(string eventName);
        }

        public interface IWhatsNewDetailPresenter
        {
            void FetchWhatsNewImage(WhatsNewModel item);

            void GetActiveWhatsNew(string itemID);

            List<string> ExtractUrls(string text);

            List<WhatsNewDetailImageModel> ExtractImage(string text);

            Task FetchWhatsNewDetailImage(List<WhatsNewDetailImageModel> containedImage);

            Task ProcessWhatsNewDetailImage(List<WhatsNewDetailImageDBModel> containedImageDB);

            void UpdateWhatsNewRead(string itemID, bool flag);

            void UpdateRewardRead(string itemID, bool flag);

        }
    }
}
