using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus_Work.Math
{
    internal class StatusChannelMvk
    {
        public string IpAddress {  get; set; }

        public int Channel {  get; set; }

        public float ValueVoltage { get; set; }

        public UInt32 StatusChannel {  get; set; }


        public StatusChannelMvk(string ipAddress, int channel, float valueVoltage, UInt32 statusChannel) 
        {
            IpAddress = ipAddress;
            Channel = channel;
            ValueVoltage = valueVoltage;
            StatusChannel = statusChannel;
        }
    }
}
