using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TimetableLib.Models.DBModels;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Classroom : IScrappable
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Timetable Timetable { get; set; }
        public T GetDBModel<T>() where T : class, IDBModel
        {
            return new ClassroomDB(this) as T;
        }
    }
}
