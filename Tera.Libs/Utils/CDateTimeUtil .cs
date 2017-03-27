using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.Types;

namespace Tera.Libs.Utils
{
    public class CDateTimeUtil
    {

        public static MySqlDateTime NetToMySql(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return new MySqlDateTime(0, 0, 0, 0, 0, 0, 0);
            else
                return new MySqlDateTime(dateTime);   // the constructor can handle all but the 0
        }

        // MySqlDateTime will convert 0 date values to DateTime.MinValue
        public static DateTime MySqlToNet(MySqlDateTime mySqlDateTime)
        {
            return (DateTime)mySqlDateTime;  // cast it an let MySqlDateTime do the work
        }

        public static int GetDaysBetweenDates(DateTime firstDate, DateTime secondDate)
        {
            return secondDate.Subtract(firstDate).Days;
        }
    }
}
