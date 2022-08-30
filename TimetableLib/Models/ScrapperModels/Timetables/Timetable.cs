using System;
using System.Collections.Generic;
using TimetableLib.Models;

namespace TimetableLib.Timetables
{
    public class Timetable
    {
        public string Title { get; set; }
        public IList<TimetableDay> Days { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}