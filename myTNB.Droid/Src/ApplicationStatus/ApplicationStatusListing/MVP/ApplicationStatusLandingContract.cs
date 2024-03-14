using System.Collections.Generic;
using myTNB.Android.Src.NewAppTutorial.MVP;

namespace myTNB.Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    public class ApplicationStatusLandingContract
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