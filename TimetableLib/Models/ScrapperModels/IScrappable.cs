using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public interface IScrappable
    {
        string Name { get; set; }
        Timetable Timetable { get; set; }
    }
}
