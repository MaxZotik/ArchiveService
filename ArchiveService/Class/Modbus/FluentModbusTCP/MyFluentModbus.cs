using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL;
using ArchiveService.Class.Database.PostgreSQL;
using ArchiveService.Class.Interface;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using FluentModbus;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus.FluentModbusTCP
{
    class MyFluentModbus : IModbus
    {
        private static bool flag=false;
        private static List<ModbusClientPatterns> deviceSettings = new List<ModbusClientPatterns>();
        private static List<ModbusClientDatabase> deviceSaver = new List<ModbusClientDatabase>();
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
            var unitIdentifier = 0xFF;
            ModbusTcpClient client = new ModbusTcpClient(); ;
            try
            {
                Task.Delay(200);
                client.ConnectTimeout = 1000;
                client.Connect(new IPEndPoint(IPAddress.Parse("192.168.8.35"), 502), ModbusEndianness.LittleEndian);
                if (client.IsConnected)
                {
                    var floatdata = client.ReadHoldingRegisters<float>(unitIdentifier, deviceSettings[index].StartAddress, 2);
                    if (floatdata[0] != 0)
                    {
                        deviceSaver.Add(new ModbusClientDatabase(floatdata[0], deviceSettings[index].Parameters, deviceSettings[index].Type, deviceSettings[index].Chanel));
                        if (deviceSaver.Count == 100)
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
