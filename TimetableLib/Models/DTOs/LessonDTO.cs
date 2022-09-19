using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class LessonDTO
    {
        public LessonDTO(LessonDB ls)
        {
            Id = ls.Id;
            Name = ls.Name;
            LessonNumber = ls.LessonNumber;
            GroupNumber = ls.GroupNumber;
            ClassId = ls.ClassId;
            TeacherId = ls.TeacherId;
            ClassroomId = ls.ClassroomId;
        }

        public long Id { get; set; }
        public byte LessonNumber { get; set; }
        public byte? GroupNumber { get; set; }
        public string Name { get; set; }
        public long? ClassId { get; set; }
        public string? ClassName { get; set; }
        public long? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public long? ClassroomId { get; set; }
        public string? ClassroomName { get; set; }

    }
}
