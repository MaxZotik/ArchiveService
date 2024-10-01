using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL;
using ArchiveService.Class.Database.MS_SQL.Table;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Modbus_Work.Math;
using ArchiveService.Class.Patterns;
using ArchiveService.Class.ServerTCP;
using ArchiveService.Class.VASTModbusTCP;
using ArchiveService.Class.VASTModbusTCP.Device.MVK_Device;
using ArchiveService.Class.VASTModbusTCP.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VastModbusTCPServer;


namespace ArchiveService.Class.Modbus_Work.VastModbus
{
    internal class MyModbusTCP
    {
        object _lock = new object();
        //private Server server = new Server();
        //public static List<Modbus> ClientTCP;

        public Modbus modbusClient;
        private int indexNumberMVK;
        public ClientTimeOut clientTimeOut = new ClientTimeOut();
        private RepositoryChannelDevice repositoryChannelDevice;
        public static List<MyModbusTCP> MyModbusTCPList { get; set; }

        static MyModbusTCP()
        {
            MyModbusTCPList = CreateMyModbusTCP();
        }
       
        public MyModbusTCP(Modbus modbusClient, int indexNumberMVK)
        {
            this.modbusClient = modbusClient;
            this.indexNumberMVK = indexNumberMVK;
            this.repositoryChannelDevice = new RepositoryChannelDevice(modbusClient.IpAddress);
        }

        static List<MyModbusTCP> CreateMyModbusTCP()
        {
            int count = DSPCounter.CounterList.Count;

            List<MyModbusTCP> list = new List<MyModbusTCP>(count);

            for (int i = 0; i < count; i++)
            {
                list.Insert(i, new MyModbusTCP(new ModbusTCP(DSPCounter.CounterList[i].IPAddress), i));
            }

            return list;
        }

        public static void Print()
        {
            foreach (MyModbusTCP my in MyModbusTCPList)
                new Loggings().WriteLogAdd($"my.modbusClient.IsConnect = {my.modbusClient.IsConnect}", StatusLog.Inform);
        }


        //public void ReadDataWithMVK(ModbusClientDatabase modbusClientDatabase, Modbus mvk)
        //{
        //    lock (_lock)
        //    {
        //        try
        //        {
        //            if (ReadConditionMVK(modbusClientDatabase, mvk))
        //            {
        //                float[] value = mvk.ReadHoldingFloat(ushort.Parse(modbusClientDatabase.StartAddress.ToString()),
        //                modbusClientDatabase.Endians == "3210" ? Endians.Endians_3210 :
        //                modbusClientDatabase.Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

        //                if (value[0] != 0)
        //                {
        //                    modbusClientDatabase.MeasumentValue = value[0];
        //                    modbusClientDatabase.Time = DateTime.Now;

        //                    RepositoryDatabase.WriteListDB.Add(modbusClientDatabase);                          
        //                }
        //            }                                    
        //        }
        //        catch (Exception ex)
        //        {
        //            new Loggings().WriteLogAdd($"Ошибка преобразования полученных параметров МВК! - {modbusClientDatabase.StartAddress} - {ex.Message}", StatusLog.Errors);
        //            ModbusTCPServer.EditRegisterData(1, 1);
        //        }             
        //    }
        //}

        

        //public bool ReadCounterWithMVK(int index, Modbus mvk)
        //{
        //    lock (_lock)
        //    {
        //        UInt32 value = 0;
        //        bool isChecked = false;

        //        try
        //        {
        //            value = mvk.ReadHoldingUInt(ushort.Parse(DSPCounter.CounterList[index].StartAddress.ToString()),
        //                DSPCounter.CounterList[index].Endians == "3210" ? Endians.Endians_3210 :
        //                DSPCounter.CounterList[index].Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

        //            if (value != 0 && value != DSPCounter.CounterList[index].Parameters)
        //            {
        //                DSPCounter.CounterList[index].Parameters = value;
        //                isChecked = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            new Loggings().WriteLogAdd($"Ошибка преобразования полученных параметров счетчика выполненых процедур МВК! - N : {DSPCounter.CounterList[index].NumberMVK} - {ex.Message}", StatusLog.Errors);
        //        }
             
        //        return isChecked;
        //    }
        //}

        /// <summary>
        /// Метод получения состояния канала МВК
        /// </summary>
        /// <param name="modbusClientDatabase">Устройство МВК</param>
        /// <param name="mvk">Объект клиента TCP</param>
        /// <returns>True - если канал рабочий и False - если канал не работает</returns>
        //public bool ReadConditionMVK(ModbusClientDatabase modbusClientDatabase, Modbus mvk)
        //{
        //    lock (_lock)
        //    {
        //        UInt32 value = 0;
        //        float valueVoltage = 0.0f;

