using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class LessonDB
    {
        public long Id { get; set; }
        public byte LessonNumber { get; set; }
        public byte GroupNumber { get; set; }
        public string Name { get; set; }
        public long ClassId { get; set; }
        public long TimetableDayId { get; set; }
        public long TeacherId { get; set; }
        public long ClassroomId { get; set; }


    }
}
