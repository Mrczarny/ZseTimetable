﻿using TimetableLib.DBAccess;

namespace TimetableLib.DataAccess
{
    public interface IDataWrapper
    {
        public TimetablesAccess TimetablesAccess { get; }
        public ChangesAccess ChangesAccess { get; }
        public DbAccess DbAccess { get; }
    }
}