        //        try
        //        {
        //            float[] valueVoltageArray = mvk.ReadHoldingFloat(ushort.Parse(AddressRegister.SetupAddressVoltage(modbusClientDatabase.Chanel.ToString())),
        //                    modbusClientDatabase.Endians == "3210" ? Endians.Endians_3210 :
        //                    modbusClientDatabase.Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

        //            valueVoltage = valueVoltageArray[0];
        //        }
        //        catch (Exception ex)
        //        {
        //            new Loggings().WriteLogAdd($"Ошибка преобразования полученного параметра напряжения на МВК! - {ex.Message}", StatusLog.Errors);
        //        }  

        //        try 
        //        { 
        //            value = mvk.ReadHoldingUInt(ushort.Parse(AddressRegister.SetupAddress(modbusClientDatabase.Chanel.ToString())),
        //                modbusClientDatabase.Endians == "3210" ? Endians.Endians_3210 :
        //                modbusClientDatabase.Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);
        //        }
        //        catch (Exception ex)
        //        {
        //            new Loggings().WriteLogAdd($"Ошибка преобразования полученного параметра состояния канала МВК! - {ex.Message}", StatusLog.Errors);
        //        }

        //        return ConditionMath.StatusChannel(value, modbusClientDatabase, valueVoltage);
        //    }
        //}



        private void PrintTime() 
        {
            object _lock = new object();
            lock (_lock)
            {
                new Loggings().WriteLogAdd($"MVKtempOne(): {modbusClient.IsConnect} - {clientTimeOut.TimeOut}", StatusLog.Errors);
            }
                
        }

        private void StartConnect()
        {
            (modbusClient as ModbusTCP).ConnectTCP();
        }

        #region MVKtempOne()
        //public void MVKtempOne()
        //{
        //    if (modbusClient.IsConnect)
        //    {
        //        if (!ReadCounterWithMVK(indexNumberMVK, modbusClient))
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            int key = DSPCounter.CounterList[indexNumberMVK].NumberMVK;
        //            int count = RepositoryDatabase.DatabaseDictionary[key].Count;

        //            for (int i = 0; i < count; i++)
        //            {
        //                ReadDataWithMVK(RepositoryDatabase.DatabaseDictionary[key][i], modbusClient);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (!clientTimeOut.TimeOut)
        //        {
        //            Thread th = new Thread(StartConnect);
        //            th.Start();
        //        }
        //    }
        //}

        #endregion


        public void MVKtempOneMulti()
        {
            if (modbusClient.IsConnect)
            {               
                if (!ReadCounterWithMvkMulti(modbusClient.IpAddress))
                {
                    return;
                }
                else
                {
                    foreach (var dicKey in repositoryChannelDevice.DatabaseDictionaryChannel)
                    {
                        int[] addressAndCount = repositoryChannelDevice.GetFirstAddressAndCountRegister(dicKey.Key);
                        //new Loggings().WriteLogAdd($"ModbusWorkStart: register: {addressAndCount[0]}, count: {addressAndCount[1]}", StatusLog.Inform);

                        ReadDataWithMvkMulti(repositoryChannelDevice.GetEndianMvk(dicKey.Key), addressAndCount[0], addressAndCount[1], dicKey.Key);
                    }
                }
            }
            else
            {
                if (!clientTimeOut.TimeOut)
                {
                    Thread th = new Thread(StartConnect);
                    th.Start();
                }
            }
        }

