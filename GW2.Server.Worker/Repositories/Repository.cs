using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

namespace GW2.Server.Worker.Repositories
{
    #region IRepository
    public interface IRepository<TEntity>
    {
        TEntity Get(int id);
        void InsertOrUpdate(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetQueryable();
        IEnumerable<TEntity> All();
        IEnumerable<TEntity> GetAllPaged(int page, int pageSize);
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity First(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        ITransaction BeginTransaction();
    }
    #endregion

    public class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly ISession _session;
        private readonly ITransactionManager _transactionManager;

        public Repository(ISession session, ITransactionManager transactionManager)
        {
            _session = session;
            _transactionManager = transactionManager;
        }

        public TEntity Get(int id)
        {
            return _session.Get<TEntity>(id);
        }

        public void InsertOrUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(entity.GetType() + " is null!");
            _session.Save(entity);
        }

        public void Delete(TEntity entity)
        {

            if (entity == null)
                throw new ArgumentNullException(entity.GetType() + " is null!");
            _session.Delete(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = Where(predicate);
            foreach (var entity in entities)
                _session.Delete(entity);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _session.Query<TEntity>();
        }

        public IEnumerable<TEntity> All()
        {
            return _session.Query<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetAllPaged(int page, int pageSize)
        {
            return _session.Query<TEntity>().Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().Where(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().Any();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().Single(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().SingleOrDefault(predicate);
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().First(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _session.Query<TEntity>().FirstOrDefault(predicate);
        }

        public int Count()
        {
            return _session.Query<TEntity>().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            //TODO Test this; if it doesn't work, .Where(preciate).Count()
            return _session.Query<TEntity>().Count(predicate);
        }

        public ITransaction BeginTransaction()
        {
            return _transactionManager.BeginTransaction();
        }
    }
}