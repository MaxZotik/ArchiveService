using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.VASTModbusTCP.Device
{
    public interface IRegister
    {
        ushort ConvertRegister(ushort chanel);
    }
}
