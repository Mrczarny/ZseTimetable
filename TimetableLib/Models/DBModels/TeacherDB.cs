using System.Data;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class TeacherDB : ITimetables
    {
        public TeacherDB()
        {
        }

        public TeacherDB(Teacher tr)
        {
            Name = tr.Name;
            ShortName = tr.ShortName;
            Timetable = new TimetableDB(tr.Timetable);
            Link = tr.Link;
        }

        [SqlType(SqlDbType.NVarChar)] public string ShortName { get; set; }

        [Identity] [SqlType(SqlDbType.BigInt)] public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)] public string Name { get; set; }

        [SqlType(SqlDbType.BigInt)] public long TimetableId { get; set; }

        [SqlType(SqlDbType.NVarChar)] public string Link { get; set; }

        public void SetLessonId(LessonDB ls)
        {
            ls.TeacherId = Id;
        }

        public void SetLessonName(LessonDB ls)
        {
            ls.TeacherName = Name;
        }

        public string? GetLessonLink(LessonDB ls)
        {
            return ls?.TeachereLink;
        }

        public string? GetLessonName(LessonDB ls)
        {
            return ls?.TeacherName;
        }

        public TimetableDB Timetable { get; set; }


        public void Dispose()
        {
            Timetable = null;
            Id = null;
            Link = null;
            Name = null;
        }
    }
}