using System;

namespace TimetableLib.Models.DTOs
{
    public class TimetableDTO
    {
        public long Id { get; set; }
        public IEquatable<TimetableDayDTO> Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
