using ArchiveService.Class.Patterns;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net.NetworkInformation;
using System.Linq;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.VASTModbusTCP;
using System.Threading;
using System;
using ArchiveService.Class.Database.MS_SQL.Table;

namespace ArchiveService.Class.Configuration
{
    internal static class ConfigurationManager
    {
        /// <summary>
        /// Содержит количество оборудвания "Device"
        /// </summary>
        public static int CountThread { get; private set; }

        /// <summary>
        /// Содержит количество оборудвания "Equipment"
        /// </summary>
        public static int CountEquipment { get; private set; }

        static ConfigurationManager()
        {
            CountThread = GetCountItemsInSettings();
            CountEquipment = GetCountEquipment();
        }

        /// <summary>
        /// Метод получения настроек подключения к БД из файла "DatabaseSettings"
        /// </summary>
        /// <param name="database">Список для храниения настроек подключения к БД</param>
        public static void GetDatabaseConfigurated(out List<DatabaseClient> database)
        {
            database = new List<DatabaseClient>();
            object _lock = new object();

            lock (_lock)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\DatabaseSettings.xml");
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                    {
                        if (element.GetAttribute("name") != GetCurrentVersionServer())
                            continue;
                        else
                            database.Add(new DatabaseClient(
                                element.GetAttribute("name"),
                                element.ChildNodes[1].InnerText, 
                                element.ChildNodes[2].InnerText,
                                element.ChildNodes[3].InnerText, 
                                element.ChildNodes[4].InnerText));
                    }
                }
            }
        }

        /// <summary>
        /// Метод получает активную БД из файла DatabaseSettings.xml
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVersionServer()
        {
            object _lock = new object();
            string current = "";

            lock (_lock)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\DatabaseSettings.xml");
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                    {
                        if (element.ChildNodes[0].InnerText != "active")
                            continue;
                        else
                            current = element.GetAttribute("name");
                    }
                }
            }
                
            return current.Trim();
        }

        #region Уточнить необходимость данного метода (Нигде не используется)

        public static string GetCurrentEndians(in int index)
        {
            object _lock = new object();
            string current = "";

            lock (_lock)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load($@"{ AppDomain.CurrentDomain.BaseDirectory }Settings\MVKSettings.xml");
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                    {
                        if (element.Attributes.GetNamedItem("Index").Value == index.ToString())
                        {
                            current = element.ChildNodes[1].InnerText;
                            break;
                        }
                        else
                            continue;
                    }
                }
            }

            return current;
        }

        #endregion

        /// <summary>
        /// Метод получает количество оборудования "Device" из файла "MVKSettings"
        /// </summary>
        /// <returns>Количество оборудования</returns>
        public static int GetCountItemsInSettings()
        {
            object _lock = new object();
            int count = 0;

            lock (_lock)
            {               
                XmlDocument xml = new XmlDocument();
                
                try
                {
                    xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\MVKSettings.xml");
                }
                catch(Exception ex)
                {
                    new Loggings().WriteLogAdd(ex.Message, StatusLog.Errors);
                }
                
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Метод получения настроек подключения MVK из файла "MVKSettings.xml"
        /// </summary>
        /// <param name="modbus">Список для заполнения и возврата</param>
        public static List<ModbusClientDatabase> GetMVKConfigurated()
        {
            List<ModbusClientDatabase> modbus = new List<ModbusClientDatabase>();
            object _lock = new object();

            lock (_lock)
            {
                XmlDocument xml = new XmlDocument();
                
                xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\MVKSettings.xml");
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                    {
                        modbus.Add(new ModbusClientDatabase(
                            0,
                            element.ChildNodes[0].InnerText,
                            element.ChildNodes[1].InnerText,
                            element.ChildNodes[2].InnerText,
                            element.ChildNodes[3].InnerText,
                            int.Parse(element.ChildNodes[4].InnerText),
                            int.Parse(element.ChildNodes[5].InnerText),
                            int.Parse(element.ChildNodes[6].InnerText),
                            int.Parse(element.ChildNodes[7].InnerText),
                            int.Parse(element.ChildNodes[8].InnerText),
                            element.ChildNodes[9].InnerText,
                            element.ChildNodes[10].InnerText));
                    }
                }
            }

            return modbus;
        }

        /// <summary>
        /// Метод подсчитывает количество оборудования "Equipment" из файла "MVKSettings.xml"
        /// </summary>
        /// <returns>Возвращает количество оборудования</returns>
        private static int GetCountEquipment()
        {
            object _lock = new object();
            int count = 0;

            lock ( _lock )
            {
                XmlDocument xml = new XmlDocument();
                xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\MVKSettings.xml");
                XmlElement root = xml.DocumentElement;

                if (root != null)
                {
                    foreach (XmlElement element in root)
                    {
                        if (count < int.Parse(element.ChildNodes[8].InnerText)) {  count++; }
                    }
                }
            }

            return count;
        }


        public static List<TableDB> GetTableConfigurated()
        {
            List<TableDB> tableDBList = new List<TableDB>();
            
            XmlDocument xml = new XmlDocument();

            xml.Load($@"{AppDomain.CurrentDomain.BaseDirectory}Settings\TimeSaveArchive.xml");
            XmlElement root = xml.DocumentElement;

            if (root != null)
            {
                foreach (XmlElement element in root)
                {
                    tableDBList.Add(new TableDB(
                        element.ChildNodes[0].InnerText,
                        int.Parse(element.ChildNodes[1].InnerText),
                        element.ChildNodes[2].InnerText,
                        int.Parse(element.ChildNodes[3].InnerText),
                        element.ChildNodes[4].InnerText));
                }
            }

            return tableDBList;
        }

        public static List<PeakDB> GetPeakValue()
        {
            List<PeakDB> tableDBList = new List<PeakDB>();

            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}Settings\PeakValue.xml";

            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                var root = xml.DocumentElement;

                if (root != null)
                {
                    tableDBList.Add(new PeakDB(
                        root.ChildNodes[0].InnerText,
                        float.Parse(root.ChildNodes[1].InnerText)));
                }
            }

            return tableDBList;
        }

        public static List<PeakValueStorage> GetPeakValueStorages()
        {
            List<PeakValueStorage> tableList = new List<PeakValueStorage>();

            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}Settings\TimeSavePeakValue.xml";

            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                var root = xml.DocumentElement;

                if (root != null)
                {
                    tableList.Add(new PeakValueStorage(
                        root.ChildNodes[0].InnerText,
                        int.Parse(root.ChildNodes[1].InnerText),
                        root.ChildNodes[2].InnerText));
                }
            }

            return tableList;
        }


        public static ServerSettings GetServerSettings()
        {
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}Settings\ServerSettings.xml";

            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                var root = xml.DocumentElement;

                if (root != null)
                {
                    return new ServerSettings(root.ChildNodes[0].InnerText, int.Parse(root.ChildNodes[1].InnerText));
                }
            }

            return null;
        }
                    
    }
}
