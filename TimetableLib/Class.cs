using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib
{
    public class Class
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ClassTimetable Timetable { get; set; }
    }
}
