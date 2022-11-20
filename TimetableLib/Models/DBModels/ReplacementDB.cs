using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;

namespace TimetableLib.Models.DBModels
{
    public class ReplacementDB : IDBModel
    {
        public ReplacementDB(){}

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long OldLessonId { get; set; }
        public LessonDB OldLesson { get; set; }
        public LessonDB NewLesson { get; set; }
        [SqlType(SqlDbType.BigInt)]
        public long NewLessonId { get; set; }
        [SqlType(SqlDbType.NVarChar)]
        public string Note { get; set; }
    }
}
