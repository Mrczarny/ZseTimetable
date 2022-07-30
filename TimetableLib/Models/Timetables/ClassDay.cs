using System;
using System.Collections.Generic;

namespace TimetableLib.Timetables
{
    public class ClassDay
    {
        public long Id { get; set; }
        public DayOfWeek Day { get; set; }
        public List<ClassLesson> Lessons { get; set; }
    }
}