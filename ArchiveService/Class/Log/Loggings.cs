using ArchiveService.Class.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Class.Log
{
    public class Loggings
    {
        private static string path;
        private static readonly string directory = $@"Loggings\LogsService";
        private static string nameFileService = "app_log.log";
        private static readonly object _lockList = new object();

        public static List<string> FileLoggingsList { get; set; }

        static Loggings()
        {
            path = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryCheck();
            FileLoggingsList = new List<string>();
        }

        /// <summary>
        /// Метод записывает коллекцию логов в файл
        /// </summary>
        public static async void WriteLogFile()
        {
            try
            {
                string pathTemp = $@"{path}{directory}\{nameFileService}";

                if (FileLoggingsList.Count == 0)
                {
                    return;
                }

                List<string> list = new List<string>();

                list = CopyList();
                FileLoggingsListClear();

                using (StreamWriter writer = new StreamWriter(pathTemp, true))
                {
                    try
                    {
                        foreach (string str in list)
                        {
                            await writer.WriteLineAsync(str);
                        }
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"Ошибка записи в файл {nameFileService} логов службы! {ex.Message}", StatusLog.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка записи в файл {nameFileService} логов службы! Файл не записан! {ex.Message}", StatusLog.Errors);
            }


        }

        /// <summary>
        /// Метод записывает логи приложения в коллекцию
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="status">Статус сообщения</param>
        public void WriteLogAdd(in string message, StatusLog status = StatusLog.Inform)
        {
            lock (_lockList)
            {
                string str = $"|{status}| {DateTime.Now} {message}";

                FileLoggingsList.Add(str);
            }
        }

        private static List<string> CopyList()
        {
            lock (_lockList)
            {
                List<string> list = new List<string>();

                list.AddRange(FileLoggingsList);

                return list;
            }
        }


        /// <summary>
        /// Метод удаления записей из списка WriteListDB
        /// </summary>
        private static void FileLoggingsListClear()
        {
            lock (_lockList)
            {
                FileLoggingsList.Clear();
            }
        }


        /// <summary>
        /// Метод проверки наличия директории
        /// </summary>
        private static void DirectoryCheck()
        {
            string dir = $@"{path}{directory}"; 

            if (Directory.Exists(dir) != true)
                Directory.CreateDirectory(dir);
        }

    }
}
