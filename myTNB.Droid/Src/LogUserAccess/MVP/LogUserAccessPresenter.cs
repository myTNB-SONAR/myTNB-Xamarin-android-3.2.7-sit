using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.LogUserAccess.MVP
{
    public class LogUserAccessPresenter : LogUserAccessContract.IUserActionsListener
    {

        private LogUserAccessContract.IView mView;
        private static DateTime ThisweekStartDate;
        private static DateTime ThisweekEndDate;


        public LogUserAccessPresenter(LogUserAccessContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public static bool DateInsideOneWeek(DateTime date1)
        {
            DayOfWeek firstDayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime startDateOfWeek = date1.Date;
            
            DateTime NewstartDateOfWeek = DateTime.Now;
            DateTime date2;
            DateTime referenceDate;
            DayOfWeek referenceDateDay;
            DateTime datadate = new DateTime();

            referenceDateDay = datadate.DayOfWeek;
            referenceDate = DateTime.Now;

            while (startDateOfWeek.DayOfWeek != firstDayOfWeek)
            { 
                startDateOfWeek = startDateOfWeek.AddDays(-1d); 
            }
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            while (NewstartDateOfWeek.DayOfWeek != firstDayOfWeek)
            {
                NewstartDateOfWeek = NewstartDateOfWeek.AddDays(-1d);
            }
            DateTime NewendDateOfWeek = NewstartDateOfWeek.AddDays(6d);

            if (referenceDate >= startDateOfWeek && referenceDate <= endDateOfWeek)
            {
                ThisweekStartDate = startDateOfWeek;
                ThisweekEndDate = endDateOfWeek;
            }
            else if ((referenceDate >= NewstartDateOfWeek && referenceDate <= NewendDateOfWeek))
            {
                ThisweekStartDate = NewstartDateOfWeek;
                ThisweekEndDate = NewendDateOfWeek;
            }
            return referenceDate >= startDateOfWeek && referenceDate <= endDateOfWeek;
        }

        public static bool DateInsideLastWeek(DateTime date1)
        {
            DayOfWeek firstDayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime startDateOfWeek = date1.Date;
            DateTime date2;
            DateTime referenceDate;
            DayOfWeek referenceDateDay;
            DateTime datadate = new DateTime();

            referenceDateDay = datadate.DayOfWeek;
            referenceDate = DateTime.Now;

            while (startDateOfWeek.DayOfWeek != firstDayOfWeek)
            {
                startDateOfWeek = startDateOfWeek.AddDays(-1d);
            }
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            ThisweekStartDate = ThisweekStartDate.AddDays(-1d);

            if (ThisweekStartDate.Day == endDateOfWeek.Day)
            {
                return date1 >= startDateOfWeek && date1 <= endDateOfWeek;
            }
            else
            {
                return false;
            }
        }

        public static bool DateInsideLastMonth(DateTime date1)
        {
            DateTime referenceDate;
            DayOfWeek referenceDateDay;
            DateTime datadate = new DateTime();
            referenceDate = DateTime.Now;

            datadate = referenceDate.AddMonths(-1);

            if (datadate.Month == date1.Month)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SortLogListDataByDate(List<LogUserAccessNewData> loglist)
        {
            try
            {
                this.mView.ShowProgressDialog();

                List<LogUserAccessNewData> thisWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastMonth = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> newloglist = new List<LogUserAccessNewData>();

                /*if (loglist.Count > 15)
                {
                    newloglist = loglist.GetRange(0, 15);
                    this.mView.FirstLoadData(15);
                }
                else
                {
                    newloglist = loglist;
                }*/

                foreach (LogUserAccessNewData entity in loglist)
                {
                    if (DateInsideOneWeek(entity.CreatedDate.Date))
                    {
                        thisWeek.Add(entity);
                    }
                    else if (DateInsideLastWeek(entity.CreatedDate.Date))
                    {
                        lastWeek.Add(entity);
                    }
                    else if (DateInsideLastMonth(entity.CreatedDate.Date))
                    {
                        lastMonth.Add(entity);
                    }
                }

                if (thisWeek.Count > 0)
                {
                    this.mView.ShowLogListThisWeek(thisWeek);
                }
                else
                {
                    this.mView.emptyThisWeekList();
                }

                if (lastWeek.Count > 0)
                {
                    this.mView.ShowLogListLastWeek(lastWeek);
                }
                else
                {
                    this.mView.emptyLastWeekList();
                }

                if (lastMonth.Count > 0)
                {
                    this.mView.ShowLogList(lastMonth);
                }
                else
                {
                    this.mView.emptyLastMonthList();
                }

                this.mView.HideShowProgressDialog();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            //throw new NotImplementedException();
        }
    }
}