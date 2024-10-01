using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus_Work.Math
{
    internal static class ConstantMath
    {
        #region Индикаторы состояния канала

        public const UInt32 OVERLOAD_MEASURING_INPUT = 1;       //Таблица 3.7.7 Бит 0
        public const UInt32 BUFFER_FOR_FREQUENCY_16 = 16;        //Таблица 3.7.7 Бит 4
        public const UInt32 BUFFER_FOR_FREQUENCY_64 = 32;        //Таблица 3.7.7 Бит 5
        public const UInt32 BUFFER_FOR_FREQUENCY_512 = 64;       //Таблица 3.7.7 Бит 6
        public const UInt32 CABLE_BREAKAGE = 1024;              //Таблица 3.7.7 Бит 10
        public const UInt32 ELECTRICAL_SHORT = 2048;            //Таблица 3.7.7 Бит 11

        #endregion
    }
}
