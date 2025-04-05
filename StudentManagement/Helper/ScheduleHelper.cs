using System;

namespace StudentManagement.Helpers
{
    public static class ScheduleHelper
    {
        public static TimeSpan GetStartTimeFromLesson(int lesson)
        {
            switch (lesson)
            {
                case int n when n >= 1 && n <= 3:
                    return TimeSpan.FromMinutes(6 * 60 + 45 + (lesson - 1) * 45);
                case int n when n >= 4 && n <= 6:
                    return TimeSpan.FromMinutes(9 * 60 + 20 + (lesson - 4) * 45);
                case int n when n >= 7 && n <= 12:
                    return TimeSpan.FromMinutes(12 * 60 + 30 + (lesson - 7) * 45);
                case int n when n >= 13 && n <= 15:
                    return TimeSpan.FromMinutes(18 * 60 + (lesson - 13) * 45);
                default:
                    return TimeSpan.Zero;
            }
        }

        public static int GetLessonStart(TimeSpan startTime)
        {
            var minutes = (int)startTime.TotalMinutes;
            var baseTime6h45 = 6 * 60 + 45;
            var baseTime9h20 = 9 * 60 + 20;
            var baseTime12h30 = 12 * 60 + 30;
            var baseTime18h = 18 * 60;

            if (minutes >= baseTime6h45 && minutes < baseTime6h45 + 45 * 3)
                return 1 + (minutes - baseTime6h45) / 45;
            else if (minutes >= baseTime9h20 && minutes < baseTime9h20 + 45 * 3)
                return 4 + (minutes - baseTime9h20) / 45;
            else if (minutes >= baseTime12h30 && minutes < baseTime12h30 + 45 * 6)
                return 7 + (minutes - baseTime12h30) / 45;
            else if (minutes >= baseTime18h && minutes < baseTime18h + 45 * 3)
                return 13 + (minutes - baseTime18h) / 45;

            return -1;
        }

        public static int GetLessonCount(TimeSpan startTime, TimeSpan endTime)
        {
            return (int)((endTime - startTime).TotalMinutes / 45);
        }
    }
}