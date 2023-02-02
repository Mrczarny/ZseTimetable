using TimetableLib.Models.DBModels;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public interface IScrappable : IPersist
    {
        string Name { get; set; }
        string Link { get; set; }
        Timetable Timetable { get; set; }
        public T GetDBModel<T>() where T : class, IDBModel;
    }
}