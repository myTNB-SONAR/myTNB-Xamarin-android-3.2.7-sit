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
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using System.Threading;
using Android.Text;
using System.Net;
using System.Net.Http;
using myTNB_Android.Src.AppLaunch.Api;
using Refit;
using myTNB_Android.Src.Base.Request;
using Newtonsoft.Json;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Database.Model;
using Android.Locations;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using Java.Text;
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Api;

namespace myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.MVP
{
    public class FeedbackPreLoginFaultyStreetLampsPresenter : FeedbackPreLoginFaultyStreetLampsContract.IUserActionsListener
    {

        private FeedbackPreLoginFaultyStreetLampsContract.IView mView;
        CancellationTokenSource cts;


        public FeedbackPreLoginFaultyStreetLampsPresenter(FeedbackPreLoginFaultyStreetLampsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        const string ALPHANUMERIC_PATTERN = "[a-zA-Z0-9\\s]*";

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try {
            if (requestCode == Constants.REQUEST_ATTACHED_CAMERA_IMAGE)
            {
                if (resultCode == Result.Ok)
                {
                    // TODO : ADD PROGRESS
                    string fileName = string.Format("{0}.jpeg", Guid.NewGuid());
                    string tempImagePath = this.mView.GetTemporaryImageFilePath(FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage"));
                    OnSaveCameraImage(tempImagePath, fileName);

                    GC.Collect();
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.jpeg", Guid.NewGuid());

                    OnSaveGalleryImage(selectedImage, fileName);
                    GC.Collect();
                }
            }
            else if (requestCode == Constants.SELECT_FEEDBACK_STATE)
            {
                if (resultCode == Result.Ok)
                {
                    FeedbackState selectedState = JsonConvert.DeserializeObject<FeedbackState>(data.Extras.GetString(Constants.SELECTED_FEEDBACK_STATE));
                    this.mView.ShowState(selectedState);
                } else {
                    this.mView.ShowState(null);   
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            try {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            try {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);


            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnAttachPhotoCamera()
        {
            this.mView.ShowCamera();
        }

        public void OnAttachPhotoGallery()
        {
            this.mView.ShowGallery();
        }

        public async void OnSubmit(string deviceId, string fullname, string mobile_no, string email, FeedbackState feedbackState, string location, string pole_no, string feedback, List<AttachedImage> attachedImages)
        {
            cts = new CancellationTokenSource();
            this.mView.ClearErrors();
            if (TextUtils.IsEmpty(fullname))
            {
                this.mView.ShowEmptyFullnameError();
                return;
            }

            if (!Utility.isAlphaNumeric(fullname))
            {
                this.mView.ShowNameError();
                return;
            }

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                return;
            }

            if (!Android.Util.Patterns.Phone.Matcher(mobile_no).Matches())
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

             
            if (!Utility.IsValidMobileNumber(mobile_no)) {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (TextUtils.IsEmpty(email))
            {
                this.mView.ShowEmptyEmaiError();
                return;
            }

            if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                this.mView.ShowInvalidEmailError();
                return;
            }

            if (TextUtils.IsEmpty(location))
            {
                this.mView.ShowEmptyLocationError();
                return;
            }

            if (TextUtils.IsEmpty(pole_no))
            {
                pole_no = "";
            }

            if (!Java.Util.Regex.Pattern.Compile(ALPHANUMERIC_PATTERN).Matcher(pole_no).Matches())
            {
                this.mView.ShowInvalidPoleNoError();
                return;
            }

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }

            if (feedbackState == null) {
                this.mView.ShowEmptyStateError();
                return;
            }

            if (mView.IsActive()) {
            this.mView.ShowProgressDialog();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var preloginFeedbackApi = RestService.For<IFeedbackApi>(httpClient);
#else
            var preloginFeedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                var request = new FeedbackRequest()
                {

                    Images = imageRequest,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    FeedbackCategoryId = "2",
                    FeedbackTypeId = "",
                    PhoneNum = mobile_no,
                    AccountNum = "",
                    Name = fullname,
                    Email = email,
                    DeviceId = deviceId,
                    FeedbackMessage = feedback,
                    StateId = feedbackState.StateId,
                    Location = location,
                    PoleNum = pole_no

                };



                var preLoginFeedbackResponse = await preloginFeedbackApi.SubmitFeedback(request, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!preLoginFeedbackResponse.Data.IsError)
                {

                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.Data.Data.FeedbackId,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "2"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    this.mView.ShowSuccess(preLoginFeedbackResponse.Data.Data.DateCreated, preLoginFeedbackResponse.Data.Data.FeedbackId, attachedImages.Count);
                }
                else
                {
                    //this.mView.ShowFail();
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Data.Message);
                }

            }

            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);

            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }

        }

        public void Start()
        {
            // TODO : SHOW FIRST STATE
            this.mView.DisableSubmitButton();
            FeedbackStateEntity.RemoveActive();
            //FeedbackStateEntity.ResetSelected();
            //FeedbackStateEntity entity = FeedbackStateEntity.GetFirstOrSelected();
            //FeedbackStateEntity.SetSelected(entity.Id);
            //this.mView.ShowState(FeedbackState.Copy(entity));

        }

        public void OnSelectFeedbackState()
        {
            this.mView.ShowSelectFeedbackState();
        }

       

        public async Task<Position> GetCurrentLocation()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                {
                    //got a cahched position, so let's use it.
                    return position;
                }

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                {
                    //not available or enabled
                    return null;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

                
            }
            catch (Exception ex)
            {
                //Display error as we have timed out or can't get location.
                Utility.LoggingNonFatalError(ex);
            }


            return position;
        }

