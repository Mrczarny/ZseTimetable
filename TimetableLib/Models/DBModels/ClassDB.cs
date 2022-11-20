﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class ClassDB : IDBModel, ITimetables
    {
        public ClassDB(){}
        public ClassDB(Class cl)
        {
            Name = cl.Name;
            Timetable = new TimetableDB(cl.Timetable);

        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }
        public TimetableDB Timetable { get; set; }

        public string TimetableLink { get; set; }
    }
}
