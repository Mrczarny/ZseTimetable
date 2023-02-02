using System.Data;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Timetables;

namespace TimetableLib.Models.DBModels
{
    public class LessonDB : IDBModel
    {
        public LessonDB(){}
        public LessonDB(Lesson ls)
        {
            Name = ls.LessonName;
            Number = (byte) ls.LessonNumber;
            Group = ls.Group;
            ClassName = ls.ClassName;
            ClassLink = ls.ClassLink;
            ClassroomName = ls.ClassroomName;
            ClassroomLink = ls.ClassroomLink;
            TeacherName = ls.TeacherName;
            TeachereLink = ls.TeacherLink;
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.TinyInt)]
        public byte Number { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Group { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long? ClassId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long? TeacherId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long? ClassroomId { get; set; }

        public string? ClassName { get; set; }
        public string? ClassLink { get; set; }

        public string? ClassroomName { get; set; }
        public string? ClassroomLink { get; set; }

        public string? TeacherName { get; set; }
        public string? TeachereLink { get; set; }


    }
}