using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace ResponseService
{
    /// <summary>
    ///  电子口岸报文服务
    /// </summary>
    public class MessageFileService
    {
        const string strxsi = "http://www.w3.org/2001/XMLSchema-instance";
        const string xsdName = "DECL_FILE.xsd";

        /// <summary>
        ///  关检关联号
        /// </summary>
        public string CusCiqNo { get; set; }

        /// <summary>
        ///  报关单
        /// </summary>
        public string EntryNo { get; set; }

        /// <summary>
        ///  状态
        /// </summary>
        public string DeclStatus { get; set; }

        /// <summary>
        ///  平台号
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="cusciqno">关检关联号</param>
        /// <param name="entryno">报文号</param>
        public MessageFileService(string cusciqno, string entryno)
        {
            this.CusCiqNo = cusciqno;
            this.EntryNo = entryno;
            this.DeclStatus = "1";
        }

        public MessageFileService(string cusciqno, string entryno, string taskid)
            : this(cusciqno, entryno)
        {
            this.TaskId = taskid;
        }

        /// <summary>
        ///  发送报文
        /// </summary>
        /// <param name="path">发送路径</param>
        public void SendMessage(string path)
        {
            XNamespace xsi = strxsi;

            XDocument document = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                                                      new XElement("Message",
                                                        new XAttribute(XNamespace.Xmlns + "xsi", strxsi),
                                                        new XAttribute(xsi + "noNamespaceSchemaLocation", xsdName)
                                               ));

            XElement root = document.Descendants("Message").LastOrDefault();
            this.CreateHead(root);
            this.CreateBody(root);

            // 保存报文
            string fileName = string.Format("{0}\\{1}.xml", path, this.CusCiqNo);
            if (File.Exists(fileName))
                File.Delete(fileName);

            document.Save(fileName);
        }

        /// <summary>
        ///  生成报文头
        /// </summary>
        /// <param name="parentNode">父节点</param>
        private void CreateHead(XElement parentNode)
        {
            parentNode.Add(new XElement("MessageHead"));
            XElement element = parentNode.Descendants("MessageHead").LastOrDefault();
            element.Add(new XElement("MessageType", "BGBJZT"));
            element.Add(new XElement("SendTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            element.Add(new XElement("Guid", Guid.NewGuid().ToString()));
        }

        /// <summary>
        ///  生成报文体
        /// </summary>
        /// <param name="parentNode">父节点</param>
        public void CreateBody(XElement parentNode)
        {
            parentNode.Add(new XElement("MessageBody"));
            XElement element = parentNode.Descendants("MessageBody").LastOrDefault();

            element.Add(new XElement("CusCiqNo", this.CusCiqNo));
            element.Add(new XElement("EntryNo", this.EntryNo));
            element.Add(new XElement("DeclStatus", this.DeclStatus));
            element.Add(new XElement("TaskId", this.TaskId));
        }
    }
}