using System;
using System.Collections.Generic;

namespace TimetableLib.Timetables
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public Class Class { get; set; }
        public List<ClassDay> DaysTimetable { get; set; } // dumb name
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}