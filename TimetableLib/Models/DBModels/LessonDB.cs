using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;
using TimetableLib.Timetables;

namespace TimetableLib.Models.DBModels
{
    public class LessonDB : IDBModel
    {
        public LessonDB(){}
        public LessonDB(Lesson ls)
        {
            Name = ls.LessonName;
            LessonNumber = (byte)ls.LessonNumber;
            Group = ls.Group; //TODO - It will cause a crash
                                                //Sure it did, for god's sake
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }
        [SqlType(SqlDbType.TinyInt)]
        public byte LessonNumber { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Group { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long? ClassId { get; set; }
        //[SqlType(SqlDbType.BigInt)]
        //public long TimetableDayId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long? TeacherId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long? ClassroomId { get; set; }



    }
}
