using System;
using System.Globalization;

namespace myTNB
{
    public static class DateHelper
    {
        static char _separator = '/';
        static bool _isNotification = false;
        /// <summary>
        /// Gets the formatted date.
        /// </summary>
        /// <returns>The formatted date.</returns>
        /// <param name="dateString">Date string.</param>
        /// <param name="format">Format.</param>
        public static string GetFormattedDate(string dateString, string format)
        {
            return GetDate(dateString).ToString(format);
        }
        /// <summary>
        /// Gets the formatted date.
        /// </summary>
        /// <returns>The formatted date.</returns>
        /// <param name="dateString">Date string.</param>
        /// <param name="format">Format.</param>
        /// <param name="isNotification">If set to <c>true</c> is notification.</param>
        public static string GetFormattedDate(string dateString, string format, bool isNotification)
        {
            _isNotification = isNotification;
            return GetDate(dateString).ToString(format);
        }
        /// <summary>
        /// Gets the formatted date.
        /// </summary>
        /// <returns>The formatted date.</returns>
        /// <param name="dateString">Date string.</param>
        /// <param name="separator">Separator.</param>
        /// <param name="format">Format.</param>
        public static string GetFormattedDate(string dateString, char separator, string format)
        {
            return GetDate(dateString, separator).ToString(format);
        }

        static DateTime GetDate(string dateString)
        {
            return GetDateTime(dateString, _separator);
        }

        static DateTime GetDate(string dateString, char separator)
        {
            return GetDateTime(dateString, separator);
        }

        static DateTime GetDateTime(string dateString, char separator)
        {
            DateTime formattedDate = new DateTime();
            try
            {
                string[] date = dateString.Split(separator);
                int day = Int32.Parse(date[0]);
                int month = Int32.Parse(date[1]);
                int year = Int32.Parse(date[2]);
                if (_isNotification)
                {
                    formattedDate = new DateTime(year, day, month);
                    _isNotification = false;
                }
                else
                {
                    formattedDate = new DateTime(year, month, day);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in parsing date: " + e.Message);
            }
            return formattedDate;
        }
        /// <summary>
        /// Add a month to current date
        /// </summary>
        /// <returns>The month.</returns>
        /// <param name="dateString">Date string.</param>
        /// <param name="format">Format.</param>
        public static string AddMonth(string dateString, string format)
        {
            DateTime formattedDate = new DateTime();
            string addDate = string.Empty;
            try
            {
                string[] date = dateString.Split(_separator);
                int day = Int32.Parse(date[0]);
                int month = Int32.Parse(date[1]);
                int year = Int32.Parse(date[2]);
                if (_isNotification)
                {
                    formattedDate = new DateTime(year, day, month);
                    _isNotification = false;
                }
                else
                {
                    formattedDate = new DateTime(year, month, day);
                }
                addDate = formattedDate.AddMonths(1).ToString(format);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in parsing date: " + e.Message);
            }
            return addDate;
        }

        /// <summary>
        /// Formats date to UTC.
        /// </summary>
        /// <returns>The to UTC.</returns>
        /// <param name="date">Date.</param>
        public static string FormatToUtc(DateTime date)
        {
            return date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern);
        }

    }
}