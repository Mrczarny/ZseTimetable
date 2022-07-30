using System;

namespace TimetableLib.Changes
{
    public class ClassReplacement
    {
        public long? Id { get; set; }
        public DayOfWeek Day { get; set; }
        public string LessonNumber { get; set; }
        public string Description { get; set; }
        public string Sub { get; set; }
        public string Note { get; set; }
    }
}