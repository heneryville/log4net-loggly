using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net.Core;
using Newtonsoft.Json.Linq;

namespace log4net.loggly
{
   public class LogglyFormatter : ILogglyFormatter
   {
      private Process _currentProcess;

      public LogglyFormatter()
      {
         _currentProcess = Process.GetCurrentProcess();
      }

      public virtual void AppendAdditionalLoggingInformation(ILogglyAppenderConfig config, LoggingEvent loggingEvent)
      {
      }

      public virtual string ToJson(LoggingEvent loggingEvent)
      {
         return PreParse(loggingEvent).ToString();
      }

      public virtual string ToJson(IEnumerable<LoggingEvent> loggingEvents)
      {
         return new JArray(loggingEvents.Select(PreParse).ToArray()).ToString();
      }

      private JObject PreParse(LoggingEvent loggingEvent)
      {
         var exceptionString = loggingEvent.GetExceptionString();
         if (string.IsNullOrWhiteSpace(exceptionString))
         {
            exceptionString = null; //ensure empty strings aren't included in the json output.
         }
         var jobject = new JObject();
         jobject.Add("level", loggingEvent.Level.DisplayName);
         jobject.Add("time", loggingEvent.TimeStamp.ToString("yyyyMMdd HHmmss.fff zzz"));
         jobject.Add("machine", Environment.MachineName);
         jobject.Add("process", _currentProcess.ProcessName);
         jobject.Add("thread", loggingEvent.ThreadName);
         jobject.Add("message", loggingEvent.MessageObject.ToString());
         jobject.Add("ex", exceptionString);
         return jobject;
      }
   }
}