using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmFaceVerify
{
   public class LogHelper
    {
        public static void WriteDebugLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);

            log.Debug(msg);
        }

        public static void WriteDebugLog(string className, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(className);

            log.Debug(msg);
        }

        public static void WriteErrorLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);

            log.Error(msg);
        }

        public static void WriteErrorLog(string className, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(className);

            log.Error(msg);
        }
        
        public static void WriteInfoLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);

            log.Info(msg);
        }

        public static void WriteInfoLog(string className, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(className);

            log.Info(msg);
        }

        public static void WriteFatalLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);

            log.Fatal(msg);
        }

        public static void WriteFatalLog(string className, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(className);

            log.Fatal(msg);
        }

        public static void WriteWarnLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);

            log.Warn(msg);
        }

        public static void WriteWarnLog(string calssName, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(calssName);

            log.Warn(msg);
        }
    }
}
