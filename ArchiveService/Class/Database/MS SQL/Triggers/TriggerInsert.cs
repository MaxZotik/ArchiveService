using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Triggers
{
    public class TriggerInsert
    {
        object _lock = new object();
        public void TriggerMSSQLInsert()
        {
            lock (_lock)
            {
                if (ConfigurationManager.GetCurrentVersionServer() == "MSSQL")
                {
                    DataTable dt = Archive.InssertArchive();
                    ConnectionMSSQL.WrireMSSQL(dt, "Archive");
                }
            }
        }
    }
}
