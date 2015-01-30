using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace ResponseService
{
    /// <summary>
    ///  ��װ������
    /// </summary>
    public class InstallerSettings
    {
        /// <summary>
        ///  ������ʾ����
        /// </summary>
        private string _DisplayName;
        /// <summary>
        ///  ��������
        /// </summary>
        private string _Description;
        /// <summary>
        ///  ��������
        /// </summary>
        private string _ServiceName;
        /// <summary>
        ///  ������ʾ����
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
        }
        /// <summary>
        ///  ��������
        /// </summary>
        public string Description
        {
            get
            {
                return _Description;
            }
        }
        /// <summary>
        ///  ������ʾ����
        /// </summary>
        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
        }

        /// <summary>
        ///  ��װʵ������
        /// </summary>
        /// <param name="settingsFilePath"></param>
        public InstallerSettings(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
                throw new FileNotFoundException(string.Format("Can't find installer settings file({0}).", settingsFilePath));

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(settingsFilePath);
                XmlNode root = xmlDoc.DocumentElement;
                if (root != null)
                {
                    string value;
                    foreach (XmlNode subXmlNode in root.ChildNodes)
                    {
                        if (XmlNodeType.Element == subXmlNode.NodeType)
                        {
                            value = subXmlNode.InnerText.Trim();
                            switch (subXmlNode.Name.ToLower())
                            {
                                case "servicename":
                                    this._ServiceName = value;
                                    break;
                                case "displayname":
                                    this._DisplayName = value;
                                    break;
                                case "description":
                                    this._Description = value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (string.IsNullOrEmpty(this.ServiceName))
            {
                throw new Exception(string.Format("ServiceName is not allow be null or empty.(Settings File: {0})" + settingsFilePath));
            }
        }
    }
}
