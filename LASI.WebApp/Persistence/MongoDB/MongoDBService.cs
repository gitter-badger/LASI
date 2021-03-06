﻿using System;
using System.Net.Sockets;
using MongoDB.Driver;

namespace LASI.WebApp.Persistence.MongoDB
{
    using Process = System.Diagnostics.Process;
    using Directory = System.IO.Directory;
    using Utilities;

    public class MongoDBService : IDisposable
    {
        public MongoDBService(MongoDBConfiguration config)
        {
            this.config = config;
            EnsureDataBaseDirectory();
            if (config.CreateProcess)
            {
                this.mongoDbProcess = StartDatabaseProcess();
            }
        }

        private Process StartDatabaseProcess() => Process.Start(fileName: config.MongodExePath, arguments: $"--dbpath {config.DataDbPath}");

        private void EnsureDataBaseDirectory()
        {
            if (!Directory.Exists(config.DataDbPath))
            {
                Directory.CreateDirectory(config.DataDbPath);
            }
        }

        private MongoDatabase GetDatabase() => new MongoClient(new MongoUrl(config.InstanceUrl)).GetServer().GetDatabase(config.ApplicationDatabaseName);
        public MongoCollection<TDocument> GetCollection<TDocument>()
        {
            try
            {
                var name = config[typeof(TDocument)];
                return GetDatabase().GetCollection<TDocument>(name);
            }
            catch (MongoConnectionException e) when (e.InnerException is SocketException)
            {
                e.Log();
                e.InnerException.Log();
                mongoDbProcess = StartDatabaseProcess();
                return GetCollection<TDocument>();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mongoDbProcess?.Dispose();
                    mongoDbProcess = null;
                }
                disposedValue = true;
            }
        }
        /// <summary>
        /// Disposes the <see cref="MongoDBService"/> instance.
        /// </summary>
        ~MongoDBService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the <see cref="MongoDBService"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private readonly MongoDBConfiguration config;

        private Process mongoDbProcess;
    }
}