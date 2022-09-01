using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DTOs
{
    public class ClassDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TimetableDTO Timetable { get; set; }
    }
}
