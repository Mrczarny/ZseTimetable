using System;
using System.Collections.Generic;
using TimetableLib.Models;

namespace TimetableLib.Timetables
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public string ClassName { get; set; }
        public IList<ClassDay> Days { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}