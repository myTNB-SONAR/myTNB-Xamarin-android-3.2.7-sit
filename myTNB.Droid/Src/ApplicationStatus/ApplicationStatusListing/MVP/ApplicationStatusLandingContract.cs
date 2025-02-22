﻿using System.Collections.Generic;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.MVP
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