using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Interfaces
{
    public interface IRepository<T> where T:class
    {
        void Insert(T entity);
        void Delete(Guid id);
        void Update(T entity);
        T Get(Guid id);
        IQueryable<T> GetAll();
    }
}
