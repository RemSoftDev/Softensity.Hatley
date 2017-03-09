using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Models.User;

namespace UnitTestProject
{
    [TestClass]
    public class ScheduleTimeTest
    {
        [TestMethod]
        public void GetNextDay()
        {
            var date1 = new DateTime(2014, 12, 3);
            var date2 = new DateTime(2014, 12, 10);
            var date3 = new DateTime(2014, 12, 17);
            var date4 = new DateTime(2014, 12, 24);
            var date5 = new DateTime(2014, 12, 31);

            var dateN1 = new DateTime(2014, 12, 4);
            var dateN2 = new DateTime(2014, 12, 11);
            var dateN3 = new DateTime(2014, 12, 18);
            var dateN4 = new DateTime(2014, 12, 25);
            var dateN5 = new DateTime(2015, 1, 1);

            var next1 = ScheduleHelper.GetNextDate(date1, ScheduleEnum.Daily, null, null);
            var next2 = ScheduleHelper.GetNextDate(date2, ScheduleEnum.Daily, null, null);
            var next3 = ScheduleHelper.GetNextDate(date3, ScheduleEnum.Daily, null, null);
            var next4 = ScheduleHelper.GetNextDate(date4, ScheduleEnum.Daily, null, null);
            var next5 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Daily, null, null);

            Assert.AreEqual(dateN1, next1);
            Assert.AreEqual(dateN2, next2);
            Assert.AreEqual(dateN3, next3);
            Assert.AreEqual(dateN4, next4);
            Assert.AreEqual(dateN5, next5);
        }

        [TestMethod]
        public void GetNextWeekday()
        {
            var date5 = new DateTime(2014, 12, 31);

            var dateN1 = new DateTime(2015, 1, 5);
            var dateN2 = new DateTime(2015, 1, 6);
            var dateN3 = new DateTime(2015, 1, 7);
            var dateN4 = new DateTime(2015, 1, 1);
            var dateN5 = new DateTime(2015, 1, 2);
            var dateN6 = new DateTime(2015, 1, 3);
            var dateN7 = new DateTime(2015, 1, 4);

            var next1 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Monday, null);
            var next2 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Tuesday, null);
            var next3 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Wednesday, null);
            var next4 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Thursday, null);
            var next5 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Friday, null);
            var next6 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Saturday, null);
            var next7 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Weekly, DayOfWeek.Sunday, null);

            Assert.AreEqual(dateN1, next1);
            Assert.AreEqual(dateN2, next2);
            Assert.AreEqual(dateN3, next3);
            Assert.AreEqual(dateN4, next4);
            Assert.AreEqual(dateN5, next5);
            Assert.AreEqual(dateN6, next6);
            Assert.AreEqual(dateN7, next7);
        }

        [TestMethod]
        public void GetNextMonthday()
        {
            var date1 = new DateTime(2014, 12, 1);
            var date2 = new DateTime(2015, 1, 1);
            var date3 = new DateTime(2015, 2, 1);
            var date4 = new DateTime(2015, 3, 2);
            var date5 = new DateTime(2015, 4, 3);

            var dateN1 = new DateTime(2014, 12, 31);
            var dateN2 = new DateTime(2015, 1, 31);
            var dateN3 = new DateTime(2015, 2, 28);
            var dateN4 = new DateTime(2015, 3, 31);
            var dateN5 = new DateTime(2015, 4, 30);

            var next1 = ScheduleHelper.GetNextDate(date1, ScheduleEnum.Monthly, null, 31);
            var next2 = ScheduleHelper.GetNextDate(date2, ScheduleEnum.Monthly, null, 31);
            var next3 = ScheduleHelper.GetNextDate(date3, ScheduleEnum.Monthly, null, 31);
            var next4 = ScheduleHelper.GetNextDate(date4, ScheduleEnum.Monthly, null, 31);
            var next5 = ScheduleHelper.GetNextDate(date5, ScheduleEnum.Monthly, null, 31);

            Assert.AreEqual(dateN1, next1);
            Assert.AreEqual(dateN2, next2);
            Assert.AreEqual(dateN3, next3);
            Assert.AreEqual(dateN4, next4);
            Assert.AreEqual(dateN5, next5);
        }

        [TestMethod]
        public void IsCurrentDateTime()
        {
            var currentTime = new DateTime(2014, 12, 1, 12, 0, 0);

            var date1 = new DateTime(2014, 12, 1, 11, 45, 0);
            var date2 = new DateTime(2014, 12, 1, 12, 0, 0);
            var date3 = new DateTime(2014, 12, 1, 12, 10, 0);
            var date4 = new DateTime(2014, 12, 1, 12, 15, 0);
            var date5 = new DateTime(2014, 12, 1, 12, 30, 0);

            var next1 = ScheduleHelper.IsCurrentDateTime(currentTime, date1, new TimeSpan(0, 20, 0));
            var next2 = ScheduleHelper.IsCurrentDateTime(currentTime, date2, new TimeSpan(0, 20, 0));
            var next3 = ScheduleHelper.IsCurrentDateTime(currentTime, date3, new TimeSpan(0, 20, 0));
            var next4 = ScheduleHelper.IsCurrentDateTime(currentTime, date4, new TimeSpan(0, 20, 0));
            var next5 = ScheduleHelper.IsCurrentDateTime(currentTime, date5, new TimeSpan(0, 20, 0));

            Assert.IsTrue(next1);
            Assert.IsTrue(next2);
            Assert.IsTrue(next3);
            Assert.IsTrue(next4);
            Assert.IsFalse(next5);
        }
    }
}
