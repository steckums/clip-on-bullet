using System;
using NHibernate;

namespace GW2.Server.Worker.Repositories
{
    public class NestedTransaction : TransactionWrapper
    {
        private readonly ISession _session;
        private bool _committed;

        public NestedTransaction(ISession session, TransactionWrapper parent, Action<TransactionWrapper> leaveTo)
            : base(session, parent, leaveTo)
        {
            _session = session;
        }
        protected override void InnerCommit()
        {
            _session.Flush();
            _committed = true;
        }

        protected override void InnerRollback(bool suppressExceptions)
        {
            Parent.Rollback(suppressExceptions);
        }

        public override bool WasRolledBack
        {
            get { return Parent.WasRolledBack; }
        }

        public override bool WasCommitted
        {
            get { return _committed || Parent.WasCommitted; }
        }
    }
}