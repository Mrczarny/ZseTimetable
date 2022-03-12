using System;
using System.Collections.Generic;

namespace TimetableLib.Changes
{
    public class TeacherChange
    {
        public long Id { get; set; }
        public Teacher Teacher { get; set; }
        public List<ClassChange> ClassChanges { get; set; }
    }
}
