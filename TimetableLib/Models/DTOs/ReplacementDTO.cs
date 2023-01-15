using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class ReplacementDTO
    {
        public ReplacementDTO(ReplacementDB rp)
        {
            Id = (long) rp.Id;
            Note = rp.Note;
            Lesson = new LessonDTO(rp.Lesson);
        }
        public long Id { get; set; }
        public LessonDTO Lesson { get; set; }
        public string NewTeacherName { get; set; }
        public long NewTeacherId { get; set; }
        public string NewClassroomName { get; set; }
        public long NewClassroomId { get; set; }
        public string Note { get; set; }
    }
}
