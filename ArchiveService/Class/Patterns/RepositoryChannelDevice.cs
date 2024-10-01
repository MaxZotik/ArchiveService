using Amazon.Runtime.Internal.Transform;
using ArchiveService.Class.VASTModbusTCP.Device.MVK_Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveService.Class.Log;
using ArchiveService.Class.Enums;

namespace ArchiveService.Class.Patterns
{
    internal class RepositoryChannelDevice
    {
        /// <summary>
        /// Словарь параметров МВК по каналу МВК
        /// </summary>
        public Dictionary<int, List<ModbusClientDatabase>> DatabaseDictionaryChannel { get; set; }

        /// <summary>
        /// IP адрес устройства МВК
        /// </summary>
        public string IPaddress { get; set; }

        public RepositoryChannelDevice(string ipAddress)
        {
            IPaddress = ipAddress;
            DatabaseDictionaryChannel = GetChannelDevices();
            //PrintDevice(1);
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
                            if (dict[ModbusClientDatabase.DeviceSettings[i].Chanel][k].StartAddress > ModbusClientDatabase.DeviceSettings[i].StartAddress)
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
            int count = 0;

            //new Loggings().WriteLogAdd($"---------------------------------------------", StatusLog.Inform);
            //new Loggings().WriteLogAdd($"SetValueDevice - [0]: {DatabaseDictionaryChannel[channel][0].StartAddress}", StatusLog.Inform);

            //for (int i = 0; i < value.Length; i++)
            //{
            //    new Loggings().WriteLogAdd($"SetValueDevice - [{i}]: value {value[i]}", StatusLog.Inform);
            //}
            

            

            for (int i = 0; i < DatabaseDictionaryChannel[channel].Count; i++)
            {

                while (count < value.Length)
                {
                    if (DatabaseDictionaryChannel[channel][i].StartAddress == address)
                    {
                        DatabaseDictionaryChannel[channel][i].MeasumentValue = value[count];
                        DatabaseDictionaryChannel[channel][i].Time = DateTime.Now;
                        address += 2;
                        count++;
                        break;
                    }

                    address += 2;
                    count++;
                }

            }
        }


        /// <summary>
        /// Метод возвращает адрес первого регистра и количество регистров для опроса до последнего регистра устройства МВК
        /// </summary>
        /// <param name="channel">Номер канала устройства МВК</param>
        /// <returns>Возвращает адрес первого регистра и количество регистров для опроса до последнего регистра устройства МВК</returns>
        public int[] GetFirstAddressAndCountRegister(int channel)
        {
            int[] result = new int[2];

            result[0] = DatabaseDictionaryChannel[channel][0].StartAddress;

            result[1] = ((DatabaseDictionaryChannel[channel][DatabaseDictionaryChannel[channel].Count - 1].StartAddress) - (DatabaseDictionaryChannel[channel][0].StartAddress) + 2) / 2;
            //new Loggings().WriteLogAdd($"GetFirstAddressAndCountRegister - channel: {result[0]}, result: {result[1]}, ", StatusLog.Inform);

            return result;
        }

        public string GetEndianMvk(int channel)
        {
            return DatabaseDictionaryChannel[channel][0].Endians;
        }

        public void PrintDevice(int channel)
        {
            for (int i = 0; i < DatabaseDictionaryChannel[channel].Count; i++)
            {
                new Loggings().WriteLogAdd($"IP: {DatabaseDictionaryChannel[channel][i].IPAddress} | Endian: {DatabaseDictionaryChannel[channel][i].Endians} | Channel: {DatabaseDictionaryChannel[channel][i].Chanel} | Address: {DatabaseDictionaryChannel[channel][i].StartAddress} | Value: {DatabaseDictionaryChannel[channel][i].MeasumentValue}", StatusLog.Inform);
            }
        }
    }
}
