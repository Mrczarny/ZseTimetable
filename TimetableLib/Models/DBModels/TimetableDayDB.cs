using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDayDB
    {
        public long Id { get; set; }
        public long TimetableId { get; set; }
        public IEnumerable<LessonDB> Lessons { get; set; }
        public DateTime Day { get; set; }
    }
}
