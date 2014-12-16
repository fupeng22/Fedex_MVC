using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{
    public class T_Users
    {


        /// <summary>
        /// 根据用户账号判断该用户账号是否已被使用
        /// </summary>
        /// <param name="UserID">用户编号</param>
        /// <returns>存在返回true反之返回false</returns>

        public bool UserExists(string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo");
            strSql.Append(" where UserNumber='" + userNumber + "'");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 用户登录检测(false时不区分大小写)
        /// </summary>
        /// <param name="UserID">用户标识</param>
        /// <param name="pwd">密码</param>
        /// <returns>通过返回true反之返回false</returns>
        /// 
        public bool CheckLogin(string UserNumber, string pwd)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo");
            strSql.Append(" where UserNumber= '" + UserNumber + "' and UserPSW='" + pwd + "'");
            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获得所有用户对象实体
        /// </summary>
        public DataSet GetUserModel()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  * from UserInfo ");
            Model.M_Users model = new Model.M_Users();
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
        /// <summary>
        /// 创建用户实体
        /// </summary>
        /// <param name="model">用户模型</param>
        /// <returns>-1:用户名已经存在 0:创建失败 1:创建成功</returns>
        public bool addUsers(Model.M_Users model)
        {

            if (!UserExists(model.UserNumber))
            {
                StringBuilder strSql = new StringBuilder();

                strSql.Append("insert into UserInfo");
                strSql.Append(" (UserNumber,UserName,UserPSW,Sex,RightClass,Department,UserLocked,DelFlag)");
                strSql.Append(" values (");
                strSql.Append(" @UserNumber,@UserName,@UserPSW,@Sex,@RightClass,@Department,@UserLocked,@DelFlag)");

                SqlParameter[] parameters = {
                    new SqlParameter("@UserNumber",SqlDbType.NVarChar),
                    new SqlParameter("@UserName", SqlDbType.NVarChar),
                    new SqlParameter("@UserPSW", SqlDbType.NVarChar),
                    new SqlParameter("@Sex", SqlDbType.Int),
                    new SqlParameter("@RightClass", SqlDbType.BigInt),
                    new SqlParameter("@Department", SqlDbType.NVarChar),
                    new SqlParameter("@UserLocked",SqlDbType.Int),
                    new SqlParameter("@DelFlag",SqlDbType.Int)
                                                    };
                parameters[0].Value = model.UserNumber;
                parameters[1].Value = model.UserName;
                parameters[2].Value = model.UserPSW;
                parameters[3].Value = model.Sex;
                parameters[4].Value = model.RightClass;
                parameters[5].Value = model.Department;
                parameters[6].Value = model.UserLocked;
                parameters[7].Value = model.DelFlag;


                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
                {
                    return true;
                }
                else
                {
                    return false;

                }

            }

            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据User_id删除用户实体
        /// </summary>
        /// <param name="model">用户模型</param>
        /// <returns>true or false</returns>
        /// 
        public bool deleteUsers(int userID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from UserInfo");
            strSql.Append(" where UserID=" + userID + "");

            SqlParameter[] parameters = {
                    new SqlParameter("@DelFlag", SqlDbType.Int)};
            parameters[0].Value = 1;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 更新用户实体
        /// </summary>
        /// <param name="model">用户模型</param>
        /// <returns>true or false</returns>
        /// 

        public bool updateUsers(Model.M_Users mUsers)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update UserInfo ");
            //strSql.Append("set UserNumber=@UserNumber,UserName=@UserName,UserPSW=@UserPSW,Sex=@Sex,RightClass=@RightClass,Department=@Department,UserLocked=@UserLocked,DelFlag=@DelFlag");
            strSql.Append("set UserNumber=@UserNumber,UserName=@UserName,UserPSW=@UserPSW,Sex=@Sex,Department=@Department,UserLocked=@UserLocked,DelFlag=@DelFlag");
            strSql.Append(" where UserID='" + mUsers.UserID + "'");

            SqlParameter[] parameters = {
                    new SqlParameter("@UserNumber", SqlDbType.NVarChar),
                    new SqlParameter("@UserName", SqlDbType.NVarChar),
                    new SqlParameter("@UserPSW", SqlDbType.NVarChar),
                    new SqlParameter("@Sex",SqlDbType.Int),
                    //new SqlParameter("@RightClass", SqlDbType.BigInt),
                    new SqlParameter("@Department", SqlDbType.NVarChar),
                    new SqlParameter("@UserLocked",SqlDbType.Int),
                    new SqlParameter("@DelFlag",SqlDbType.Int)
                                      };

            parameters[0].Value = mUsers.UserNumber;
            parameters[1].Value = mUsers.UserName;
            parameters[2].Value = mUsers.UserPSW;
            parameters[3].Value = mUsers.Sex;
            //parameters[4].Value = mUsers.RightClass;
            parameters[4].Value = mUsers.Department;
            parameters[5].Value = mUsers.UserLocked;
            parameters[6].Value = mUsers.DelFlag;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataSet getNameRightClass(string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT dbo.UserInfo.UserName, dbo.UserInfo.RightClass");
            strSql.Append(" FROM dbo.[UserInfo]");
            strSql.Append(" WHERE (dbo.UserInfo.UserNumber = '" + userNumber + "')");
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
        public string getUserName(string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select UserName from UserInfo ");
            strSql.Append(" where UserNumber='" + userNumber + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds.Tables[0].Rows[0][0].ToString();
        }

        //获取全部用户
        public DataSet getUsers()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo ");
            strSql.Append(" where DelFlag=0");

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


        //获取40个岗位权限
        public DataSet getAllRightValue()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT CPMemo,RightValue FROM [CheckPoint]");

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


        //获取最大userID
        public int getMaxUserID()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select max(UserID) from UserInfo ");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds != null)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public bool insert(long rightValue, int j)
        {

            StringBuilder strSql = new StringBuilder();

            strSql.Append("insert into UserInfo");
            strSql.Append(" (UserNumber,UserName,UserPSW,Sex,RightClass)");
            strSql.Append(" values (");
            strSql.Append("@UserNumber,@UserName,@UserPSW,@Sex,@RightClass)");

            SqlParameter[] parameters = {
                    new SqlParameter("@UserNumber",SqlDbType.NVarChar),
                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                    new SqlParameter("@UserPSW",SqlDbType.NVarChar),
                    new SqlParameter("@Sex",SqlDbType.Int),
                    new SqlParameter("@RightClass",SqlDbType.BigInt),
                  
                                                    };

            parameters[0].Value = "X" + j;
            parameters[1].Value = "X" + j;
            parameters[2].Value = "X" + j;
            parameters[3].Value = 0;
            parameters[4].Value = rightValue;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }

        }

        public DataSet getUserInfoByUserId(string userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo ");
            strSql.Append(" where UserID=" + userId);

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

        public DataSet getUserInfoByUserNumber(string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo ");
            strSql.Append(" where UserNumber='" + userNumber + "'");

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

        /// <summary>
        /// 根据用户账号判断该用户账号是否已被使用
        /// </summary>
        /// <param name="UserID">用户编号</param>
        /// <returns>存在返回true反之返回false</returns>
        public bool UserExists(string userId, string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo");
            strSql.Append(" where UserID<>" + userId + " and UserNumber='" + userNumber + "'");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据User_id删除用户实体
        /// </summary>
        /// <param name="model">用户模型</param>
        /// <returns>true or false</returns>
        /// 
        public bool deleteUsers(string userIDs)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from UserInfo");
            strSql.Append(" where UserID in (" + userIDs + ")");

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 根据用户账号判断该用户账号是否已被使用
        /// </summary>
        /// <param name="UserID">用户编号</param>
        /// <returns>存在返回true反之返回false</returns>
        public bool UserExistsWithDepartId(string departId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo");
            strSql.Append(" where Department='" + departId + "'");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataSet getUserInfoByDeptId(string deptId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from UserInfo ");
            strSql.Append(" where Department='" + deptId + "'");

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

        /// <summary>
        /// 根据指定用户名判断是否为组员，而不是组队长
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        public bool IsEmployee(string userNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from Department where ParentDepId
                            in
                            (
	                            select Department from UserInfo where UserNumber='{0}'
                            )", userNumber);

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
