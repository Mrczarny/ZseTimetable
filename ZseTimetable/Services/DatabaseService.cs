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
using TimetableLib.Models.DBModels.DBAttributes;

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

        public override void Update<T>(long id, T record)
        {
            throw new NotImplementedException();
        }

        public override long Create<T>(T record)
        {
            long id;
            var command = 
                new SqlCommand($"dbo.sp{record.GetType().Name[..^2]}_Insert")
            {
                CommandType = CommandType.StoredProcedure
            };


            foreach (var property in record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute) && x.AttributeType != typeof(IdentityAttribute))))
            {
                var parameter = new SqlParameter
                {
                    ParameterName = $"@{property.Name}",
                    Value = property.GetValue(record),
                    SqlDbType = property.GetCustomAttribute<SqlTypeAttribute>().type
                };

                command.Parameters.Add(parameter);
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var cmdEx = command.ExecuteScalar(); // Doing it in one line throws nullreference
                id = (long)cmdEx; 
            }

            return id;
        }

        public override void Delete<T>(long id)
        {

            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_Delete");
            command.Parameters.Add(new SqlParameter("@Id", id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public override T Get<T>(long id) 
        {
            SqlDataReader record;
            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetById");
            command.Parameters.Add(new SqlParameter("@Id", id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                
                connection.Open();
                record = command.ExecuteReader();
            }

            T result = new T();
            foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute))))
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
            foreach (var property in typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute))))
            {
                property.SetValue(result, record.GetValue($"{property.Name}"));
            }

            return result;
        }

        public override T GetByName<T>(string name)
        {
            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetByName")
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@Name", name));

            T record = new T();
            var properties = record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));
            

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                while (sqlData.Read())
                {
                    foreach (var property in properties)
                    {
                        property.SetValue(record, sqlData.GetValue($"{property.Name}"));
                    }
                }
            }



            return record;
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