using System;
using System.Collections.Generic;

namespace TimetableLib.Timetables
{
    public class TimetableDay
    {
        public long Id { get; set; }
        public DayOfWeek Day { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}