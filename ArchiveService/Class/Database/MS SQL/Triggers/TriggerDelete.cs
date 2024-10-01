using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL.Table;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Triggers
{
    public class TriggerDelete
    {
        object _lock = new object();
        public void TriggerMSSQLDelete()
        {
            lock (_lock)
            {
                if (ConfigurationManager.GetCurrentVersionServer() == "MSSQL")
                {
                    for (int i = 0; i < TableDB.TableDBList.Count; i++)
                    {
                        ConnectionMSSQL.DeleteTable(TableDB.TableDBList[i].TableName, TableDB.TableDBList[i].Time, TableDB.TableDBList[i].TimeMesuament);

                        Thread.Sleep(31000);
                    }

                    ConnectionMSSQL.ShrinkFile();
                }
            }
        }

        public void TriggerMSSQLDeletePeak()
        {
            lock (_lock)
            {
                if (ConfigurationManager.GetCurrentVersionServer() == "MSSQL")
                {
                    for (int i = 0; i < PeakValueStorage.PeakValueStoragesList.Count; i++)
                    {
                        ConnectionMSSQL.DeleteTable(PeakValueStorage.PeakValueStoragesList[i].NameTable, 
                            PeakValueStorage.PeakValueStoragesList[i].Time, 
                            PeakValueStorage.PeakValueStoragesList[i].TimeMesuament);
                    }
                }
            }
        }
    }
}
