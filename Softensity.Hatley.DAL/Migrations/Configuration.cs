using System;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(UnitOfWork context)
        {
            if (context.UserManager.FindByEmail("anden-dom@tut.by") == null)
            {
                context.Users.AddOrUpdate(u => u.Email, new User
                {
                    Email = "anden-dom@tut.by",
                    FullName = "AD",
                    UserName = "anden-dom@tut.by",
                    EmailConfirmed = true,
                    PhoneNumber = "0297582083",
                    PasswordHash = new PasswordHasher().HashPassword("123"),
                    SecurityStamp = Guid.NewGuid().ToString()
                });
            }
            context.Roles.AddOrUpdate(u => u.Name, new CustomRole
            {
                Name = "User"
            },
            new CustomRole
            {
                Name = "Admin"
            });
            context.SaveChanges();
        }
    }
}
