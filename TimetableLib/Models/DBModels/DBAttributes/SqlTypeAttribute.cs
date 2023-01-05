using System;
using System.Data;

namespace TimetableLib.Models.DBModels.DBAttributes
{
    /// <summary>
    /// Attribute to mark fields that represents columns in database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SqlTypeAttribute : Attribute
    {
        /// <summary>
        /// SQL type of data of represented column in database
        /// </summary>
        public SqlDbType type;

        public SqlTypeAttribute(SqlDbType t)
        {
            type = t;
        }
    }
}
