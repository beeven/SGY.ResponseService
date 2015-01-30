using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ResponseService
{
    public class LogHelper
    {
        /// <summary>
        ///  将日志信息写入到系统日志中
        /// </summary>
        /// <param name="logmessage">日志信息</param>
        /// <param name="sourcename">日志源名称</param>
        public static void WriteSysLog(String logmessage, String sourcename)
        {
            EventLogEntryType errotype = EventLogEntryType.SuccessAudit;
            try
            {
                EventLog mylog = new EventLog();
                if (!EventLog.SourceExists(sourcename))
                {
                    EventLog.CreateEventSource(sourcename, sourcename);
                }

                mylog.Source = sourcename;

                mylog.MaximumKilobytes = 1024 * 20;

                mylog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 30);

                mylog.WriteEntry(logmessage, errotype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  一般的文本日志记录
        /// </summary>
        /// <param name="logmessage">日志信息</param>
        /// <param name="path">日志文件的路径</param>
        public static void WriteFileLog(String logmessage, String path)
        {
            if (!File.Exists(path))
            {
                //没有找到文件  新建文件
                FileStream fs = File.Create(path);   //注意：创建文件最好采用FileStream 而不采用FileInfo.create() 因为FileInfo 无法释放进程
                fs.Close();

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(String.Format("时间：{0} 内容：{1}", System.DateTime.Now.ToShortTimeString(), logmessage));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(String.Format("时间：{0} 内容：{1}", System.DateTime.Now.ToShortTimeString(), logmessage));
                }
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="logmessage">日志信息</param>
        public static void WriteErrorLog(String logmessage)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\log\\"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\log\\");
            }

            String efilename = AppDomain.CurrentDomain.BaseDirectory+ "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            WriteFileLog(logmessage, efilename);
        }
    }
}
