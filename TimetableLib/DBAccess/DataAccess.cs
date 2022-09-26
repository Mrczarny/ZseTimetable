using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.DataAccess
{
    public abstract class DataAccess
    {

        protected DataAccess()
        {
        }

        //Update
        public abstract void Update<T>(int id, T record);

        //Create
        public abstract void Create<T>(T record);

        // Delete 
        public abstract void Delete<T>(int id);

        // Get by id
        public abstract T Get<T>(int id) where T : new();

        // Get latest
        public abstract T Get<T>() where T : new();

        // Gets all records 
        public abstract IEnumerable<T> GetAll<T>() where T : new();

    }
}
