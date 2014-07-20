using System;

namespace GW2.Server.Worker.Repositories
{
    public interface ITransaction : IDisposable
    {
        ITransaction Parent { get; }
        bool IsActive { get; }
        bool WasRolledBack { get; }
        bool WasCommitted { get; }
        void Commit();
        void Rollback();
        void Rollback(bool suppressExceptions);

    }
}