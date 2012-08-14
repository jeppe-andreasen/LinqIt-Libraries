using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToNearestWorkingDay(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday: return date.AddDays(2);
                case DayOfWeek.Sunday: return date.AddDays(1);
                default: return date;
            }
        }

        public static DateTime AddWorkingDays(this DateTime date, int workingDays)
        {
            int daysToAdd = (workingDays / 5) * 7;
            var result = date.ToNearestWorkingDay().AddDays(daysToAdd);
            for (int i = 0; i < (workingDays % 5); i++)
                result = result.AddDays(1).ToNearestWorkingDay();
            return result;
        }

        public static DateTime AddWeeks(this DateTime date, int weeks)
        {
            return date.AddDays(7);
        }

        public static DateTime GetStartOfWeek(this DateTime date)
        {
            int dayOfWeek = ((int) date.DayOfWeek + 6)%7;
            return date.AddDays(-dayOfWeek).Date;
        }

        public static TimeSpan TimeIntoWeek(this DateTime date)
        {
            return date.Subtract(date.GetStartOfWeek());
        }

        public static string ToXml(this DateTime value)
        {
            return value.ToString("s") + "Z";
        }
    }
}
