using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using NrsRepository.Dependency;
using NrsRepository.Entities;
using Npgsql;
using NpgsqlTypes;
using NrsRepository.AuditLog;

namespace NrsRepository.Implementation
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        public readonly DbContext _context;

        private readonly DbContextTransaction _dbContextTransaction;

        private Dictionary<string, object> _repositories;

        private bool _autoCommit = true;

        private bool _transactionRolledBack;

        private bool _disposed;

        private readonly bool _useTransaction;

        public EntityFrameworkUnitOfWork(bool useTransaction)
        {
            _context = new NrsRepositoryNetRoadshowEntities();
            LogToDatabase = true;
            _useTransaction = useTransaction;
            if (_useTransaction)
            {
                _dbContextTransaction = _context.Database.BeginTransaction();
            }
        }

        #region IUnitOfWork Interface Implementation

        public IUnitOfWorkRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                _repositories.Add(type, new EntityFrameworkUnitOfWorkRepository<TEntity>(_context));
            }

            return (IUnitOfWorkRepository<TEntity>) _repositories[type];
        }

        public T RepositoryService<T>() where T : class
        {
            object[] args = {this};
            return (T) Activator.CreateInstance(typeof(T), args);
        }

        public void SaveChanges()
        {
            SaveChangesPrivate(false).GetAwaiter().GetResult();
        }

        public async Task<int> ExecuteSqlCommandAsync(string sqlQuery, params object[] parameters)
        {
            int rowsAffected = await _context.Database.ExecuteSqlCommandAsync(sqlQuery, parameters);
            return rowsAffected;
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesPrivate(true);
        }

        /// <summary>
        /// This is the internal save changes method, which is called by 2 other public methods in this class.
        /// The purpose is to centralize code.
        /// </summary>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        private async Task SaveChangesPrivate(bool isAsync)
        {
            var auditHelper = new AuditLogHelper();
            AuditLog.AuditLog auditLog = new AuditLog.AuditLog();

            if (LogToDatabase)
            {
                auditLog = auditHelper.GetAuditLogData(_context);
            }

            if (isAsync)
            {
                await _context.SaveChangesAsync();
                _transactionRolledBack = false;
            }
            else
            {
                _context.SaveChanges();
                _transactionRolledBack = false;
            }

            if (LogToDatabase)
            {
                auditHelper.SaveToLogDatabase(auditLog, _repositories);
            }

            _transactionRolledBack = false;
        }

        public void AutoCommit(bool autoCommitP)
        {
            this._autoCommit = autoCommitP;
        }

        public void RollbackTransaction()
        {
            _dbContextTransaction.Rollback();
            _transactionRolledBack = true;
        }

        public bool LazyLoadingEnabled
        {
            get { return _context.Configuration.LazyLoadingEnabled; }
            set { _context.Configuration.LazyLoadingEnabled = value; }
        }

        public bool LogToDatabase { get; set; }

        public void CommitTransaction()
        {
            if (_dbContextTransaction != null)
            {
                _dbContextTransaction.Commit();
            }
        }

        #endregion

        #region IDisposable Interface Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_dbContextTransaction != null && _autoCommit && !_transactionRolledBack && _useTransaction)
                {
                    _dbContextTransaction.Commit();
                }

                if (null != _repositories)
                {
                    _repositories.Clear();
                    _repositories = null;
                }

                if (_dbContextTransaction != null)
                {
                    _dbContextTransaction.Dispose();
                }

                if (_context != null)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}