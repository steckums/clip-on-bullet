using System;
using NHibernate;

namespace GW2.Server.Worker.Repositories
{
    public interface ITransactionManager
    {
        ITransaction BeginTransaction();
    }

    public class TransactionManager : ITransactionManager, IDisposable
    {
        private readonly ISession _session;
        private TransactionWrapper _activeTransaction;

        public TransactionManager(ISession session)
        {
            _session = session;
        }

        public ITransaction BeginTransaction()
        {
            _activeTransaction = _activeTransaction == null ?
                                new RootTransaction(_session, _session.BeginTransaction(), LeaveTo) :
                                _activeTransaction.BeginNested(LeaveTo);
            return _activeTransaction;
        }

        private void LeaveTo(TransactionWrapper parent)
        {
            _activeTransaction = parent;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _activeTransaction.Dispose();
        }
    }
}