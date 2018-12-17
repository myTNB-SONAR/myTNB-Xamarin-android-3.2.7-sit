﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.FindUs.Response;

namespace myTNB_Android.Src.FindUs.MVP
{
    public class FindUsContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show progress dialog for location service
            /// </summary>
            void ShowGetLocationsDialog();


            /// <summary>
            /// Hide progress dialog for location service
            /// </summary>
            void HideGetLocationsDialog();

            /// <summary>
            /// Show get location success with Location details response
            /// </summary>
            void ShowGetLocationsSuccess(GetLocationsResponse response, GetGoogleLocationsResponse results);

            /// <summary>
            /// Show error message for location service
            /// </summary>
            void ShowGetLocationsError(string message);

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Get location api call Api : GetLocations
            /// </summary>
            void GetLocations(string apiKeyId, string googleApiKey, string latitude, string longitude, string locationType, string locationDes);

            /// <summary>
            /// Get location by keyword api call Api : GetLocationsByKeyword
            /// </summary>
            void GetLocationsByKeyword(string apiKeyId, string googelApiKey, string latitude, string longitude, string locationType, string locationDes, string keyword);
        }

    }
}