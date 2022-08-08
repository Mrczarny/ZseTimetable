using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Changes;

namespace TimetableLib.Models.Replacements
{
    //Contains one day's replacements 
    public class DayReplacement
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public IEnumerable<TeacherReplacement>? Replacements { get; set; }
    }
}
