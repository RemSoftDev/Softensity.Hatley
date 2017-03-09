using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.Ajax.Utilities;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Models.User;

namespace Softensity.Hatley.Web.Controllers
{
    public class ScheduledBackupController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private TimeSpan _timespan = new TimeSpan(0, 0, 20);
        private ILog logger = LogManager.GetLogger(typeof(ScheduledBackupController));
        public ScheduledBackupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string Run()
        {
            try
            {
                logger.Info("Scheduled backup started");
                var manager = new BackupManager(_unitOfWork);             
                var currentDate = DateTime.UtcNow.Date;
                var users =
                    _unitOfWork.UserRepository.GetAll().Where(
                        x =>
                            x.BackupPeriod != (int)ScheduleEnum.None && x.NextBackupDate != null &&
                            currentDate.Year == x.NextBackupDate.Value.Year &&
                            currentDate.Month == x.NextBackupDate.Value.Month &&
                            currentDate.Day == x.NextBackupDate.Value.Day
                            ).ToList().Where(x => ScheduleHelper.IsCurrentDateTime((DateTime)x.NextBackupDate, _timespan)).ToList();

                foreach (var user in users)
                {             
                    if (manager.BackupData(user))
                    {
                        user.LastBackupDate = user.NextBackupDate;
                    }

                    user.NextBackupDate = ScheduleHelper.GetNextDate((DateTime)user.NextBackupDate, (ScheduleEnum)user.BackupPeriod, (DayOfWeek?)user.DayOfWeek, user.DayOfMonth);

                    _unitOfWork.Commit();
                }
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string Test()
        {
            try
            {
                logger.Info("Test Scheduled backup started");
                var manager = new BackupManager(_unitOfWork);
                var users =
                    _unitOfWork.UserRepository.GetAll().Where(
                        x =>
                            x.BackupPeriod != (int)ScheduleEnum.None).ToList();

                foreach (var user in users)
                {

                    if (manager.BackupData(user))
                    {
                        user.LastBackupDate = user.NextBackupDate;
                    }

                    user.NextBackupDate = ScheduleHelper.GetNextDate((DateTime)user.NextBackupDate, (ScheduleEnum)user.BackupPeriod, (DayOfWeek?)user.DayOfWeek, user.DayOfMonth);

                    _unitOfWork.Commit();
                }
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}