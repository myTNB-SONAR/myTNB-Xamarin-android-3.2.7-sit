using Android.Util;
using myTNB.AndroidApp.Src.FindUs.Api;
using myTNB.AndroidApp.Src.FindUs.Request;
using myTNB.AndroidApp.Src.FindUs.Response;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.FindUs.MVP
{
    public class FindUsPresenter : FindUsContract.IUserActionsListener
    {
        private readonly string TAG = "FindUsPresenter";
        private FindUsContract.IView mView;
        CancellationTokenSource cts;

        public FindUsPresenter(FindUsContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {

        }

        public void GetLocations(string apiKeyId, string googelApiKey, string latitude, string longitude, string locationType, string locationDes)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetLocationAsync(apiKeyId, googelApiKey, latitude, longitude, locationType, locationDes);
        }

        private async void GetLocationAsync(string apiKeyId, string googleApiKey, string latitude, string longitude, string locationType, string locationDes)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetLocationsDialog();
            }
            ////remove google api call to search 7 eleven
            //var httpClientForGoogle = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri("https://maps.googleapis.com") };
            //var googelApi = RestService.For<GetGoogleLocationApi>(httpClientForGoogle);

            try
            {
                GetLocationsResponse result = new GetLocationsResponse();
                GetGoogleLocationsResponse results = null;// new GetGoogleLocationsResponse();
                if (locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                {
                    GetLocationListResponse locationListResponse = await ServiceApiImpl.Instance.GetLocations(new GetLocationListRequest(latitude, longitude, "KT"));

                    if (!locationListResponse.IsSuccessResponse())
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideGetLocationsDialog();
                        }
                        this.mView.ShowZeroLocationFoundDialog();
                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideGetLocationsDialog();
                        }
                        result.D = new GetLocationsResponse.LocationsResponse();
                        result.D.LocationList = locationListResponse.GetData();
                        this.mView.ShowGetLocationsSuccess(result, results);
                    }
                }

                //if (!locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                //{
                //    //Since 7-eleven is the only option the we have than Kedia Tenanga
                //    locationDes = "7-Eleven";
                //    results = await googelApi.GetLocationsFromGoogle(googleApiKey, latitude + "," + longitude, "5000", locationDes, locationDes, cts.Token);
                //}
            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                this.mView.ShowGetLocationsError(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                this.mView.ShowGetLocationsError(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                this.mView.ShowGetLocationsError(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void GetLocationsByKeyword(string apiKeyId, string googelApiKey, string latitude, string longitude, string locationType, string locationDes, string keyword)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetLocationsByKewordAsync(apiKeyId, googelApiKey, latitude, longitude, locationType, locationDes, keyword);
        }


        private async void GetLocationsByKewordAsync(string apiKeyId, string googleApiKey, string latitude, string longitude, string locationType, string locationDes, string keyword)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetLocationsDialog();
            }

            ////remove google api call to search 7 eleven
            //var httpClientForGoogle = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri("https://maps.googleapis.com") };
            //var googelApi = RestService.For<GetGoogleLocationApi>(httpClientForGoogle);

            try
            {
                GetGoogleLocationsResponse results = new GetGoogleLocationsResponse();
                GetLocationsResponse result = null; // new GetLocationsResponse();

                if (locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                {
                    GetLocationListByKeywordResponse locationListResponse = await ServiceApiImpl.Instance.GetLocationsByKeyword(new GetLocationListByKeywordRequest(latitude, longitude, "KT", keyword));

                    if (!locationListResponse.IsSuccessResponse())
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideGetLocationsDialog();
                        }
                        this.mView.ShowZeroLocationFoundDialog();
                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideGetLocationsDialog();
                        }
                        result = new GetLocationsResponse();
                        result.D = new GetLocationsResponse.LocationsResponse();
                        result.D.LocationList = locationListResponse.GetData();
                        this.mView.ShowGetLocationsSuccess(result, results);
                    }

                }
                //if (!locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                //{
                //    results = await googelApi.GetLocationsFromGoogle(googleApiKey, latitude + "," + longitude, "1000", keyword, locationDes, cts.Token);
                //}
            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                Log.Debug(TAG, cancelledException.StackTrace);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                Log.Debug(TAG, apiException.StackTrace);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetLocationsDialog();
                }
                Log.Debug(TAG, unknownException.StackTrace);
                Utility.LoggingNonFatalError(unknownException);
            }
        }
    }
}
