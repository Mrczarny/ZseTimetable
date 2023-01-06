using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.Models.DBModels.DBAttributes
{
    /// <summary>
    /// Attribute to mark fields that represents identities in database  
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class IdentityAttribute : Attribute
    {
        
    }
}
