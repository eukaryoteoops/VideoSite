using System;

namespace Comic.Common.ExtensionMethods
{
    public static class DateTimeExtension
    {
        public static long StartTimeOfMonth(this DateTime time)
        {
            var date = time.Date;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            return firstDayOfMonth.WithOffset(8).ToUnixTimeSeconds();
        }

        public static long EndTimeOfMonth(this DateTime time)
        {
            var date = time.Date;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var endTimeOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
            return endTimeOfMonth.WithOffset(8).ToUnixTimeSeconds();
        }

        public static long StartTimeOfDay(this DateTime time)
        {
            var date = time.Date;
            return date.WithOffset(8).ToUnixTimeSeconds();
        }

        public static long EndTimeOfDay(this DateTime time)
        {
            var date = time.Date;
            return date.AddDays(1).AddSeconds(-1).WithOffset(8).ToUnixTimeSeconds();
        }

        public static DateTimeOffset WithOffset(this DateTime time, int offset)
        {
            return new DateTimeOffset(time, TimeSpan.FromHours(offset));
        }

        public static int ToDateInteger(this DateTimeOffset time)
        {
            return Convert.ToInt32(time.ToString("yyyyMMdd"));
        }
    }
}
