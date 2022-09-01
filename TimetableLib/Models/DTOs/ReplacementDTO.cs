namespace TimetableLib.Models.DTOs
{
    public class ReplacementDTO
    {
        public long Id { get; set; }
        public LessonDTO OldLesson { get; set; }
        public LessonDTO NewLesson { get; set; }
        public string Note { get; set; }
    }
}
