using System;
using System.Data;
using TimetableLib.Models.DBModels.DBAttributes;

namespace TimetableLib.Models.DBModels
{
    /// <summary>
    ///     Models of objects that contain timetable within themselves,
    ///     such as Classes, Classrooms or Teachers
    /// </summary>
    public interface ITimetables : IDBModel, IDisposable
    {
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)] public string Name { get; set; }

        public TimetableDB Timetable { get; set; }

        [SqlType(SqlDbType.BigInt)] public long TimetableId { get; set; }

        [SqlType(SqlDbType.NVarChar)] public string Link { get; set; }

        public void SetLessonId(LessonDB ls);

        public void SetLessonName(LessonDB ls);

        public string? GetLessonLink(LessonDB ls);

        public string? GetLessonName(LessonDB ls);
    }
}