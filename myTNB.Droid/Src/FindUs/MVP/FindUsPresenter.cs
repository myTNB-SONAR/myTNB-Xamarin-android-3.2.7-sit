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
using myTNB_Android.Src.Utils;
using System.Net;
using Refit;
using myTNB_Android.Src.FindUs.Api;
using myTNB_Android.Src.FindUs.Response;
using System.Net.Http;
using myTNB_Android.Src.FindUs.Request;
using System.Threading;
using Android.Util;

namespace myTNB_Android.Src.FindUs.MVP
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
            cts = new CancellationTokenSource();

            this.mView.ShowGetLocationsDialog();
            // TODO : UPDATE Replace string with Constants.SERVER_URL

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetLocationsApi>(httpClient);

#else
            var api = RestService.For<GetLocationsApi>(Constants.SERVER_URL.END_POINT);

#endif
            ////remove google api call to search 7 eleven
            //var httpClientForGoogle = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri("https://maps.googleapis.com") };
            //var googelApi = RestService.For<GetGoogleLocationApi>(httpClientForGoogle);

            try
            {
                GetLocationsResponse result = new GetLocationsResponse();
                GetGoogleLocationsResponse results = null;// new GetGoogleLocationsResponse();
                if (locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all") )
                {
                    result = await api.GetLocationsV5(new GetLocationsRequest(apiKeyId, latitude, longitude, "KT"), cts.Token);
                }

                //if (!locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                //{
                //    //Since 7-eleven is the only option the we have than Kedia Tenanga
                //    locationDes = "7-Eleven";
                //    results = await googelApi.GetLocationsFromGoogle(googleApiKey, latitude + "," + longitude, "5000", locationDes, locationDes, cts.Token);
                //}

                

                if (result.D != null && result.D.IsError)
                {
                    this.mView.HideGetLocationsDialog();
                    this.mView.ShowGetLocationsError(result.D.Message);
                }
                else
                {
                    this.mView.HideGetLocationsDialog();
                    this.mView.ShowGetLocationsSuccess(result, results);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideGetLocationsDialog();
                this.mView.ShowGetLocationsError("Something went wrong! Please try again later");
            }
            catch (ApiException apiException)
            {
                this.mView.HideGetLocationsDialog();
                this.mView.ShowGetLocationsError("Something went wrong! Please try again later");
            }
            catch (Exception unknownException)
            {
                this.mView.HideGetLocationsDialog();
                this.mView.ShowGetLocationsError("Something went wrong! Please try again later");
            }
        }

        public void GetLocationsByKeyword(string apiKeyId, string googelApiKey, string latitude, string longitude, string locationType, string locationDes, string keyword)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetLocationsByKewordAsync(apiKeyId, googelApiKey, latitude, longitude, locationType, locationDes, keyword);
        }


        private async void GetLocationsByKewordAsync(string apiKeyId, string googleApiKey, string latitude, string longitude, string locationType, string locationDes, string keyword)
        {
            cts = new CancellationTokenSource();

            this.mView.ShowGetLocationsDialog();
            // TODO : UPDATE Replace string with Constants.SERVER_URL

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetLocationsByKeywordApi>(httpClient);

#else
            var api = RestService.For<GetLocationsByKeywordApi>(Constants.SERVER_URL.END_POINT);

#endif
            ////remove google api call to search 7 eleven
            //var httpClientForGoogle = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri("https://maps.googleapis.com") };
            //var googelApi = RestService.For<GetGoogleLocationApi>(httpClientForGoogle);

            try
            {
                GetGoogleLocationsResponse results = new GetGoogleLocationsResponse();
                GetLocationsResponse result = null; // new GetLocationsResponse();

                if (locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                {
                    result = await api.GetLocationsByKeyword(new GetLocationsByKeywordRequest(apiKeyId, latitude, longitude, "KT", keyword), cts.Token);
                }
                //if (!locationType.ToLower().Equals("kt") || locationType.ToLower().Equals("all"))
                //{
                //    results = await googelApi.GetLocationsFromGoogle(googleApiKey, latitude + "," + longitude, "1000", keyword, locationDes, cts.Token);
                //}

                if (result.D != null && result.D.IsError)
                {
                    this.mView.HideGetLocationsDialog();
                }
                else
                {
                    this.mView.HideGetLocationsDialog();
                    this.mView.ShowGetLocationsSuccess(result, results);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideGetLocationsDialog();
                Log.Debug(TAG, cancelledException.StackTrace);
            }
            catch (ApiException apiException)
            {
                this.mView.HideGetLocationsDialog();
                Log.Debug(TAG, apiException.StackTrace);
            }
            catch (Exception unknownException)
            {
                this.mView.HideGetLocationsDialog();
                Log.Debug(TAG, unknownException.StackTrace);
            }
        }
    }
}
