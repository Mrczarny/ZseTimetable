using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableLib.DataAccess
{
    public class DataAccess
    {
        private string ConnectionString;
        public DataAccess(string _connectionString)
        {
            ConnectionString = _connectionString;
        }

        public void Update<T>(int id)
        {
            // Update 
        }
        public void Create<T>()
        {
            // Create 
        }
        public void Delete<T>(int id)
        {
            // Delete 
        }
        public T Get<T>(int id)
        {
            // Get 
            throw new NotImplementedException(); 
        }
        public T Get<T>()
        {
            // Get 
            throw new NotImplementedException();
        }

    }
}
