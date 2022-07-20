using System.Collections.Generic;
using TimetableLib.Models;

namespace TimetableLib.Changes
{
    public class TeacherReplacement
    {
        public long? Id { get; set; }
        public Teacher Teacher { get; set; }
        public List<ClassReplacement> ClassReplacements { get; set; }
    }
}