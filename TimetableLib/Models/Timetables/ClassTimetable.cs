using System;
using System.Collections.Generic;
using TimetableLib.Models;

namespace TimetableLib.Timetables
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public Class Class { get; set; }
        public List<ClassDay> DaysTimetable { get; set; } // dumb name
        public DateTime startDate { get; set; }                 //as all names in this procject...
        public DateTime endDate { get; set; }
    }
}