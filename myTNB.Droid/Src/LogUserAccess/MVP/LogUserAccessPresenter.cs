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
            DateTime startDateSat = new DateTime();
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

                if (referenceDate.DayOfWeek.ToString().Contains("Sunday"))
                {
                    startDate = referenceDate;
                    startDateSun = referenceDate.AddDays(-1);
                    if ((startDate.Day == periods.Day) && startDate.Month == periods.Month)
                    {
                        return true;
                    }
                    else if ((startDateSun.Day == periods.Day) && startDateSun.Month == periods.Month)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Monday"))
                {
                    startDate = referenceDate;
                    startDateSun = referenceDate.AddDays(-1);

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
                else if(referenceDate.DayOfWeek.ToString().Contains("Tuesday"))
                {
                    startDate = referenceDate;
                    startDateMon = referenceDate.AddDays(-1);
                    startDateSun = referenceDate.AddDays(-2);

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
                else if(referenceDate.DayOfWeek.ToString().Contains("Wednesday"))
                {
                    startDate = referenceDate;
                    startDateTue = referenceDate.AddDays(-1);
                    startDateMon = referenceDate.AddDays(-2);
                    startDateSun = referenceDate.AddDays(-3);

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
                else if(referenceDate.DayOfWeek.ToString().Contains("Thursday"))
                {
                    startDate = referenceDate;
                    startDateWed = referenceDate.AddDays(-1);
                    startDateTue = referenceDate.AddDays(-2);
                    startDateMon = referenceDate.AddDays(-3);
                    startDateSun = referenceDate.AddDays(-4);

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
                else if(referenceDate.DayOfWeek.ToString().Contains("Friday"))
                {
                    startDate = referenceDate;
                    startDateThu = referenceDate.AddDays(-1);
                    startDateWed = referenceDate.AddDays(-2);
                    startDateTue = referenceDate.AddDays(-3);
                    startDateMon = referenceDate.AddDays(-4);
                    startDateSun = referenceDate.AddDays(-5);

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
                    startDateFri = referenceDate.AddDays(-1);
                    startDateThu = referenceDate.AddDays(-2);
                    startDateWed = referenceDate.AddDays(-3);
                    startDateTue = referenceDate.AddDays(-4);
                    startDateMon = referenceDate.AddDays(-5);
                    startDateSun = referenceDate.AddDays(-6);
                    
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

                if (referenceDate.DayOfWeek.ToString().Contains("Sunday"))
                {
                    startDateSat = referenceDate.AddDays(-1);
                    startDateFri = referenceDate.AddDays(-2);
                    startDateThu = referenceDate.AddDays(-3);
                    startDateWed = referenceDate.AddDays(-4);
                    startDateTue = referenceDate.AddDays(-5);
                    startDateMon = referenceDate.AddDays(-6);
                    startDateSun = referenceDate.AddDays(-7);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Monday"))
                {
                    startDateSat = referenceDate.AddDays(-2);
                    startDateFri = referenceDate.AddDays(-3);
                    startDateThu = referenceDate.AddDays(-4);
                    startDateWed = referenceDate.AddDays(-5);
                    startDateTue = referenceDate.AddDays(-6);
                    startDateMon = referenceDate.AddDays(-7);
                    startDateSun = referenceDate.AddDays(-8);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Tuesday"))
                {
                    startDateSat = referenceDate.AddDays(-3);
                    startDateFri = referenceDate.AddDays(-4);
                    startDateThu = referenceDate.AddDays(-5);
                    startDateWed = referenceDate.AddDays(-6);
                    startDateTue = referenceDate.AddDays(-7);
                    startDateMon = referenceDate.AddDays(-8);
                    startDateSun = referenceDate.AddDays(-9);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Wednesday"))
                {
                    startDateSat = referenceDate.AddDays(-4);
                    startDateFri = referenceDate.AddDays(-5);
                    startDateThu = referenceDate.AddDays(-6);
                    startDateWed = referenceDate.AddDays(-7);
                    startDateTue = referenceDate.AddDays(-8);
                    startDateMon = referenceDate.AddDays(-9);
                    startDateSun = referenceDate.AddDays(-10);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }
                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Thursday"))
                {
                    startDateSat = referenceDate.AddDays(-5);
                    startDateFri = referenceDate.AddDays(-6);
                    startDateThu = referenceDate.AddDays(-7);
                    startDateWed = referenceDate.AddDays(-8);
                    startDateTue = referenceDate.AddDays(-9);
                    startDateMon = referenceDate.AddDays(-10);
                    startDateSun = referenceDate.AddDays(-11);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }

                }
                else if (referenceDate.DayOfWeek.ToString().Contains("Friday"))
                {
                    startDateSat = referenceDate.AddDays(-6);
                    startDateFri = referenceDate.AddDays(-7);
                    startDateThu = referenceDate.AddDays(-8);
                    startDateWed = referenceDate.AddDays(-9);
                    startDateTue = referenceDate.AddDays(-10);
                    startDateMon = referenceDate.AddDays(-11);
                    startDateSun = referenceDate.AddDays(-12);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    startDateSat = referenceDate.AddDays(-7);
                    startDateFri = referenceDate.AddDays(-8);
                    startDateThu = referenceDate.AddDays(-9);
                    startDateWed = referenceDate.AddDays(-10);
                    startDateTue = referenceDate.AddDays(-11);
                    startDateMon = referenceDate.AddDays(-12);
                    startDateSun = referenceDate.AddDays(-13);

                    if ((startDateSat.Day == periods.Day) && startDateSat.Month == periods.Month)
                    {
                        return true;
                    }
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