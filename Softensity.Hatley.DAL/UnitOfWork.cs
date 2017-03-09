using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Migrations;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.DAL
{

    public class UnitOfWork : IdentityDbContext<User, CustomRole, Guid, CustomUserLogin, CustomUserRole, CustomUserClaim>, IUnitOfWork
    {
        public UnitOfWork()
            : base("DefaultConnection")
        {
            Database.SetInitializer<UnitOfWork>(new MigrateDatabaseToLatestVersion<UnitOfWork, Configuration>());

            UserManager = new UserManager<User, Guid>(new UserStore<User, CustomRole, Guid, CustomUserLogin, CustomUserRole, CustomUserClaim>(this))
            {
                EmailService = new EmailService()
            };
            UserManager.UserValidator = new UserValidator<User, Guid>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            RolesManager = new RoleManager<CustomRole, Guid>(new RoleStore<CustomRole, Guid, CustomUserRole>(this));
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }
        public RoleManager<CustomRole, Guid> RolesManager { get; set; }
        public UserManager<User, Guid> UserManager { get; set; }


        private Repository<User> userRepository;
        public IRepository<User> UserRepository
        {
            get { return userRepository ?? (userRepository = new Repository<User>(Users, this)); }
        }

        public DbSet<DropboxAccount> DropboxAccounts { get; set; }
        private Repository<DropboxAccount> dropboxRepository;
        public IRepository<DropboxAccount> DropboxRepository
        {
            get { return dropboxRepository ?? (dropboxRepository = new Repository<DropboxAccount>(DropboxAccounts, this)); }
        }

        public DbSet<GoogleDriveAccount> GoogleDriveAccounts { get; set; }
        private Repository<GoogleDriveAccount> googleDriveRepository;
        public IRepository<GoogleDriveAccount> GoogleDriveRepository
        {
            get { return googleDriveRepository ?? (googleDriveRepository = new Repository<GoogleDriveAccount>(GoogleDriveAccounts, this)); }
        }

        public DbSet<InfusionSoftAccount> InfusionSoftAccount { get; set; }
        private Repository<InfusionSoftAccount> infusionSoftRepository;
        public IRepository<InfusionSoftAccount> InfusionSoftRepository
        {
            get { return infusionSoftRepository ?? (infusionSoftRepository = new Repository<InfusionSoftAccount>(InfusionSoftAccount, this)); }
        }

        public DbSet<ActiveCampaignAccount> ActiveCampaignAccount { get; set; }
        private Repository<ActiveCampaignAccount> activeCampaignRepository;

        public IRepository<ActiveCampaignAccount> ActiveCampaignRepository
        {
            get
            {
                return activeCampaignRepository ?? (activeCampaignRepository = new Repository<ActiveCampaignAccount>(ActiveCampaignAccount, this));
            }
        }

        public DbSet<MailChimpAccount> MailChimpAccount { get; set; }
        private Repository<MailChimpAccount> mailChimpRepository;

        public IRepository<MailChimpAccount> MailChimpRepository
        {
            get
            {
                return mailChimpRepository ?? (mailChimpRepository = new Repository<MailChimpAccount>(MailChimpAccount, this));
            }
        }
        public DbSet<OntraportAccount> OntraportAccount { get; set; }
        private Repository<OntraportAccount> ontraportRepository;

        public IRepository<OntraportAccount> OntraportRepository
        {
            get
            {
                return ontraportRepository ?? (ontraportRepository = new Repository<OntraportAccount>(OntraportAccount, this));
            }
        }
        public DbSet<GetResponseAccount> GetResponseAccount { get; set; }
        private Repository<GetResponseAccount> getResponseRepository;

        public IRepository<GetResponseAccount> GetResponseRepository
        {
            get
            {
                return getResponseRepository ?? (getResponseRepository = new Repository<GetResponseAccount>(GetResponseAccount, this));
            }
        }
        public DbSet<AweberAccount> AweberAccount { get; set; }
        private Repository<AweberAccount> aweberRepository;

        public IRepository<AweberAccount> AweberRepository
        {
            get
            {
                return aweberRepository ?? (aweberRepository = new Repository<AweberAccount>(AweberAccount, this));
            }
        }

        public DbSet<Payment> Payment { get; set; }
        private Repository<Payment> paymentRepository;

        public IRepository<Payment> PaymentRepository
        {
            get
            {
                return paymentRepository ?? (paymentRepository = new Repository<Payment>(Payment, this));

            }
        }

        public DbSet<Backup> Backup { get; set; }
        private Repository<Backup> backupRepository;

        public IRepository<Backup> BackupRepository
        {
            get
            {
                return backupRepository ?? (backupRepository = new Repository<Backup>(Backup,this));
            }
        } 
    }

}
