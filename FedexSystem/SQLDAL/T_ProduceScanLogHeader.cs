using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_ProduceScanLogHeader
    {
        /// <summary>
        /// 获取开箱模型
        /// </summary>
        /// <returns></returns>
        public DataSet getAllProduceScanLogHeaderInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from ProduceScanLogHeader order by ScanLogRange desc");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
    }
}
