using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    public class ApplicationStatusLandingPresenter : ApplicationStatusLandingContract.IPresenter
    {
        ApplicationStatusLandingContract.IView mView;
        private ISharedPreferences mPref;

        public ApplicationStatusLandingPresenter(ApplicationStatusLandingContract.IView view, ISharedPreferences pref)
        {
            mView = view;
            this.mPref = pref;
        }
        public ApplicationStatusLandingPresenter(ApplicationStatusLandingContract.IView view)
        {
            mView = view;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingTitle"), // "Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingDescription"),//"Your submitted applications will automatically appear here so you can view their status. Use the filter to search through the list easily."
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchDescription"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;

        }
        public List<NewAppModel> OnGeneraNewAppTutorialEmptyList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingEmptyTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingEmptyDescription"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "Extra",
                IsButtonShow = false
            });
            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchDescription"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;

        }
    }
}