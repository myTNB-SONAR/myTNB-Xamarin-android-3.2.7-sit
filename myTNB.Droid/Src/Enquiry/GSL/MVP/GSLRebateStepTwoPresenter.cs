using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
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
            this.view?.SetUpViews();
        }

        private void OnInit()
        {
            this.rebateModel = new GSLRebateModel();
            this.rebateModel.IncidentList = new List<GSLRebateIncidentModel>();
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
        }

        public void SetIncidentData(GSLIncidentDateTimePicker picker, DateTime dateTime)
        {
            switch (picker)
            {
                case GSLIncidentDateTimePicker.INCIDENT_DATE:
                    System.Console.WriteLine("INCIDENT_DATE** " + dateTime.ToShortDateString());
                    break;
                case GSLIncidentDateTimePicker.INCIDENT_TIME:
                    System.Console.WriteLine("INCIDENT_TIME** " + dateTime.ToShortDateString());
                    break;
                case GSLIncidentDateTimePicker.RESTORATION_DATE:
                    System.Console.WriteLine("RESTORATION_DATE** " + dateTime.ToShortDateString());
                    break;
                case GSLIncidentDateTimePicker.RESTORATION_TIME:
                    System.Console.WriteLine("RESTORATION_TIME** " + dateTime.ToShortDateString());
                    break;
                default:
                    break;
            }
        }
    }
}
