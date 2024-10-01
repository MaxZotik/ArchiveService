using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Enums
{
    internal enum Register
    {
        VoltageOnSensor = 8192,             //Таблица 3.5 Параметр 1 "Напряжение на датчике"
        ChannelStatusIndicators = 8350      //Таблица 3.5 Параметр 80 "Индикатор состояния канала"
    }
}
