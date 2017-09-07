using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileViewer.Data.Dependency
{
    public interface IUnitOfWorkRepository<T>
    {
        IQueryable<T> Queryable();
        
        void Add(T obj);
        
        void AddRange(List<T> obj);
        
        /// <summary>
        /// Adds batchSize to the Dbset, calls save, sleeps for batchDelayMilliSeconds, and repeats
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="batchSize">number of records to save at a time</param>
        /// <param name="batchDelayMilliSeconds">time to wait after save</param>
        /// <returns></returns>
        Task AddRangeSaveAsync(List<T> obj, int batchSize, int batchDelayMilliSeconds);
        
        void Delete(T obj);

        Task DeleteRangeSaveAsync(List<T> objList, int batchSize, int batchDelayMilliSeconds);
    }
}