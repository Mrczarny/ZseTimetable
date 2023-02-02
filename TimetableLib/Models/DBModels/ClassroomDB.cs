using System.Data;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class ClassroomDB : ITimetables
    {
        public ClassroomDB(){}
        public ClassroomDB(Classroom cr)
        {
            Name = cr.Name;
            Timetable = new TimetableDB(cr.Timetable);
            Link = cr.Link;
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.SmallInt)]
        public short Number { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Link { get; set; }

        public void SetLessonId(LessonDB ls)
        {
            ls.ClassroomId = Id;
        }

        public void SetLessonName(LessonDB ls)
        {
            ls.ClassroomName = Name;
        }

        public string? GetLessonLink(LessonDB ls) => ls?.ClassroomLink;

        public string? GetLessonName(LessonDB ls) => ls?.ClassroomName;


        public TimetableDB Timetable { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string TimetableLink { get; set; }

        public void Dispose()
        {
            Timetable = null;
            Id = null;
            Link = null;
            Name = null;
            TimetableLink = null;
        }
    }
}