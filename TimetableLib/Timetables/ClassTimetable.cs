﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib
{
    public class ClassTimetable
    {
        public long Id { get; set; }
        public List<ClassDay> DaysTimetable { get; set; } // dumb name

    }
}
