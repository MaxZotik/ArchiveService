using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL.Table
{
    public static class ArchiveMath
    {
        public static float GetDeviationArchive(int idParameter, int idFrequency, int chanel, int mvkNumber, int modeWork, int equipment, int count, float avg, in DataTable dt)
        {
            var list = from equip in dt.AsEnumerable()
                       where equip.Field<int>("ID Parameters") == idParameter &&
                       equip.Field<int>("ID Frequency") == idFrequency && equip.Field<int>("Chanel") == chanel &&
                       equip.Field<int>("MVK Number") == mvkNumber && equip.Field<int>("Mode Work") == modeWork &&
                       equip.Field<int>("Equipment") == equipment
                       select new { MVKvalue = (float)equip.Field<double>("MVK Value") };

            float devTemp = 0;
            float deviation = 0;

            foreach (var item in list)
            {
                devTemp += (float)Math.Pow(Math.Abs(item.MVKvalue) - Math.Abs(avg), 2);
               
            }

            if (devTemp == 0)
                deviation = 0;
            else
                deviation = (float)Math.Sqrt(devTemp / count);

            return deviation;
        }

        public static float GetDeviationArchiveLevel(int idParameter, int idFrequency, int chanel, int mvkNumber, int modeWork, int equipment, int count, float avg, in DataTable dt)
        {
            var list = from equip in dt.AsEnumerable()
                       where equip.Field<int>("ID Parameters") == idParameter &&
                       equip.Field<int>("ID Frequency") == idFrequency && equip.Field<int>("Chanel") == chanel &&
                       equip.Field<int>("MVK Number") == mvkNumber && equip.Field<int>("Mode Work") == modeWork &&
                       equip.Field<int>("Equipment") == equipment
                       select new { MVKvalueAvg = (float)equip.Field<double>("MVK Value Avg"), 
                                    Counts = (int)equip.Field<int>("Counts") };

            float deviation = 0;
            float devTemp = 0;
            float devAvg = 0;

            foreach (var item in list)
            {
                devTemp += (float)(item.MVKvalueAvg * item.Counts);
            }

            float resultDevTemp = devTemp / count;

            foreach (var item in list)
            {
                devAvg += (float)Math.Pow(Math.Abs(item.MVKvalueAvg) - Math.Abs(resultDevTemp), 2) * item.Counts;
            }

            if (devAvg == 0)
                deviation = 0;
            else
                deviation = (float)Math.Sqrt(devAvg / count);

            return deviation;
        }

        public static TimeSpan CreateTimeSpan(int time, string formatTime)
        {
            if (formatTime == "минута")
                return new TimeSpan(0, time, 0);
            else if (formatTime == "час")
                return new TimeSpan(time, 0, 0);
            else if (formatTime == "день")
                return new TimeSpan(time, 0, 0, 0);
            else
                return new TimeSpan(0, 0, 0);
        }

        public static DateTime CreateDateTime(int time, string formatTime)
        {
            DateTime dt = DateTime.Now;

            if (formatTime == "минута")
                return dt.AddMinutes((double)time);
            else if (formatTime == "час")
                return dt.AddHours((double)time);
            else if (formatTime == "день")
                return dt.AddDays((double)time);
            else if (formatTime == "год")
                return dt.AddYears(time);
            else
                return dt;
        }

        /// <summary>
        /// Метод переводит русский в английский
        /// </summary>
        /// <param name="date">Строка времени</param>
        /// <returns></returns>
        public static string GetDate(string date)
        {
            if (date == "минута")
                return "minute";
            else if (date == "час")
                return "hour";
            else if (date == "день")
                return "day";
            else
                return "year";
        }
    }
}
