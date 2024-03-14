using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
using Java.Text;
using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Base.Api;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Feedback_Login_FaultyStreetLamps.Api;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.Feedback_Login_FaultyStreetLamps.MVP
{
    class FeedbackLoginFaultyStreetLampsPresenter : FeedbackLoginFaultyStreetLampsContract.IUserActionsListener
    {

        FeedbackLoginFaultyStreetLampsContract.IView mView;
        CancellationTokenSource cts;


        public FeedbackLoginFaultyStreetLampsPresenter(FeedbackLoginFaultyStreetLampsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        const string ALPHANUMERIC_PATTERN = "[a-zA-Z0-9\\s]*";

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
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
                    }
                    else
                    {
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
            try
            {
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
            try
            {
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

        public async void OnSubmit(string deviceId, FeedbackState feedbackState, string location, string pole_no, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();


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


            if (feedbackState == null)
            {
                this.mView.ShowEmptyStateError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("2", "", "", userEntity.DisplayName, userEntity.MobileNo, feedback, feedbackState.StateId, location, pole_no);
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.IsSuccessResponse())
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.GetData().ServiceReqNo,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "2"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    FeedbackStateEntity entity = FeedbackStateEntity.GetFirstOrSelected();
                    this.mView.ShowState(FeedbackState.Copy(entity));
                    this.mView.ShowSuccess(preLoginFeedbackResponse.GetData().DateCreated, preLoginFeedbackResponse.GetData().ServiceReqNo, attachedImages.Count);
                }
                else
                {
                    //this.mView.ShowFail();
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Response.DisplayMessage);
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
            try
            {
                // TODO : SHOW FIRST STATE
                this.mView.DisableSubmitButton();
                FeedbackStateEntity.RemoveActive();
                //FeedbackStateEntity.ResetSelected();
                //FeedbackStateEntity entity = FeedbackStateEntity.GetFirstOrSelected();
                //FeedbackStateEntity.SetSelected(entity.Id);
                //this.mView.ShowState(FeedbackState.Copy(entity));

                UserEntity userEntity = UserEntity.GetActive();
                if (TextUtils.IsEmpty(userEntity.MobileNo))
                {
                    this.mView.ShowMobileNo();
                }
                else
                {
                    this.mView.HideMobileNo();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSelectFeedbackState()
        {
            this.mView.ShowSelectFeedbackState();
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

        public void CheckRequiredFields(string location, string feedback, string state)
        {
            //if (!TextUtils.IsEmpty(location) && !TextUtils.IsEmpty(feedback))
            //{


            this.mView.ClearErrors();


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
            //    return;
            //}

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                //this.mView.ShowEmptyFeedbackError();
                this.mView.DisableSubmitButton();
                return;
            }


            //if (string.IsNullOrEmpty(state))
            //{
            //    this.mView.ShowEmptyStateError();
            //    this.mView.DisableSubmitButton();
            //    return;
            //}

            this.mView.EnableSubmitButton();
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }

        public async void OnLocationRequest(string reverseKey)
        {
            var reverseGecoderApi = RestService.For<IFeedbackLoginFaultyStreetLampsApi>("https://maps.googleapis.com");
            string geocodedString = string.Empty;
            var currentPosition = await GetCurrentLocation();
            try
            {
                if (currentPosition != null)
                {
                    var reverseGeocoderResponse = await reverseGecoderApi.ReverseGeocode(reverseKey, $"{currentPosition.Latitude},{currentPosition.Longitude}");
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

        public async void OnSubmit(string deviceId, string mobile_no, FeedbackState feedbackState, string location, string pole_no, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();

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

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                return;
            }


            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }

            if (feedbackState == null)
            {
                this.mView.ShowEmptyStateError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("2", "", "", userEntity.DisplayName, mobile_no, feedback, feedbackState.StateId, location, pole_no);
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.IsSuccessResponse())
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.GetData().ServiceReqNo,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "2"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    FeedbackStateEntity entity = FeedbackStateEntity.GetFirstOrSelected();
                    this.mView.ShowState(FeedbackState.Copy(entity));
                    this.mView.ShowSuccess(preLoginFeedbackResponse.GetData().DateCreated, preLoginFeedbackResponse.GetData().ServiceReqNo, attachedImages.Count);
                }
                else
                {
                    //this.mView.ShowFail();
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Response.DisplayMessage);
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

        public void CheckRequiredFields(string mobile_no, string location, string feedback, string state)
        {
            //if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(location) && !TextUtils.IsEmpty(feedback))
            //{
            this.mView.ClearErrors();
            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowEmptyMobileNoError();
                return;
            }

            if (TextUtils.IsEmpty(location))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowEmptyLocationError();
                return;
            }

            if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowInvalidMobileNoError();
                return;
            }
            else
            {
                this.mView.ClearMobileNoError();
            }

            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowInvalidMobileNoError();
                return;
            }
            else
            {
                this.mView.ClearMobileNoError();
            }


            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.DisableSubmitButton();
                //this.mView.ShowEmptyFeedbackError();
                return;
            }


            //if (string.IsNullOrEmpty(state)) {
            //    this.mView.ShowEmptyStateError();
            //    this.mView.DisableSubmitButton();
            //    return;
            //}

            this.mView.EnableSubmitButton();
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }




    }
}
