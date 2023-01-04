using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Models.DBModels;

namespace TimetableLib.DataAccess
{
    public abstract class DbAccess
    {

        protected DbAccess()
        {
        }

        //Update
        public abstract void Update<T>(long id, T record) where T: IDBModel;

        //Create
        public abstract long Create<T>(T record) where T: IDBModel;

        // Delete 
        public abstract void Delete<T>(long id) where T: IDBModel;

        // Get by id
        public abstract T? Get<T>(long id) where T : class,IDBModel, new();

        // Get latest
        public abstract T? Get<T>() where T : class, IDBModel, new();


        // Gets all records 
        public abstract IEnumerable<T>? GetAll<T>() where T : class, IDBModel, new();

    }
}
