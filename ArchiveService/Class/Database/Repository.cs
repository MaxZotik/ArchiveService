using ArchiveService.Class.Configuration;
using ArchiveService.Class.Patterns;
using System.Collections.Generic;

namespace ArchiveService.Class.Database
{
    static class Repository
    {
        private static string connection;

        private static string nameDb;

        static Repository()=>connection = CreateConnectionString();

        /// <summary>
        /// Метод конфигурации строки подключения
        /// </summary>
        /// <returns></returns>
        private static string CreateConnectionString()
        {
            ConfigurationManager.GetDatabaseConfigurated(out List<DatabaseClient> databases);

            nameDb = databases[0].DatabaseName;

            if (databases[0].Server == "MSSQL")
            {
                return $"Data Source={databases[0].ServerName};Initial Catalog={databases[0].DatabaseName};User ID={databases[0].Login};Password={databases[0].Password}";
            }   
            else
            {
                return $"Host={databases[0].ServerName};Database={databases[0].DatabaseName};Username={databases[0].Login};Password={databases[0].Password};";
            }               
        }

        /// <summary>
        /// Метод доступа к строке подключения
        /// </summary>
        /// <returns>Возвращает строку подключения к БД "MSSQL"</returns>
        public static string GetConnectionString()
        {
            return connection;
        }

        /// <summary>
        /// Метод доступа к строке названия БД
        /// </summary>
        /// <returns>Возвращает строку с названием БД</returns>
        public static string GetNameDb()
        {
            return nameDb;
        }
    }
}
