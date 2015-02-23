using System;
using EventStore.ClientAPI;
using NEventStore.Logging;

namespace NEventStore.Persistence.EventStore.Services
{
    public class EventStoreLogger : ILogger
    {
        private readonly ILog _logger;

        public EventStoreLogger(ILog logger)
        {
            _logger = logger;
        }

        public void Error(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            _logger.Error("Exception : {0} , Message {1}", ex.ToString(), string.Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            _logger.Info("Exception : {0} , Message {1}", ex.ToString(), string.Format(format, args));
        }

        public void Debug(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            _logger.Debug("Exception : {0} , Message {1}", ex.ToString(), string.Format(format, args));
        }
    }
}