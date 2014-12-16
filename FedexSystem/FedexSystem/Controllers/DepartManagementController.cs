using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using System.Data;
using System.Text;
using Model;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class DepartManagementController : Controller
    {
        //
        // GET: /DepartManagement/

        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetData(string state)
        {
            string strResult = "";
            StringBuilder sb = new StringBuilder("");
            T_Department t_Department = null;
            DataSet ds = null;
            DataTable dt = null;

            t_Department = new T_Department();
            ds = t_Department.getDepartByParentId("");
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"text\":\"{0}\",\"state\":\"open\",\"id\":\"{1}\",", dt.Rows[i]["DepName"].ToString(), dt.Rows[i]["DepId"].ToString());
                        CreateDepartJSON(dt.Rows[i]["DepId"].ToString(), ref sb, state);
                        if (sb.ToString().EndsWith(","))
                        {
                            sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append("},");
                        }

                    }
                    sb.Append("]");

                }
            }

            strResult = sb.ToString();
            return strResult;
        }

        protected string CreateDepartJSON(string DeptId, ref StringBuilder sb, string state)
        {
            string strResult = "";
            DataSet ds = null;
            DataTable dt = null;

            ds = new T_Department().getDepartByParentId(DeptId);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("\"children\":[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"text\":\"{0}\",\"state\":\"" + state + "\",\"id\":\"{1}\",", dt.Rows[i]["DepName"].ToString(), dt.Rows[i]["DepId"].ToString());
                        CreateDepartJSON(dt.Rows[i]["DepId"].ToString(), ref sb, state);
                        if (sb.ToString().EndsWith(","))
                        {
                            sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append("},");
                        }
                    }

                    sb.Append("]");
                }
            }
            return strResult;
        }

        /// <summary>
        /// 判断部门名称是否使用（针对新建顶级部门的判断）
        /// </summary>
        /// <param name="departName"></param>
        /// <returns></returns>
        [HttpGet]
        public string TestExistDepartName(string departName)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"判断失败，原因未知\"}";
            departName = Server.UrlDecode(departName);
            if (new T_Department().TestExistDepartName(departName))
            {
                strRet = "{\"result\":\"error\",\"message\":\"此部门名称已经使用，请重新填写\"}";
            }
            else
            {
                strRet = "{\"result\":\"ok\",\"message\":\"部门名称可以使用\"}";
            }
            return strRet;
        }

        /// <summary>
        /// 判断此部门名称是否已经被其他的部门所使用
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="departId"></param>
        /// <returns></returns>
        [HttpGet]
        public string TestAlreadyUsedDepartName(string departName, string departId)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"判断失败，原因未知\"}";
            departName = Server.UrlDecode(departName);
            departId = Server.UrlDecode(departId);
            if (new T_Department().TestExistDepartName(departName, departId))
            {
                strRet = "{\"result\":\"error\",\"message\":\"此部门名称已经使用，请重新填写\"}";
            }
            else
            {
                strRet = "{\"result\":\"ok\",\"message\":\"部门名称可以使用\"}";
            }
            return strRet;
        }

        /// <summary>
        /// 判断是否是父部门（下面有子部门存在）
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="departId"></param>
        /// <returns></returns>
        [HttpGet]
        public string TestIsSubDepart(string departId)
        {
            string strRet = "{\"result\":\"ok\",\"message\":\"删除部门成功\"}";

            DataSet dsParentDept = null;
            DataTable dtParentDept = null;

            M_Department m_Department = new M_Department();
            try
            {
                departId = Server.UrlDecode(departId);
                dsParentDept = new T_Department().getDepartByParentId(departId);
                if (dsParentDept != null)
                {
                    dtParentDept = dsParentDept.Tables[0];
                    if (dtParentDept != null && dtParentDept.Rows.Count > 0)
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"删除部门失败,此部门下面有子部门\"}";
                    }
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"删除部门失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        /// <summary>
        /// 添加子部门的action
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="parentDepartId"></param>
        /// <returns></returns>
        [HttpGet]
        public string InsertDepartment(string departName, string parentDepartId)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"添加部门信息失败，原因未知\"}";
            DataSet dsParentDept = null;
            DataTable dtParentDept = null;

            string departFullName = "";
            string departId = "";
            M_Department m_Department = new M_Department();

            try
            {
                departName = Server.UrlDecode(departName);
                parentDepartId = Server.UrlDecode(parentDepartId);

                departId = parentDepartId + "001";
                //获取上级部门的部门全称以便组合新部门的部门名称
                dsParentDept = new T_Department().getDepartByDeptId(parentDepartId);
                if (dsParentDept != null)
                {
                    dtParentDept = dsParentDept.Tables[0];
                    if (dtParentDept != null && dtParentDept.Rows.Count > 0)
                    {
                        departFullName = dtParentDept.Rows[0]["DepFullName"].ToString() + ">>" + departName;
                    }
                }

                //获取此父级部门下的所有子部门，并得到下一个子部门的部门ID
                dsParentDept = new T_Department().getMaxDepartIdByParentId(parentDepartId);
                if (dsParentDept != null)
                {
                    dtParentDept = dsParentDept.Tables[0];
                    if (dtParentDept != null && dtParentDept.Rows.Count > 0)
                    {
                        string strDepartIdTmp = dtParentDept.Rows[0]["DepId"].ToString();
                        if (strDepartIdTmp.EndsWith("999"))
                        {
                            throw new Exception("部门编号已经达到最大值，请联系开发商进行重置");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strDepartIdTmp))
                            {
                                departId = strDepartIdTmp.Substring(0, strDepartIdTmp.Length - 3) + (Convert.ToInt32(strDepartIdTmp.Substring(strDepartIdTmp.Length - 3, 3)) + 1).ToString("000");
                            }

                        }

                    }
                }

                m_Department.DelFlag = 0;
                m_Department.DepFullName = departFullName;
                m_Department.DepId = departId;
                m_Department.DepName = departName;
                m_Department.DepOrder = 1;
                m_Department.mMemo = "";
                m_Department.ParentDepId = parentDepartId;
                if (new T_Department().InsertDepartment(m_Department))
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"添加部门信息成功\"}";
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"添加部门信息失败\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"添加部门信息失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        /// <summary>
        /// 添加顶级部门
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="parentDepartId"></param>
        /// <returns></returns>
        [HttpGet]
        public string InsertTopDepartment(string departName)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"添加顶级部门信息失败，原因未知\"}";
            DataSet dsParentDept = null;
            DataTable dtParentDept = null;

            string departId = "";
            M_Department m_Department = new M_Department();

            try
            {
                departName = Server.UrlDecode(departName);

                departId = "001";
                //获取顶级部门最大的部门ID并生成新的部门ID
                dsParentDept = new T_Department().getMaxTopDepartId();
                if (dsParentDept != null)
                {
                    dtParentDept = dsParentDept.Tables[0];
                    if (dtParentDept != null && dtParentDept.Rows.Count > 0)
                    {
                        string strDepartIdTmp = dtParentDept.Rows[0]["DepId"].ToString();
                        if (strDepartIdTmp.EndsWith("999"))
                        {
                            throw new Exception("顶级部门编号已经达到最大值，请联系开发商进行重置");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strDepartIdTmp))
                            {
                                departId = (Convert.ToInt32(strDepartIdTmp) + 1).ToString("000");
                            }
                        }

                    }
                }

                m_Department.DelFlag = 0;
                m_Department.DepFullName = departName;
                m_Department.DepId = departId;
                m_Department.DepName = departName;
                m_Department.DepOrder = 1;
                m_Department.mMemo = "";
                m_Department.ParentDepId = "";
                if (new T_Department().InsertDepartment(m_Department))
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"添加顶级部门信息成功\"}";
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"添加顶级部门信息失败\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"添加顶级部门信息失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpGet]
        public string UpdateDepartment(string newDepartName, string departId)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"修改部门信息失败，原因未知\"}";
            DataSet dsParentDept = null;
            DataTable dtParentDept = null;

            string departFullName = "";
            M_Department m_Department = new M_Department();

            try
            {
                newDepartName = Server.UrlDecode(newDepartName);
                departId = Server.UrlDecode(departId);

                //根据部门ID获取其上级部门ID
                dsParentDept = new T_Department().getParentDepartFullNameBySubDeptId(departId);
                if (dsParentDept != null)
                {
                    dtParentDept = dsParentDept.Tables[0];
                    if (dtParentDept != null && dtParentDept.Rows.Count > 0)
                    {
                        departFullName = dtParentDept.Rows[0]["DepFullName"].ToString() + ">>" + newDepartName;
                    }
                }

                m_Department.DepFullName = departFullName;
                m_Department.DepId = departId;
                m_Department.DepName = newDepartName;
                if (new T_Department().UpdateDepartment(m_Department))
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"修改部门信息成功\"}";
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"修改部门信息失败\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"修改部门信息失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpGet]
        public string DeleDepartment(string departId)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"删除部门信息失败，原因未知\"}";

            departId = Server.UrlDecode(departId);
            M_Department m_Department = new M_Department();

            try
            {
                m_Department.DepId = departId;
                if (new T_Department().DeleDepartment(m_Department))
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"删除部门信息成功\"}";
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"删除部门信息失败\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"删除部门信息失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        /// <summary>
        /// 判断是否已经被员工使用了
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="departId"></param>
        /// <returns></returns>
        [HttpGet]
        public string TestHasUsed(string departId)
        {
            string strRet = "{\"result\":\"ok\",\"message\":\"未使用\"}";

            try
            {
                departId = Server.UrlDecode(departId);

                if (new T_Users().UserExistsWithDepartId(departId))
                {
                    strRet = "{\"result\":\"error\",\"message\":\"此部门已经使用，不可删除\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"判断过程中出现错误，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string GetDataWithUsers(string state)
        {
            string strResult = "";
            StringBuilder sb = new StringBuilder("");
            T_Department t_Department = null;
            DataSet ds = null;
            DataTable dt = null;

            DataSet dsUserInfo = null;
            DataTable dtUserInfo = null;

            t_Department = new T_Department();
            ds = t_Department.getDepartByParentId("");
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"text\":\"{0}\",\"state\":\"open\",\"id\":\"{1}\",\"iconCls\":\"icon-department\",", dt.Rows[i]["DepName"].ToString(), dt.Rows[i]["DepId"].ToString());
                        
                        //加载员工
                        dsUserInfo = new T_Users().getUserInfoByDeptId(dt.Rows[i]["DepId"].ToString());
                        if (dsUserInfo != null)
                        {
                            dtUserInfo = dsUserInfo.Tables[0];
                            if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                            {
                                sb.Append("\"children\":[");
                                for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                                {
                                    sb.Append("{");
                                    sb.AppendFormat("\"text\":\"{0}[<font style=\'color:red;font-weight:bold\'>用户</font>]\",\"state\":\"" + state + "\",\"id\":\"{1}\",\"iconCls\":\"icon-man\"", dtUserInfo.Rows[j]["UserName"].ToString(), "Users_" + dtUserInfo.Rows[j]["UserID"].ToString());
                                    if (j == dtUserInfo.Rows.Count - 1)
                                    {
                                        sb.Append("}");
                                    }
                                    else
                                    {
                                        sb.Append("},");
                                    }
                                    //sb.Append("},");
                                }
                                sb.Append("],");
                            }
                        }

                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref sb, state);
                        if (sb.ToString().EndsWith(","))
                        {
                            sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append("},");
                        }

                    }
                    sb.Append("]");

                }
            }

            strResult = sb.ToString();
            return strResult;
        }

        protected string CreateDepartJSONWithUsers(string DeptId, ref StringBuilder sb, string state)
        {
            string strResult = "";
            DataSet ds = null;
            DataTable dt = null;

            DataSet dsUserInfo = null;
            DataTable dtUserInfo = null;

            ds = new T_Department().getDepartByParentId(DeptId);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("\"children\":[");

                    //加载员工
                    dsUserInfo = new T_Users().getUserInfoByDeptId(DeptId);
                    if (dsUserInfo != null)
                    {
                        dtUserInfo = dsUserInfo.Tables[0];
                        if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                        {
                            //sb.Append("\"children\":[");
                            for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                            {
                                sb.Append("{");
                                sb.AppendFormat("\"text\":\"{0}[<font style=\'color:red;font-weight:bold\'>用户</font>]\",\"state\":\"" + state + "\",\"id\":\"{1}\",\"iconCls\":\"icon-man\"", dtUserInfo.Rows[j]["UserName"].ToString(), "Users_" + dtUserInfo.Rows[j]["UserID"].ToString());
                                //if (j == dtUserInfo.Rows.Count - 1)
                                //{
                                //    sb.Append("}");
                                //}
                                //else
                                //{
                                //    sb.Append("},");
                                //}
                                sb.Append("},");
                            }
                            //sb.Append("],");
                        }
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //加载子部门
                        sb.Append("{");
                        sb.AppendFormat("\"text\":\"{0}\",\"state\":\"" + state + "\",\"id\":\"{1}\",\"iconCls\":\"icon-department\",", dt.Rows[i]["DepName"].ToString(), dt.Rows[i]["DepId"].ToString());

                        //加载员工
                        dsUserInfo = new T_Users().getUserInfoByDeptId(dt.Rows[i]["DepId"].ToString());
                        if (dsUserInfo != null)
                        {
                            dtUserInfo = dsUserInfo.Tables[0];
                            if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                            {
                                sb.Append("\"children\":[");
                                for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                                {
                                    sb.Append("{");
                                    sb.AppendFormat("\"text\":\"{0}[<font style=\'color:red;font-weight:bold\'>用户</font>]\",\"state\":\"" + state + "\",\"id\":\"{1}\",\"iconCls\":\"icon-man\"", dtUserInfo.Rows[j]["UserName"].ToString(), "Users_" + dtUserInfo.Rows[j]["UserID"].ToString());
                                    if (j == dtUserInfo.Rows.Count - 1)
                                    {
                                        sb.Append("}");
                                    }
                                    else
                                    {
                                        sb.Append("},");
                                    }
                                    //sb.Append("},");
                                }
                                sb.Append("],");
                            }
                        }

                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref sb, state);
                        if (sb.ToString().EndsWith(","))
                        {
                            sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append("},");
                        }
                    }

                    sb.Append("]");
                }
            }
            return strResult;
        }

        [HttpGet]
        public string GetUserIdsByDepartmentId(string deptId)
        {
            string strResult = "";
            List<String> lstUserIds = new List<string>();
            T_Department t_Department = null;
            DataSet ds = null;
            DataTable dt = null;

            DataSet dsUserInfo = null;
            DataTable dtUserInfo = null;

            t_Department = new T_Department();
            ds = t_Department.getDepartByParentId(deptId);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //加载员工
                        dsUserInfo = new T_Users().getUserInfoByDeptId(deptId);
                        if (dsUserInfo != null)
                        {
                            dtUserInfo = dsUserInfo.Tables[0];
                            if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                            {
                                for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                                {
                                    if (!lstUserIds.Contains(dtUserInfo.Rows[j]["UserID"].ToString()))
                                    {
                                        lstUserIds.Add(dtUserInfo.Rows[j]["UserID"].ToString());
                                    }
                                    
                                }
                            }
                        }
                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref lstUserIds);
                    }
                  
                }
            }
            for (int i = 0; i < lstUserIds.Count; i++)
            {
                strResult = strResult +lstUserIds[i]+",";
            }
            if (strResult.EndsWith(","))
            {
                strResult = strResult.Substring(0,strResult.Length-1);
            }
            return strResult;
        }

        protected void CreateDepartJSONWithUsers(string DeptId, ref List<string> lstUserIds)
        {
            DataSet ds = null;
            DataTable dt = null;

            DataSet dsUserInfo = null;
            DataTable dtUserInfo = null;

            ds = new T_Department().getDepartByParentId(DeptId);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    //加载员工
                    dsUserInfo = new T_Users().getUserInfoByDeptId(DeptId);
                    if (dsUserInfo != null)
                    {
                        dtUserInfo = dsUserInfo.Tables[0];
                        if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                        {
                            for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                            {
                                if (!lstUserIds.Contains(dtUserInfo.Rows[j]["UserID"].ToString()))
                                {
                                    lstUserIds.Add(dtUserInfo.Rows[j]["UserID"].ToString());
                                }
                            }
                        }
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //加载员工
                        dsUserInfo = new T_Users().getUserInfoByDeptId(dt.Rows[i]["DepId"].ToString());
                        if (dsUserInfo != null)
                        {
                            dtUserInfo = dsUserInfo.Tables[0];
                            if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                            {
                                for (int j = 0; j < dtUserInfo.Rows.Count; j++)
                                {
                                    if (!lstUserIds.Contains(dtUserInfo.Rows[j]["UserID"].ToString()))
                                    {
                                        lstUserIds.Add(dtUserInfo.Rows[j]["UserID"].ToString());
                                    }
                                }
                            }
                        }
                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref lstUserIds);
                    }
                }
            }
        }
    }
}
