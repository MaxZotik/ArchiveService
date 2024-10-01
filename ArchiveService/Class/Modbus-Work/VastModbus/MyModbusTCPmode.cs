using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using ArchiveService.Class.VASTModbusTCP.Enums;
using ArchiveService.Class.VASTModbusTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VastModbusTCPServer;
using ArchiveService.Class.Configuration;

namespace ArchiveService.Class.Modbus_Work.VastModbus
{
    internal class MyModbusTCPmode
    {
        private Modbus modbusClient;
        private ServerSettings serverSettings;
        private ClientTimeOut clientTimeOut = new ClientTimeOut();

        public MyModbusTCPmode()
        {
            serverSettings = ConfigurationManager.GetServerSettings();
            modbusClient = new ModbusTCP(serverSettings.IP, serverSettings.Port);
        }


        private void StartConnect()
        {
            (modbusClient as ModbusTCP).ConnectTCP();
        }

        public void MVKtempOneMode()
        {
            if (modbusClient.IsConnect)
            {
                modbusClient.ReadHoldingMode();
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
    }
}
