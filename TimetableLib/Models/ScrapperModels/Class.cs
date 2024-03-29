﻿using TimetableLib.Models.DBModels;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Class : IScrappable
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Timetable Timetable { get; set; }

        public T GetDBModel<T>() where T : class, IDBModel
        {
            return new ClassDB(this) as T;
        }
    }
}