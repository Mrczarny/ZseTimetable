using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;

namespace TimetableLib.DBAccess
{
    public abstract class TimetablesAccess : DbAccess
    {
        //Get by Name
        public abstract T? GetByName<T>(string name) where T : class, ITimetables, new(); // TODO - add restraint to this

        //Get by Name
        public abstract T? GetByLink<T>(string name) where T : class, ITimetables, new();
    }
}
