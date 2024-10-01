using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public static class ArchiveLevel
    {
        
        public static DataTable InsertArchiveLevel(DataTable dataTable, DateTime time)
        {

            Stopwatch timer = Stopwatch.StartNew();

            DataTable dt = new DataTable("ArchiveLevel");

            try
            {
                dt.Columns.Add(new DataColumn() { ColumnName = "ID", DataType = typeof(Nullable) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Parameters", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Frequency", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Equipment", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Time", DataType = typeof(DateTime) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Value Max", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Value Avg", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Chanel", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Deviation", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Counts", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Number", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Mode Work", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "DreamDb channel_name", DataType = typeof(string) });

                var dataRows = dataTable.AsEnumerable().GroupBy(row => new
                {
                    IDparameter = row.Field<int>("ID Parameters"),
                    IDfrequency = row.Field<int>("ID Frequency"),
                    Equipment = row.Field<int>("Equipment"),
                    Chanel = row.Field<int>("Chanel"),
                    MVKnumber = row.Field<int>("MVK Number"),
                    ModeWork = row.Field<int>("Mode Work"),
                    DreamDb = row.Field<string>("DreamDb channel_name"),
                }).Select(group => new
                {
                    IDparameter = group.Key.IDparameter,
                    IDfrequency = group.Key.IDfrequency,
                    Equipment = group.Key.Equipment,
                    Chanel = group.Key.Chanel,
                    MVKnumber = group.Key.MVKnumber,
                    ModeWork = group.Key.ModeWork,
                    Counts = group.Count(),
                    MVKValueAvg = (float)group.Average(row => row.Field<double>("MVK Value")),
                    MVKValueMax = (float)group.Max(row => row.Field<double>("MVK Value")),
                    DreamDb = group.Key.DreamDb,
                });

                foreach (var group in dataRows)
                {
                    DataRow newRow = dt.NewRow();

                    newRow["ID"] = null;
                    newRow["ID Parameters"] = group.IDparameter;
                    newRow["ID Frequency"] = group.IDfrequency;
                    newRow["Equipment"] = group.Equipment;
                    newRow["Time"] = time;
                    newRow["MVK Value Max"] = group.MVKValueMax;
                    newRow["MVK Value Avg"] = group.MVKValueAvg;
                    newRow["Chanel"] = group.Chanel;
                    newRow["Deviation"] = ArchiveMath.GetDeviationArchive(group.IDparameter, group.IDfrequency, group.Chanel, group.MVKnumber, group.ModeWork, group.Equipment, group.Counts, group.MVKValueAvg, dataTable);
                    newRow["Counts"] = group.Counts;
                    newRow["MVK Number"] = group.MVKnumber;
                    newRow["Mode Work"] = group.ModeWork;
                    newRow["DreamDb channel_name"] = group.DreamDb;

                    dt.Rows.Add(newRow);
                }               

                timer.Stop();
                int times2 = Convert.ToInt32(timer.ElapsedMilliseconds);
                //ConnectionMSSQL.TimeWork(times2, "InsertArchiveLevel - Создание DataTable");

                return dt;
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка вставки данных в БД! - InsertArchiveLevel - {ex.Message}", StatusLog.Errors);

                timer.Stop();
                int times = Convert.ToInt32(timer.ElapsedMilliseconds);
                //ConnectionMSSQL.TimeWork(times, "InsertArchiveLevel - Создание DataTable");

                return null;
            }


        }


        public static DataTable InsertArchiveLevelNext(DataTable dataTable, DateTime time)
        {
            DataTable dt = new DataTable();

            try
            {
                dt.Columns.Add(new DataColumn() { ColumnName = "ID", DataType = typeof(Nullable) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Parameters", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Frequency", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Equipment", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Time", DataType = typeof(DateTime) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Value Max", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Value Avg", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Chanel", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Deviation", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Counts", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Number", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Mode Work", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "DreamDb channel_name", DataType = typeof(string) });

                var dataRows = dataTable.AsEnumerable().GroupBy(row => new
                {
                    IDparameter = row.Field<int>("ID Parameters"),
                    IDfrequency = row.Field<int>("ID Frequency"),
                    Equipment = row.Field<int>("Equipment"),
                    Chanel = row.Field<int>("Chanel"),
                    MVKnumber = row.Field<int>("MVK Number"),
                    ModeWork = row.Field<int>("Mode Work"),
                    DreamDb = row.Field<string>("DreamDb channel_name"),
                }).Select(group => new
                {
                    IDparameter = group.Key.IDparameter,
                    IDfrequency = group.Key.IDfrequency,
                    Equipment = group.Key.Equipment,
                    Chanel = group.Key.Chanel,
                    MVKnumber = group.Key.MVKnumber,
                    ModeWork = group.Key.ModeWork,
                    //Counts = group.Count(),
                    Counts = (int)group.Sum(row => row.Field<int>("Counts")),
                    MVKValueAvg = (float)group.Average(row => row.Field<double>("MVK Value Avg")),
                    MVKValueMax = (float)group.Max(row => row.Field<double>("MVK Value Max")),
                    DreamDb = group.Key.DreamDb,
                });

                foreach (var group in dataRows)
                {
                    DataRow newRow = dt.NewRow();

                    newRow["ID"] = null;
                    newRow["ID Parameters"] = group.IDparameter;
                    newRow["ID Frequency"] = group.IDfrequency;
                    newRow["Equipment"] = group.Equipment;
                    newRow["Time"] = time;
                    newRow["MVK Value Max"] = group.MVKValueMax;
                    newRow["MVK Value Avg"] = group.MVKValueAvg;
                    newRow["Chanel"] = group.Chanel;
                    newRow["Deviation"] = ArchiveMath.GetDeviationArchiveLevel(group.IDparameter, group.IDfrequency, group.Chanel, group.MVKnumber, group.ModeWork, group.Equipment, group.Counts, group.MVKValueAvg, dataTable);
                    newRow["Counts"] = group.Counts;
                    newRow["MVK Number"] = group.MVKnumber;
                    newRow["Mode Work"] = group.ModeWork;
                    newRow["DreamDb channel_name"] = group.DreamDb;

                    dt.Rows.Add(newRow);
                }

                return dt;
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка вставки данных в БД! - InsertArchiveLevelNext - {ex.Message}", StatusLog.Errors);

                return null;
            }


        }
    }
}
