using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Managers.Scheduler.Utilities;
using myTNB.Mobile.API.Models.Scheduler.GetAvailableAppointment;
using myTNB.Mobile.API.Models.Scheduler.PostSetAppointment;
using myTNB.Mobile.API.Services.Scheduler;
using myTNB.Mobile.Extensions;
using Refit;

namespace myTNB.Mobile.API.Managers.Scheduler
{
    public sealed class ScheduleManager
    {
        private static readonly Lazy<ScheduleManager> lazy =
             new Lazy<ScheduleManager>(() => new ScheduleManager());

        public static ScheduleManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public ScheduleManager() { }

        #region GetAvailableAppointment
        /// <summary>
        /// Get All Available Appointment
        /// </summary>
        /// <param name="businessArea">Business Area Property from Appliciation Details</param>
        /// <returns></returns>
        public async Task<SchedulerDisplay> GetAvailableAppointment(string businessArea)
        {
            SchedulerDisplay display = new SchedulerDisplay();
            try
            {
                ISchedulerService service = RestService.For<ISchedulerService>(Constants.ApiDomain);
                try
                {
                    GetAvailableAppointmentResponse response = await service.GetAvailableAppointment(AppInfoManager.Instance.GetUserInfo()
                        , businessArea
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    if (response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_GetAvailableAppointment.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetAvailableAppointmentResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_GetAvailableAppointment.GetStatusDetails(Constants.DEFAULT);
                    }

                    if (response != null && response.StatusDetail != null && response.StatusDetail.IsSuccess)
                    {
                        display = response.Parse();
                        if (display.ScheduleList == null || display.ScheduleList.Count == 0)
                        {
                            display.StatusDetail = Constants.Service_GetAvailableAppointment.GetStatusDetails(Constants.EMPTY);
                        }
                    }
                    return display;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetAvailableAppointment]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetAvailableAppointment]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            display = new SchedulerDisplay
            {
                StatusDetail = new StatusDetail()
            };
            display.StatusDetail = Constants.Service_GetAvailableAppointment.GetStatusDetails(Constants.DEFAULT);
            return display;
        }
        #endregion

        #region SetAppointment
        /// <summary>
        /// Sets application appointment
        /// </summary>
        /// <param name="applicationID"></param>
        /// <param name="applicationType"></param>
        /// <param name="srNo"></param>
        /// <param name="srType"></param>
        /// <param name="businessArea"></param>
        /// <param name="appointmentDate"></param>
        /// <param name="appointmentStartTime"></param>
        /// <param name="appointmentEndTime"></param>
        /// <returns></returns>
        public async Task<PostSetAppointmentResponse> SetAppointment(string applicationID
            , string applicationType
            , string srNo
            , string srType
            , string businessArea
            , DateTime appointmentDate
            , DateTime appointmentStartTime
            , DateTime appointmentEndTime)
        {
            try
            {
                ISchedulerService service = RestService.For<ISchedulerService>(Constants.ApiDomain);
                try
                {
                    PostSetAppointmentRequest request = new PostSetAppointmentRequest
                    {
                        SetAppointment = new SetAppointment
                        {
                            ApplicationId = applicationID,
                            ApplicationType = applicationType,
                            SrNo = srNo,
                            SrType = srType,
                            BusinessArea = businessArea,
                            AppointmentDate = appointmentDate,
                            AppointmentStartTime = appointmentStartTime,
                            AppointmentEndTime = appointmentEndTime
                        }
                    };

                    HttpResponseMessage rawResponse = await service.SetAppointment(request
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());

                    PostSetAppointmentResponse response = await rawResponse.ParseAsync<PostSetAppointmentResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_PostSetAppointment.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostSetAppointmentResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_PostSetAppointment.GetStatusDetails(Constants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SetAppointment]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SetAppointment]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostSetAppointmentResponse res = new PostSetAppointmentResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_PostSetAppointment.GetStatusDetails(Constants.DEFAULT);
            return res;
        }
        #endregion
    }
}