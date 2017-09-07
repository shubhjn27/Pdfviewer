using System;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace FileViewer.Data.Dependency
{
    /// <summary>
    /// An interface that helps to perform multiple CRUD on multiple different objects within one transaction.
    /// A class that implements this interface should set the auto commit to true
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWorkRepository<TEntity> Repository<TEntity>() where TEntity : class;

        /// <summary>
        /// Interface to use NetRoadshow Repository layer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T RepositoryService<T>() where T : class;

        /// <summary>
        /// Execute update and delete sql queries
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlCommandAsync(string sqlQuery, params object[] parameters);

        void SaveChanges();

        Task SaveChangesAsync();

        void AutoCommit(bool autoCommitP);

        void CommitTransaction();

        void RollbackTransaction();

        bool LazyLoadingEnabled { get; set; }

        bool LogToDatabase { get; set; }
    }
}