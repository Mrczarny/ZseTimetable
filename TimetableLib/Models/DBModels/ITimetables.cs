using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;

namespace TimetableLib.Models.DBModels
{
    public interface ITimetables : IDBModel
    {
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }
        public TimetableDB Timetable { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Link { get; set; }
    }
}
