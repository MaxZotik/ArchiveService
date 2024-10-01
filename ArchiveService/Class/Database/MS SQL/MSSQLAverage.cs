using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveService.Class.Database.MS_SQL
{
    public class MSSQLAverage
    {
        /// <summary>
        /// Дата прореживания таблицы
        /// </summary>
        public DateTime DateAverage { get; set; }

        /// <summary>
        /// Дата текущая
        /// </summary>
        public DateTime DateCurrent { get; set; }
    }
}
