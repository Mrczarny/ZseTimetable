using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;
using TimetableLib.Timetables;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDB : IDBModel
    {
        public TimetableDB(){}
        public TimetableDB(Timetable tt)
        {
            StartDate = tt.StartDate;
            EndDate = tt.EndDate;
            Days = tt.Days.Select(x => new TimetableDayDB(x));
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }
        public IEnumerable<TimetableDayDB> Days { get; set; }
        [SqlType(SqlDbType.Date)]
        public DateTime StartDate { get; set; }
        [SqlType(SqlDbType.Date)]
        public DateTime? EndDate { get; set; }
    }
}
