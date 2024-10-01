using ArchiveService.Class.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus_Work.Math
{
    internal static class AddressRegister
    {
        public static string SetupAddress(string channel)
        {
            return ((int)Register.ChannelStatusIndicators + (512 * (int.Parse(channel) - 1))).ToString();
        }

        public static string SetupAddressVoltage(string channel)
        {
            return ((int)Register.VoltageOnSensor + (512 * (int.Parse(channel) - 1))).ToString();
        }
    }
}
