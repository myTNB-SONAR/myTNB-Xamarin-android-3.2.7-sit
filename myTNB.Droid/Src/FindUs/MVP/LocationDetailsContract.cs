using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.FindUs.Response;
using System.Threading;

namespace myTNB.Android.Src.FindUs.MVP
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