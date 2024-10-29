using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveService.Class.Database.MS_SQL.Table;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System.Xml;
using System.CodeDom.Compiler;

namespace ArchiveService.Class.Database.MS_SQL
{
    public static class ConnectionMSSQL
    {
        static string ConnectionString { get; set; }
        static string NameDb { get; set; }

        static ConnectionMSSQL()
        {
            ConnectionString = Repository.GetConnectionString();
            NameDb = Repository.GetNameDb();
        }
    
        public static void WrireMSSQL(DataTable dt, string nameTable)
        {
            object _lock = new object();

            lock (_lock)
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConnectionString))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = $"dbo.{nameTable}";

                        sqlBulkCopy.BatchSize = 1000;
                        sqlBulkCopy.BulkCopyTimeout = 30;

                        sqlBulkCopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка записи в БД таблицы! - {nameTable} - {ex.Message}", StatusLog.Errors);
                    }
                }
            }
        }

        public static void WrireMSSQLTree(DataTable dt, string nameTable)
        {
            object _lock = new object();

            lock (_lock)
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConnectionString))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = $"dbo.{nameTable}";

                        sqlBulkCopy.BatchSize = 1000;
                        sqlBulkCopy.BulkCopyTimeout = 30;

                        sqlBulkCopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка записи в БД таблицы! - {ex.Message}", StatusLog.Errors);
                    }
                }
            }
        }


        /// <summary>
        /// Метод получает данные из таблицы БД
        /// </summary>
        /// <param name="nameTable">Наименование таблицы в БД</param>
        /// <param name="date">Дата и время получения данных</param>
        /// <returns>Возвращает объект DataTable содержащего все записи из таблицы БД больше указанной даты</returns>
        public static DataTable ReadMSSQL(string nameTable, DateTime date)
        {
            DataTable dt;
            //string SqlSelectDate = $"select * from dbo.{nameTable} where [Time] > Convert(datetime, '{date.ToString("yyyy-MM-ddTHH:mm:ss.fff")}', 127) order by [Time]";
            string SqlSelectDate = $"select * from dbo.{nameTable} where [Time] > Convert(datetime, '{date.ToString("yyyy-MM-ddTHH:mm:ss.fff")}', 127)";
            object _lock = new object();

            lock(_lock)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        // Создаем объект DataAdapter
                        SqlDataAdapter adapter = new SqlDataAdapter(SqlSelectDate, connection);
                        // Создаем объект Dataset
                        DataSet ds = new DataSet();
                        // Заполняем Dataset
                        adapter.Fill(ds);

                        dt = ds.Tables[0];
                    }

                    return dt;
                }
                catch (Exception ex)
                {
                    new Loggings().WriteLogAdd($"Ошибка чтение данных из БД! - {nameTable} - {ex.Message}", StatusLog.Errors);
                }
            }
            
                                
            return null;
        }

        /// <summary>
        /// Метод удаления записей в таблицах БД
        /// </summary>
        /// <param name="nameTable">Наименование таблицы в БД</param>
        /// <param name="timeValue">Количество времени</param>
        /// <param name="timeAverage">Единица измерения времени "минута/час/день и тд"</param>
        public static void DeleteTable(string nameTable, int timeValue, string timeAverage)
        {
            int temp = timeValue * -1;
            //string SqlSelect = $"delete FROM [dbo].[{nameTable}] WHERE datediff({ArchiveMath.GetDate(timeAverage)}, [Time], getdate()) > {timeValue}";
            string SqlSelect = $"delete FROM [dbo].[{nameTable}] WHERE [Time] < dateadd({ArchiveMath.GetDate(timeAverage)}, {temp}, getdate())";
            object _lock = new object();

            lock(_lock)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(SqlSelect, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка удаления данных в БД! - {nameTable} - {ex.Message}", StatusLog.Errors);
                    }
                }
            }           
        }

        public static void CheckIdentRessedTable(string nameTable)
        {
            string SqlSelect = $"DBCC Checkident ('[{nameTable}]', Reseed, 0)";

            object _lock = new object();

            lock (_lock)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(SqlSelect, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка сброса идентификатора таблицы {nameTable} в БД! - {ex.Message}", StatusLog.Errors);
                    }
                }
            }
        }


        /// <summary>
        /// Метод сжимает файл логов БД
        /// </summary>>
        public static void ShrinkFile()
        {

            string nameLog = $"{NameDb}_log";

            string SqlSelect = $"USE [{NameDb}] DBCC SHRINKFILE (N'{nameLog}', 0, TRUNCATEONLY)";

            object _lock = new object();

            lock (_lock)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(SqlSelect, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка сжатия файла логов в БД! - {NameDb} - {ex.Message}", StatusLog.Errors);
                    }
                }
            }
        }





        public static void TimeWork(int time, string name)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Время работы метода {name}: {time} мсек", StatusLog.Inform);
            }           
        }

        public static void TimeWorkBool(bool th, string name)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Работа потока {name}: {th}", StatusLog.Inform);
            }               
        }

        public static void TimeWorkKey(int time, string name, int key)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Время работы метода {name} - key = {key} : {time} мсек", StatusLog.Inform);
            }           
        }

        public static void ThreadInfo(string name, bool status)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Поток: {name} - статус: {status}", StatusLog.Inform);
            }           
        }

        public static void Info(bool name, bool status)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Поток: {name} - статус: {status}", StatusLog.Inform);
            }
        }

        public static void Info(string name, bool status)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"Поток: {name} - статус: {status}", StatusLog.Inform);
            }
        }


        public static void TimeWork(bool b1, bool b2)
        {
            object _lock = new object();

            lock (_lock)
            {
                new Loggings().WriteLogAdd($"- {b1} - {b2}", StatusLog.Inform);
            }
        }
    }
}
