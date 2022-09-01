using System.Text.RegularExpressions;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Teacher 
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Timetable Timetable { get; set; }
        public Regex Regex { get; set; }
    }
}