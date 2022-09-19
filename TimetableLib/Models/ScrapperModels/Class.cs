﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TimetableLib.Models.DBModels;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Class : IScrappable
    {
        public string Name { get; set; }
        public Timetable Timetable { get; set; }
        public IDBModel GetDBModel()
        {
            return new ClassDB(this);
        }
    }
}
