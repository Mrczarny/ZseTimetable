namespace TimetableLib.Models.DTOs
{
    public class TeacherDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public TimetableDTO Timetable { get; set; }

    }
}
