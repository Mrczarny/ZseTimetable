using System;
using System.Collections.Generic;
using TimetableLib.Models.DBModels;

namespace TimetableLib.DataAccess
{
    public abstract class ChangesAccess : DbAccess
    {
        public abstract IEnumerable<T>? GetByDate<T>(DateTime date) where T : ReplacementDB, new();

        public abstract long? GetLessonId(long teacherId, byte lessonNumber, DayOfWeek day);
    }
}