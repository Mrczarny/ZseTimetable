using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDayLessonDB : IDBModel
    {
        public TimetableDayLessonDB() {}

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableDayId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long LessonId { get; set; }
    }
}
