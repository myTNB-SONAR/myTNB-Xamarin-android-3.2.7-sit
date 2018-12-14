using System;
using Foundation;

namespace myTNB.Home.Feedback
{
    public static class FeedbackFileNameHelper
    {
        static int imgCount = 0;

        static void SaveImageCount(int count)
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetInt(count, "ImageCount");
            sharedPreference.Synchronize();
        }

        static void SaveImageDate(string imgDate)
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetString(imgDate, "ImageDate");
            sharedPreference.Synchronize();
        }

        static int GetImageCount()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            int count = (int)sharedPreference.IntForKey("ImageCount");
            return count;
        }

        static string GetImageDate()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            string imgDate = sharedPreference.StringForKey("ImageDate");
            return imgDate;
        }

        static bool IsSameDate(string current, string stored)
        {
            bool isSame = stored.Equals(current);
            return isSame;
        }

        static string GetCurrentDate()
        {
            NSDate date = new NSDate();
            string dateString = date.ToString().Split(' ')[0];
            string formattedDate = GetFormattedDate(dateString);
            return formattedDate;
        }

        static string GetFormattedDate(string dateString)
        {
            int year = int.Parse(dateString.Split('-')[0]);
            int month = int.Parse(dateString.Split('-')[1]);
            int day = int.Parse(dateString.Split('-')[2]);
            DateTime dateTime = new DateTime(year, month, day);
            string formattedDate = dateTime.ToString("yyyyMMdd");
            return formattedDate;
        }

        public static string GenerateFileName()
        {
            string imgDate = GetImageDate();

            if (string.IsNullOrEmpty(imgDate))
            {
                imgDate = GetCurrentDate();
                SaveImageDate(imgDate);
                if (imgCount > 0)
                {
                    imgCount = 0;
                }
                imgCount++;
            }
            else
            {
                string currentDate = GetCurrentDate();
                bool isSame = IsSameDate(currentDate, imgDate);
                if (isSame)
                {
                    imgCount = GetImageCount();
                    imgCount++;
                }
                else
                {
                    imgDate = currentDate;
                    SaveImageDate(imgDate);
                    if (imgCount > 0)
                    {
                        imgCount = 0;
                    }
                    imgCount++;
                }
            }
            SaveImageCount(imgCount);
            string fileName = string.Format("{0}-MYTNB-{1}.jpeg", imgDate, imgCount);
            return fileName;
        }
    }
}