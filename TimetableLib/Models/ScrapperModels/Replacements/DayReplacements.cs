using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Changes;

namespace TimetableLib.Models.Replacements
{
    //Contains one day's replacements 
    public class DayReplacements
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public IEnumerable<TeacherReplacements>? Replacements { get; set; }
    }
}
