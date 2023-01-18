using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Changes;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;

namespace TimetableLib.Models.DBModels
{
    public class ReplacementDB : IDBModel
    {
        public ReplacementDB(){}

        public ReplacementDB(LessonReplacement lr)
        {
            TeacherName = lr.OriginalTeacher;
            ClassName = lr.ClassName;
            ClassroomName = lr.ClassroomName;
            Group = lr.Group;
            LessonNumber = lr.LessonNumber;

        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long LessonId { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long TeacherId { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long ClassroomId { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string Note { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long ClassId { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string Group { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string TeacherName { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string ClassName { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string ClassroomName { get; set; }



        public byte LessonNumber { get; set; }
        //public LessonDB Lesson { get; set; }
        //public TeacherDB Teacher { get; set; }
        //public ClassroomDB Classroom { get; set; }
       // public LessonDB NewLesson { get; set; }
    }
}
