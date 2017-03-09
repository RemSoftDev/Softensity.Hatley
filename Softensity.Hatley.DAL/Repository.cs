using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.DAL
{

    public class Repository<T>: IRepository<T>
        where T : class
    {
        internal IDbSet<T> DbSet;
        internal IdentityDbContext<User, CustomRole, Guid, CustomUserLogin, CustomUserRole, CustomUserClaim> Context;
        public Repository(IDbSet<T> dbSet, IdentityDbContext<User, CustomRole, Guid, CustomUserLogin, CustomUserRole, CustomUserClaim> context)
        {
            DbSet = dbSet;
            Context = context;
        }

        public void Insert(T entity)
        {
            if (entity != null)
            {
                DbSet.Add(entity);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void Delete(Guid id)
        {
            var entity = DbSet.Find(id);
            if (entity != null)
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public T Get(Guid id)
        {
            var entity = DbSet.Find(id);
            if (entity != null)
            {
                return entity;
            }
            return null;
        }

        public IQueryable<T> GetAll()
        {
            var entities = DbSet;
            if (entities != null)
            {
                return entities;
            }
            return null;
        }
    }
}
