using System;
using System.Collections.Generic;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailPresenter
    {
        public interface IView
        {
            void UpdateUI();
        }

        public interface IPresenter
        {
            List<NewAppModel> OnGeneraNewAppTutorialList();
            List<NewAppModel> OnGeneraNewAppTutorialEmptyList();
        }
    
    }
}

