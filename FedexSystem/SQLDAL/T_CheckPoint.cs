using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{
 public   class T_CheckPoint
    {
        //初始化权限

        public bool insertRightValue(long rightValue, int userID)
        {


            StringBuilder strSql = new StringBuilder();

            strSql.Append("insert into [dbo].[CheckPoint]");
            strSql.Append(" (CPMemo,RightValue,CurUserID,DelFlag)");
            strSql.Append(" values (");
            strSql.Append("@CPMemo,@RightValue,@CurUserID,@DelFlag)");

            SqlParameter[] parameters = {
                    new SqlParameter("@CPMemo",SqlDbType.NVarChar),
                    new SqlParameter("@RightValue",SqlDbType.BigInt),
                    new SqlParameter("@CurUserID",SqlDbType.Int),
                    new SqlParameter("@DelFlag",SqlDbType.Int)
                  
                                                    };

            parameters[0].Value = "";
            parameters[1].Value = rightValue;
            parameters[2].Value = userID;
            parameters[3].Value = 0;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }

        }


        //获取模型
        public DataSet GetCheckPoint()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [dbo].[CheckPoint]");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (ds.Tables[0].Rows.Count != 0)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }

        //获取模型
        public DataSet GetRightInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [dbo].[RightInfo]");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (ds.Tables[0].Rows.Count != 0)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }



       
    }

}
