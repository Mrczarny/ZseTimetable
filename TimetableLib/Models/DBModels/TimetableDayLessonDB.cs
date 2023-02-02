using System.Data;
using TimetableLib.Models.DBModels.DBAttributes;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDayLessonDB : IDBModel
    {
        [SqlType(SqlDbType.BigInt)] public long? TimetableDayId { get; set; }

        [SqlType(SqlDbType.BigInt)] public long? LessonId { get; set; }

        [Identity] [SqlType(SqlDbType.BigInt)] public long? Id { get; set; }
    }
}