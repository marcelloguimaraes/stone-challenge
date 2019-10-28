using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task AddAsync(TEntity obj);
        Task<TEntity> GetByIdAsync(string id);
        Task UpdateAsync(TEntity obj);
        Task<int> SaveChangesAsync();
    }
}
