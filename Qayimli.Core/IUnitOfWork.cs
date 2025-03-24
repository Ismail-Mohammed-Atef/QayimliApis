using Qayimli.Core.Entities;
using Qayimli.Core.RepositoriesContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();
    }
}
