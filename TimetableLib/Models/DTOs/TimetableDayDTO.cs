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
            Lessons = tb.Lessons.Select(x => new LessonDTO(x)).ToList();
        }

        public long Id { get; set; }
        public List<LessonDTO> Lessons { get; set; }
        public DayOfWeek Day { get; set; }
    }
}