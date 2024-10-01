using ArchiveService.Class.Patterns;
using System.Threading.Tasks;
using VastModbusTCPServer;

namespace ArchiveService.Class.ServerTCP
{
    class Server
    {
        private ModbusTCPServer modbus = new ModbusTCPServer();
        //internal async Task StartServer() => await Task.Run(() => modbus.StartServer());
        //internal async Task EditRegisterData(int index, byte value) => await Task.Run(() => modbus.EditRegisterData(index, value));

        internal void StartServer() => modbus.StartServer();

        internal static void EditRegisterData(int index, byte value) => ModbusTCPServer.EditRegisterData(index, value);
    }
}

