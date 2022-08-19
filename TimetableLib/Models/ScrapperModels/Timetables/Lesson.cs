namespace TimetableLib.Timetables
{
    public class Lesson
    {
        public long Id { get; set; }
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public string TeacherName { get; set; }
        public string TeacherLink { get; set; }
        public string Classroom { get; set; }
        public string ClassroomLink { get; set; }
        public string Group { get; set; }
    }
}