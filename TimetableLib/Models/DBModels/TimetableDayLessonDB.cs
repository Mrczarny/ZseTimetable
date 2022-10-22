using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDayLessonDB : IDBModel
    {
        public long? Id { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableDayId { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long LessonId { get; set; }
    }
}
