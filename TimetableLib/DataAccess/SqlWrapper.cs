using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DBModels.DBAttributes;

namespace TimetableLib.DataAccess
{
    public class SqlWrapper : IDataWrapper
    {
        protected readonly string _connectionString;

        public SqlWrapper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TimetablesAccess TimetablesAccess => new SqlTimetableWrapper(_connectionString);
        public ChangesAccess ChangesAccess => new SqlChangesWrapper(_connectionString);
        public DbAccess DbAccess => new SqlBaseWrapper(_connectionString);

        private static bool IsDbProperty(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(SqlTypeAttribute));
        }

        private class SqlBaseWrapper : DbAccess
        {
            private readonly string _connectionString;

            public SqlBaseWrapper(string connectionString)
            {
                _connectionString = connectionString;
            }

            public override void Update<T>(long id, T record)
            {
                var command =
                    new SqlCommand($"dbo.sp{record.GetType().Name[..^2]}_Update")
                    {
                        CommandType = CommandType.StoredProcedure
                    };


                foreach (var property in record.GetType().GetProperties().Where(x => IsDbProperty(x) && x.Name != "Id"))
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


                foreach (var property in record.GetType().GetProperties().Where(x => IsDbProperty(x) && x.Name != "Id"))
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
                    id = (long) cmdEx;
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
                var record = new T();
                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (sqlData.HasRows)
                        while (sqlData.Read())
                            foreach (var property in properties)
                            {
                                var value = sqlData.GetValue(property.Name);
                                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                            }
                    else
                    {
                        connection.Close();
                        return null;
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

                var result = new T();
                foreach (var property in typeof(T).GetProperties().Where(IsDbProperty))
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

                var record = new T();
                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (sqlData.HasRows)
                        while (sqlData.Read())
                            foreach (var property in properties)
                            {
                                var value = sqlData.GetValue(property.Name);
                                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                            }
                    else
                    {
                        return null;
                }

                return record;
            }

            public override IEnumerable<T> GetAll<T>()
            {
                var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetAll")
                {
                    CommandType = CommandType.StoredProcedure
                };

                var record = new T();
                var records = new List<T>();
                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader();
                    if (sqlData.HasRows)
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
                    else
                        return Enumerable.Empty<T>();
                }

                return records;
            }
        }

        private class SqlTimetableWrapper : TimetablesAccess
        {
            private readonly SqlBaseWrapper _baseWrapper;
            private readonly string _connectionString;

            public SqlTimetableWrapper(string connectionString)
            {
                _connectionString = connectionString;
                _baseWrapper = new SqlBaseWrapper(connectionString);
            }


            public override void Update<T>(long id, T record)
            {
                _baseWrapper.Update(id, record);
            }

            public override long Create<T>(T record)
            {
                return _baseWrapper.Create(record);
            }

            public override void Delete<T>(long id)
            {
                _baseWrapper.Delete<T>(id);
            }

            public override T Get<T>(long id)
            {
                return _baseWrapper.Get<T>(id);
            }

            public override T Get<T>()
            {
                return _baseWrapper.Get<T>();
            }

            public override IEnumerable<T> GetAll<T>()
            {
                return _baseWrapper.GetAll<T>();
            }

            public override T GetByName<T>(string name)
            {
                return _baseWrapper.GetByName<T>(name);
            }

            public override T GetByLink<T>(string name) where T : class
            {
                var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetByLink")
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Link", name));

                var record = new T();
                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (sqlData.HasRows)
                        while (sqlData.Read())
                            foreach (var property in properties)
                            {
                                var value = sqlData.GetValue(property.Name);
                                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                            }
                    else
                        return null;
                }

                return record;
            }

            public override IEnumerable<TimetableDayDB> GetTDaysByTId(long timetableId)
            {
                var command = new SqlCommand("dbo.spTimetableDay_GetByTimetableId")
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@TimetableId", timetableId));

                var record = new TimetableDayDB();
                var records = new List<TimetableDayDB>();


                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader();
                    if (sqlData.HasRows)
                        while (sqlData.Read())
                        {
                            record = new TimetableDayDB();
                            foreach (var property in properties)
                            {
                                var value = sqlData.GetValue(property.Name);
                                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                            }

                            records.Add(record);
                        }
                    else
                        return Enumerable.Empty<TimetableDayDB>();
                }

                return records;
            }

            public override IEnumerable<LessonDB> GetLessonsByTDayId(long tDayId)
            {
                var command = new SqlCommand("dbo.spLesson_GetByTimetableDayId")
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@TimetableDayId", tDayId));

                var record = new LessonDB();
                var records = new List<LessonDB>();
                var properties = record.GetType().GetProperties().Where(IsDbProperty);


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader();
                    if (sqlData.HasRows)
                        while (sqlData.Read())
                        {
                            record = new LessonDB();
                            foreach (var property in properties)
                            {
                                var value = sqlData.GetValue(property.Name);
                                property.SetValue(record, value.Equals(DBNull.Value) ? null : value);
                            }

                            records.Add(record);
                        }
                    else
                        return Enumerable.Empty<LessonDB>();
                }

                return records;
            }
        }

        private class SqlChangesWrapper : ChangesAccess
        {
            private readonly SqlBaseWrapper _baseWrapper;
            private readonly string _connectionString;

            public SqlChangesWrapper(string connectionString)
            {
                _connectionString = connectionString;
                _baseWrapper = new SqlBaseWrapper(connectionString);
            }

            public override void Update<T>(long id, T record)
            {
                _baseWrapper.Update(id, record);
            }

            public override long Create<T>(T record)
            {
                return _baseWrapper.Create(record);
            }

            public override void Delete<T>(long id)
            {
                _baseWrapper.Delete<T>(id);
            }

            public override T Get<T>(long id)
            {
                return _baseWrapper.Get<T>(id);
            }

            public override T Get<T>()
            {
                return _baseWrapper.Get<T>();
            }

            public override T GetByName<T>(string name)
            {
                return _baseWrapper.GetByName<T>(name);
            }

            public override IEnumerable<T> GetAll<T>()
            {
                return _baseWrapper.GetAll<T>();
            }

            public override IEnumerable<T> GetByDate<T>(DateTime date)
            {
                var command = new SqlCommand($"dbo.sp{typeof(T).Name[..^2]}_GetByDate")
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Date", date.Date));

                T record = new T();
                List<T> records = new List<T>();
                var properties = record.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (sqlData.HasRows)
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
                    else
                        return Enumerable.Empty<T>();
                }

                return records;
            }

            public override long? GetLessonId(long teacherId, byte lessonNumber, DayOfWeek day)
            {
                var command = new SqlCommand("dbo.spLesson_GetByTeacherId")
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@TeacherId", teacherId));
                command.Parameters.Add(new SqlParameter("@LessonNumber", lessonNumber));
                command.Parameters.Add(new SqlParameter("@Day", day));

                var properties = typeof(LessonDB).GetProperties().Where(x =>
                    x.CustomAttributes.Any(x => x.AttributeType == typeof(SqlTypeAttribute)));


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    var sqlData = command.ExecuteReader(CommandBehavior.SingleRow);
                    long? id = null;
                    if (sqlData.HasRows)
                    {
                        while (sqlData.Read()) id = (long) sqlData[0];

                        return id;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private static bool IsDbProperty(PropertyInfo property)
        {
           return Attribute.IsDefined(property, typeof(SqlTypeAttribute));
        }
    }
}