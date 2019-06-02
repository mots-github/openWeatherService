using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Configuration;


namespace OpenWeatherService.Common
{
    // Enums for level of logging
    public enum EventType
    {
        Normal,
        Exception,       
        Error,
        Warning,
       
    }
    public static class Logger
    {
        private const string CLASS = "Daimler.SalesAndMarketing.SalesCampaign.Common.Logger";
        private static bool? toLog = null;
        private static string logFolderPath = string.Empty;
        private static string eventLogSource = string.Empty;


        public static void Publish(EventType eventType, string sourceClass, string sourceMethod, string message)
        {
            Publish(eventType, sourceClass, sourceMethod, message, string.Empty, string.Empty);
        }
        public static void Publish(EventType eventType, string sourceClass, string sourceMethod, string message, string innerMessage)
        {
            Publish(eventType, sourceClass, sourceMethod, message, innerMessage, string.Empty);
        }
        /// <summary>
        /// This method publish the message to Log File and Event Log
        /// </summary>
        /// <param name="eventType">This represents eventType (error, warning etc.)</param>
        /// <param name="sourceClass">This represents sourceClass for which the log is being written</param>
        /// <param name="sourceMethod">This represents sourceMethod for which the log is being written</param>
        /// <param name="message">This represents the actual message</param>
        /// <param name="userId">This represents  userId</param>
        /// <param name="innerMessage">This represents InnerException in case of exception</param>
        /// <param name="stackTraceMessage">This represents complete stackTrace in case of exception</param>
        public static void Publish(EventType eventType, string sourceClass, string sourceMethod, string message, string innerMessage, string stackTraceMessage)
        {
            WriteToLogFile(eventType, sourceClass, sourceMethod, message, innerMessage, stackTraceMessage);
            if (eventType == EventType.Exception || eventType == EventType.Error || eventType == EventType.Warning)
            {
                WriteToEventLog(eventType, sourceClass, sourceMethod, message, innerMessage, stackTraceMessage);
            }
        }

