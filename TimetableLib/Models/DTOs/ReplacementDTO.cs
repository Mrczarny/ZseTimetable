using System;
using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class ReplacementDTO
    {
        public ReplacementDTO(ReplacementDB rp, LessonDB ls)
        {
           // Id = (long) rp.Id;
            Note = rp.Note;
            Lesson = new LessonDTO(ls);
            NewClassroomName = rp.ClassroomName;
            NewTeacherName = rp.SubTeacherName;
            ClassName = rp.ClassName;
        }
        public ReplacementDTO(ReplacementDB rp)
        {
            // Id = (long) rp.Id;
            Note = rp.Note;
            NewClassroomName = rp.ClassroomName;
            NewTeacherName = rp.SubTeacherName;
            ClassName = rp.ClassName;
        }

        //public long Id { get; set; }
        public DateTime ReplacementDate { get; set; }
        public LessonDTO Lesson { get; set; }
        public string NewTeacherName { get; set; }
        public string ClassName { get; set; }
        //public long NewTeacherId { get; set; }
        public string NewClassroomName { get; set; }
        //public long NewClassroomId { get; set; }
        public string Note { get; set; }
    }
}