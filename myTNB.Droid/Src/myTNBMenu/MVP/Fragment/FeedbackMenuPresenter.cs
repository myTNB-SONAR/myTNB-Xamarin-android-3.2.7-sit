using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class FeedbackMenuPresenter : FeedbackMenuContract.IUserActionsListener
    {
        FeedbackMenuContract.IView mView;
        CancellationTokenSource cts;

        public FeedbackMenuPresenter(FeedbackMenuContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void onSubmittedFeedbackNew()
        {
            this.mView.ShowSubmittedFeedbackNew();
        }

        public void OnBillingPayment()
        {
            this.mView.ShowBillingPayment();
        }

        public void OnSubmitNewEnquiry()
        {
            this.mView.ShowSubmitNewEnquiry();
        }

        public void OnFaultyStreetLamps()
        {
            this.mView.ShowFaultyStreetLamps();
        }

        public void OnOthers()
        {
            this.mView.ShowOthers();
        }


        public void OnResume()
        {
            int count = SubmittedFeedbackEntity.Count();
            this.mView.ShowSubmittedFeedbackCount(count);
        }

        public async void OnRetry()
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                string email = string.Empty;
                if (UserEntity.IsCurrentlyActive())
                {
                    email = UserEntity.GetActive().Email;
                }

                var submittedFeedbackResponse = await ServiceApiImpl.Instance.SubmittedFeedbackList(new SubmittedFeedbackListRequest());
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (submittedFeedbackResponse.IsSuccessResponse())
                {
                    SubmittedFeedbackEntity.Remove();
                    foreach (SubmittedFeedbackListResponse.ResponseData responseData in submittedFeedbackResponse.GetData())
                    {
                        SubmittedFeedback sf = new SubmittedFeedback();
                        sf.FeedbackId = responseData.ServiceReqNo;
                        sf.FeedbackCategoryId = responseData.FeedbackCategoryId;
                        sf.DateCreated = responseData.DateCreated;
                        sf.FeedbackMessage = responseData.FeedbackMessage;
                        sf.FeedbackCategoryName = responseData.FeedbackCategoryName;
                        sf.FeedbackNameInListView = responseData.FeedbackNameInListView;
                        sf.StatusDesc = responseData.StatusDesc;
                        sf.StatusCode = responseData.StatusCode;
                        SubmittedFeedbackEntity.InsertOrReplace(sf);
                    }

                    int count = SubmittedFeedbackEntity.Count();
                    this.mView.ShowSubmittedFeedbackCount(count);
                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

            }
            catch (System.OperationCanceledException e)
            {

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnSubmittedFeedback()
        {
            this.mView.ShowSubmittedFeedback();
        }

        public void Start()
        {
            try
            {
                if (FeedbackCategoryEntity.HasRecords())
                {
                    List<FeedbackCategoryEntity> feedbackCategoryList = FeedbackCategoryEntity.GetActiveList();
                    this.mView.ShowFeedbackMenu(feedbackCategoryList);
                }
                // TODO : 
                //            cts = new CancellationTokenSource();
                //            this.mView.ShowProgressDialog();
                //#if DEBUG
                //            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                //            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);
                //#else

                //            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
                //#endif
                //            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                //            try
                //            {
                //                string email = string.Empty;
                //                if (UserEntity.IsCurrentlyActive())
                //                {
                //                    email = UserEntity.GetActive().Email;
                //                }
                //                var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                //                {
                //                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                //                    Email = email,
                //                    DeviceId = this.mView.GetDeviceId()

                //                }, cts.Token);
                //                if (!submittedFeedbackResponse.Data.IsError)
                //                {
                //                    SubmittedFeedbackEntity.Remove();
                //                    foreach (SubmittedFeedback sf in submittedFeedbackResponse.Data.Data)
                //                    {
                //                        SubmittedFeedbackEntity.InsertOrReplace(sf);

                //                    }

                //                    int count = SubmittedFeedbackEntity.Count();
                //                    this.mView.ShowSubmittedFeedbackCount(count);
                //                }
                //                else
                //                {
                //                    this.mView.ShowRetryOptionsCancelledException(null);
                //                }

                //            }
                //            catch (System.OperationCanceledException e)
                //            {
                //                // ADD OPERATION CANCELLED HERE
                //                this.mView.ShowRetryOptionsCancelledException(e);
                //            }
                //            catch (ApiException apiException)
                //            {
                //                // ADD HTTP CONNECTION EXCEPTION HERE
                //                this.mView.ShowRetryOptionsApiException(apiException);
                //            }
                //            catch (Exception e)
                //            {
                //                // ADD UNKNOWN EXCEPTION HERE
                //                this.mView.ShowRetryOptionsUnknownException(e);
                //            }

                //            this.mView.HideProgressDialog();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public async void GetDownTime()
        {
           
            DSSTableResponse downTimeResponse = await ServiceApiImpl.Instance.GetDSSTableData(new DSSTableRequest());

            if (downTimeResponse != null && downTimeResponse.Response.ErrorCode != null)
            {
                if (downTimeResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    DownTimeEntity.RemoveActive();
                    foreach (DownTime cat in downTimeResponse.Response.data)
                    {
                      int newRecord = DownTimeEntity.InsertOrReplace(cat);
                    }

                    if (DownTimeEntity.IsBCRMDown())
                    {
                        this.mView.OnCheckBCRMDowntime();
                       
                    }
                    else
                    {

                        this.mView.RestartFeedbackMenu();
                        OnSubmitNewEnquiry();
                    }
                }
            }

           
        }
    }
}