        /// <summary>
        /// This method writes messages to Log File.
        /// </summary>
        /// <param name="eventType">This represents eventType (error, warning etc.)</param>
        /// <param name="sourceClass">This represents sourceClass for which the log is being written</param>
        /// <param name="sourceMethod">This represents sourceMethod for which the log is being written</param>
        /// <param name="message">This represents the actual message</param>
        /// <param name="userId">This represents  userId</param>
        /// <param name="innerMessage">This represents InnerException in case of exception</param>
        /// <param name="stackTraceMessage">This represents complete stackTrace in case of exception</param>
        private static void WriteToLogFile(EventType eventType, string sourceClass, string sourceMethod, string message, string innerMessage, string stackTraceMessage)
        {
            const string METHOD = "WriteToLogFile";

            try
            {
                if (toLog == null)
                {
                    int Log;
                    Log = Int32.Parse(ConfigurationManager.AppSettings["Log"]);
                    if (Log == 1)
                    {
                        logFolderPath = ConfigurationManager.AppSettings["LogFolderPath"];
                        if (!Directory.Exists(logFolderPath))
                        {
                            WriteToEventLog(EventType.Warning, CLASS, METHOD, Resources.logMsg001.Replace("{1}", logFolderPath), string.Empty, string.Empty);
                            try
                            {
                                Directory.CreateDirectory(logFolderPath);
                                WriteToEventLog(EventType.Normal, CLASS, METHOD, Resources.logMsg002.Replace("{1}", logFolderPath), string.Empty, string.Empty);
                                toLog = true;
                            }
                            catch (Exception ex)
                            {
                                WriteToEventLog(EventType.Error, CLASS, METHOD, Resources.logMsg003, ex.Message, string.Empty);
                                toLog = false;
                            }
                        }
                        else
                        {
                            WriteToEventLog(EventType.Normal, CLASS, METHOD, Resources.logMsg002.Replace("{1}", logFolderPath), string.Empty, string.Empty);
                            toLog = true;
                        }
                    }
                    else
                    {
                        WriteToEventLog(EventType.Normal, CLASS, METHOD, Resources.logMsg004, string.Empty, string.Empty);
                        toLog = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog(EventType.Error, CLASS, METHOD, Resources.logMsg005, ex.Message, string.Empty);
                toLog = false;
            }

            if (toLog.Value)
            {
                FileStream logFileStream = null;
                StreamWriter streamWriter = null;
                string logFilePath = null;
                try
                {
                    switch (eventType)
                    {
                        case EventType.Normal:
                            {
                                logFilePath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy.MM.dd") + ".Activity.log");
                                logFileStream = new FileStream(logFilePath, FileMode.Append);
                                streamWriter = new StreamWriter(logFileStream);
                                streamWriter.WriteLine("Time           : " + DateTime.Now.TimeOfDay);
                                streamWriter.WriteLine("Source Class   : " + sourceClass);
                                streamWriter.WriteLine("Source Method  : " + sourceMethod);
                                streamWriter.WriteLine("Message        : " + message);
                                streamWriter.WriteLine("Additional Info: " + innerMessage);
                                streamWriter.WriteLine(Resources.logTextLineSeparator);
                                break;
                            }
                        case EventType.Exception:
                            {
                                logFilePath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy.MM.dd") + ".Exception.log");
                                logFileStream = new FileStream(logFilePath, FileMode.Append);
                                streamWriter = new StreamWriter(logFileStream);
                                streamWriter.WriteLine("Time           : " + DateTime.Now.TimeOfDay);
                                streamWriter.WriteLine("Source Class   : " + sourceClass);
                                streamWriter.WriteLine("Source Method  : " + sourceMethod);
                                streamWriter.WriteLine("Exception      : " + message);
                                streamWriter.WriteLine("Inner Exception: " + innerMessage);
                                streamWriter.WriteLine("Stack Trace    : " + stackTraceMessage);
                                streamWriter.WriteLine(Resources.logTextLineSeparator);
                                break;
                            }                    
                        case EventType.Error:
                            {
                                logFilePath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy.MM.dd") + ".Error.log");
                                logFileStream = new FileStream(logFilePath, FileMode.Append);
                                streamWriter = new StreamWriter(logFileStream);
                                streamWriter.WriteLine("Time           : " + DateTime.Now.TimeOfDay);
                                streamWriter.WriteLine("Severity       : Error");
                                streamWriter.WriteLine("Source Class   : " + sourceClass);
                                streamWriter.WriteLine("Source Method  : " + sourceMethod);
                                streamWriter.WriteLine("Message        : " + message);
                                streamWriter.WriteLine("Additional Info: " + innerMessage);
                                streamWriter.WriteLine(Resources.logTextLineSeparator);
                                break;
                            }
                        case EventType.Warning:
                            {
                                logFilePath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy.MM.dd") + ".Error.log");
                                logFileStream = new FileStream(logFilePath, FileMode.Append);
                                streamWriter = new StreamWriter(logFileStream);
                                streamWriter.WriteLine("Time           : " + DateTime.Now.TimeOfDay);
                                streamWriter.WriteLine("Severity       : Warning");
                                streamWriter.WriteLine("Source Class   : " + sourceClass);
                                streamWriter.WriteLine("Source Method  : " + sourceMethod);
                                streamWriter.WriteLine("Message        : " + message);
                                streamWriter.WriteLine("Additional Info: " + innerMessage);
                                streamWriter.WriteLine(Resources.logTextLineSeparator);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(logFilePath))
                    {
                        WriteToEventLog(EventType.Exception, CLASS, METHOD, Resources.logMsg006.Replace("{1}", logFilePath), ex.Message, ex.StackTrace);
                    }
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                    }
                    if (logFileStream != null)
                    {
                        logFileStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// This method writes messages to EventLog. This method also creates EventLogSource.
        /// </summary>
        /// <param name="eventType">This represents eventType (error, warning etc.)</param>
        /// <param name="sourceClass">This represents sourceClass for which the log is being written</param>
        /// <param name="sourceMethod">This represents sourceMethod for which the log is being written</param>
        /// <param name="message">This represents the actual message</param>
        /// <param name="userId">This represents  userId</param>
        /// <param name="innerMessage">This represents InnerException in case of exception</param>
        /// <param name="stackTraceMessage">This represents complete stackTrace in case of exception</param>
        private static void WriteToEventLog(EventType eventType, string sourceClass, string sourceMethod, string message, string innerMessage, string stackTraceMessage)
        {

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";

                StringBuilder strLogMessage = new StringBuilder();
                strLogMessage.Append("Source: " + sourceClass + ", " + sourceMethod + "()\n");
                if (message != string.Empty)
                {
                    strLogMessage.Append("Message:" + message + "\n");
                }
                if (innerMessage != string.Empty)
                {
                    strLogMessage.Append("Inner Message:" + innerMessage + "\n");
                }
                if (stackTraceMessage != string.Empty)
                {
                    strLogMessage.Append("Stack Trace:" + stackTraceMessage + "\n");
                }

                switch (eventType)
                {
                    case EventType.Exception:
                        {
                            eventLog.WriteEntry(strLogMessage.ToString().TrimEnd(), EventLogEntryType.Error);
                            break;
                        }
                    case EventType.Warning:
                    case EventType.Error:
                        {
                            eventLog.WriteEntry(strLogMessage.ToString().TrimEnd(), EventLogEntryType.Warning);
                            break;
                        }
                    case EventType.Normal:                    
                        {
                            eventLog.WriteEntry(strLogMessage.ToString().TrimEnd(), EventLogEntryType.Information);
                            break;
                        }
                }
            }
        }
    }
}
