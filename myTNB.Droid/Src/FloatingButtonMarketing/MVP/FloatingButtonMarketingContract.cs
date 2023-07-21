using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.SitecoreCMS.Model;

namespace myTNB_Android.Src.FloatingButtonMarketing.MVP
{
    public class FloatingButtonMarketingContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            public void SetFBMarketingDetail(FloatingButtonMarketingModel item);

            //void SetToolBarContentTitle(FloatingButtonMarketingModel item);

            string GetLocalItemID();

            void SetContentDetailImage(List<FBMarketingDetailImageModel> containedImage);

            void OnSavedFBContentTimeStampRecieved(string timestamp);

            void OnFBContentTimeStampRecieved(string timestamp);

            void OnUpdateFullScreenImage(Bitmap fullBitmap);

            void OnUpdateFullScreenPdf(string path);

            void UpdateContentDetail(FloatingButtonMarketingModel item);

            string GenerateTmpFilePath();

            void SetupFullScreenShimmer();
            //void SetCustomFloatingButtonImage(FloatingButtonModel item);

            //bool GetFBContentSiteCoreDoneFlag();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void GetFBMarketingContent(string itemID);

            Task ProcessContentImage(List<FBMarketingDetailImageDBModel> containedImageDB);

            Task FetchContentImage(List<FBMarketingDetailImageModel> containedImage);

            List<FBMarketingDetailImageModel> ExtractImage(string text);

            void OnGetFBContentTimeStamp();

            void OnGetFBContentItem();

            void GetSavedFBContentTimeStamp();

            Task OnGetFBContentCache();
        }
    }
}

