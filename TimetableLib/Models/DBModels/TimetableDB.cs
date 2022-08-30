using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class TimetableDB
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
