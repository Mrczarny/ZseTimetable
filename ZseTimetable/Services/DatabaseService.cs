﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.DBAccess;

namespace ZseTimetable.Services
{
    public class DatabaseService : IDataWrapper, IHostedService, IDisposable
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IDataWrapper _wrapper;

        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DefaultConnection");
            _wrapper = new SqlWrapper(_connectionString);
        }

        public TimetablesAccess TimetablesAccess => _wrapper.TimetablesAccess;
        public ChangesAccess ChangesAccess => _wrapper.ChangesAccess;
        public DbAccess DbAccess => _wrapper.DbAccess;

        public void Dispose()
        {
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database Service running.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
        }
    }
}