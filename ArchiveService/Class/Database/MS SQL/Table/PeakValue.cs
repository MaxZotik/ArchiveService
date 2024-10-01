using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public static class PeakValue
    {
        public static DataTable InsertPeakValue(DataTable data, DataTable dataNow)
        {           
            DataTable dtResult = new DataTable();

            try
            {
                dtResult.Columns.Add(new DataColumn() { ColumnName = "ID", DataType = typeof(Nullable) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "ID Parameters", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "ID Frequency", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "Equipment", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "Time", DataType = typeof(DateTime) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "MVK Value", DataType = typeof(float) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "Chanel", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "MVK Number", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "Mode Work", DataType = typeof(int) });
                dtResult.Columns.Add(new DataColumn() { ColumnName = "DreamDb channel_name", DataType = typeof(string) });

                float coefficientPeak = PeakDB.PeakDBList[0].CoefficientPeak;

                foreach (DataRow dataNowRow in dataNow.Rows)
                {
                    float valueAvg = float.Parse(dataNowRow["MVK Value Avg"].ToString());
                    float valueDeviaton = float.Parse(dataNowRow["Deviation"].ToString());
                    float valuePeak = valueAvg + (valueDeviaton * coefficientPeak);

                    int param = int.Parse(dataNowRow["ID Parameters"].ToString());
                    int freq = int.Parse(dataNowRow["ID Frequency"].ToString());
                    int equip = int.Parse(dataNowRow["Equipment"].ToString());
                    int chanel = int.Parse(dataNowRow["Chanel"].ToString());
                    int number = int.Parse(dataNowRow["MVK Number"].ToString());
                    int mode = int.Parse(dataNowRow["Mode Work"].ToString());


                    foreach (DataRow dataRow in data.Rows)
                    {
                        float valueMVK = float.Parse(dataRow["MVK Value"].ToString());

                        int param_2 = int.Parse(dataRow["ID Parameters"].ToString());
                        int freq_2 = int.Parse(dataRow["ID Frequency"].ToString());
                        int equip_2 = int.Parse(dataRow["Equipment"].ToString());
                        int chanel_2 = int.Parse(dataRow["Chanel"].ToString());
                        int number_2 = int.Parse(dataRow["MVK Number"].ToString());
                        int mode_2 = int.Parse(dataRow["Mode Work"].ToString());

                        if (param == param_2 && freq == freq_2 && equip == equip_2 && chanel == chanel_2 && number == number_2 && mode == mode_2 && valueMVK >= valuePeak)
                        {
                            DataRow row = dtResult.NewRow();

                            row["ID"] = null;
                            row["ID Frequency"] = int.Parse(dataRow["ID Frequency"].ToString());
                            row["ID Parameters"] = int.Parse(dataRow["ID Parameters"].ToString());
                            row["Equipment"] = int.Parse(dataRow["Equipment"].ToString());
                            row["Time"] = (DateTime)dataRow["Time"];
                            row["MVK Value"] = float.Parse(dataRow["MVK Value"].ToString());
                            row["Chanel"] = int.Parse(dataRow["Chanel"].ToString());
                            row["MVK Number"] = int.Parse(dataRow["MVK Number"].ToString());
                            row["Mode Work"] = int.Parse(dataRow["Mode Work"].ToString());
                            row["DreamDb channel_name"] = dataRow["DreamDb channel_name"].ToString();

                            dtResult.Rows.Add(row);
                        }
                    }
                }

                return dtResult;
            }
            catch(Exception ex)
            {
                new Loggings().WriteLogAdd($"Ошибка вставки данных в БД! - InsertPeakValue - {ex.Message}", StatusLog.Errors);
                return null;
            }         
        }
    }
}
