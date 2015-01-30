using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration;

namespace ResponseService
{
    /// <summary>
    ///  报文回执处理类
    /// </summary>
    public class ResponseWrapper
    {
        private static ResponseWrapper m_instance;
        private static object syncObject = new object();
        private System.Timers.Timer myTimer = null;
        private bool inWorking = false;


        /// <summary>
        /// 得到单例实体
        /// </summary>
        /// <param name="path">回执报文路径</param>
        /// <returns></returns>
        public static ResponseWrapper getInstance()
        {
            //保证是线程安全
            lock (syncObject)
            {
                if (m_instance == null)
                {
                    m_instance = new ResponseWrapper();

                }
                return m_instance;
            }
        }

        public ResponseWrapper()
        {
            if (null == System.Configuration.ConfigurationManager.AppSettings["LoopingTime"])
                throw new Exception("轮询时间不能空");

            string loopingTime = System.Configuration.ConfigurationManager.AppSettings["LoopingTime"].ToString();

            //轮询时间
            int Interval = Convert.ToInt32(loopingTime) * 1000 * 60;
            if (Interval <= 0) Interval = 60000;

            myTimer = new System.Timers.Timer();
            this.myTimer.Interval = Interval;
            this.myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
            Start();
        }

        /// <summary>
        ///  启动服务
        /// </summary>
        public void Start()
        {
            LogHelper.WriteErrorLog("服务启动");
            myTimer.Start();
        }

        /// <summary>
        ///  停止服务
        /// </summary>
        public void Stop()
        {
            myTimer.Stop();
        }

        void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (null == System.Configuration.ConfigurationManager.AppSettings["ResponsePath"])
                throw new Exception("报文回执目录不能为空");

            string path = System.Configuration.ConfigurationManager.AppSettings["ResponsePath"].ToString();

            this.ReceiveNotAccess(path);
        }

        /// <summary>
        ///  轮询报文回执文件夹
        /// </summary>
        /// <param name="path">报文回执文件夹</param>
        void ReceiveNotAccess(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("回执报文目录为空");

            if (!Directory.Exists(path))
                throw new Exception("回执报文目录不存在");

            try
            {
                // 备份文件路径
                string bakPath = string.Format(@"{0}\bak\{1}\",path,System.DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(bakPath))
                    Directory.CreateDirectory(bakPath);

                string[] list = Directory.GetFiles(path, "*.xml");
                FileInfo info;
                if (!inWorking)
                {
                    inWorking = true;
                    
                    foreach (string file in list)
                    {
                        info = new FileInfo(file);
                        LogHelper.WriteErrorLog("解析回执："+ info.Name);
                        this.ParseXml(file);

                        // 将报文转移到备份文件夹中
                        info.CopyTo(bakPath + info.Name,true);
                        info.Delete();
                    }

                    inWorking = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  解析报文
        /// </summary>
        /// <param name="path"></param>
        public void ParseXml(string path)
        {
            try
            {
                XmlDocument document = XmlHelper.Getdocument(path);
                if (null != document)
                {
                    XmlNode node = XmlHelper.GetXmlNode(document, "TCS101Message/MessageBody");
                    if (null != node)
                    {
                        XmlNamespaceManager xnm = new XmlNamespaceManager(document.NameTable);
                        xnm.AddNamespace("e", "http://www.chinaport.gov.cn/tcs/v2");

                        //获取body的第一个节点
                        XmlNode firstnode = node.FirstChild;
                        if (firstnode.NodeType.Equals(XmlNodeType.Element)
                                                        && null != firstnode)
                        {
                            string taskid = string.Empty;
                            string type = string.Empty;
                            string resultcode = string.Empty;
                            string notes = string.Empty;
                            String name = firstnode.Name.ToLower();
                            String entryNo = string.Empty;
                            String message = document.DocumentElement.OuterXml;
                            String eportNo = String.Empty;
                            switch (name)
                            {
                                //TCS回执
                                case "tcsflow201response":
                                    type = "TCS";
                                    taskid = GetInnerText(firstnode, "e:ResponseHead/e:TaskId", xnm);
                                    resultcode = GetInnerText(firstnode, "e:ResponseList/e:ActionResult/e:ResultCode", xnm);
                                    notes = GetInnerText(firstnode, "e:ResponseList/e:ActionResult/e:ResultValue", xnm);
                                    break;
                                //QP回执
                                case "tcsflow201":
                                    type = "QP";
                                    taskid = GetInnerText(firstnode, "e:TcsFlow/e:TaskId", xnm);
                                    resultcode = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:Channel", xnm);
                                    notes = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:Note", xnm);
                                    if (string.IsNullOrEmpty(notes))
                                        notes = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:ResultInformation", xnm);
                                    entryNo = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:EntryNo", xnm);
                                    eportNo = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:EportNo", xnm);
                                    break;
                            }
                            try
                            {
                                // 获取接收ID
                                string receiverid = GetInnerText(document.DocumentElement, "MessageHead/ReceiverId");

                                if (!string.IsNullOrEmpty(taskid) && !string.IsNullOrEmpty(receiverid))
                                {
                                    // 根据接收ID 获取数据库配置节点
                                    string dbconfig = System.Configuration.ConfigurationManager.AppSettings[receiverid];

                                    // 保存回执信息
                                    DataAccess access = null != dbconfig ? new DataAccess(dbconfig) : new DataAccess();
                                    access.SaveMessage(taskid, type, resultcode, notes, path, entryNo, message, eportNo);

                                    // H2000 回执
                                    if (type == "QP" && resultcode == "011")
                                    {
                                        string entryno = GetInnerText(firstnode, "e:TcsData/e:ENTRY_RESULT/e:EntryNo", xnm);
                                        string cus_ciq_no = access.GetCustomsCIQNo(taskid);

                                        // 生成报文  发送
                                        MessageFileService service = new MessageFileService(cus_ciq_no, entryno,taskid);
                                        service.SendMessage(ConfigurationManager.AppSettings["EportPath"].ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(String.Format("Stack trace:{0} Message:{1}",ex.StackTrace,ex.Message));
            }
        }

        /// <summary>
        /// 获取innerText 值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        private string GetInnerText(XmlNode node, string xpath)
        {
            XmlNode currentnode = XmlHelper.GetXmlNode(node, xpath);
            if (null != currentnode)
                return currentnode.InnerText.Trim();
            else
                return string.Empty;
        }

        /// <summary>
        ///  获取InnerText
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        private string GetInnerText(XmlNode node, string xpath, XmlNamespaceManager namespaces)
        {
            XmlNode currentnode = XmlHelper.GetXmlNode(node, xpath, namespaces);
            if (null != currentnode)
                return currentnode.InnerText.Trim();
            else
                return string.Empty;
        }
    }
}
