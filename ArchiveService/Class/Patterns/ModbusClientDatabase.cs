using ArchiveService.Class.Configuration;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Net;

namespace ArchiveService.Class.Patterns
{
    public class ModbusClientDatabase
    {
        public string IPAddress { get; }
        public string Endians { get; }
        public int StartAddress { get; }
        internal float MeasumentValue { get; set; }
        public string Parameters { get; }
        public string Type { get; }
        public int Chanel { get; }
        public int NumberMVK { get; }
        public int ModeWork { get; set; }
        public int Equipment { get; set; }

        public string EquipmentName { get; set; }

        public string DreamChannelName { get; set; }

        public DateTime Time { get; set; }
        public static List<ModbusClientDatabase> DeviceSettings { get; set; }

        /// <summary>
        /// Содержит перечень номеров оборудования (не повторяющиеся)
        /// </summary>
        public static List<int> DeviceNumber { get; set; }

        static ModbusClientDatabase()
        {
            DeviceSettings = ConfigurationManager.GetMVKConfigurated();
            DeviceNumber = SetDeviceNumber();
        }

        public ModbusClientDatabase(float measumentValue, string ipaddress, string endians, string parameters, string type, int chanel, 
                                    int startaddress, int numberMVK, int modeWork, int equipment, string equipmentName, string dreamChannelName)
        {
            MeasumentValue = measumentValue;
            IPAddress = ipaddress;
            StartAddress = startaddress;
            Endians = endians;
            Parameters = parameters;
            Type = type;
            Chanel = chanel;
            NumberMVK = numberMVK;
            ModeWork = modeWork;
            Equipment = equipment;
            EquipmentName = equipmentName;
            DreamChannelName = dreamChannelName;
        }     


        static List<int> SetDeviceNumber()
        {
            List<int> DeviceNumberTemp = new List<int>();

            DeviceNumberTemp.Add(DeviceSettings[0].Equipment);

            for (int i = 1; i < DeviceSettings.Count; i++)
            {
                int temp = -1;

                for (int k = 0; k < DeviceNumberTemp.Count; k++)
                {
                    if (DeviceNumberTemp[k] == DeviceSettings[i].Equipment)
                    {
                        temp = -1;
                        break;
                    }

                    temp = i;
                }

                if (temp != -1)
                {
                    DeviceNumberTemp.Add(DeviceSettings[i].Equipment);
                }
            }

            DeviceNumberTemp.Sort();

            return DeviceNumberTemp;
        }
    }
}
