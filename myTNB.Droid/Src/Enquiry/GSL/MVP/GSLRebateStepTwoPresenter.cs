using System;
using System.Collections.Generic;
using System.Globalization;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepTwoPresenter : GSLRebateStepTwoContract.IUserActionsListener
    {
        private readonly GSLRebateStepTwoContract.IView view;

        private GSLRebateModel rebateModel;

        public GSLRebateStepTwoPresenter(GSLRebateStepTwoContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            OnInit();
            this.view?.UpdateButtonState(false);
            this.view?.SetUpViews();
        }

        private void OnInit()
        {
            this.rebateModel = new GSLRebateModel
            {
                IncidentList = new List<GSLRebateIncidentModel>()
            };
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
            this.rebateModel.IncidentList = new List<GSLRebateIncidentModel>
            {
                new GSLRebateIncidentModel()
            };
        }

        public void SetIncidentData(GSLIncidentDateTimePicker picker, DateTime dateTime, int index)
        {
            if (this.rebateModel.IncidentList.Count > 0 && this.rebateModel.IncidentList.Count > index)
            {
                try
                {
                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                    switch (picker)
                    {
                        case GSLIncidentDateTimePicker.INCIDENT_DATE:
                            this.rebateModel.IncidentList[index].IncidentDateTime = dateTime.ToString(GSLRebateConstants.DATETIME_PARSE_FORMAT, dateCultureInfo);
                            break;
                        case GSLIncidentDateTimePicker.INCIDENT_TIME:
                            this.rebateModel.IncidentList[index].IncidentDateTime = dateTime.ToString(GSLRebateConstants.DATETIME_PARSE_FORMAT, dateCultureInfo);
                            break;
                        case GSLIncidentDateTimePicker.RESTORATION_DATE:
                            this.rebateModel.IncidentList[index].RestorationDateTime = dateTime.ToString(GSLRebateConstants.DATETIME_PARSE_FORMAT, dateCultureInfo);
                            break;
                        case GSLIncidentDateTimePicker.RESTORATION_TIME:
                            this.rebateModel.IncidentList[index].RestorationDateTime = dateTime.ToString(GSLRebateConstants.DATETIME_PARSE_FORMAT, dateCultureInfo);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            this.view?.UpdateButtonState(CheckDateTimeFields());
        }

        public void ResetIncidentData(GSLIncidentDateTimePicker picker, int index)
        {
            if (this.rebateModel.IncidentList.Count > 0 && this.rebateModel.IncidentList.Count > index)
            {
                switch (picker)
                {
                    case GSLIncidentDateTimePicker.INCIDENT_DATE:
                        this.rebateModel.IncidentList[index].IncidentDateTime = string.Empty;
                        break;
                    case GSLIncidentDateTimePicker.INCIDENT_TIME:
                        this.rebateModel.IncidentList[index].IncidentDateTime = string.Empty;
                        break;
                    case GSLIncidentDateTimePicker.RESTORATION_DATE:
                        this.rebateModel.IncidentList[index].RestorationDateTime = string.Empty;
                        break;
                    case GSLIncidentDateTimePicker.RESTORATION_TIME:
                        this.rebateModel.IncidentList[index].RestorationDateTime = string.Empty;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool CheckDateTimeFields()
        {
            try
            {
                return this.rebateModel.IncidentList[0].IncidentDateTime.IsValid() && this.rebateModel.IncidentList[0].RestorationDateTime.IsValid();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return false;
            }
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }
    }
}
