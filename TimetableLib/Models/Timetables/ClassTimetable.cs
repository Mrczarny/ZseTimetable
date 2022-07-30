using System;
using System.Collections.Generic;
using TimetableLib.Models;

namespace TimetableLib.Timetables
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public string ClassName { get; set; }
        public IList<ClassDay> Days { get; set; } // dumb name
        public DateTime StartDate { get; set; }                 //as all names in this procject...
        public DateTime EndDate { get; set; }
    }
}