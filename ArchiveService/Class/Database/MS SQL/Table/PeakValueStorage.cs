using ArchiveService.Class.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public class PeakValueStorage
    {
        public string NameTable {  get; set; }

        public int Time {  get; set; }

        public string TimeMesuament { get; set; }

        private DateTime timeTableCheckidentReseed;

        public bool CheckidentReseed
        {
            get
            {
                DateTime dt = DateTime.Now;

                if (dt >= timeTableCheckidentReseed)
                {
                    timeTableCheckidentReseed = ArchiveMath.CreateDateTime(Time * ConstantDb.COEFFICIENT_TIME_CHECKIDENT, TimeMesuament);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<PeakValueStorage> PeakValueStoragesList { get; set; }

        static PeakValueStorage()
        {
            PeakValueStoragesList = ConfigurationManager.GetPeakValueStorages();
        }

        public PeakValueStorage(string nameTable, int time, string timeMesuament) 
        { 
            NameTable = nameTable;
            Time = time;
            TimeMesuament = timeMesuament;
            timeTableCheckidentReseed = ArchiveMath.CreateDateTime(time * ConstantDb.COEFFICIENT_TIME_CHECKIDENT, timeMesuament);
        }
    }
}
