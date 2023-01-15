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
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Timetables;

namespace ZseTimetable.Services
{
    public class DatabaseService : IDataWrapper, IHostedService,IDisposable
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly string _connectionString;
        private readonly IDataWrapper _wrapper;

        public TimetablesAccess TimetablesAccess => _wrapper.TimetablesAccess;
        public ChangesAccess ChangesAccess => _wrapper.ChangesAccess;
        public DbAccess DbAccess => _wrapper.DbAccess;

        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DefaultConnection");
            _wrapper = new SqlWrapper(_connectionString);
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



    }
}