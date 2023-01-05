using System;
using System.Collections.Generic;
using System.Text;
using TimetableLib.Models.DBModels;

namespace TimetableLib.DataAccess
{
    /// <summary>
    /// Class responsible for handling connection and traffic to and from database 
    /// </summary>
    public abstract class DataAccess
    {

        protected DbAccess()
        {
        }

        /// <summary>
        /// Updates existing record in database
        /// </summary>
        /// <typeparam name="T">DB model of this record</typeparam>
        /// <param name="id">Id of record that has to be updated</param>
        /// <param name="record">Updated record</param>
        public abstract void Update<T>(long id, T record) where T: IDBModel;

        //Create
        /// <summary>
        /// Creates new record in database
        /// </summary>
        /// <typeparam name="T">DB model of a new record</typeparam>
        /// <param name="record">Record to be created</param>
        /// <returns>Id of a new record in database</returns>
        public abstract long Create<T>(T record) where T: IDBModel;

        // Delete 
        /// <summary>
        /// Deletes record in database
        /// </summary>
        /// <typeparam name="T">DB model of this record</typeparam>
        /// <param name="id">DB Id of this record</param>
        public abstract void Delete<T>(long id) where T: IDBModel;

        // Get by id
        /// <summary>
        /// Gets existing record from database by DB id
        /// </summary>
        /// <typeparam name="T">DB model of this record</typeparam>
        /// <param name="id">DB Id of this record</param>
        /// <returns>If existing - returns record with matching Id,
        /// otherwise null</returns>
        public abstract T Get<T>(long id) where T : IDBModel, new();

        // Get latest
        /// <summary>
        /// Gets latest existing record from database
        /// </summary>
        /// <typeparam name="T">DB model of this record</typeparam>
        /// <returns>If existing - returns latest record,
        /// otherwise null</returns>
        public abstract T Get<T>() where T : IDBModel, new();

        //Get by Name
        /// <summary>
        /// Gets existing record from database by DB id
        /// </summary>
        /// <typeparam name="T">DB model of this record</typeparam>
        /// <param name="name">Field "Name" of this record</param>
        /// <returns>If existing - returns record with matching "Name",
        /// otherwise null</returns>
        public abstract T GetByName<T>(string name) where T : IDBModel, new();

        // Gets all records 
        /// <summary>
        /// Gets all existing records from database 
        /// </summary>
        /// <typeparam name="T">DB model of this records</typeparam>
        /// <returns>If existing - returns all records in given table,
        /// otherwise null</returns>
        public abstract IEnumerable<T> GetAll<T>() where T : IDBModel, new();

    }
}
