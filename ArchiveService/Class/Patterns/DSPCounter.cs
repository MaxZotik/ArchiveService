using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Patterns
{
    public class DSPCounter
    {
        public string IPAddress { get; }
        public string Endians { get; }
        public int StartAddress { get; }
        public UInt32 Parameters { get; set; }
        public int NumberMVK { get; }

        public static List<DSPCounter> CounterList { get; set; }

        static DSPCounter()
        {
            CounterList = GetCounterList();
        }

        public DSPCounter(string ipAddress, string endians, int numberMVK, int startAddress = 4132, UInt32 parameter = 0)
        {
            IPAddress = ipAddress;
            Endians = endians;
            NumberMVK = numberMVK;
            StartAddress = startAddress;
            Parameters = parameter;
        }

        private static List<DSPCounter> GetCounterList()
        {
            List<DSPCounter> list = new List<DSPCounter>();

            list.Add(new DSPCounter(
                        ModbusClientDatabase.DeviceSettings[0].IPAddress,
                        ModbusClientDatabase.DeviceSettings[0].Endians,
                        ModbusClientDatabase.DeviceSettings[0].NumberMVK));

            for (int i = 1; i < ModbusClientDatabase.DeviceSettings.Count; i++)
            {
                int temp = -1;

                for (int k = 0; k < list.Count; k++)
                {                   
                    //if (list[k].NumberMVK == ModbusClientDatabase.DeviceSettings[i].NumberMVK)
                    if (list[k].IPAddress == ModbusClientDatabase.DeviceSettings[i].IPAddress)
                    {
                        temp = -1;
                        break;
                    }

                    temp = k;                   
                }

                if (temp != -1)
                {
                    list.Add(new DSPCounter(
                        ModbusClientDatabase.DeviceSettings[i].IPAddress,
                        ModbusClientDatabase.DeviceSettings[i].Endians,
                        ModbusClientDatabase.DeviceSettings[i].NumberMVK));
                }
            }

            return list;
        }

        public static int GetDspCounterIndex(string ipAddress)
        {
            int result = -1;

            for (int i = 0; i < CounterList.Count; i++)
            {
                if (CounterList[i].IPAddress == ipAddress)
                {
                    result = i;
                }
            }

            return result;
        }

        

        public static int GetIndexOfKey(int key)
        {
            int index = 0;

            for (int i = 0; i < CounterList.Count; i ++)
            {
                if (CounterList[i].NumberMVK == key)
                {
                    index = i; 
                    break;
                }
            }

            return index;
        }
    }
}