        /// <summary>
        /// Метод отправляет запрос к МВК на изменение счетчика выполненых процедур МВК
        /// </summary>
        /// <param name="ipAddress">IP адрес устройства</param>
        /// <returns>ВОзвращает True - счетчик изменился или False - счетчик не изменился</returns>
        public bool ReadCounterWithMvkMulti(string ipAddress)
        {
            lock (_lock)
            {
                UInt32 value = 0;
                bool isChecked = false;
                int index = DSPCounter.GetDspCounterIndex(ipAddress);

                if(index == - 1)
                    return isChecked;

                try
                {
                    value = modbusClient.ReadHoldingUInt(ushort.Parse(DSPCounter.CounterList[index].StartAddress.ToString()),
                        DSPCounter.CounterList[index].Endians == "3210" ? Endians.Endians_3210 :
                        DSPCounter.CounterList[index].Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

                    if (value != 0 && value != DSPCounter.CounterList[index].Parameters)
                    {
                        DSPCounter.CounterList[index].Parameters = value;
                        isChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    new Loggings().WriteLogAdd($"Ошибка преобразования полученных параметров счетчика выполненых процедур МВК! - N: {DSPCounter.CounterList[index].NumberMVK} - {ex.Message}", StatusLog.Errors);
                }

                return isChecked;
            }
        }

        /// <summary>
        /// Метод отправляет запрос к МВК для получения значений регистров
        /// </summary>
        /// <param name="endian">Последовательность передачи Byte</param>
        /// <param name="address">Начальный адрес регистров для опроса</param>
        /// <param name="count">Колличество регистров опроса</param>
        /// <param name="channel">Канал МВК для опроса</param>
        private void ReadDataWithMvkMulti(string endian, int address, int count, int channel)
        {
            try
            {
                    float[] value = modbusClient.ReadHoldingFloatMulti(ushort.Parse(address.ToString()),
                endian == "3210" ? Endians.Endians_3210 : endian == "0123" ? Endians.Endians_0123 : Endians.Endians_2301, (ushort)count);

                repositoryChannelDevice.SetValueDevice(value, channel);

                InsertDevice(channel);

                //repositoryChannelDevice.PrintDevice(channel);
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка преобразования полученных параметров МВК! - {ex.Message}", StatusLog.Errors);
            }
        }

        public bool ReadConditionMvkMulti(string ipAddress, int channel)
        {
            lock (_lock)
            {
                UInt32 value = 0;
                float valueVoltage = 0.0f;

                int index = DSPCounter.GetDspCounterIndex(ipAddress);

                if (index == -1)
                    return false;

                try
                {
                    float[] valueVoltageArray = modbusClient.ReadHoldingFloat(ushort.Parse(AddressRegister.SetupAddressVoltage(channel.ToString())),
                            DSPCounter.CounterList[index].Endians == "3210" ? Endians.Endians_3210 :
                            DSPCounter.CounterList[index].Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

                    valueVoltage = valueVoltageArray[0];
                }
                catch (Exception ex)
                {
                    new Loggings().WriteLogAdd($"Ошибка преобразования полученного параметра напряжения на МВК! - {ex.Message}", StatusLog.Errors);
                }

                try
                {
                    value = modbusClient.ReadHoldingUInt(ushort.Parse(AddressRegister.SetupAddress(channel.ToString())),
                        DSPCounter.CounterList[index].Endians == "3210" ? Endians.Endians_3210 :
                        DSPCounter.CounterList[index].Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);
                }
                catch (Exception ex)
                {
                    new Loggings().WriteLogAdd($"Ошибка преобразования полученного параметра состояния канала МВК! - {ex.Message}", StatusLog.Errors);
                }

                StatusChannelMvk statusChannelMvk = new StatusChannelMvk(ipAddress, channel, valueVoltage, value);

                return ConditionMath.StatusChannel(statusChannelMvk);
            }
        }


        private void InsertDevice(int channel)
        {            
            //RepositoryDatabase.WriteListDB.AddRange(repositoryChannelDevice.DatabaseDictionaryChannel[channel]);

            for (int i = 0; i < ModbusClientDatabase.DeviceSettings.Count; i++)
            {
                for (int k = 0; k < repositoryChannelDevice.DatabaseDictionaryChannel[channel].Count; k++)
                {
                    if (ModbusClientDatabase.DeviceSettings[i].IPAddress == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].IPAddress &&
                        ModbusClientDatabase.DeviceSettings[i].Chanel == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].Chanel &&
                        ModbusClientDatabase.DeviceSettings[i].StartAddress == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].StartAddress)
                    {
                        ModbusClientDatabase.DeviceSettings[i].MeasumentValue = repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].MeasumentValue;
                        ModbusClientDatabase.DeviceSettings[i].Time = repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].Time;

                        //new Loggings().WriteLogAdd($"InsertDevice - IP: {ModbusClientDatabase.DeviceSettings[i].IPAddress};" +
                        //    $"Time: {ModbusClientDatabase.DeviceSettings[i].Time};" +
                        //    $"Channel: {ModbusClientDatabase.DeviceSettings[i].Chanel};" +
                        //    $"StartAddress: {ModbusClientDatabase.DeviceSettings[i].StartAddress};" +
                        //    $"MeasumentValue: {ModbusClientDatabase.DeviceSettings[i].MeasumentValue}", StatusLog.Inform);

                        RepositoryDatabase.WriteListDB.Add(ModbusClientDatabase.DeviceSettings[i]);
                    }
                }
            }

            //new Loggings().WriteLogAdd($"---------------------------------------------", StatusLog.Inform);

            //for (int i = 0; i < ModbusClientDatabase.DeviceSettings.Count; i++) 
            //{
            //    for (int k = 0; k < repositoryChannelDevice.DatabaseDictionaryChannel[channel].Count; k++)
            //    {
            //        if (ModbusClientDatabase.DeviceSettings[i].IPAddress == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].IPAddress &&
            //            ModbusClientDatabase.DeviceSettings[i].Chanel == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].Chanel &&
            //            ModbusClientDatabase.DeviceSettings[i].StartAddress == repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].StartAddress)
            //        {
            //            ModbusClientDatabase.DeviceSettings[i].MeasumentValue = repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].MeasumentValue;
            //            ModbusClientDatabase.DeviceSettings[i].Time = repositoryChannelDevice.DatabaseDictionaryChannel[channel][k].Time;
            //        }
            //    }
            //}
        }
    }
}
