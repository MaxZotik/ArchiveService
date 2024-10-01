using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArchiveService.Class.Patterns
{
    public static class RepositoryDatabase
    {
        /// <summary>
        /// Словарь параметров МВК по номеру МВК
        /// </summary>
        public static Dictionary<int, List<ModbusClientDatabase>> DatabaseDictionary { get; set; }

        /// <summary>
        /// Список ключей (номеров МВК) в словаре
        /// </summary>
        public static List<int> KeysDictionary { get; set; }


        public static List<ModbusClientDatabase> WriteListDB { get; set; }

        static RepositoryDatabase()
        {
            DatabaseDictionary = GetDatabaseDictionary();
            KeysDictionary = GetKeysDictionary();
            WriteListDB = new List<ModbusClientDatabase>();
        }

        /// <summary>
        /// Метод получения списка ключей словаря
        /// </summary>
        /// <returns>Список ключей словаря</returns>
        private static List<int> GetKeysDictionary()
        {
            List<int> keys = new List<int>();

            foreach(var key in DatabaseDictionary)
                keys.Add((int)key.Key);

            return keys;
        }

        /// <summary>
        /// Метод заполнения словаря данными по номерам МВК
        /// </summary>
        /// <returns>Словарь МВК по номерам</returns>
        private static Dictionary<int, List<ModbusClientDatabase>> GetDatabaseDictionary()
        {
            Dictionary<int, List<ModbusClientDatabase>> dict = new Dictionary<int, List<ModbusClientDatabase>> ();

            for (int i = 0; i < ModbusClientDatabase.DeviceSettings.Count; i++)
            {
                if (!dict.ContainsKey(ModbusClientDatabase.DeviceSettings[i].NumberMVK) || dict.Count == 0)
                {
                    dict.Add(ModbusClientDatabase.DeviceSettings[i].NumberMVK, new List<ModbusClientDatabase>() { ModbusClientDatabase.DeviceSettings[i] });
                }
                else
                {
                    dict[ModbusClientDatabase.DeviceSettings[i].NumberMVK].Add(ModbusClientDatabase.DeviceSettings[i]);
                }
            }

            return dict;
        }

        public static void WriteListDBClear()
        {
            WriteListDB.Clear();
        }

        public static List<ModbusData> GetListModbusData()
        {
            List<ModbusData> modbusDatas = new List<ModbusData>();

            for (int i = 0; i < KeysDictionary.Count; i++)
            {
                int key = KeysDictionary[i];

                for (int k = 0; k < DatabaseDictionary[key].Count; k++)
                {
                    if (DatabaseDictionary[key][k].MeasumentValue != 0)
                    {
                        ModbusData md = new ModbusData(
                        DatabaseDictionary[key][k].Parameters,
                        DatabaseDictionary[key][k].Type,
                        DateTime.Now,
                        DatabaseDictionary[key][k].MeasumentValue,
                        DatabaseDictionary[key][k].Chanel,
                        DatabaseDictionary[key][k].NumberMVK,
                        DatabaseDictionary[key][k].ModeWork);

                        modbusDatas.Add(md);
                    }                   
                }
            }

            return modbusDatas;
        }


    }
}
