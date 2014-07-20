using System.Configuration;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;

namespace GW2.Server.Worker.Configuration
{
    public static class DbConfiguration
    {
        public static ISessionFactory Configure(this Assembly assembly)
        {

            var config = ReadConfigFromCacheFileOrBuildIt(assembly);
            return config.BuildSessionFactory();
        }
        private static MsSqlConfiguration GetDatabaseConfig()
        {
            if (!ConfigurationManager.ConnectionStrings.Any()) throw new ConfigurationErrorsException("No connection string defined!");
            var cs = ConfigurationManager.ConnectionStrings.First() as ConnectionStringSettings;
            if (cs == null) throw new ConfigurationErrorsException("Connection string settings is null!");
            return MsSqlConfiguration.MsSql2008.ConnectionString(cs.ConnectionString);
        }
        private static NHibernate.Cfg.Configuration ReadConfigFromCacheFileOrBuildIt(Assembly assembly)
        {
            NHibernate.Cfg.Configuration nhConfigurationCache;
            var nhCfgCache = new ConfigurationFileCache(assembly);
            var cachedCfg = nhCfgCache.LoadConfigurationFromFile();
            if (cachedCfg == null)
            {
                nhConfigurationCache = BuildConfiguration();
                nhCfgCache.SaveConfigurationToFile(nhConfigurationCache);
            }
            else
            {
                nhConfigurationCache = cachedCfg;
            }
            return nhConfigurationCache;
        }

        private static NHibernate.Cfg.Configuration BuildConfiguration()
        {
            var config = Fluently.Configure()
                .Database(GetDatabaseConfig)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ServerRegistry>())
                .ExposeConfiguration(BuildSchema)
                .BuildConfiguration();
            SchemaMetadataUpdater.QuoteTableAndColumns(config);
            return config;
        }

        private static void BuildSchema(NHibernate.Cfg.Configuration configuration)
        {
            new SchemaUpdate(configuration).Execute(true, true); //This won't
            //new SchemaExport(configuration).Create(true, true); //This will drop every table and rebuild
        }
    }

}