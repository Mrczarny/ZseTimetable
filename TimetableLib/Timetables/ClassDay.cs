using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Timetables
{
    public class ClassDay
    {
        public long Id { get; set; }
        public Class Class { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public List<ClassLesson> Lessons { get; set; }

    }
}
