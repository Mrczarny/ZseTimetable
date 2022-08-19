using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Classroom : IScrappable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Timetable Timetable { get; set; }
        public Regex Regex { get; set; }
    }
}
