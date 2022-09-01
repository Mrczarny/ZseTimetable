namespace TimetableLib.Models.DTOs
{
    public class ClassroomDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public short Number { get; set; }
        public TimetableDTO Timetable { get; set; }

    }
}
