using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.FindUs.Response;

namespace myTNB.AndroidApp.Src.FindUs.MVP
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

            /// <summary>
            /// Show zero location error
            /// </summary>
            void ShowZeroLocationFoundDialog();

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