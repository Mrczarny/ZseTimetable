using System;

namespace TimetableLib.Changes
{
    public class LessonReplacement
    {
        public long? Id { get; set; }
        public string LessonNumber { get; set; }
        public string Description { get; set; }
        public string Sub { get; set; }
        public string Note { get; set; }
    }
}