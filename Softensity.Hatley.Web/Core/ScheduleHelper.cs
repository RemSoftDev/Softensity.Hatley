using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Softensity.Hatley.Web.Models.User;

namespace Softensity.Hatley.Web.Core
{
    public static class ScheduleHelper
    {
        private static DateTime GetNextDay(DateTime start)
        {
            return start.AddDays(1);
        }

        private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd;
            if (start.DayOfWeek == day)
            {
                daysToAdd = 7;
            }
            else
            {
                daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            }
            return start.AddDays(daysToAdd);
        }

        private static DateTime GetNextMonthday(DateTime start, int days)
        {
            int monthdays = Math.Min(DateTime.DaysInMonth(start.Year, start.Month), days);
            if (start.Day < days)
            {
                return new DateTime(start.Year, start.Month, 1, start.Hour, start.Minute, 0).AddDays(monthdays - 1);
            }
            else
            {
                return new DateTime(start.Year, start.Month, 1, start.Hour, start.Minute, 0).AddMonths(1).AddDays(monthdays - 1);
            }
        }

        public static DateTime? GetNextDate(ScheduleEnum scheduleType, int time, int timezone, DayOfWeek dayOfWeek,
            int dayOfMonth)
        {
            var currentDate = DateTime.UtcNow.Date.AddHours(time).AddMinutes(timezone);
            return GetNextDate(currentDate, scheduleType, dayOfWeek, dayOfMonth);
        }

        public static DateTime? GetNextDate(DateTime start, ScheduleEnum scheduleType, DayOfWeek? dayOfWeek, int? dayOfMonth)
        {
            DateTime? nextdate = null;
            switch (scheduleType)
            {
                case ScheduleEnum.Daily:
                    nextdate = GetNextDay(start);
                    break;
                case ScheduleEnum.Weekly:
                    nextdate = GetNextWeekday(start, (DayOfWeek)dayOfWeek);
                    break;
                case ScheduleEnum.Monthly:
                    nextdate = GetNextMonthday(start, (int)dayOfMonth);
                    break;
                case ScheduleEnum.None:
                default:
                    nextdate = null;
                    break;
            }
            return nextdate;
        }

        public static bool IsCurrentDateTime(DateTime nextDateTime, TimeSpan timeSpan)
        {
            return IsCurrentDateTime(DateTime.UtcNow, nextDateTime, timeSpan);
        }

        public static bool IsCurrentDateTime(DateTime currentDateTime, DateTime nextDateTime, TimeSpan timeSpan)
        {
            if (nextDateTime <= currentDateTime || (nextDateTime - currentDateTime) <= timeSpan)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}