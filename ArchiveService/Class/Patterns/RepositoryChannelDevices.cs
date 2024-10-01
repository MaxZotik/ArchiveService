using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Patterns
{
    public class RepositoryChannelDevices
    {
        /// <summary>
        /// Словарь параметров МВК по каналу МВК
        /// </summary>
        public Dictionary<int, List<ModbusClientDatabase>> DatabaseDictionaryChannel { get; set; }

        /// <summary>
        /// IP адрес устройства МВК
        /// </summary>
        public string IPaddress {  get; set; }

        public RepositoryChannelDevices(string ipAddress)
        {
            IPaddress = ipAddress;
            DatabaseDictionaryChannel = GetChannelDevices();
        }

        /// <summary>
        /// Метод создает и заполняет словарь устройств МВК с заданным IP адресом с ключем по номеру канала МВК
        /// </summary>
        /// <returns>Возвращает словарь устройств МВК по IP адресу с ключем по каналу МВК</returns>
        private Dictionary<int, List<ModbusClientDatabase>> GetChannelDevices()
        {
            Dictionary<int, List<ModbusClientDatabase>> dict = new Dictionary<int, List<ModbusClientDatabase>>();

            for (int i = 0; i < ModbusClientDatabase.DeviceSettings.Count; i++)
            {
                if (ModbusClientDatabase.DeviceSettings[i].IPAddress == IPaddress)
                {
                    if (!dict.ContainsKey(ModbusClientDatabase.DeviceSettings[i].Chanel) || dict.Count == 0)
                    {
                        dict.Add(ModbusClientDatabase.DeviceSettings[i].Chanel, new List<ModbusClientDatabase>() { ModbusClientDatabase.DeviceSettings[i] });
                    }
                    else
                    {
                        for (int k = 0; k < dict[ModbusClientDatabase.DeviceSettings[i].Chanel].Count; k++)
                        {
                            if (dict[ModbusClientDatabase.DeviceSettings[i].Chanel][k].StartAddress < ModbusClientDatabase.DeviceSettings[i].StartAddress)
                            {
                                dict[ModbusClientDatabase.DeviceSettings[i].Chanel].Insert(k, ModbusClientDatabase.DeviceSettings[i]);
                                break;
                            }
                            else if (k == dict[ModbusClientDatabase.DeviceSettings[i].Chanel].Count - 1)
                            {
                                dict[ModbusClientDatabase.DeviceSettings[i].Chanel].Add(ModbusClientDatabase.DeviceSettings[i]);
                                break;
                            }
                        }                       
                    }
                }                
            }

            return dict;
        }


        public void SetValueDevice(float[] value, int channel)
        {
            int address = DatabaseDictionaryChannel[channel][0].StartAddress;
            DatabaseDictionaryChannel[channel][0].MeasumentValue = value[0];
            int count = 1;

            for (int i = 1; i < DatabaseDictionaryChannel[channel].Count; i++)
            {

                while (count < value.Length)
                {
                    address += 2;
                    
                    if (DatabaseDictionaryChannel[channel][i].StartAddress == address)
                    {
                        DatabaseDictionaryChannel[channel][0].MeasumentValue = value[count];
                        count++;
                        break;
                    }

                    count++;
                }

            }
        }

    }
}
