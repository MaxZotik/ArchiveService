using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Patterns
{
    public class ModbusData
    {
        public string IDParameters { get; set; }

        public string IDFrequency {  get; set; }

        public DateTime Time { get; set; }

        public float MVKValue { get; set; }

        public int Chanel {  get; set; }

        public int MVKNumber { get; set; }

        public int ModeWork { get; set; }



        public ModbusData(string idParameters, string idFrequency, DateTime time, float mvkValue, int chanel, int mvkNumber, int modeWork) 
        {
            IDParameters = idParameters;
            IDFrequency = idFrequency;
            Time = time;
            MVKValue = mvkValue;
            Chanel = chanel;
            MVKNumber = mvkNumber;
            ModeWork = modeWork;
        }

    }
}
