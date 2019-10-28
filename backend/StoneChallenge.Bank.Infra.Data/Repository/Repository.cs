using Microsoft.EntityFrameworkCore;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Infra.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly BankContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(BankContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public async virtual Task AddAsync(TEntity obj)
        {
            await DbSet.AddAsync(obj);
            await SaveChangesAsync();
        }

        public async virtual Task<TEntity> GetByIdAsync(string id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task UpdateAsync(TEntity obj)
        {
            DbSet.Update(obj);
            await SaveChangesAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
