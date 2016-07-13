using System;
using System.IO;
using NLog;

namespace PrintLogger
{
    public class NLogger : INLogger
    {
        private static readonly Logger NLog = LogManager.GetCurrentClassLogger();

        public void Trace(string msg)
        {
            NLog.Trace(msg);
        }

        public void Error(string msg)
        {
            NLog.Error(msg);
        }

        public void Info(string msg)
        {
            NLog.Info(msg);
        }

        public void Debug(string msg)
        {
            NLog.Debug(msg);
        }
        public void Warning(string msg)
        {
            NLog.Warn(msg);
        }

    }
}
