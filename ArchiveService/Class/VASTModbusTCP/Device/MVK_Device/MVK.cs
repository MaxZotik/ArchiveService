using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.VASTModbusTCP.Device.MVK_Device
{
    public enum Register
    {
        PickFactor_25Hz = 8284,
        VibroAcceleration_25Hz = 8286,
        VibroAcceleration_10_3000Hz = 8288,
        VibroSpeed_10_3000Hz = 8290,
        VibroMoving_10_3000Hz = 8292,
        VibroAcceleration_10_2000Hz = 8294,
        VibroSpeed_10_2000Hz = 8296,
        VibroMoving_10_2000Hz = 8298,
        VibroAcceleration_10_1000Hz = 8300,
        VibroSpeed_10_1000Hz = 8302,
        VibroMoving_10_1000Hz = 8304,
        VibroAcceleration_2_1000Hz = 8306,
        VibroSpeed_2_1000Hz = 8308,
        VibroMoving_2_1000Hz = 8310,
        VibroAcceleration_300Hz = 8312,
        VibroSpeed_300Hz = 8314,
        VibroMoving_300Hz = 8316,
        VibroAcceleration_150Hz = 8318,
        VibroSpeed_150Hz = 8320,
        VibroMoving_150Hz = 8322,
        VibroAcceleration_Filter1 = 8324,
        VibroSpeed_Filter1 = 8326,
        VibroMoving_Filter1 = 8328,
        VibroAcceleration_Filter2 = 8330,
        VibroSpeed_Filter2 = 8332,
        VibroMoving_Filter2 = 8334,
        StatusChanel = 8350
    }
    public class MVK : IRegister
    {
        private static Register numberRegister;
        private const int DifferenceBetweenChanel = 512;
        public MVK(Register register) => numberRegister = register;
        public ushort ConvertRegister(ushort chanel)
        {
            int resualt = (int)numberRegister + (DifferenceBetweenChanel * (chanel - 1));
            return (ushort)resualt;
        }
    }
}
