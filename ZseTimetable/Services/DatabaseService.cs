using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;

namespace ZseTimetable.Services
{
    public class DatabaseService :  DataAccess,IHostedService,IDisposable
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly string _connectionString;

        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database Service running.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            
        }

        public override void Update<T>(int id, T record)
        {
            throw new NotImplementedException();
        }

        public override void Create<T>(T record)
        {
            var command = new SqlCommand($"dbo.{typeof(T).Name[..^2]}_Insert");
            foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.OfType<SqlDbType>() != null))
            {
                var parameter = new SqlParameter($"@{property.Name}",
                    property.GetValue(record));
                command.Parameters.Add(parameter);
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public override void Delete<T>(int id)
        {
            var command = new SqlCommand($"dbo.{typeof(T).Name[..^2]}_Delete");
            command.Parameters.Add(new SqlParameter("@Id", id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public override T Get<T>(int id)
        {
            SqlDataReader record;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand($"dbo.{typeof(T).Name[..^2]}_GetById", connection);
                connection.Open();
                record = command.ExecuteReader();
            }

            T result = new T();
            foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.OfType<SqlDbType>() != null))
            {
                property.SetValue(result, record.GetValue($"{property.Name}"));
            }

            return result;
        }

        public override T Get<T>()
        {
            SqlDataReader record;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand($"dbo.{typeof(T).Name[..^2]}_GetLatest", connection);
                connection.Open();
                record = command.ExecuteReader();
            }
            T result = new T();
            foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.OfType<SqlDbType>() != null))
            {
                property.SetValue(result, record.GetValue($"{property.Name}"));
            }

            return result;
        }

        public override IEnumerable<T> GetAll<T>()
        {
            SqlDataReader record;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand($"dbo.{typeof(T).Name[..^2]}_GetAll", connection);
                connection.Open();
                record = command.ExecuteReader();
            }
            List<T> results = new List<T>();
            while (record.Read())
            {
                T result = new T();
                foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.OfType<SqlDbType>() != null))
                {
                    property.SetValue(result, record.GetValue($"{property.Name}"));
                }

                results.Add(result);
            }
            return results;
        }
    }
}