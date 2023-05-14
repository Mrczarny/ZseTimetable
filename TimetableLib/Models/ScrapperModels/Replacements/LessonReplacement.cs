using System;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Changes
{
    public class LessonReplacement : IPersist
    {
        public string OriginalTeacher { get; set; }
        public DateTime? DayOfReplacement { get; set; }
        public byte LessonNumber { get; set; }
        public string ClassName { get; set; }
        public string ClassroomName { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string Sub { get; set; }
        public string Note { get; set; }

        public T GetDBModel<T>() where T : class, IDBModel
        {
            return new ReplacementDB(this) as T;
        }
    }
}