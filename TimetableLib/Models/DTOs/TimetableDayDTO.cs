using System;
using System.Collections.Generic;

namespace TimetableLib.Models.DTOs
{
    public class TimetableDayDTO
    {
        public long Id { get; set; }
        public IEnumerable<LessonDTO> Lessons { get; set; }
        public DateTime Day { get; set; }
    }
}
