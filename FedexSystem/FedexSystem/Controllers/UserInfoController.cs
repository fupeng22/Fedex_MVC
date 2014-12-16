using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using Microsoft.Reporting.WebForms;
using System.IO;
using SQLDAL;
using Model;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class UserInfoController : Controller
    {
        //SQLDAL.T_User tUser = new SQLDAL.T_User();
        //Model.M_User mUser = new Model.M_User();

        public const string strFileds = "DepFullName,UserNumber,UserName,UserPSW,Sex,SexDescription,RightClass,UserLocked,DelFlag,UserID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/UserInfo.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string GetData(string order, string page, string rows, string sort)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users_Department";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "UserID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", Convert.ToInt32(param[7].Value.ToString()));
            sb.Append(",\"rows\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "SexDescription":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["Sex"].ToString() == "0" ? "男" : "女");
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["Sex"].ToString() == "0" ? "男" : "女");
                            }
                            break;
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                    }
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
            dt = null;
            if (sb.ToString().EndsWith(","))
            {
                sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }


        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users_Department";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "UserID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];
            // ",,,,,,,,";
            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("DepFullName", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserPSW", Type.GetType("System.String"));
            dtCustom.Columns.Add("Sex", Type.GetType("System.String"));
            dtCustom.Columns.Add("SexDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("RightClass", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserLocked", Type.GetType("System.String"));
            dtCustom.Columns.Add("DelFlag", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "SexDescription":
                            drCustom[strFiledArray[j]] = dt.Rows[i]["Sex"].ToString() == "0" ? "男" : "女";
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["UserID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("UserInfo_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>8in</PageWidth>" +
                " <PageHeigth>11in</PageHeigth>" +
                " <MarginTop>0.5in</MarginTop>" +
                " <MarginLeft>1in</MarginLeft>" +
                " <MarginRight>1in</MarginRight>" +
                " <MarginBottom>0.5in</MarginBottom>" +
                " </DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType);
        }


        [HttpGet]
        public ActionResult Excel(string order, string page, string rows, string sort, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users_Department";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "UserID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];
            // ",,,,,,,,";
            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("DepFullName", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserPSW", Type.GetType("System.String"));
            dtCustom.Columns.Add("Sex", Type.GetType("System.String"));
            dtCustom.Columns.Add("SexDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("RightClass", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserLocked", Type.GetType("System.String"));
            dtCustom.Columns.Add("DelFlag", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "SexDescription":
                            drCustom[strFiledArray[j]] = dt.Rows[i]["Sex"].ToString() == "0" ? "男" : "女";
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["UserID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("UserInfo_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = localReport.Render(
               "Excel", null, out mimeType, out encoding, out extension,
               out streamids, out warnings);
            string strFileName = Server.MapPath(STR_TEMPLATE_EXCEL);
            FileStream fs = new FileStream(strFileName, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            string strOutputFileName = "用户信息维护_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            switch (browserType.ToLower())
            {
                case "safari":
                    break;
                case "mozilla":
                    break;
                default:
                    strOutputFileName = HttpUtility.UrlEncode(strOutputFileName);
                    break;
            }

            return File(strFileName, "application/vnd.ms-excel", strOutputFileName);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string txtUserID = collection["txtUserID"].ToString();
                string txtUserPassword = collection["txtUserPassword"].ToString();
                string txtReUserPassword = collection["txtReUserPassword"].ToString();
                string txtUserName = collection["txtUserName"].ToString();
                string ddlSex = collection["ddlSex"].ToString();
                string ddlAuthority = collection["ddlAuthority"].ToString();
                string ddlDepart = collection["ddlDepart"].ToString();

                Int64 iRightClass = 0;
                string[] RightClassArr = ddlAuthority.Split(',');
                for (int i = 0; i < RightClassArr.Length; i++)
                {
                    if (RightClassArr[i] != "")
                    {
                        iRightClass = iRightClass + Convert.ToInt64(RightClassArr[i]);
                    }
                }

                M_Users m_Users = new M_Users();

                m_Users.UserNumber = txtUserID;
                m_Users.UserName = txtUserName;
                m_Users.UserPSW = Get_MD5(txtUserPassword);
                m_Users.Sex = Convert.ToInt32(ddlSex);
                m_Users.RightClass = iRightClass;
                m_Users.Department = ddlDepart;
                m_Users.UserLocked = 0;
                m_Users.DelFlag = 0;
                new T_Users().addUsers(m_Users);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Edit(int id)
        {
            M_Users m_Users = new M_Users();
            DataSet ds = new T_Users().getUserInfoByUserId(id.ToString());
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    Int64 iRightClass = 0;
                    string[] RightClassArr = dt.Rows[0]["RightClass"].ToString().Split(',');
                    for (int i = 0; i < RightClassArr.Length; i++)
                    {
                        if (RightClassArr[i] != "")
                        {
                            iRightClass = iRightClass + Convert.ToInt64(RightClassArr[i]);
                        }
                    }
                    m_Users.UserID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    m_Users.UserNumber = dt.Rows[0]["UserNumber"].ToString();
                    m_Users.UserName = dt.Rows[0]["UserName"].ToString();
                    m_Users.Sex = Convert.ToInt32(dt.Rows[0]["Sex"]);
                    m_Users.RightClass = iRightClass;
                    m_Users.UserPSW = dt.Rows[0]["UserPSW"].ToString();
                    m_Users.Department = dt.Rows[0]["Department"].ToString();
                    m_Users.UserLocked = Convert.ToInt32(dt.Rows[0]["UserLocked"].ToString());
                    m_Users.DelFlag = Convert.ToInt32(dt.Rows[0]["DelFlag"].ToString());
                }
            }

            return View(m_Users);
        }


        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                string txtId = collection["txtId"].ToString();
                string txtUserID = collection["txtUserID_Edit"].ToString();
                string txtUserPassword = collection["txtUserPassword_Edit"].ToString();
                string txtReUserPassword = collection["txtReUserPassword_Edit"].ToString();
                string txtUserName = collection["txtUserName_Edit"].ToString();
                string ddlSex = collection["ddlSex_Edit"].ToString();
                string ddlAuthority = "1"; //collection["ddlAuthority_Edit"].ToString();
                string ddlDepart = collection["ddlDepart_Edit"].ToString();

                Int64 iRightClass = 0;
                string[] RightClassArr = ddlAuthority.Split(',');
                for (int i = 0; i < RightClassArr.Length; i++)
                {
                    if (RightClassArr[i] != "")
                    {
                        iRightClass = iRightClass + Convert.ToInt64(RightClassArr[i]);
                    }
                }

                M_Users m_Users = new M_Users();

                m_Users.UserNumber = txtUserID;
                m_Users.UserName = txtUserName;
                m_Users.UserPSW = Get_MD5(txtUserPassword);
                m_Users.Sex = Convert.ToInt32(ddlSex);
                m_Users.RightClass = iRightClass;
                m_Users.Department = ddlDepart;
                m_Users.UserLocked = 0;
                m_Users.DelFlag = 0;
                m_Users.UserID = Convert.ToInt32(txtId);
                new T_Users().updateUsers(m_Users);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult Delete(string ids)
        {
            try
            {
                //string[] idsArr = ids.Split(',');
                //for (int i = 0; i < idsArr.Length; i++)
                //{
                //    if (idsArr[i].Trim() != "")
                //    {
                //        tUser.deleteUser(Convert.ToInt32(idsArr[i]));
                //    }
                //}
                new T_Users().deleteUsers(ids);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public string ExistUserName(string strUsername)
        {
            string strRet = "false";
            strUsername = Server.UrlDecode(strUsername);
            strRet = new T_Users().UserExists(strUsername).ToString();
            return strRet;
        }

        [HttpGet]
        public string ExistUserName_Update(int id, string strUsername)
        {
            string strRet = "false";
            strUsername = Server.UrlDecode(strUsername);
            strRet = new T_Users().UserExists(id.ToString(), strUsername).ToString();
            return strRet;
        }

//        [HttpGet]
//        public string BakUpdate()
//        {
//            string strRet = "false";
//            StringBuilder strSql = new StringBuilder();
//            strSql.Append(@"select  * from UserInfo  where UserNumber IN ( 'pd0050', 'pd0039', 'pd0038', 'pd0066', 'pd4295',
//                        'pd4115', 'pd4056', 'pd4054', 'pd4009', 'pd4182',
//                        'pd4117', 'pd4118', 'pd4119', 'pd4120', 'pd4121',
//                        'pd4316', 'pd0431', 'pd4128', 'pd4069', 'pd4129',
//                        'pd4130', 'pd4133', 'pd4141', 'pd4019', 'pd4074',
//                        'pd4196', 'pd4335', 'pd4143', 'pd4156', 'pd4147',
//                        'pd4149', 'pd4150', 'pd4151', 'pd4152', 'pd4038',
//                        'pd4089', 'pd4212', 'pd4206', 'pd4211', 'pd0432',
//                        'pd4162', 'pd4158', 'pd4159', 'pd4160', 'pd4161',
//                        'pd4163', 'pd4051', 'pd4107', 'pd4225', 'pd4329' )");

//            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
//            DataTable dt = ds.Tables[0];
//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                StringBuilder strSql1 = new StringBuilder();
//                strSql1.Append(@"update userinfo  SET UserNumber=SUBSTRING(UserNumber,3,4),UserName=SUBSTRING(UserName,3,4),UserPSW='" + Get_MD5(dt.Rows[i]["UserNumber"].ToString().Substring(2)) + "' where UserNumber='" + dt.Rows[i]["UserNumber"].ToString() + "'");
//                DBUtility.SqlServerHelper.ExecuteSql(strSql1.ToString());
//            }

//            return strRet;
//        }

        //MD5加密
        public string Get_MD5(string strSource)
        {
            byte[] dataToHash = (new System.Text.ASCIIEncoding()).GetBytes(strSource);
            byte[] hashvalue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(dataToHash);
            StringBuilder ret = new StringBuilder();
            foreach (byte b in hashvalue)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString().ToLower();
        }
    }
}
