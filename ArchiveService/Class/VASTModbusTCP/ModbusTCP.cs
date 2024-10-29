using ArchiveService.Class.Log;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using ArchiveService.Class.Enums;
using System.Net;
using ArchiveService.Class.Database.MS_SQL;
using ArchiveService.Class.Patterns;

namespace ArchiveService.Class.VASTModbusTCP
{
    public class ModbusTCP : Modbus
    {
        private const byte BeginCodeError = 128;
        private Socket socket;
        private IPAddress iPAddress;
        private IPEndPoint endPoint;


        /// <summary>
        /// Конструктор экземпляра
        /// </summary>
        /// <param name="ipAddress">IP адрес "string"</param>
        /// <param name="port">Номер порта, по умолчанию - "502"</param>
        public ModbusTCP(string ipAddress, int port = 502)
        {
            IpAddress = ipAddress;
            iPAddress = IPAddress.Parse(ipAddress);
            endPoint = new IPEndPoint(iPAddress, port);
            IsConnect = false;
        }

        public void ConnectTCP()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(this.endPoint);
                socket.SendTimeout = Constant.DEFAULT_TIME_OUT;
                socket.ReceiveTimeout = Constant.DEFAULT_TIME_OUT;
                IsConnect = true;
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} установлено!", StatusLog.Action);
            }
            catch (SocketException se)
            {
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} не установлено! ConnectTCP (SocketException) - {se.Message}", StatusLog.Errors);
                IsConnect = false;
            }
            catch(Exception ex)
            {                
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} не установлено! ConnectTCP (Exception) - {ex.Message}", StatusLog.Errors);
                IsConnect = false;
            }               
        }

        public void CloseTCP() 
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(true);
            IsConnect = false;
        }

        public bool IsChecked()
        {
            if(socket == null)
            {
                IsConnect = false;
                return IsConnect;
            }

            try
            {
                byte[] temp = new byte[1];

                socket.Blocking = false;
                socket.Send(temp, 0, 0);

                if (!socket.Connected)
                {
                    IsConnect = false;
                    //socket = null;
                }
   
            }
            catch (SocketException ex)
            {
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} разорвано! {ex.Message}", StatusLog.Errors);
                IsConnect = false;
            }
            finally 
            {
                socket.Blocking = true;
            }
            
            return IsConnect;
        }

        /// <summary>
        /// Метод отправки и получения пакета Modbus
        /// </summary>
        /// <param name="packet">Пакет для оправки byte[]</param>
        /// <returns>Пакет пулученый byte[]</returns>
        /// <exception cref="IOException">ModbusTCP ошибка - 128</exception>
        private byte[] SendReceive(byte[] packet)
        {
            try
            {
                byte[] mbap = new byte[7];
                byte[] response;
                ulong count;

                socket.Send(packet);
                socket.Receive(mbap, 0, mbap.Length, SocketFlags.None);
                count = mbap[4];
                count <<= Byte;
                count += mbap[5];
                response = new byte[count - 1];
                socket.Receive(response, 0, response.Count(), SocketFlags.None);

                return response;
            }
            catch (SocketException se)
            {
                //IsConnect = false;
                CloseTCP();
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} разорвано! SendReceive {se.Message}", StatusLog.Errors);
                return new byte[1] { 0 };
            }
            catch(Exception ex)
            {
                //IsConnect = false;
                CloseTCP();
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} разорвано! SendReceive {ex.Message}", StatusLog.Errors);
                return new byte[1] { 0 };
            }           
        }


        /// <summary>
        /// Метод отправки и получения пакета Modbus
        /// </summary>
        /// <param name="packet">Пакет для оправки byte[]</param>
        /// <returns>Пакет пулученый byte[]</returns>
        /// <exception cref="IOException">ModbusTCP ошибка - 128</exception>
        protected override byte[] SendReceiveMulti(byte[] packet)
        {
            try
            {
                int count = packet[packet.Length - 1] * 2 + 9;
                byte[] mbap = new byte[count];

                socket.Send(packet);
                socket.Receive(mbap, 0, mbap.Length, SocketFlags.None);

                return mbap;
            }
            catch (SocketException se)
            {
                //IsConnect = false;
                CloseTCP();
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} разорвано! SendReceiveMulti (SocketException) - {se.Message}", StatusLog.Errors);
                return new byte[1] { 0 };
            }
            catch (Exception ex)
            {
                //IsConnect = false;
                CloseTCP();
                new Loggings().WriteLogAdd($"Соединение по IP: {IpAddress} разорвано! SendReceiveMulti (Exception) {ex.Message}", StatusLog.Errors);
                return new byte[1] { 0 };
            }
        }


        /// <summary>
        /// Метод чтения данных от Modbus
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="count"></param>
        /// <returns>Пакет byte[]</returns>
        protected override byte[] ReadMulti(byte function, ushort register, ushort count)
        {
            PacketModbus packetModbus = new PacketModbus();

            byte[] rtn;
            byte[] packet = packetModbus.MakePacket(function, register, count);

            byte[] mbap = packetModbus.MakeMBAP();

            byte[] response = SendReceiveMulti(mbap.Concat(packet).ToArray());

            if (response[0] == 0 && response.Length == 1 || response[8] == 0)
            {
                return new byte[1] { 0 };
            }

            rtn = new byte[response[8]];
            Array.Copy(response, 9, rtn, 0, rtn.Length);

            return rtn;
        }




        /// <summary>
        /// Метод чтения данных от Modbus
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="count"></param>
        /// <returns>Пакет byte[]</returns>
        protected override byte[] Read(byte function, ushort register, ushort count)
        {
            byte[] rtn;
            byte[] packet = MakePacket(function, register, count);
            byte[] mbap = MakeMBAP((ushort)packet.Count());
            byte[] response = SendReceive(mbap.Concat(packet).ToArray());

            if (response[0] == 0)
            {
                return response;
            }

            rtn = new byte[response[1]];
            Array.Copy(response, 2, rtn, 0, rtn.Length);
            return rtn;

        }

        /// <summary>
        /// Метод отправки пакета
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="data"></param>
        /// <exception cref="IOException">Код функции должен быть 16 для записи</exception>
        protected override void Write(byte function, ushort register, byte[] data)
        {
            byte[] packet;
            if (function == FuncForWrite)
                packet = MakePacket(function, register, data);
            else
                throw new IOException("Код функции должен быть 16 для записи");
            SendReceive(MakeMBAP((ushort)packet.Count()).Concat(packet).ToArray());
        }


        protected override byte[] SendReceiveMode()
        {
            byte[] bufferReseive = new byte[256];

            try
            {
                socket.Send(bufferReseive);
                socket.Receive(bufferReseive);

                return bufferReseive;
            }
            catch(Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка: {IpAddress}! {ex.Message}", StatusLog.Errors);
                IsConnect = false;
                return new byte[1] { 128 };
            }
        }


    }
}
