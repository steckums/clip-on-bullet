using System;
using NHibernate;

namespace GW2.Server.Worker.Repositories
{
    public abstract class TransactionWrapper : ITransaction
    {
        private readonly ISession _session;
        private readonly TransactionWrapper _parent;
        private readonly Action<TransactionWrapper> _leaveTo;
        private bool _leftTransaction;

        protected TransactionWrapper(ISession session, TransactionWrapper parent, Action<TransactionWrapper> leaveTo)
        {
            _session = session;
            _parent = parent;
            _leaveTo = leaveTo;
        }

        private void Leave()
        {
            if (_leftTransaction) return;
            _leftTransaction = true;
            _leaveTo(_parent);
        }

        public TransactionWrapper BeginNested(Action<TransactionWrapper> leaveTo)
        {
            return new NestedTransaction(_session, this, leaveTo);
        }
        #region ITransaction Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && IsActive)
            {
                Rollback(true);
            }
        }

        public void Rollback(bool suppressExceptions)
        {
            try
            {
                Leave();
                InnerRollback(suppressExceptions);
            }
            finally
            {
                _session.Clear();
            }
        }

        public void Commit()
        {
            if (!IsActive)
                throw new InvalidOperationException("Transaction is not active");
            Leave();
            InnerCommit();
        }

        public void Rollback()
        {
            Rollback(false);
        }

        protected abstract void InnerCommit();
        protected abstract void InnerRollback(bool suppressExceptions);

        public ITransaction Parent { get { return _parent; } }
        public bool IsActive { get { return !WasCommitted && !WasRolledBack; } }
        public abstract bool WasRolledBack { get; }
        public abstract bool WasCommitted { get; }
        #endregion
    }
}