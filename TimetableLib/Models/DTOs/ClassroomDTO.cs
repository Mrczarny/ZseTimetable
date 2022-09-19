using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class ClassroomDTO
    {
        public ClassroomDTO(ClassroomDB cr)
        {
            Id = cr.Id;
            Name = cr.Name;
            Number = cr.Number;
            Timetable = new TimetableDTO(cr.Timetable);
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public short Number { get; set; }
        public TimetableDTO Timetable { get; set; }

    }
}
