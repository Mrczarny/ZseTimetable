using System;
using System.Collections.Generic;
using System.Linq;
using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class TimetableDayDTO
    {
        public TimetableDayDTO(TimetableDayDB tb)
        {
            Id = (long) tb.Id;
            Day = tb.Day;
            Lessons = tb.Lessons.Select(x => new LessonDTO(x));
        }
        public long Id { get; set; }
        public IEnumerable<LessonDTO> Lessons { get; set; }
        public DayOfWeek Day { get; set; }
    }
}
