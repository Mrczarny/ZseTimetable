using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    public class TimetableServiceOption
    {
        public const string Position = "Types";
        public string type { get; set; }
        public char letter { get; set; }
    }
}
