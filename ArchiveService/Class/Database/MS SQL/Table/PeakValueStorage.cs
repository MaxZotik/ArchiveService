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
        }
    }
}
