using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using ArchiveService.Class.VASTModbusTCP.Device.MVK_Device;
using ArchiveService.Class.VASTModbusTCP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public static class Archive
    {
        public static DataTable InssertArchive()
        {
            List<ModbusClientDatabase> lists = new List<ModbusClientDatabase>(RepositoryDatabase.WriteListDB);
            //new Loggings().WriteLogAdd($"----------------------------------------------------------", StatusLog.Inform);
            //new Loggings().WriteLogAdd($"InssertArchive -  lists: {lists.Count}", StatusLog.Inform);
            


            //int temp = 0;

            RepositoryDatabase.WriteListDBClear();

            DataTable dt = new DataTable("Archive");

            try
            {
                dt.Columns.Add(new DataColumn() { ColumnName = "ID", DataType = typeof(Nullable) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Frequency", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "ID Parameters", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Equipment", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Time", DataType = typeof(DateTime) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Value", DataType = typeof(float) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Chanel", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "MVK Number", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Mode Work", DataType = typeof(int) });
                dt.Columns.Add(new DataColumn() { ColumnName = "DreamDb channel_name", DataType = typeof(string) });

                for (int i = 0; i < lists.Count; i++)
                {
                    //temp = i;

                    //new Loggings().WriteLogAdd($"ID Frequency = {RepositoryService.GetValueFrequency(lists[i].Type)};" +
                    //    $"ID Parameters = {RepositoryService.GetValueParameters(lists[i].Parameters)};" +
                    //    $"Equipment = {lists[i].Equipment};" +
                    //    $"Time = {lists[i].Time};" +
                    //    $"MVK Value = {lists[i].MeasumentValue};" +
                    //    $"Chanel = {lists[i].Chanel};" +
                    //    $"MVK Number = {lists[i].NumberMVK};" +
                    //    $"Mode Work = {lists[i].ModeWork};" +
                    //    $"DreamDb channel_name = {lists[i].DreamChannelName}", StatusLog.Inform);

                    DataRow row = dt.NewRow();

                    row["ID"] = null;
                    row["ID Frequency"] = RepositoryService.GetValueFrequency(lists[i].Type);
                    row["ID Parameters"] = RepositoryService.GetValueParameters(lists[i].Parameters);
                    row["Equipment"] = lists[i].Equipment;
                    row["Time"] = lists[i].Time;
                    row["MVK Value"] = lists[i].MeasumentValue;
                    row["Chanel"] = lists[i].Chanel;
                    row["MVK Number"] = lists[i].NumberMVK;
                    row["Mode Work"] = lists[i].ModeWork;
                    row["DreamDb channel_name"] = lists[i].DreamChannelName;

                    dt.Rows.Add(row);
                }

                return dt;
            }
            catch (Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка формирования временной таблицы! - {ex.Message}", StatusLog.Errors);

                //new Loggings().WriteLogAdd($"ID Frequency = {RepositoryService.GetValueFrequency(lists[temp].Type)};" +
                //    $"ID Parameters = {RepositoryService.GetValueParameters(lists[temp].Parameters)};" +
                //    $"Equipment = {lists[temp].Equipment};" +
                //    $"Time = {lists[temp].Time};" +
                //    $"MVK Value = {lists[temp].MeasumentValue};" +
                //    $"Chanel = {lists[temp].Chanel};" +
                //    $"MVK Number = {lists[temp].NumberMVK};" +
                //    $"Mode Work = {lists[temp].ModeWork};" +
                //    $"DreamDb channel_name = {lists[temp].DreamChannelName}", StatusLog.Errors);

                return null;
                //return dt;
            }

        }

    }
}