        public void CheckRequiredFields(string fullname, string mobile_no, string email, string location, string feedback, string state)
        {
            //if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) && !TextUtils.IsEmpty(location) && !TextUtils.IsEmpty(location))
            //{

            try {
            this.mView.ClearErrors();
            if (TextUtils.IsEmpty(fullname))
            {
                this.mView.ShowEmptyFullnameError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (!Utility.isAlphaNumeric(fullname))
            {
                this.mView.ShowNameError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (!Android.Util.Patterns.Phone.Matcher(mobile_no).Matches())
            {
                this.mView.ShowInvalidMobileNoError();
                this.mView.DisableSubmitButton();
                return;
            }


            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (TextUtils.IsEmpty(email))
            {
                this.mView.ShowEmptyEmaiError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                this.mView.ShowInvalidEmailError();
                this.mView.DisableSubmitButton();
                return;
            }

            if (TextUtils.IsEmpty(location))
            {
                this.mView.ShowEmptyLocationError();
                this.mView.DisableSubmitButton();
                return;
            }

            //if (TextUtils.IsEmpty(pole_no))
            //{
            //    pole_no = "";
            //}

            //if (!Java.Util.Regex.Pattern.Compile(ALPHANUMERIC_PATTERN).Matcher(pole_no).Matches())
            //{
            //    this.mView.ShowInvalidPoleNoError();
            //    this.mView.DisableSubmitButton();
            //    return;
            //}

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                //this.mView.ShowEmptyFeedbackError();
                this.mView.DisableSubmitButton();
                return;
            }


            //if (string.IsNullOrEmpty(state)) {
            //    this.mView.ShowEmptyStateError();
            //    this.mView.DisableSubmitButton();
            //    return;
            //}

                this.mView.EnableSubmitButton();
        }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }


        public async void OnLocationRequest(Geocoder geoCoder)
        {
            //TODO IMPLEMENT ASYNC GETTING COORDINATES AND CONVERT TO REVERSE GEOCODE
            string geocodedString = string.Empty;
            var currentPosition = await GetCurrentLocation();
            try
            {
                if (currentPosition != null)
                {
                    var addresses = await geoCoder.GetFromLocationAsync(currentPosition.Latitude, currentPosition.Longitude, 1);
                    if (addresses.Any())
                    {
                        geocodedString = addresses[0].GetAddressLine(0);
                        if (!TextUtils.IsEmpty(geocodedString))
                        {
                            this.mView.ShowGeocodedLocation(geocodedString);
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public async void OnLocationRequest(string reverseKey)
        {
            var reverseGecoderApi = RestService.For<IFeedbackPreloginFaultyStreetLampsApi>("https://maps.googleapis.com");
            string geocodedString = string.Empty;
            var currentPosition = await GetCurrentLocation();
            try
            {
                if (currentPosition != null)
                {
                    var reverseGeocoderResponse = await reverseGecoderApi.ReverseGeocode(reverseKey , $"{currentPosition.Latitude},{currentPosition.Longitude}");
                    if (reverseGeocoderResponse.Status.Equals("OK"))
                    {
                        if (reverseGeocoderResponse.Result.Any())
                        {
                            geocodedString = reverseGeocoderResponse.Result[0].FormattedAddress;
                            if (!TextUtils.IsEmpty(geocodedString) && this.mView.IsActive())
                            {
                                this.mView.ShowGeocodedLocation(geocodedString);
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }
    }
}