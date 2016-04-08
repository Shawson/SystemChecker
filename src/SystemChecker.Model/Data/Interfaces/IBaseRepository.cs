using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemChecker.Model.Data.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        List<TEntity> GetAll();
        TEntity GetById(int id);
        int Insert(TEntity instance);
    }
}
