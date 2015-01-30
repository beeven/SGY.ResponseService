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
        ///  ����־��Ϣд�뵽ϵͳ��־��
        /// </summary>
        /// <param name="logmessage">��־��Ϣ</param>
        /// <param name="sourcename">��־Դ����</param>
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
        ///  һ����ı���־��¼
        /// </summary>
        /// <param name="logmessage">��־��Ϣ</param>
        /// <param name="path">��־�ļ���·��</param>
        public static void WriteFileLog(String logmessage, String path)
        {
            if (!File.Exists(path))
            {
                //û���ҵ��ļ�  �½��ļ�
                FileStream fs = File.Create(path);   //ע�⣺�����ļ���ò���FileStream ��������FileInfo.create() ��ΪFileInfo �޷��ͷŽ���
                fs.Close();

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(String.Format("ʱ�䣺{0} ���ݣ�{1}", System.DateTime.Now.ToShortTimeString(), logmessage));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(String.Format("ʱ�䣺{0} ���ݣ�{1}", System.DateTime.Now.ToShortTimeString(), logmessage));
                }
            }
        }

        /// <summary>
        /// ��¼������־
        /// </summary>
        /// <param name="logmessage">��־��Ϣ</param>
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
