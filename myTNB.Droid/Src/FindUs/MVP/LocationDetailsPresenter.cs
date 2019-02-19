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
using System.Net;
using System.Threading;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.FindUs.Api;
using myTNB_Android.Src.FindUs.Response;
using Android.Util;

namespace myTNB_Android.Src.FindUs.MVP
{
    public class LocationDetailsPresenter : LocationDetailsContract.IUserActionsListener
    {
        private readonly string TAG = "FindUsPresenter";
        private LocationDetailsContract.IView mView;
        CancellationTokenSource cts;

        public LocationDetailsPresenter(LocationDetailsContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }


        public void GetLocationDetailsFromGoogle(string placeId, string googelApiKey, CancellationTokenSource ctx)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetLocationDetailsFromGoogleAsync(placeId, googelApiKey, cts);
        }

        public void Start()
        {
            
        }

        private async void GetLocationDetailsFromGoogleAsync(string placeId, string googleApiKey, CancellationTokenSource cts)
        {
            //cts = new CancellationTokenSource();

            var httpClientForGoogle = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri("https://maps.googleapis.com") };
            var googelApi = RestService.For<GetGoogleLocationDetailsApi>(httpClientForGoogle);

            try
            {
                GetGoogleLocationDetailsResponse results = new GetGoogleLocationDetailsResponse();

                results = await googelApi.GetLocationDetailsFromGoogle(placeId, googleApiKey, cts.Token);

                if (results != null)
                {
                    this.mView.ShowGetGoogleLocationDetailsSucess(results);
                }
                else
                {
                    
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                
                Log.Debug(TAG, cancelledException.StackTrace);
            }
            catch (ApiException apiException)
            {
                
                Log.Debug(TAG, apiException.StackTrace);
            }
            catch (Exception unknownException)
            {
                
                Log.Debug(TAG, unknownException.StackTrace);
            }
        }
    }
}