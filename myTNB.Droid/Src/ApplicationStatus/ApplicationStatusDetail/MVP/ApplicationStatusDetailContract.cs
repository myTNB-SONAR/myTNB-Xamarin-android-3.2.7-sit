using System.Collections.Generic;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailContract
    {
        public interface IView
        {
            void UpdateUI();
        }

        public interface IPresenter
        {
            List<NewAppModel> OnGeneraNewAppTutorialNoActionList();
            List<NewAppModel> OnGeneraNewAppTutorialActionList();
            List<NewAppModel> OnGeneraNewAppTutorialNoneList();
            List<NewAppModel> OnGeneraNewAppTutorialInProgressList();
        }
    }
}