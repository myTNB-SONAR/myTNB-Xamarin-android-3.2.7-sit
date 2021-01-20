﻿using System.Collections.Generic;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughContract
    {
        public interface IView
        {
            string GetAppString(int id);
        }

        public interface IPresenter
        {
            List<NewWalkthroughModel> GenerateNewWalkthroughList(string currentAppNavigation);
        }
    }
}