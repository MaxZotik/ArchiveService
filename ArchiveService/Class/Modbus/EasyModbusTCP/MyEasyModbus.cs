using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL;
using ArchiveService.Class.Database.PostgreSQL;
using ArchiveService.Class.Interface;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using EasyModbus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus.EasyModbusTCP
{
    class MyEasyModbus : IModbus
    {
        private static List<ModbusClientPatterns> deviceSettings=new List<ModbusClientPatterns>();
        private static List<ModbusClientDatabase> deviceSaver = new List<ModbusClientDatabase>();
        private float value = 0;
        static bool flag = false;
        private static Mutex mutex = new Mutex(false, "sync");
        private readonly Logger logger = new Logger();
        private readonly MSSQL mssql = new MSSQL();
        private readonly PostgreSQL postgres = new PostgreSQL();
        public void ReadDataWithMVK(int index)
        {
            mutex.WaitOne();
            if (flag != true)
            {
                ConfigurationManager.GetMVKConfigurated(out List<ModbusClientPatterns> device);
                deviceSettings = device;
                flag = true;
            }
                try
                {
                    ModbusClient client = new ModbusClient(deviceSettings[index].IPAddress, 502);
                    client.ConnectionTimeout = 1000;
                    client.Connect();
                    if (client.Connected)
                    {
                        var arraydata = client.ReadHoldingRegisters(deviceSettings[index].StartAddress, 2);
                        value = ModbusClient.ConvertRegistersToFloat(arraydata, deviceSettings[index].Endians == "2301" ? ModbusClient.RegisterOrder.LowHigh : ModbusClient.RegisterOrder.HighLow);
                        if (value != 0)
                        {
                            deviceSaver.Add(new ModbusClientDatabase(value, deviceSettings[index].Parameters, deviceSettings[index].Type, deviceSettings[index].Chanel));
                           SaveDataInDatabase(ref deviceSaver);
                        }
                        client.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteLog($"Поток {Thread.CurrentThread.Name} : {ex.Message}", Enums.StatusLog.Errors);
                }
            mutex.ReleaseMutex();
        }

        public void SaveDataInDatabase(ref List<ModbusClientDatabase> modbus)
        {
            mutex.WaitOne();
            if (ConfigurationManager.GetCurrentVersionServer() == "MSSQL")
                mssql.InsertData(ref modbus);
            else
                postgres.InsertData(ref modbus);
            mutex.ReleaseMutex();
        }
    }
}
