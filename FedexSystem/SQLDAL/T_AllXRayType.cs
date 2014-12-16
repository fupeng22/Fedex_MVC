using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_AllXRayType
    {
        public DataSet getAllXRayTypeInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from AllXRayType  order by cId");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public DataSet getXRayTypeInfoByXRayType(string XRayType)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from AllXRayType where XRayType='"+XRayType+"' order by cId");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
    }
}
