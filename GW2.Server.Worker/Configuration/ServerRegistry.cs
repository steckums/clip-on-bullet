using System.Reflection;
using GW2.Server.Worker.Repositories;
using NHibernate;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace GW2.Server.Worker.Configuration
{
    public class ServerRegistry : Registry
    {
        public ServerRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                scanner.AssemblyContainingType<ServerRegistry>();
                scanner.ConnectImplementationsToTypesClosing(typeof(IRepository<>));
                scanner.TheCallingAssembly();
            });
            For<ISessionFactory>().Singleton().Use(Assembly.GetExecutingAssembly().Configure());

            For<ISession>().Use(c => c.GetInstance<ISessionFactory>().OpenSession());
            For(typeof(IRepository<>)).Use(typeof(Repository<>));
        }
    }
}