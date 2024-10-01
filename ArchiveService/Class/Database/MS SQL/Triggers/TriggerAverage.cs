using ArchiveService.Class.Configuration;
using ArchiveService.Class.Database.MS_SQL.Table;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Triggers
{
    public class TriggerAverage
    {
        object _lock = new object();

        public void TriggerMSSQLAverage()
        {
            lock (_lock)
            {
                try
                {
                    if (ConfigurationManager.GetCurrentVersionServer() == "MSSQL")
                    {
                        int counts = TableDB.TableDBList.Count;
                        DateTime dateNow = DateTime.Now;

                        for (int i = 0; i < counts - 1; i++)
                        {
                            TimeSpan timeNow = dateNow.Subtract(TableDB.TableDBList[i].TimeTable);
                            TimeSpan timeValue = ArchiveMath.CreateTimeSpan(TableDB.TableDBList[i].TimeValue, TableDB.TableDBList[i].TimeAverage);

                            if (!(timeNow >= timeValue))
                                continue;

                            if (i == 0)
                            {
                                DataTable dt = ConnectionMSSQL.ReadMSSQL(TableDB.TableDBList[i].TableName, TableDB.TableDBList[i].TimeTable);
                                TableDB.TableDBList[i].TimeTable = dateNow;
                                DataTable dtNow = ArchiveLevel.InsertArchiveLevel(dt, dateNow);
                                ConnectionMSSQL.WrireMSSQL(dtNow, TableDB.TableDBList[i + 1].TableName);
                                DataTable dtPeak = PeakValue.InsertPeakValue(dt, dtNow);
                                ConnectionMSSQL.WrireMSSQL(dtPeak, "PeakValue");                              
                            }
                            else if (i > 0)
                            {
                                DataTable dt = ConnectionMSSQL.ReadMSSQL(TableDB.TableDBList[i].TableName, TableDB.TableDBList[i].TimeTable);
                                TableDB.TableDBList[i].TimeTable = dateNow;
                                DataTable dtNow = ArchiveLevel.InsertArchiveLevelNext(dt, dateNow);
                                ConnectionMSSQL.WrireMSSQL(dtNow, TableDB.TableDBList[i + 1].TableName);
                            }

                            Thread.Sleep(11000);
                        }
                    }
                }
                catch(Exception ex) 
                {
                    new Loggings().WriteLogAdd($"Ошибка отработки триггера! - TriggerMSSQLAverage - {ex.Message}", StatusLog.Errors);
                }
                
            }
        }
    }
}
