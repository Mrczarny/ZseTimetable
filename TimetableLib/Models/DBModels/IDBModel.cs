using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Models.DTOs;

namespace TimetableLib.Models.DBModels
{
    /// <summary>
    /// Models that represents tables in database
    /// </summary>
    public interface IDBModel
    {
        public long? Id { get; set; }
    }
}
