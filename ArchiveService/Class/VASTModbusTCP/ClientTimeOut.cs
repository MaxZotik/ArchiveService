using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.VASTModbusTCP
{
    public class ClientTimeOut
    {
        private DateTime time;
        private double interval;

        public bool TimeOut
        {
            get {

                DateTime now = DateTime.Now;

                if (time <= now)
                {
                    time = now.AddSeconds(interval);
                    return false;                   
                }
                else
                {
                    return true;
                }
            }
        }

        public ClientTimeOut()
        {
            //timeout = false;
            interval = 30;
            time = DateTime.Now;
        }
    }
}
