using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Models.Scheduler.GetAvailableAppointment;

namespace myTNB.Mobile.API.Managers.Scheduler.Utilities
{
    internal static class SchedulerUtility
    {
        internal static SchedulerDisplay Parse(this GetAvailableAppointmentResponse response)
        {
            SchedulerDisplay display = new SchedulerDisplay
            {
                StatusDetail = response.StatusDetail
            };
            if (response.Content == null || response.Content.Count < 1)
            {
                return display;
            }
            try
            {
                Dictionary<string, List<SchedulerDisplayModel>> scheduleDictionary = new Dictionary<string, List<SchedulerDisplayModel>>();
                List<MonthYearDisplayModel> monthYearDisplay = new List<MonthYearDisplayModel>();
                for (int i = 0; i < response.Content.Count; i++)
                {
                    SchedulerDisplayModel schedule = new SchedulerDisplayModel();
                    GetAvailableAppointmentModel item = response.Content[i];

                    schedule.AppointmentDate = item.AppointmentDate;
                    schedule.IsAvailable = true;
                    schedule.TimeSlotDisplay = item.AppointmentTimeSlot.Select(x => new AppointmentTimeSlotDisplay
                    {
                        SlotStartTime = x.SlotStartTime,
                        SlotEndTime = x.SlotEndTime,
                        AvailableSlotCount = x.AvailableSlotCount
                    }).ToList();

                    if (scheduleDictionary.ContainsKey(schedule.DateTitleDisplay))
                    {
                        List<SchedulerDisplayModel> list = scheduleDictionary[schedule.DateTitleDisplay];
                        list.Add(schedule);
                        scheduleDictionary[schedule.DateTitleDisplay] = list;
                    }
                    else
                    {
                        scheduleDictionary.Add(schedule.DateTitleDisplay, new List<SchedulerDisplayModel> { schedule });
                        monthYearDisplay.Add(new MonthYearDisplayModel
                        {
                            Month = int.Parse(schedule.MonthOrder),
                            Year = int.Parse(schedule.YearDisplay),
                            MonthYearDisplay = schedule.DateTitleDisplay
                        });
                    }
                }
                UpdateWithMissingDays(ref scheduleDictionary);
                display.ScheduleList = scheduleDictionary;
                display.MonthYearList = monthYearDisplay;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] SchedulerDisplay Error 0: " + e.Message);
            }
            return display;
        }

        /// <summary>
        /// Add Missing Dates as response only returns available dates.
        /// </summary>
        /// <param name="scheduleDictionary"></param>
        private static void UpdateWithMissingDays(ref Dictionary<string, List<SchedulerDisplayModel>> scheduleDictionary)
        {
            if (scheduleDictionary == null || scheduleDictionary.Count == 0)
            {
                return;
            }
            try
            {
                List<string> keys = scheduleDictionary.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    List<SchedulerDisplayModel> scheduleList = scheduleDictionary[keys[i]];
                    if (scheduleList == null || scheduleList.Count == 0)
                    {
                        continue;
                    }

                    SchedulerDisplayModel fisrtItem = scheduleList[0];

                    int numberOfDays = DateTime.DaysInMonth(int.Parse(fisrtItem.YearDisplay)
                        , int.Parse(fisrtItem.MonthOrder));

                    List<SchedulerDisplayModel> dateList = new List<SchedulerDisplayModel>();
                    for (int j = 1; j <= numberOfDays; j++)
                    {
                        SchedulerDisplayModel schedule = new SchedulerDisplayModel();
                        int index = scheduleList.FindIndex(x => x.Day == j.ToString());
                        if (index > -1)
                        {
                            schedule = scheduleList[index];
                        }
                        else
                        {
                            schedule = new SchedulerDisplayModel
                            {
                                IsAvailable = false,
                                AppointmentDate = new DateTime(int.Parse(fisrtItem.YearDisplay)
                                    , int.Parse(fisrtItem.MonthOrder)
                                    , j)
                            };
                        }
                        dateList.Add(schedule);
                    }
                    scheduleDictionary[keys[i]] = dateList;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] UpdateWithMissingDays Error: " + e.Message);
            }
        }
    }
}