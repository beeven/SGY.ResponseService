using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace ResponseService
{
    /// <summary>
    /// XML操作帮助类 目前只支持两级操作
    /// </summary>
    public static class XmlHelper
    {
        public static XmlDocument Getdocument(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("没有对应的xml文件，请提供正确的xml文件路径！");

            if (System.IO.Path.GetExtension(path) != ".xml")
                throw new Exception("读取的不是xml文件，请提供正确的xml文件！");

            try
            {
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(path);
                return xmldoc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取xml文件中的单个节点
        /// </summary>
        /// <param name="path">xml文件的路径</param>
        /// <param name="xpath">单个节点的名称，此名称符合xpath规则</param>
        /// <returns>node==null 表示读取xml节点失败，否则返回符合条件的单个节点</returns>
        public static XmlNode GetXmlNode(XmlDocument xmldoc, string xpath)
        {
            if (null == xmldoc) throw new Exception("读取xml文件失败，请检查xml文件的路径或格式！");

            if (string.IsNullOrEmpty(xpath)) throw new Exception("没有提供有效的xpath，请提供有效的xpath!");

            XmlNode node = xmldoc.SelectSingleNode(xpath);            

            return node;
        }
        
        public static XmlNode GetXmlNode(XmlDocument xmldoc, string xpath,XmlNamespaceManager manager)
        {
            if (null == xmldoc) throw new Exception("读取xml文件失败，请检查xml文件的路径或格式！");

            if (string.IsNullOrEmpty(xpath)) throw new Exception("没有提供有效的xpath，请提供有效的xpath!");

            XmlNode node = xmldoc.SelectSingleNode(xpath,manager);            
            
            return node;
        }
        
        public static XmlNode GetXmlNode(XmlNode xmldoc, string xpath,XmlNamespaceManager manager)
        {
            if (null == xmldoc) throw new Exception("读取xml文件失败，请检查xml文件的路径或格式！");

            if (string.IsNullOrEmpty(xpath)) throw new Exception("没有提供有效的xpath，请提供有效的xpath!");

            XmlNode node = xmldoc.SelectSingleNode(xpath,manager);            
            
            return node;
        }
        
        
        public static XmlNodeList GetXmlNodes(XmlDocument xmldoc, string xpath)
        {
            if (null == xmldoc) throw new Exception("读取xml文件失败，请检查xml文件的路径或格式！");

            if (string.IsNullOrEmpty(xpath)) throw new Exception("没有提供有效的xpath，请提供有效的xpath!");

            XmlNodeList node = xmldoc.SelectNodes(xpath);            

            return node;
        }

        /// <summary>
        /// 获取xml文件中父节点下的单个子节点
        /// </summary>
        /// <param name="xmlnode">父节点</param>
        /// <param name="xpath">单个节点的名称，此名称符合xpath规则</param>
        /// <returns>node==null 表示读取xml节点失败，否则返回符合条件的单个节点</returns>
        public static XmlNode GetXmlNode(XmlNode xmlnode, string xpath)
        {
            XmlNode node = null;

            if (string.IsNullOrEmpty(xpath))
            {
                throw new Exception("没有提供有效的xpath，请提供有效的xpath!");
            }

            node = xmlnode.SelectSingleNode(xpath);

            return node;
        }

        #region GetPropertyValue 
        /// <summary>
        /// 获取xml节点的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="proName"></param>
        /// <returns></returns>
		public static string GetPropertyValue(XmlNode obj, string proName)
		{
            try
            {
                if (null == obj)
                {
                    return string.Empty;
                }

                if (obj.Attributes.Count > 0 && obj.Attributes[proName] != null)
                {
                    return obj.Attributes[proName].Value;
                }

                foreach (XmlNode propNode in obj.ChildNodes)
                {
                    if (propNode.Name == proName)
                    {
                        return propNode.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
			return string.Empty;
		}
		#endregion 

        #region GetInnerText
        /// <summary>
        /// 得到节点的InnerText
        /// </summary>
        /// <param name="nodel"></param>
        /// <returns></returns>
        public static string GetInnerText(XmlNode nodel)
        {
            if (null != nodel)
            {
                return nodel.InnerText;
            }
            else {
                return string.Empty;
            }
        }
        #endregion

        #region SetPropertyValue
        public static void SetPropertyValue(XmlNode objNode, string proName ,string val )
		{	
			XmlHelper.SetPropertyValue(objNode ,proName ,val ,XmlPropertyPosition.ChildNode) ;
		}

        public static void SetPropertyValue(XmlNode objNode, string proName, string val, XmlPropertyPosition pos)
        {
            XmlHelper.SetPropertyValue(objNode, proName, val, pos, true);
        }

        #region SetPropertyValue
        public static void SetPropertyValue(XmlNode objNode, string proName, string val, XmlPropertyPosition pos, bool overrideExist)
        {
            if (pos == XmlPropertyPosition.ChildNode)
            {
                if (overrideExist)
                {
                    foreach (XmlNode childNode in objNode.ChildNodes)
                    {
                        if (childNode.Name == proName)
                        {
                            childNode.InnerText = val;
                            return;
                        }
                    }
                }

                XmlNode newChildNode = objNode.OwnerDocument.CreateElement(proName);
                newChildNode.InnerText = val;
                objNode.AppendChild(newChildNode);
            }
            else
            {
                if (overrideExist)
                {
                    foreach (XmlAttribute attr in objNode.Attributes)
                    {
                        if (attr.Name == proName)
                        {
                            attr.Value = val;
                            return;
                        }
                    }
                }

                XmlAttribute newAttr = objNode.OwnerDocument.CreateAttribute(proName);
                objNode.Attributes.Append(newAttr);
                newAttr.Value = val;
            }
        } 
        #endregion
		#endregion

        #region FillObjectNode
        /// <summary>
        /// FillObjectNode 使用obj各个属性的名字和值为objNode添加子节点
        /// </summary>        
        public static void FillObjectNode(XmlNode objNode, object obj)
        {
            Type t = obj.GetType();

            foreach (PropertyInfo info in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default))
            {
                string proValue = "";
                object val = info.GetValue(obj, null);
                if (val != null)
                {
                    proValue = val.ToString();
                }

                XmlHelper.SetPropertyValue(objNode, info.Name, proValue, XmlPropertyPosition.ChildNode);
            }
        } 
        #endregion

		#region GetChildNode
        /// <summary>
        /// GetChildNode 获取满足指定名称的第一个Child Node
        /// </summary> 
		public static XmlNode GetChildNode(XmlNode obj, string childNodeName)
		{
			foreach (XmlNode propNode in obj.ChildNodes)
			{
				if (propNode.Name.Equals(childNodeName,StringComparison.CurrentCultureIgnoreCase))
				{
					return propNode;
				}
			}

			return null;
		}
		#endregion

		#region GetChildNodes
        /// <summary>
        /// GetChildNodes 获取指定名称的Child Node的列表
        /// </summary>       
		public static IList<XmlNode> GetChildNodes(XmlNode obj, string childNodeName) //List中为XmlNode
		{
			IList<XmlNode> list = new List<XmlNode>() ;
			foreach (XmlNode propNode in obj.ChildNodes)
			{
				if (propNode.Name == childNodeName)
				{
					list.Add(propNode);
				}
			}
            
			return list;
		}
		#endregion		

        #region ParseXmlNodeString ,GetXmlNodeString
        /// <summary>
        /// ParseXmlNodeString 将OutXml字符串解析为XmlNode
        /// </summary>       
        public static XmlNode ParseXmlNodeString(string outXml)
        {
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(outXml);
            return xmlDoc2.ChildNodes[0];
        }

        /// <summary>
        /// GetXmlNodeString 获取Node的OuterXml字符串
        /// </summary>       
        public static string GetXmlNodeString(XmlNode node)
        {
            return node.OuterXml;
        } 
        #endregion       
	}

	public enum XmlPropertyPosition
	{
		Attribute ,ChildNode
	}
}
