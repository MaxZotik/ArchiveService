using System.Net.Sockets;
using System.Net;
using System;
using ArchiveService.Class.Log;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Patterns;
using System.Collections.Generic;
using ArchiveService.Class.Configuration;
using System.Runtime.CompilerServices;
using MongoDB.Driver.Core.Operations;

namespace VastModbusTCPServer
{
    public class ModbusTCPServer
    {
        private static string ip;
        private static byte[] register = { 0, 0, 0 };
        private byte ReadFunction = 3;
        private byte ErrorCode = 2;
        private byte Message = 7;
        private byte SecondElement = 2;
        private byte ThirdEleement = 3;
        private byte MessageError = 3;
        private byte StartError = 128;

        private List<Byte[]> Equipment; 

        [Obsolete]
        static ModbusTCPServer()
        {
            ip = LocalIPAddress.GetCurrentIPAddress();
            //Equipment = new List<Byte>();
        }

        //public ModbusTCPServer() => Equipment = new List<Byte>();

        #region Старый метод "StartServer()"
        /// <summary>
        /// Метод запускает сервер для соединения со SCADA
        /// </summary>
        //public void StartServer()
        //{
        //    IPAddress IP = IPAddress.Parse(ip);
        //    IPEndPoint endPoint = new IPEndPoint(IP, 1502);
        //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    socket.Bind(endPoint);
        //    socket.Listen(10);
        //    while (true)
        //    {
        //        try
        //        {
        //            Socket listener = socket.Accept();
        //            byte[] buffer = new byte[256];
        //            int size = 0;
        //            do
        //            {
        //                size = listener.Receive(buffer);
        //                byte[] buffer_message = buffer;
        //                buffer_message[5] = Message;
        //                if (buffer_message[7] == ReadFunction)
        //                {
        //                    if (buffer_message[9] == SecondElement)
        //                    {
        //                        buffer_message[8] = (byte)(2 * (buffer[11] + 2)); //для SCADA
        //                        buffer_message[9] = register[1];
        //                        buffer_message[11] = 0;
        //                    }
        //                    else if (buffer_message[9] == ThirdEleement)
        //                    {
        //                        buffer_message[8] = (byte)(2 * (buffer[11] + 2)); //для SCADA
        //                        buffer_message[9] = register[2];
        //                        buffer_message[11] = 0;
        //                    }
        //                    else
        //                    {
        //                        buffer_message[5] = MessageError;
        //                        buffer_message[7] = (byte)(buffer_message[7] + StartError);
        //                        buffer_message[8] = ErrorCode;
        //                        buffer_message[9] = 0;
        //                        buffer_message[11] = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    if (buffer_message[14] == 1)
        //                    {
        //                        register[1] = 0;
        //                        register[2] = 0;
        //                    }
        //                }
        //                listener.SendTo(buffer_message, endPoint);

        //                new Loggings().WtiteLogService($"ModbusTCPServer - StartServer()", StatusLog.Inform);

        //                //foreach (byte b in buffer_message)
        //                //{
        //                //    new Loggings().WtiteLogService($"{ b }", StatusLog.Inform);
        //                //}


        //            }
        //            while (listener.Available > 0);

        //            listener.Shutdown(SocketShutdown.Both);
        //            listener.Close();
        //        }
        //        catch (SocketException ex) 
        //        {
        //            new Loggings().WtiteLogService($"ModbusTCPServer - StartServer() - catch (SocketException ex) - { ex.Message }", StatusLog.Errors);
        //        }
        //    }
        //}

        #endregion


        #region Новый метод "StartServer()"
        /// <summary>
        /// Метод запускает сервер для соединения со SCADA
        /// </summary>
        public void StartServer()
        {
            IPAddress IP = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(IP, 1502);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(10);
            while (true)
            {
                Equipment = new List<Byte[]>(); //[0] - Equipment, [1] - Mode Work, [2] - Code Work

                try
                {
                    Socket listener = socket.Accept();
                    byte[] buffer = new byte[3];
                    //int size = 0;

                    do
                    {
                        listener.Receive(buffer);
                        Equipment.Add(buffer);

                        //size++;
                    }
                    while (socket.Available > 0);

                    //while (size < ConfigurationManager.CountEquipment);

                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
                catch (SocketException ex)
                {
                    new Loggings().WriteLogAdd($"ModbusTCPServer - StartServer() - catch (SocketException ex) - {ex.Message}", StatusLog.Errors);
                }

                SetModeWork();
            }
        }

        #endregion

        //public void EditRegisterData(int index, byte value) => register[index] = value;

            public static void EditRegisterData(int index, byte value) => register[index] = value;
        

        private void SetModeWork()
        {
            foreach (ModbusClientDatabase mkd in ModbusClientDatabase.DeviceSettings)
            {
                for (int i = 0; i < Equipment.Count; i++)
                {
                    if (mkd.Equipment == Equipment[0][i])
                    {                       
                        mkd.ModeWork = (int)Equipment[0][1];
                    }
                }
            }
        }
    }
}
