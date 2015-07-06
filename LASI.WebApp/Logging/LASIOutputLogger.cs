﻿using System;
using Microsoft.Framework.Logging;

namespace LASI.WebApp.Logging
{
    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> filter;

        public OutputLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            this.filter = filter;
        }
        public ILogger CreateLogger(string name)
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create),
                $"WebApp_log{DateTime.Now.ToFileTimeUtc()}.txt");
            Utilities.Logger.SetToFile(null);
            return new Logger(name, filter);
        }

        private class Logger : ILogger
        {
            private readonly Func<string, LogLevel, bool> filter;
            private readonly string name;

            public Logger(string name, Func<string, LogLevel, bool> filter)
            {
                this.name = name ?? typeof(Utilities.Logger).FullName;
                this.filter = filter ?? delegate { return true; };
            }
            public IDisposable BeginScope(object state) => null;

            public bool IsEnabled(LogLevel logLevel) => filter(name, logLevel);

            public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    var message = formatter != null ? formatter(state, exception) : $"{exception.Message}\n{exception.StackTrace}";

                    var severity = logLevel.ToString().ToUpperInvariant();
                    Utilities.Logger.Log($"[{severity}:{name}] {message}");
                }
            }
        }
    }
}