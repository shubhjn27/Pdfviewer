using System;

using NrsRepository.Dependency;
using NrsRepository.Entities;

namespace NrsRepository.Implementation
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
            var context = new NrsRepositoryNetRoadshowEntities();
            return context;
        }
    }
}