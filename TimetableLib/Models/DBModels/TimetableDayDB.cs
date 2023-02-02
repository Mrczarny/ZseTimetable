using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Timetables;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDayDB : IDBModel
    {
        public TimetableDayDB()
        {
        }

        public TimetableDayDB(TimetableDay td)
        {
            Day = td.Day;
            Lessons = td.Lessons.Select(x => new LessonDB(x)).ToList();
        }

        [SqlType(SqlDbType.BigInt)] public long TimetableId { get; set; }

        public List<LessonDB>? Lessons { get; set; }

        [SqlType(SqlDbType.SmallInt)] public DayOfWeek Day { get; set; }

        [Identity] [SqlType(SqlDbType.BigInt)] public long? Id { get; set; }
    }
}