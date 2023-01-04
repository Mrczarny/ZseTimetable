using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class ClassroomDB : IDBModel, ITimetables
    {
        public ClassroomDB(){}
        public ClassroomDB(Classroom cr)
        {
            Name = cr.Name;
            Timetable = new TimetableDB(cr.Timetable);
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.SmallInt)]
        public short Number { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }

        public string Link { get; set; }
        public TimetableDB Timetable { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string TimetableLink { get; set; }

    }
}
