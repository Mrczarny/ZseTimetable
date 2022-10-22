using System;
using System.Collections.Generic;
using System.Linq;
using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class TimetableDTO
    {
        public TimetableDTO(TimetableDB tb)
        {
            Id = (long) tb.Id;
            StartDate = tb.StartDate;
            EndDate = tb.EndDate;
            Days = tb.Days.Select(x => new TimetableDayDTO(x));
        }
        public long Id { get; set; }
        public IEnumerable<TimetableDayDTO> Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
