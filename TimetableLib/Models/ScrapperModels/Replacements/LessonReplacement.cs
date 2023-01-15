using System;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Changes
{
    public class LessonReplacement : IPersist
    {
        public string TeacherName { get; set; }
        public byte LessonNumber { get; set; }
        public string Description { get; set; }
        public string Sub { get; set; }
        public string Note { get; set; }

        public T GetDBModel<T>() where T : class, IDBModel
        {
            return new ReplacementDB(this) as T;
        }
    }
}