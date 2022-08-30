using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class LessonDB
    {
        long Id { get; set; }
        string Name { get; set; }
        long ClassId { get; set; }
        long TeacherId { get; set; }
        long ClassroomId { get; set; }
        string ClassroomLink { get; set; }
        string TeacherLink { get; set; }

    }
}
