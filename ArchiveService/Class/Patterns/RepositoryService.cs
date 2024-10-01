using ArchiveService.Class.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Patterns
{
    public static class RepositoryService
    {
        public static Dictionary<string, int> Parameters { get; set; }
        public static Dictionary<string, int> Frequency { get; set; }

        static RepositoryService()
        {
            Frequency = new Dictionary<string, int>()
            {
                { "Пик-Фактор", 1 },
                { "Виброускорение", 2 },
                { "Виброскорость", 3 },
                { "Виброперемещение", 4}
            };

            Parameters = new Dictionary<string, int>()
            {
                { "10-2000Гц", 1 },
                { "10-1000Гц", 2 },
                { "2-1000Гц", 3 },
                { "x-25Гц", 4 },
                { "10-3000Гц", 5 },
                { "0.8-300Гц", 6 },
                { "0.8-150Гц", 7 },
                { "Фильтр 1", 8 },
                { "Фильтр 2", 9 }
            };
        }
        

        public static int GetValueParameters(string key)
        {
            Parameters.TryGetValue(key, out int value);

            return value;
        }

        public static int GetValueFrequency(string key)
        {
            Frequency.TryGetValue(key, out int value);

            return value;
        }

        public static void Read()
        {
            new Loggings().WriteLogAdd($"{Parameters.Count()}", Enums.StatusLog.Action);
            new Loggings().WriteLogAdd($"{Frequency.Count()}", Enums.StatusLog.Action);
        }
    }
}
