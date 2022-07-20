using TimetableLib.Timetables;

namespace TimetableLib.Models
{
    public class Class
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ClassTimetable Timetable { get; set; }
    }
}