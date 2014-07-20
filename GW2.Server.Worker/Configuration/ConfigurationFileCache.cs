using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace GW2.Server.Worker.Configuration
{
    /// <summary>
    /// This class allows for the NHibernate configuiration be saved to the disk, so it doesn't have to be generated every time.
    /// </summary>
    public class ConfigurationFileCache
    {
        private readonly string _cacheFile;
        private readonly Assembly _definitionsAssembly;

        public ConfigurationFileCache(Assembly definitionsAssembly)
        {
            _definitionsAssembly = definitionsAssembly;
            _cacheFile = "nh.cfg";
            //if (HttpContext.Current != null) //for the web apps
            //    _cacheFile = HttpContext.Current.Server.MapPath(
            //                    string.Format("~/App_Data/{0}", _cacheFile)
            //                    );
        }

        public void DeleteCacheFile()
        {
            if (File.Exists(_cacheFile))
                File.Delete(_cacheFile);
        }

        public bool IsConfigurationFileValid
        {
            get
            {
                if (!File.Exists(_cacheFile))
                    return false;
                var configInfo = new FileInfo(_cacheFile);
                var asmInfo = new FileInfo(_definitionsAssembly.Location);

                if (configInfo.Length < 5*1024)
                    return false;

                return configInfo.LastWriteTime >= asmInfo.LastWriteTime;
            }
        }

        public void SaveConfigurationToFile(NHibernate.Cfg.Configuration configuration)
        {
            using (var file = File.Open(_cacheFile, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, configuration);
            }
        }

        public NHibernate.Cfg.Configuration LoadConfigurationFromFile()
        {
            if (!IsConfigurationFileValid)
                return null;

            using (var file = File.Open(_cacheFile, FileMode.Open, FileAccess.Read))
            {
                var bf = new BinaryFormatter();
                return bf.Deserialize(file) as NHibernate.Cfg.Configuration;
            }
        }

    }
}