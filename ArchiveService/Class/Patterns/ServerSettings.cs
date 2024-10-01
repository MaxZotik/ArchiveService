using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Patterns
{
    public class ServerSettings
    {
        /// <summary>
        /// IP адрес вервера
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Port сервера
        /// </summary>
        public int Port { get; set; }

        public ServerSettings(string ip, int port)
        {
            IP = ip;
            Port = port;
        }
    }
}
