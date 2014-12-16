using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class T_Department
    {
        public DataSet getDepartByParentId(string parentId)
        {
            StringBuilder strSql = new StringBuilder();
            if (parentId == "")//查询最高级别部门
            {
                strSql.Append("select * from Department where ParentDepId is null or ParentDepId=''");
            }
            else
            {
                strSql.AppendFormat("select * from Department where ParentDepId='{0}'",parentId);
            }
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public DataSet getMaxDepartIdByParentId(string parentId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select max(DepId) DepId from Department where ParentDepId='{0}'", parentId);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public DataSet getMaxTopDepartId()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select max(DepId) DepId from Department where ParentDepId='' or ParentDepId is null");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public Boolean TestExistDepartName(string DepartName)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select * from Department where DepName='{0}'", DepartName);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds!=null)
            {
                if (ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
                {
                    bExist = true;
                }
            }
            return bExist;
        }

        public Boolean TestExistDepartName(string DepartName,string DepartId)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select * from Department where DepName='{0}' and DepId<>'{1}'", DepartName,DepartId);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    bExist = true;
                }
            }
            return bExist;
        }

        public DataSet getDepartByDeptId(string DeptId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select * from Department where DepId='{0}'", DeptId);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 根据子部门ID获取其上级部门部门全称
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public DataSet getParentDepartFullNameBySubDeptId(string DeptId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select  DepFullName from Department where DepId in
                                    (
	                                    select ParentDepId from Department where DepId='{0}'
                                    )",DeptId);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public Boolean InsertDepartment(M_Department m_Department)
        {
            Boolean bOk = false;

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("insert into Department");
                strSql.Append(" (DepId,ParentDepId,DepOrder,DepName,DepFullName,DelFlag,mMemo)");
                strSql.Append(" values (");
                strSql.Append(" @DepId,@ParentDepId,@DepOrder,@DepName,@DepFullName,@DelFlag,@mMemo)");

                SqlParameter[] parameters = {
                    new SqlParameter("@DepId",SqlDbType.NVarChar),
                    new SqlParameter("@ParentDepId", SqlDbType.NVarChar),
                    new SqlParameter("@DepOrder", SqlDbType.Int),
                    new SqlParameter("@DepName", SqlDbType.NVarChar ),
                    new SqlParameter("@DepFullName", SqlDbType.NVarChar ),
                    new SqlParameter("@DelFlag",SqlDbType.Int),
                    new SqlParameter("@mMemo",SqlDbType.Text)
                                                    };
                parameters[0].Value = m_Department.DepId;
                parameters[1].Value = m_Department.ParentDepId;
                parameters[2].Value = m_Department.DepOrder;
                parameters[3].Value = m_Department.DepName;
                parameters[4].Value = m_Department.DepFullName;
                parameters[5].Value = m_Department.DelFlag;
                parameters[6].Value = m_Department.mMemo;


                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
                {
                    bOk = true;
                }
                else
                {
                    bOk = false;
                }
            }
            catch (Exception ex)
            {
                bOk = false;
            }

            return bOk;
        }

        /// <summary>
        /// 根据部门ID修改部门信息（特指部门名称）
        /// </summary>
        /// <param name="m_Department"></param>
        /// <returns></returns>
        public Boolean UpdateDepartment(M_Department m_Department)
        {
            Boolean bOk = false;

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("update Department set ");
                strSql.Append(" DepName=@DepName,DepFullName=@DepFullName ");
                strSql.Append(" where DepId=@DepId ");

                SqlParameter[] parameters = {
                    new SqlParameter("@DepId",SqlDbType.NVarChar),
                    new SqlParameter("@DepName", SqlDbType.NVarChar ),
                    new SqlParameter("@DepFullName", SqlDbType.NVarChar )
                                                    };
                parameters[0].Value = m_Department.DepId;
                parameters[1].Value = m_Department.DepName;
                parameters[2].Value = m_Department.DepFullName;

                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
                {
                    bOk = true;
                }
                else
                {
                    bOk = false;
                }
            }
            catch (Exception ex)
            {
                bOk = false;
            }

            return bOk;
        }

        public Boolean DeleDepartment(M_Department m_Department)
        {
            Boolean bOk = false;

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("delete Department ");
                strSql.Append(" where DepId=@DepId ");

                SqlParameter[] parameters = {
                    new SqlParameter("@DepId",SqlDbType.NVarChar)
                                                    };
                parameters[0].Value = m_Department.DepId;

                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
                {
                    bOk = true;
                }
                else
                {
                    bOk = false;
                }
            }
            catch (Exception ex)
            {
                bOk = false;
            }

            return bOk;
        }

        public Boolean IsParentDepartment( string DepartId)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select * from Department where DepId='{0}' and (ParentDepId is null or ParentDepId='')", DepartId);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    bExist = true;
                }
            }
            return bExist;
        }
    }
}
