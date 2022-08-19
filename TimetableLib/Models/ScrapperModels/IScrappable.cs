using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TimetableLib.Timetables;

namespace TimetableLib.Models.ScrapperModels
{
    public interface IScrappable
    {
        int Id { get; set; }
        string Name { get; set; }
        Timetable Timetable { get; set; }
    }
}
