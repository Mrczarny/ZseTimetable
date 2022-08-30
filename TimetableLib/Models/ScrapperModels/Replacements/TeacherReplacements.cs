using System.Collections.Generic;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Changes
{
    public class TeacherReplacements
    {
        public Teacher Teacher { get; set; }
        public IEnumerable<LessonReplacement> ClassReplacements { get; set; }
    }
}