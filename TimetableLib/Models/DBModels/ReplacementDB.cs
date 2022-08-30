using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class ReplacementDB
    {
        public long Id { get; set; }
        public long LessonId { get; set; }
        public string Note { get; set; }
    }
}
