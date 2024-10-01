using ArchiveService.Class.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public class PeakDB
    {
        public string NameAverage { get; set; }
        
        public float CoefficientPeak { get; set; }

        public static List<PeakDB> PeakDBList { get; set; }

        static PeakDB()
        {
            PeakDBList = ConfigurationManager.GetPeakValue();
        }

        public PeakDB(string nameAverage, float coefficientPeak)
        {
            NameAverage = nameAverage;
            CoefficientPeak = coefficientPeak;
        }
    }
}
