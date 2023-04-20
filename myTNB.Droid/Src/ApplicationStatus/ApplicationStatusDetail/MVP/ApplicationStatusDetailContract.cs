using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailContract
    {
        public interface IView
        {
            void UpdateUI();

            void ShowProgressDialog();

            void HideProgressDialog();

            /// <summary>
            /// Triggers the share functionality for the downloaded file
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="fileExtension"></param>
            /// <param name="fileTitle"></param>
            void ShareDownloadedFile(string filePath, string fileExtension, string fileTitle);
        }

        public interface IPresenter
        {
            List<NewAppModel> OnGeneraNewAppTutorialNoActionList();
            List<NewAppModel> OnGeneraNewAppTutorialActionList();
            List<NewAppModel> OnGeneraNewAppTutorialNoneList();
            List<NewAppModel> OnGeneraNewAppTutorialInProgressList();

            /// <summary>
            /// Task to download file from webURL
            /// </summary>
            /// <param name="webURL"></param>
            /// <returns></returns>
            Task DownloadFile(string webURL);
        }
    }
}