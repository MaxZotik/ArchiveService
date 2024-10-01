using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.VASTModbusTCP.Device.UMKSP_Device
{
    public enum Register
    {
        Power = 33028,
        Voltage = 33056,
        Amperage = 33060
    }
    public class UMKSP : IRegister
    {
        private Register numberRegister;
        private const int DifferenceBetweenChanel = 2;

        public UMKSP(Register Register) => numberRegister = Register;

        public ushort ConvertRegister(ushort chanel)
        {
            int result = (int)numberRegister + (DifferenceBetweenChanel * (chanel - 1));
            return (ushort)result;
        }
    }
}
