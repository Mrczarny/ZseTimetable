using System.Collections.Generic;

namespace TimetableLib.Timetables
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public List<ClassDay> DaysTimetable { get; set; } // dumb name
    }
}