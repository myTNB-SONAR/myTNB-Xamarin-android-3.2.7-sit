using System;
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
using System.Threading;

namespace myTNB_Android.Src.FindUs.MVP
{
    public class LocationDetailsContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show location details from google with google location service response
            /// </summary>
            void ShowGetGoogleLocationDetailsSucess(GetGoogleLocationDetailsResponse response);

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Get location details from the google api service Api : /maps/api/place/details/json
            /// </summary>
            void GetLocationDetailsFromGoogle(string placeId, string key, CancellationTokenSource cts);
        }

    }
}