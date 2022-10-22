using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Models.DTOs;

namespace TimetableLib.Models.DBModels
{
    public interface IDBModel
    {
        public long? Id { get; set; }
    }
}
