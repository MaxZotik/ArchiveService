using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System;

namespace VastModbusTCPServer
{
    static class LocalIPAddress
    {

        /// <summary>
        /// Метод получает IP адрес компьютера для сервера
        /// </summary>
        /// <returns>Строку IP адреса</returns>
        [Obsolete]
        public static string GetCurrentIPAddress()
        {
            return Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        }

    }
}
