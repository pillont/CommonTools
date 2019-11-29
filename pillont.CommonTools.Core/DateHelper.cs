using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace pillont.CommonTools.Core
{
    public static class DateHelper
    {
        /// <summary>
        /// get the first day of week
        /// </summary>
        /// <param name="year">year of the week xanted</param>
        /// <param name="weekNumber">number of the week wanted</param>
        public static DateTime FirstDayOfWeek(int year, int weekNumber, DayOfWeek firstInWeek = DayOfWeek.Monday)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = firstInWeek - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);

            int dayNumber = (weekNumber - 1) * 7;
            return firstMonday.AddDays(dayNumber);
        }

        /// <summary>
        ///  get the number of current date
        /// </summary>
        /// <param name="firstDay">first day in a week</param>
        public static int GetWeekNumber(DayOfWeek firstDay = DayOfWeek.Monday)
        {
            return GetWeekNumber(DateTime.Now, firstDay);
        }

        /// <summary>
        ///  get the number of wanted date
        /// </summary>
        /// <param name="firstDay">first day in a week</param>
        public static int GetWeekNumber(this DateTime date, DayOfWeek firstDay = DayOfWeek.Monday, CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek)
        {
            return CultureInfo.InvariantCulture
                                .Calendar
                                .GetWeekOfYear(date,
                                                rule,
                                                firstDay);
        }

        /// <summary>
        /// get the first day of week associated to the wanted date
        /// </summary>
        /// <param name="startOfWeek">first day in a week</param>
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (date.DayOfWeek - startOfWeek) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}