using System.Collections.Generic;
using System.Linq;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;

namespace TimetableLib.DBAccess
{
    public abstract class TimetablesAccess : DbAccess
    {
        //Get by Name
        public abstract T? GetByLink<T>(string name) where T : class, ITimetables, new();

        /// <summary>
        ///     Gets all TimetableDays records by TimetableId
        /// </summary>
        /// <param name="timetableId"></param>
        /// <returns>IEnumerable of TimetableDay records</returns>
        public abstract IEnumerable<TimetableDayDB> GetTDaysByTId(long timetableId);

        public abstract IEnumerable<LessonDB> GetLessonsByTDayId(long TDayId);

        public void FillITimetablesModel<T>(T record) where T : class, ITimetables, new()
        {
            record.Timetable = Get<TimetableDB>(record.TimetableId);
            record.Timetable.Days = GetTDaysByTId(record.TimetableId);

            foreach (var day in record.Timetable.Days)
            {
                day.Lessons = this.GetLessonsByTDayId((long)day.Id)?.ToList();
            //day.Lessons = (from dayLessonDb in dayLessons
            //               join lessonDb in Lessons on dayLessonDb.LessonId equals
            //                   lessonDb.Id //
            //               select lessonDb ).ToList(); 
            }
        }
    }
}
