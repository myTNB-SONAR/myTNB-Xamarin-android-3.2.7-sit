﻿using myTNB_Android.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB_Android.Src.TermsAndConditions.MVP
{
    public class TermsAndConditionContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show terms and condition service sucess
            /// </summary>
            /// <param name="success"></param>
            void ShowTermsAndCondition(bool success);

            void HideProgressBar();
        }


        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Call sitecore service to get terms and conditions 
            /// On success it will save data into local DB
            /// </summary>
            /// <returns></returns>
            Task GetTermsAndConditionData();
        }

    }
}