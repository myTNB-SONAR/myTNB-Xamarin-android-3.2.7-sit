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

        public LogUserAccessPresenter(LogUserAccessContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public bool GetPeriodsBool(DateTime periods, string checkSelection)
        {

            DateTime referenceDate;
            DayOfWeek referenceDateDay;
            DateTime datadate = new DateTime();
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DateTime startDateFri = new DateTime();
            DateTime startDateWed = new DateTime();
            DateTime startDateTue = new DateTime();
            DateTime startDateMon = new DateTime();
            DateTime startDateSun = new DateTime();
            DateTime startDateThu = new DateTime();

            string dateString = string.Empty;


            if (checkSelection.Equals("thisweek"))
            {
                datadate = periods;
                referenceDateDay = datadate.DayOfWeek;
                referenceDate = DateTime.Now;

                if (referenceDateDay.ToString().Contains("Sunday"))
                {
                    startDate = referenceDate;
                    if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDateDay.ToString().Contains("Monday"))
                {
                    startDate = referenceDate;
                    startDateSun = referenceDate.AddDays(-2);

                    if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(referenceDateDay.ToString().Contains("Tuesday"))
                {
                    startDate = referenceDate;
                    startDateMon = referenceDate.AddDays(-2);
                    startDateSun = referenceDate.AddDays(-3);

                    if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(referenceDateDay.ToString().Contains("Wednesday"))
                {
                    startDate = referenceDate;
                    startDateTue = referenceDate.AddDays(-2);
                    startDateMon = referenceDate.AddDays(-3);
                    startDateSun = referenceDate.AddDays(-4);

                    if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(referenceDateDay.ToString().Contains("Thursday"))
                {
                    startDate = referenceDate;
                    startDateWed = referenceDate.AddDays(-2);
                    startDateTue = referenceDate.AddDays(-3);
                    startDateMon = referenceDate.AddDays(-4);
                    startDateSun = referenceDate.AddDays(-5);

                    if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(referenceDateDay.ToString().Contains("Friday"))
                {
                    startDate = referenceDate;
                    startDateThu = referenceDate.AddDays(-2);
                    startDateWed = referenceDate.AddDays(-3);
                    startDateTue = referenceDate.AddDays(-4);
                    startDateMon = referenceDate.AddDays(-5);
                    startDateSun = referenceDate.AddDays(-6);

                    if ((startDateThu.Day == periods.Day) && startDateThu.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    startDate = referenceDate;
                    startDateFri = referenceDate.AddDays(-2);
                    startDateThu = referenceDate.AddDays(-3);
                    startDateWed = referenceDate.AddDays(-4);
                    startDateTue = referenceDate.AddDays(-5);
                    startDateMon = referenceDate.AddDays(-6);
                    startDateSun = referenceDate.AddDays(-7);
                    
                    if ((startDateFri.Day == periods.Day) && startDateFri.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateThu.Day == periods.Day) && startDateThu.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (checkSelection.Equals("lastweek"))
            {
                datadate = periods;
                referenceDateDay = datadate.DayOfWeek;
                referenceDate = DateTime.Now;

                if (referenceDateDay.ToString().Contains("Sunday"))
                {
                    startDate = referenceDate;
                    if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDateDay.ToString().Contains("Monday"))
                {
                    startDate = referenceDate;
                    startDateSun = referenceDate.AddDays(-9);

                    if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDateDay.ToString().Contains("Tuesday"))
                {
                    startDate = referenceDate;
                    startDateMon = referenceDate.AddDays(-9);
                    startDateSun = referenceDate.AddDays(-10);

                    if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDateDay.ToString().Contains("Wednesday"))
                {
                    startDate = referenceDate;
                    startDateTue = referenceDate.AddDays(-9);
                    startDateMon = referenceDate.AddDays(-10);
                    startDateSun = referenceDate.AddDays(-11);

                    if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDateDay.ToString().Contains("Thursday"))
                {
                    startDate = referenceDate;
                    startDateWed = referenceDate.AddDays(-9);
                    startDateTue = referenceDate.AddDays(-10);
                    startDateMon = referenceDate.AddDays(-11);
                    startDateSun = referenceDate.AddDays(-12);

                    if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (referenceDateDay.ToString().Contains("Friday"))
                {
                    startDate = referenceDate;
                    startDateThu = referenceDate.AddDays(-9);
                    startDateWed = referenceDate.AddDays(-10);
                    startDateTue = referenceDate.AddDays(-11);
                    startDateMon = referenceDate.AddDays(-12);
                    startDateSun = referenceDate.AddDays(-13);

                    if ((startDateThu.Day == periods.Day) && startDateThu.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    startDate = referenceDate;
                    startDateFri = referenceDate.AddDays(-9);
                    startDateThu = referenceDate.AddDays(-10);
                    startDateWed = referenceDate.AddDays(-11);
                    startDateTue = referenceDate.AddDays(-12);
                    startDateMon = referenceDate.AddDays(-13);
                    startDateSun = referenceDate.AddDays(-14);

                    if ((startDateFri.Day == periods.Day) && startDateFri.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateThu.Day == periods.Day) && startDateThu.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateWed.Day == periods.Day) && startDateWed.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateTue.Day == periods.Day) && startDateTue.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateMon.Day == periods.Day) && startDateMon.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else 
            {
                datadate = periods;
                referenceDate = DateTime.Now;

                startDate = referenceDate.AddMonths(-1);

                if (startDate.Month == periods.Month)
                {
                    return true;
                }
                /*else if (startDate.Month != periods.Month)
                {
                    return true;
                }*/
                else
                {
                    return false;
                }
            }            
        }

        public void SortLogListDataByDate(List<LogUserAccessNewData> loglist)
        {
            try
            {

                List<LogUserAccessNewData> thisWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastMonth = new List<LogUserAccessNewData>();

                foreach (LogUserAccessNewData entity in loglist)
                {
                    if (GetPeriodsBool(entity.CreatedDate.Date, "thisweek"))
                    {
                        thisWeek.Add(entity);
                    }
                    else if (GetPeriodsBool(entity.CreatedDate.Date, "lastweek"))
                    {
                        lastWeek.Add(entity);
                    }
                    else if (GetPeriodsBool(entity.CreatedDate.Date, "lastmonth"))
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}