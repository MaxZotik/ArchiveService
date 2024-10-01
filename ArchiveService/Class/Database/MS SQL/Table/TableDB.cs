using ArchiveService.Class.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public class TableDB
    {
        public string TableName { get; set; }                      

        public int Time { get; set; }               

        public string TimeMesuament { get; set; }

        public int TimeValue { get; set; }

        public string TimeAverage { get; set; }

        public DateTime TimeTable {  get; set; }

        public static List<TableDB> TableDBList { get; set; }

        static TableDB() 
        {
            TableDBList = ConfigurationManager.GetTableConfigurated();
        }
        
        public TableDB(string tableName, int time, string timeMesuament, int timeValue, string timeAverage) 
        { 
            TableName = tableName;
            Time = time;
            TimeMesuament = timeMesuament;
            TimeValue = timeValue;
            TimeAverage = timeAverage;
            TimeTable = DateTime.Now;
        }
    }                   
}
