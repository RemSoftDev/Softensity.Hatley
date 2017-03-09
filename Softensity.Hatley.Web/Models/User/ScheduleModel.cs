using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Softensity.Hatley.Web.Models.User
{
    public class ScheduleModel
    {
        public double LastBackup { get; set; }
        public double NextBackup { get; set; }
        public ScheduleEnum ScheduleType { get; set; }
     
        public int? Time { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }

        public ScheduleModel(DateTime? lastBackup, DateTime? nextBackup, int scheduleType, int? time, DayOfWeek? dayOfWeek, int? dayOfMonth)
        {
            LastBackup = lastBackup == null ? 0 : lastBackup.Value.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;// == null ? "-" : lastBackup.Value.ToString();//@"MM/dd/yy", CultureInfo.InvariantCulture);
            NextBackup = nextBackup == null ? 0 : nextBackup.Value.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;// == null ? "-" : nextBackup.Value.ToString();//@"MM/dd/yy", CultureInfo.InvariantCulture);
            ScheduleType = (ScheduleEnum)scheduleType;

            Time = time;
            DayOfWeek = dayOfWeek;
            DayOfMonth = dayOfMonth;
        }

        public bool IsDisable
        {
            get { return ScheduleType == ScheduleEnum.None; }
        }

        public string ScheduleTypeString
        {
            get
            {
                return ScheduleType.ToString();
            }
        }
    }
}