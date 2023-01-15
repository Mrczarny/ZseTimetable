using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.ScrapperModels
{
    public interface IPersist
    {
        public T GetDBModel<T>() where T : class, IDBModel;
    }
}
