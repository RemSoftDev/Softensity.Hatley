using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Softensity.Hatley.DAL.Models
{

	public class User : IdentityUser<Guid, CustomUserLogin, CustomUserRole, CustomUserClaim>
	{
		public string StringId
		{
			get
			{
				return Id.ToString("N");
			}
		}

		public User()
		{
			Id = Guid.NewGuid();
		}

		[Required]
		[MaxLength(20)]
		public string FullName { get; set; }
        public int BackupPeriod { get; set; }
        public DateTime? LastBackupDate { get; set; }
	    public DateTime? NextBackupDate { get; set; }
        public int? Time { get; set; }
        public int? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
		public virtual Payment Payment { get; set; }
		public virtual AweberAccount AweberAccount { get; set; }
		public virtual ActiveCampaignAccount ActiveCampaignAccount { get; set; }
		public virtual InfusionSoftAccount InfusionSoftAccount { get; set; }
		public virtual DropboxAccount DropboxAccount { get; set; }
        public virtual MailChimpAccount MailChimpAccount { get; set; }
        public virtual OntraportAccount OntraportAccount { get; set; }
        public virtual GetResponseAccount GetResponseAccount { get; set; }
		public virtual GoogleDriveAccount GoogleDriveAccount { get; set; }
        public virtual ICollection<Backup> Backups { get; set; } 
	}

	public class CustomRole : IdentityRole<Guid, CustomUserRole>
	{
		public CustomRole()
		{
			Id = Guid.NewGuid();
		}
		public CustomRole(string name) { Name = name; }
	}

	public class CustomUserRole : IdentityUserRole<Guid> { }

	public class CustomUserClaim : IdentityUserClaim<Guid> { }

	public class CustomUserLogin : IdentityUserLogin<Guid> { }
}
