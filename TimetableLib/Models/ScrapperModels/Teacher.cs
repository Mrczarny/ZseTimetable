using System.Text.RegularExpressions;
using TimetableLib.Models.DBModels;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public class Teacher : IScrappable
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Timetable Timetable { get; set; }
        public IDBModel GetDBModel()
        {
            return new TeacherDB(this);
        }
    }
}