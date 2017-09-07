using System;
using FileViewer.Data.Dependency;
using FileViewer.Data.Entities;

namespace FileViewer.Data.Implementation
{
    public class EntityFrameworkUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork GetUnitOfWork(bool useTransaction = false)
        {
            return new EntityFrameworkUnitOfWork(useTransaction);
        }

        /// <summary>
        /// This was a one off thing that we did to execute a stored procedure. This should be avoided.
        /// </summary>
        /// <returns></returns>
        public IDisposable GetDatabaseContext()
        {
            var context = new FileViewerEntities();
            return context;
        }
    }
}