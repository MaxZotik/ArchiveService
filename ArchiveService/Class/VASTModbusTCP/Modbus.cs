using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using ArchiveService.Class.VASTModbusTCP.Device;
using ArchiveService.Class.VASTModbusTCP.Enums;
using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArchiveService.Class.VASTModbusTCP
{
    public abstract class Modbus
    {
        protected byte address;
        private protected const byte FuncForWrite = 16;
        private protected const byte FuncForRead = 3;
        private protected const byte Byte = 8;
        private protected const byte UshortLenth = 2;
        private protected const byte FloatLenth = 4;
        private ushort id = 0;
        public bool IsConnect { get; set; }
        public string IpAddress { get; set; }

        protected abstract byte[] Read(byte function, ushort register, ushort count);
        protected abstract void Write(byte function, ushort register, byte[] data);

        protected abstract byte[] SendReceiveMode();

        protected abstract byte[] SendReceiveMulti(byte[] packet);

        protected abstract byte[] ReadMulti(byte function, ushort register, ushort count);

        /// <summary>
        /// Метод преобразования бит получения от модбаса
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected byte[] MakePacket(byte function, ushort register, ushort count)
        {
            return new byte[] {
                address,                //slave address
                function,               //function code
                (byte)(register >> Byte),  //start register high
                (byte)register,         //start register low
                (byte)(count >> Byte),     //# of registers high
                (byte)count             //# of registers low
            };
        }

        /// <summary>
        /// Метод преобразования бит для отправки модбасу
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected byte[] MakePacket(byte function, ushort register, byte[] data)
        {
            ushort count = (ushort)(data.Count() / UshortLenth);
            byte[] header = new byte[] {
                address,                   //slave address
                function,                  //function code
                (byte)(register >> Byte),  //start register high
                (byte)register,            //start register low
                (byte)(count >> Byte),     //# of registers high
                (byte)count,               //# of registers low
                (byte)data.Count()         //# of bytes to follow
            };
            return header.Concat(data).ToArray();
        }

        protected byte[] MakeMBAP(ushort count)
        {
            byte[] idBytes = BitConverter.GetBytes((short)id);
            return new byte[] {
                idBytes[0],             //message id high byte
                idBytes[1],             //message id low byte
                0,                      //protocol id high byte
                0,                      //protocol id low byte
                (byte)(count >> Byte),  //length high byte
                (byte)(count)           //length low byte
            };
        }

        #region Уточнить необходимость данного метода (Нигде не используется)

        //public float[] ReadHoldingFloat(IRegister register, ushort chanel, Endians endians = Endians.Endians_2301, ushort count = 1)
        //{
        //    byte[] rVal = Read(FuncForRead, register.ConvertRegister(chanel), (ushort)(count * UshortLenth));
        //    float[] values = new float[rVal.Length / FloatLenth];
        //    for (int i = 0; i < rVal.Length; i += FloatLenth)
        //    {
        //        if (endians == Endians.Endians_2301)
        //            values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0); // 1 - 0 - 3 - 2 (работает с МВК 2301)
        //        else if (endians == Endians.Endians_0123)
        //            values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
        //        else
        //            values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
        //    }
        //    return values;
        //}

        #endregion

        /// <summary>
        /// Метод получения расчетных параметров МВК
        /// </summary>
        /// <param name="register">Адрес регистра</param>
        /// <param name="endians">Последовательность передачи байт</param>
        /// <param name="count">Коэфициент длинны пакета. По умолчанию = 1</param>
        /// <returns>Возвращает расчетный параметр МВК</returns>
        public float[] ReadHoldingFloat(ushort register, Endians endians = Endians.Endians_2301, ushort count = 1)
        {
            byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));

            if (rVal[0] == 0 && rVal.Length == 1)
            {
                return new float[1] { 0 };
            }

            float[] values = new float[rVal.Length / 4];

            try
            {
                //byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));
                //float[] value = new float[rVal.Length/4];

                for (int i = 0; i < rVal.Length; i += FloatLenth)
                {
                    if (endians == Endians.Endians_2301)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                    }
                    else if (endians == Endians.Endians_0123)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
                    }
                    else
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка преобразования полученных расчетных параметров МВК! {ex.Message}", StatusLog.Errors);
            }
            
            return values;
        }

        /// <summary>
        /// Метод получения параметров счетчика выполненых процедур МВК
        /// </summary>
        /// <param name="register">Адрес регистра</param>
        /// <param name="endians">Последовательность передачи байт</param>
        /// <param name="count">Коэфициент длинны пакета. По умолчанию = 1</param>
        /// <returns>Возвращает состояние счетчика выполненых процедур</returns>
        public uint ReadHoldingUInt(ushort register, Endians endians = Endians.Endians_2301, ushort count = 1)
        {
            UInt32 values = 0;

            try
            {
                //byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));

                byte[] rVal = ReadMulti(Constant.FUNC_FOR_READ, register, (ushort)(count * Constant.USHORT_LENGTH)); ;

                if (rVal[0] == 0 && rVal.Length == 1)
                {
                    return values;
                }

                for (int i = 0; i < rVal.Length; i += Constant.FLOAT_LENGTH)
                {
                    if (endians == Endians.Endians_2301)
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                    }
                    else if (endians == Endians.Endians_0123)
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
                    }
                    else
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка преобразования полученных расчетных параметров счетчика выполненых процедур МВК! {ex.Message}", StatusLog.Errors);
            }
            
            return values;
        }


        public void ReadHoldingMode()
        {           
            try
            {
                byte[] packet = SendReceiveMode();

                if (packet[0] == 128 && packet.Length == 1)
                {
                    return;
                }

                for (int dev = 0; dev < ModbusClientDatabase.DeviceNumber.Count; dev++)
                {
                    int numberEquip = ModbusClientDatabase.DeviceNumber[dev];

                    int temp = 8 + ModbusClientDatabase.DeviceNumber[dev] * 2;

                    for (int k = 0; k < ModbusClientDatabase.DeviceSettings.Count; k++)
                    {
                        if (ModbusClientDatabase.DeviceSettings[k].Equipment == numberEquip)
                        {
                            ModbusClientDatabase.DeviceSettings[k].ModeWork = (int)packet[temp];
                        }
                    }

                    temp++;
                }
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка получения режима работы МВК! {ex.Message}", StatusLog.Errors);
            }
        }


        /// <summary>
        /// Метод получения расчетных параметров МВК
        /// </summary>
        /// <param name="register">Адрес регистра</param>
        /// <param name="endians">Последовательность передачи байт</param>
        /// <param name="count">Количество требуемых регистров. По умолчанию = 1</param>
        /// <returns>Возвращает расчетный параметр МВК</returns>
        public float[] ReadHoldingFloatMulti(ushort register, Endians endians = Endians.Endians_2301, ushort count = 1)
        {
            try
            {
                byte[] rVal = ReadMulti(Constant.FUNC_FOR_READ, register, (ushort)(count * Constant.USHORT_LENGTH));

                //new Loggings().WriteLogAdd($"ReadHoldingFloat - rVal: {rVal.Length}", StatusLog.Errors);

                float[] values = new float[rVal.Length / 4];

                //new Loggings().WriteLogAdd($"ReadHoldingFloat - values: {values.Length}", StatusLog.Errors);

                for (int i = 0; i < rVal.Length; i += Constant.FLOAT_LENGTH)
                {
                    if (endians == Endians.Endians_2301)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                    }
                    else if (endians == Endians.Endians_0123)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
                    }
                    else
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                    }
                }

                //for (int i = 0; i < values.Length; i++)
                //{
                //    new Loggings().WriteLogAdd($"{values[i]}", StatusLog.Action);
                //}

                return values;
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка преобразования полученных расчетных параметров! {ex.Message}", StatusLog.Errors);
            }




            return Array.Empty<float>();
        }






        public void WriteHolding(ushort register, ushort[] data)
        {
            byte[] bdata = new byte[data.Count() * UshortLenth];
            int i = 0;
            foreach (ushort item in data)
            {
                bdata[i] = (byte)(item >> Byte);
                bdata[i + 1] = (byte)item;
                i += UshortLenth;
            }
            Write(FuncForWrite, register, bdata);
        }


        #region Уточнить необходимость данного метода (Нигде не используется)

        //public void WriteHolding(ushort register, ushort data) => WriteHolding(register, new ushort[] { data });

        #endregion

        public void WriteHoldingFloat(ushort register, float[] data)
        {
            byte[] bdata = new byte[data.Count() * FloatLenth];
            byte[] mydata;
            int i = 0;
            foreach (float item in data)
            {
                mydata = BitConverter.GetBytes(item);
                bdata[i + 1] = mydata[0];
                bdata[i] = mydata[1];
                bdata[i + 3] = mydata[2];
                bdata[i + 2] = mydata[3];
                i += FloatLenth;
            }
            Write(FuncForWrite, register, bdata);
        }

        public void WriteHoldingFloat(ushort register, float data) => WriteHoldingFloat(register, new float[] { data });

        #region Уточнить необходимость данного метода (Нигде не используется)

        //public void WriteHoldingFloat(ushort register, double data) => WriteHoldingFloat(register, (float)data);

        #endregion
    }
}
