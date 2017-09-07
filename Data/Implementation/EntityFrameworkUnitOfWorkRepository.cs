using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NrsRepository.Dependency;

namespace NrsRepository.Implementation
{
    public class EntityFrameworkUnitOfWorkRepository<TEntity> : IUnitOfWorkRepository<TEntity> where TEntity : class
    {
        public readonly DbSet<TEntity> _dbSet;

        private readonly DbContext _context;

        public EntityFrameworkUnitOfWorkRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Add(TEntity obj)
        {
            _dbSet.Add(obj);
        }

        public void AddRange(List<TEntity> objList)
        {
            _dbSet.AddRange(objList);
        }

        public async Task AddRangeSaveAsync(List<TEntity> objList, int batchSize, int batchDelayMilliSeconds)
        {
            if (batchSize != 0)
            {
                int index = 0;
                while (index + batchSize <= objList.Count)
                {
                    _dbSet.AddRange(objList.Skip(index).Take(batchSize).ToList());
                    index += batchSize;
                    await _context.SaveChangesAsync();
                    if (batchDelayMilliSeconds > 0)
                    {
                        await Task.Delay(batchDelayMilliSeconds);
                    }
                }

                if (index < objList.Count)
                {
                    _dbSet.AddRange(objList.Skip(index).Take(objList.Count - index).ToList());
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                _dbSet.AddRange(objList);
                await _context.SaveChangesAsync();
            }
        }
        
        public void Delete(TEntity obj)
        {
            _dbSet.Remove(obj);
        }

        public async Task DeleteRangeSaveAsync(List<TEntity> objList, int batchSize, int batchDelayMilliSeconds)
        {
            if (batchSize != 0)
            {
                int index = 0;
                while (index + batchSize <= objList.Count)
                {
                    _dbSet.RemoveRange(objList.Skip(index).Take(batchSize).ToList());
                    index += batchSize;
                    await _context.SaveChangesAsync();
                    if (batchDelayMilliSeconds > 0)
                    {
                        await Task.Delay(batchDelayMilliSeconds);
                    }
                }

                if (index < objList.Count)
                {
                    _dbSet.RemoveRange(objList.Skip(index).Take(objList.Count - index).ToList());
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                _dbSet.RemoveRange(objList);
                await _context.SaveChangesAsync();
            }
        }
    }
}