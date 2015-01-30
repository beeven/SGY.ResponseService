using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;

namespace ResponseService
{
    /// <summary>
    ///  数据操作类
    /// </summary>
    public class DataAccess
    {
        private Database Db;
        public DataAccess()
        {
            Db = EnterpriseLibraryContainer.Current.GetInstance<Database>("DefaultSqlDb");
        }

        public DataAccess(string dbconfig)
        {
            Db = EnterpriseLibraryContainer.Current.GetInstance<Database>(dbconfig);
        }

        /// <summary>
        ///  保存报文回执到数据库中
        /// </summary>
        /// <param name="taskid">报文平台号</param>
        /// <param name="m_type">回执类型</param>
        /// <param name="code">回执代码</param>
        /// <param name="notes">回执说明</param>
        /// <param name="filename">回执问津</param>
        public void SaveMessage(string taskid, string m_type, string code, string notes, string filename, string entryNo, string message, string eportNo)
        {
            try
            {
                string sql = @"INSERT INTO [DECL_RETURN](TASK_ID,RETURN_TYPE,RETURN_CODE,RETURN_INFO,FILE_NAME,ENTRY_NO,MESSAGE, EPORT_NO)
                                            VALUES(@TASK_ID,@RETURN_TYPE,@RETURN_CODE,@RETURN_INFO,@FILE_NAME,@ENTRY_NO,@MESSAGE, @EPORT_NO)";

                var cmd = Db.GetSqlStringCommand(sql);
                Db.AddInParameter(cmd, "TASK_ID", DbType.String, taskid);
                Db.AddInParameter(cmd, "RETURN_TYPE", DbType.String, m_type);
                Db.AddInParameter(cmd, "RETURN_CODE", DbType.String, code);
                Db.AddInParameter(cmd, "RETURN_INFO", DbType.String, notes);
                Db.AddInParameter(cmd, "FILE_NAME", DbType.String, filename);
                Db.AddInParameter(cmd, "ENTRY_NO", DbType.String, entryNo);
                Db.AddInParameter(cmd, "MESSAGE", DbType.Xml, message);
                Db.AddInParameter(cmd, "EPORT_NO", DbType.String, eportNo);
                Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  通过taskid 获取关检关联号
        /// </summary>
        /// <param name="taskid">报文的taskid</param>
        /// <returns>返回关检关联号</returns>
        public string GetCustomsCIQNo(string taskid)
        {
            try
            {
                string sql = @"SELECT CUS_CIQ_NO FROM DECL_DATA WHERE TCS_TASKID=@TCS_TASKID";
                var cmd = Db.GetSqlStringCommand(sql);
                Db.AddInParameter(cmd, "TCS_TASKID", DbType.String, taskid);

                object obj = Db.ExecuteScalar(cmd);
                return null == obj ? string.Empty : obj.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
