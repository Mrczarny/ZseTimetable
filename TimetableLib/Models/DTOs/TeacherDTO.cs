using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class TeacherDTO
    {
        public TeacherDTO(TeacherDB tr)
        {
            Id = (long) tr.Id;
            Name = tr.Name;
            ShortName = tr.ShortName;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public TimetableDTO Timetable { get; set; }

    }
}
