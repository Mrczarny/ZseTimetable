using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DTOs;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class TeacherDB : IDBModel
    {
        public TeacherDB(Teacher tr)
        {
            Name = tr.Name;
            ShortName = tr.ShortName;
            Timetable = new TimetableDB(tr.Timetable);
        }

        [SqlType(SqlDbType.BigInt)]
        public long Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string ShortName { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }
        public TimetableDB Timetable { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string TimetableLink { get; set; }

    }
}
