using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using SQLDAL;
using System.Data.SqlClient;
using DBUtility;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class XRayDutyRealController : Controller
    {
        public const string strFileds = "CPID,CPMemo,wlUserID,wlWorkStart,wlWorkEnd,wlUseTime,UserNumber,UserName,Department,wlWorkStartDes,wlWorkEndDes,wlUseTimeDes,wlID";
        //
        // GET: /XRayDutyReal/

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
        public string LoadHistoryWorkingInfo(string order, string page, string rows, string sort, string inputDate, string CPID)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRay_All_WorkingInfo";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wlID";

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
            rows = "1000000";
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

            inputDate = Server.UrlDecode(inputDate);
            CPID = Server.UrlDecode(CPID);

            string strWhereTemp = "   (wlWorkEnd IS NOT NULL) AND (wlWorkStart IS NOT NULL) ";
            if (inputDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CONVERT(NVARCHAR(10),wlWorkStart,120)='"+Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd")+"')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "    (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd") + "')";
                }
            }

            if (CPID!="")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and  (CPMemo like '%X{0};%') ",CPID);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (CPMemo like '%X{0};%') ",CPID);
                }
            }

            param[6].Value = strWhereTemp;

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
                        case "wlWorkStartDes":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm"));
                            }
                            break;
                        case "wlWorkEndDes":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()).ToString("HH:mm"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()).ToString("HH:mm"));
                            }
                            break;
                        case "wlUseTimeDes":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], (Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()) - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).TotalMinutes.ToString("0.00"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], (Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()) - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).TotalMinutes.ToString("0.00"));
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

        /// <summary>
        /// 根据传入的CP编号组成的字符串获取X光机当前员工工作信息
        /// </summary>
        /// <param name="CPs">如:0,1,2,3,4</param>
        /// <returns></returns>
        [HttpPost]
        public string LoadXRayWorkingInfo(string CPs)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"获取X光机当前员工工作信息失败，原因未知\"}";
            DataSet ds = null;
            DataTable dt = null;
            string[] arrCPs = null;
            string strCurCP = "";
            StringBuilder sb = new StringBuilder();

            string strCurUserId = "";
            string strCurUserName = "";
            string strCurUserNameDes = "";
            string strWorkingLong = "";
            string strBeginWorking = "";
            string strEndWorking = "";

            CPs = Server.UrlDecode(CPs);
            arrCPs = CPs.Split(',');
            try
            {
                ds = new T_WorkingLog().getXRayCurWorkingInfo(CPs);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            strCurCP = "-1";
                            for (int j = 0; j < arrCPs.Length; j++)
                            {
                                if (dt.Rows[i]["CPMemo"].ToString().IndexOf("X" + arrCPs[j] + ";") != -1)
                                {
                                    strCurCP = arrCPs[j];
                                }
                            }
                            if (strCurCP != "-1")
                            {
                                if (dt.Rows[i]["CurUserID"].ToString() == "0")//目前该X光机无人工作
                                {
                                    strCurUserId = "";
                                    strCurUserName = "";
                                    strCurUserNameDes = "无人工作";
                                    strWorkingLong = "";
                                    strBeginWorking = "";
                                    strEndWorking = "";
                                }
                                else
                                {
                                    strCurUserId = dt.Rows[i]["UserID"] == DBNull.Value ? "" : dt.Rows[i]["UserID"].ToString();
                                    strCurUserName = dt.Rows[i]["UserName"] == DBNull.Value ? "" : dt.Rows[i]["UserName"].ToString();
                                    strCurUserNameDes = strCurUserName;
                                    strWorkingLong = (DateTime.Now - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).TotalMinutes.ToString("00.00");
                                    strBeginWorking = Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm");
                                    strEndWorking = Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).AddMinutes(45).ToString("HH:mm");
                                }

                                sb.Append("{");
                                sb.AppendFormat("\"iWhichCP\":\"{0}\",\"userID\":\"{1}\",\"UserName\":\"{2}\",\"UserNameDes\":\"{3}\",\"WorkingLong\":\"{4}\",\"StartTime\":\"{5}\",\"EndTime\":\"{6}\"", strCurCP, strCurUserId, strCurUserName, strCurUserNameDes, strWorkingLong, strBeginWorking, strEndWorking);
                                sb.Append("}");
                                if (i != dt.Rows.Count - 1)
                                {
                                    sb.Append(",");
                                }
                            }
                        }
                        return "{\"result\":\"ok\",\"rows\":[" + sb.ToString() + "],\"message\":\"\"}";
                    }
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"获取X光机当前员工工作信息失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string LoadXRayWorkingInfoByXRayType(string XRayType)
        {
            string strRet = "{\"result\":\"error\",\"XRayTypeName\":\"\",\"message\":\"获取X光机名称失败，原因未知\"}";
            DataSet ds = null;
            DataTable dt = null;

            XRayType = Server.UrlDecode(XRayType);

            try
            {
                ds = new T_AllXRayType().getXRayTypeInfoByXRayType(XRayType);
                if (ds!=null)
                {
                    dt = ds.Tables[0];
                    if (dt!=null && dt.Rows.Count>0)
                    {
                        strRet = "{\"result\":\"ok\",\"XRayTypeName\":\"" + dt.Rows[0]["XRayTypeDes"].ToString() + "\",\"message\":\"获取X光机名称成功\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"XRayTypeName\":\"\",\"message\":\"获取X光机名称失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
