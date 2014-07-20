using System;
using NHibernate;

namespace GW2.Server.Worker.Repositories
{
    public class RootTransaction : TransactionWrapper
    {
        private readonly NHibernate.ITransaction _transaction;

        public RootTransaction(ISession session, NHibernate.ITransaction transaction
            , Action<TransactionWrapper> leaveTo)
            : base(session, null, leaveTo)
        {
            _transaction = transaction;
        }

        protected override void InnerCommit()
        {
            _transaction.Commit();
        }

        protected override void InnerRollback(bool suppressExceptions)
        {
            try
            {
                _transaction.Rollback();
            }
            catch (Exception)
            {
                if (!suppressExceptions)
                    throw;
            }
        }

        public override bool WasRolledBack
        {
            get { return _transaction.WasRolledBack; }
        }

        public override bool WasCommitted
        {
            get { return _transaction.WasCommitted; }
        }
    }
}