using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib
{
    public class ClassLesson
    {
        public long Id { get; set; }
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public string TeacherName { get; set; }
        public string Classroom { get; set; }
    }
}
