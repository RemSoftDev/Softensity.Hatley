using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        void Commit();
        RoleManager<CustomRole, Guid> RolesManager { get; set; }
        UserManager<User, Guid> UserManager { get; set; }
        IRepository<User> UserRepository { get; }
        IRepository<DropboxAccount> DropboxRepository { get; }
        IRepository<GoogleDriveAccount> GoogleDriveRepository { get; }
        IRepository<InfusionSoftAccount> InfusionSoftRepository { get; }
        IRepository<ActiveCampaignAccount> ActiveCampaignRepository { get; }
        IRepository<AweberAccount> AweberRepository { get; }
        IRepository<MailChimpAccount> MailChimpRepository { get; }
        IRepository<OntraportAccount> OntraportRepository { get; }
        IRepository<GetResponseAccount> GetResponseRepository { get; } 
        IRepository<Payment> PaymentRepository { get; }
        IRepository<Backup> BackupRepository { get; }
    }
}
