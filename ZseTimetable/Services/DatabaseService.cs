﻿using System;
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
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DBModels.DBAttributes;

namespace ZseTimetable.Services
{
    public class DatabaseService :  TimetablesAccess,IHostedService,IDisposable
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
            var command =
                new SqlCommand($"dbo.sp{record.GetType().Name[..^2]}_Update")
                {
                    CommandType = CommandType.StoredProcedure
                };


            foreach (var property in record.GetType().GetProperties().
                         Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(SqlTypeAttribute)) &&
                                    x.CustomAttributes.All(y => y.AttributeType != typeof(IdentityAttribute))))
            {
                var parameter = new SqlParameter
                {
                    ParameterName = $"@{property.Name}",
                    Value = property.GetValue(record),
                    SqlDbType = property.GetCustomAttribute<SqlTypeAttribute>().type
                };

                command.Parameters.Add(parameter);
            }
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@Id",
                Value = id,
                SqlDbType = SqlDbType.BigInt
            });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var cmdEx = command.ExecuteScalar(); // Doing it in one line throws nullreference
            }
        }

        public override long Create<T>(T record)
        {
            long id;
            var command = 
                new SqlCommand($"dbo.sp{record.GetType().Name[..^2]}_Insert")
            {
                CommandType = CommandType.StoredProcedure
            };


            foreach (var property in record.GetType().GetProperties().
                         Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(SqlTypeAttribute)) &&
                                            x.CustomAttributes.All(y => y.AttributeType != typeof(IdentityAttribute))))
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

            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_Delete")
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@Id", id));
           
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var cmdEx = command.ExecuteScalar(); // Doing it in one line throws nullreference
            }

        }

        public override T Get<T>(long id) 
        {
            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetById")
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@Id", id));
            T record = new T();
            var properties = record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                if (sqlData.HasRows)
                {
                    while (sqlData.Read())
                    {
                        foreach (var property in properties)
                        {
                            var value = sqlData.GetValue(property.Name);
                            property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return record;
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
                var value = record.GetValue(property.Name);
                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
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
                if (sqlData.HasRows)
                {
                    while (sqlData.Read())
                    {
                        foreach (var property in properties)
                        {
                            var value = sqlData.GetValue(property.Name);
                            property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return record;
        }

        public override T GetByLink<T>(string name) where T : class
        {
            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetByLink")
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@Link", name));

            T record = new T();
            var properties = record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                if (sqlData.HasRows)
                {
                    while (sqlData.Read())
                    {
                        foreach (var property in properties)
                        {
                            property.SetValue(record, sqlData.GetValue($"{property.Name}"));
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return record;
        }

        public override IEnumerable<T> GetAll<T>()
        {
            var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetAll")
            {
                CommandType = CommandType.StoredProcedure
            };

            T record = new T();
            List<T> records = new List<T>();
            var properties = record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                var sqlData = command.ExecuteReader();
                if (sqlData.HasRows)
                {
                    while (sqlData.Read())
                    {
                        record = new T();
                        foreach (var property in properties)
                        {
                            var value = sqlData.GetValue(property.Name);
                            property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                        }
                        records.Add(record);
                    }
                }
                else
                {
                    return null;
                }
            }

            return records;
        }
    }
}