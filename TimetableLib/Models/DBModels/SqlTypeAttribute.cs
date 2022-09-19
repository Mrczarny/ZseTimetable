using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace TimetableLib.Models.DBModels
{
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SqlTypeAttribute : Attribute
    {
        public SqlDbType type;

        public SqlTypeAttribute(SqlDbType t)
        {
            this.type = t;
        }
    }
}
