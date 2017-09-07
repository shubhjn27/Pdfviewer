using System;

namespace FileViewer.Data.Dependency
{
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Returns a context with open database connection with autoCommit=true. There is no need to call .saveChanges() method unless you need to
        /// </summary>
        /// <returns></returns>
        IUnitOfWork GetUnitOfWork(bool useTransaction = false);

        /// <summary>
        /// Gives EntityFramework DbContext. Only to be used under special circumstances
        /// </summary>
        /// <returns></returns>
        IDisposable GetDatabaseContext();
    }
}