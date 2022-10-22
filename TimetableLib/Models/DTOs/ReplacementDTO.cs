using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class ReplacementDTO
    {
        public ReplacementDTO(ReplacementDB rp)
        {
            Id = (long) rp.Id;
            Note = rp.Note;
            NewLesson = new LessonDTO(rp.NewLesson);
            OldLesson = new LessonDTO(rp.OldLesson);
        }
        public long Id { get; set; }
        public LessonDTO OldLesson { get; set; }
        public LessonDTO NewLesson { get; set; }
        public string Note { get; set; }
    }
}
