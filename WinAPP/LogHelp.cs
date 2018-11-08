using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinAPP
{
    class LogHelp
    {
        public static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("Log");
        /// <summary>
        /// 记录正常的消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        public static void Info(string msg)
        {
            _logger.Info(msg);
        }
        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="msg">异常信息内容</param>
        public static void Error(string msg)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            _logger.Error("类名:" + methodBase.ReflectedType.Name + " 方法名:" + methodBase.Name + " 信息:" + msg);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
