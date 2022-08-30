using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class ClassDB
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TimetableId { get; set; }
        public string TimetableLink { get; set; }
    }
}
