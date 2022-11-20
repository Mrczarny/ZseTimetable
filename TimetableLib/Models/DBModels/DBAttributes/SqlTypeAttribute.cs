using System;
using System.Data;

namespace TimetableLib.Models.DBModels.DBAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SqlTypeAttribute : Attribute
    {
        public SqlDbType type;

        public SqlTypeAttribute(SqlDbType t)
        {
            type = t;
        }
    }
}
