using System;
using System.Collections.Generic;
using Android.Content;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailPresenter
    {

        ApplicationStatusDetailContract.IView mView;


        public ApplicationStatusDetailPresenter(ApplicationStatusDetailContract.IView view, ISharedPreferences pref)
        {
            mView = view;
        }
        public ApplicationStatusDetailPresenter(ApplicationStatusDetailContract.IView view)
        {
            mView = view;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialNoActionList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsNoActionTitle"), // "Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsNoActionMessage"),//"Your submitted applications will automatically appear here so you can view their status. Use the filter to search through the list easily."
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialActionList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionOneTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionOneMessage"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "Extra",
                IsButtonShow = false
            });
            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionTwoTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionTwoMessage"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialInProgressList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsInProgressTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsInProgressMessage"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });
            return newList;
        }
    }
}