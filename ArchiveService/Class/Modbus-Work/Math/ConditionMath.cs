using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Modbus_Work.Math
{
    internal class ConditionMath
    {
        private static List<StatusChannelMvk> conditionMVK = new List<StatusChannelMvk>();
        public static bool StatusChannel(StatusChannelMvk statusChannelMvk)
        {
            int measuringInput = ((statusChannelMvk.StatusChannel & ConstantMath.OVERLOAD_MEASURING_INPUT) == ConstantMath.OVERLOAD_MEASURING_INPUT) ? 1 : 0;
            int cableBreakage = ((statusChannelMvk.StatusChannel & ConstantMath.CABLE_BREAKAGE) == ConstantMath.CABLE_BREAKAGE) ? 1 : 0;
            int electricalShort = ((statusChannelMvk.StatusChannel & ConstantMath.ELECTRICAL_SHORT) == ConstantMath.ELECTRICAL_SHORT) ? 1 : 0;
            int bufferForFrequency16 = ((statusChannelMvk.StatusChannel & ConstantMath.BUFFER_FOR_FREQUENCY_16) == ConstantMath.BUFFER_FOR_FREQUENCY_16) ? 1 : 0;
            int bufferForFrequency64 = ((statusChannelMvk.StatusChannel & ConstantMath.BUFFER_FOR_FREQUENCY_64) == ConstantMath.BUFFER_FOR_FREQUENCY_64) ? 1 : 0;
            int bufferForFrequency512 = ((statusChannelMvk.StatusChannel & ConstantMath.BUFFER_FOR_FREQUENCY_512) == ConstantMath.BUFFER_FOR_FREQUENCY_512) ? 1 : 0;

            //UInt32 value, ModbusClientDatabase mvk, float voltageValue

            bool temp = (measuringInput == 0 && cableBreakage == 0 && electricalShort == 0 && bufferForFrequency16 == 0 &&
                bufferForFrequency64 == 0 && bufferForFrequency512 == 0) ? true : false;

            if (!temp)
            {
                bool result = false;

                for (int i = 0; i < conditionMVK.Count; i++)
                {
                    if (conditionMVK[i].IpAddress == statusChannelMvk.IpAddress && conditionMVK[i].Channel == statusChannelMvk.Channel)
                    {
                        result = true;
                        break;
                    }
                }

                if (!result)
                {
                    conditionMVK.Add(statusChannelMvk);

                    if (measuringInput != 0)
                        WriteStatusError(statusChannelMvk, $"Перегрузка измерительного входа!");
                    if (cableBreakage != 0)
                        WriteStatusVoltageError(statusChannelMvk, $"Постоянное напряжение на датчике больше максимально допустимого, вероятен обрыв кабеля!");
                    if (electricalShort != 0)
                        WriteStatusVoltageError(statusChannelMvk, $"Постоянное напряжение на датчике меньше минимально допустимого, вероятно короткое замыкание в кабеле!");
                    if (bufferForFrequency16 != 0)
                        WriteStatusError(statusChannelMvk, $"Еще не заполнен буфер для частоты дискретизации 16 Гц; фильтры с нижней частотой менее 3.9 Гц не активны!");
                    if (bufferForFrequency64 != 0)
                        WriteStatusError(statusChannelMvk, $"Еще не заполнен буфер для частоты дискретизации 64 Гц; фильтры с нижней частотой менее 25 Гц не активны!");
                    if (bufferForFrequency512 != 0)
                        WriteStatusError(statusChannelMvk, $"Еще не заполнен буфер для частоты дискретизации 512 Гц; все фильтры не активны!");
                }
            }
            else
            {
                int result = -1;

                for (int i = 0; i < conditionMVK.Count; i++)
                {
                    if (conditionMVK[i].IpAddress == statusChannelMvk.IpAddress && conditionMVK[i].Channel == statusChannelMvk.Channel)
                    {
                        result = i;
                        break;
                    }
                }

                if (result != -1)
                {
                    try
                    {
                        conditionMVK.RemoveAt(result);
                        WriteStatusError(statusChannelMvk, $"Работа измерительного канала востановлена!");
                    }
                    catch (Exception ex)
                    {
                        new Loggings().WriteLogAdd($"{ex.Message}", StatusLog.Errors);
                    }
                }
            }

            return temp;
        }

        private static void WriteStatusError(StatusChannelMvk mvk, string str)
        {
            new Loggings().WriteLogAdd($"IP: {mvk.IpAddress}, Канал: {mvk.Channel}, Состояние канала: {str}", StatusLog.Errors);
        }

        private static void WriteStatusVoltageError(StatusChannelMvk mvk, string str)
        {
            new Loggings().WriteLogAdd($"IP: {mvk.IpAddress}, Канал: {mvk.Channel}, Состояние канала: {str}, Напряжение: {mvk.ValueVoltage}", StatusLog.Errors);
        }
    }
}
