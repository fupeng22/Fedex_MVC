using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class OnDutyStatisticByUserController : Controller
    {
        public const string strFileds = "XRayTypeDes,CPMemo,dDate,dDateDes,LoginCount,XRayType,cId";

        public const string strFiledsXRayOnDutyDetail = "CPMemo,wlUserID,wlWorkStart,wlWorkEnd,wlUseTime,wlWorkStartDes,wlWorkEndDes,wlUseTimeDes,UserNumber,UserName,Department,wlID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/ShowSubWayBillDetail.rdlc";
        public const string STR_REPORT_WAYBIILL_URL = "~/Content/Reports/ShowWayBillInfo.rdlc";

        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult PrintXRayOnDutyDetail(string order, string page, string rows, string sort, string inputDate, string ddlXRayType)
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

            inputDate = Server.UrlDecode(inputDate.ToString());
            ddlXRayType = Server.UrlDecode(ddlXRayType.ToString());

            string strWhereTemp = "";
            if (inputDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
            }

            if (ddlXRayType.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
                }
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CPMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUserID", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkStart", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkEnd", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUseTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkStartDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkEndDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUseTimeDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("Department", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFiledsXRayOnDutyDetail.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wlWorkStartDes":
                            string strwlWorkStartDes = "";
                            strwlWorkStartDes = dt.Rows[i]["wlWorkStart"] == null ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlWorkStartDes;
                            break;
                        case "wlWorkEndDes":
                            string strwlWorkEndDes = "";
                            strwlWorkEndDes = dt.Rows[i]["wlWorkEnd"] == null ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlWorkEndDes;
                            break;
                        case "wlUseTimeDes":
                            string strwlUseTimeDes = "";
                            strwlUseTimeDes = dt.Rows[i]["wlWorkEnd"] == null ? "" : (Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()) - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlUseTimeDes;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }

                if (drCustom["wlID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowSubWayBillDetail_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>12in</PageWidth>" +
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
        public ActionResult ExcelXRayOnDutyDetail(string order, string page, string rows, string sort, string inputDate, string ddlXRayType, string browserType)
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

            inputDate = Server.UrlDecode(inputDate.ToString());
            ddlXRayType = Server.UrlDecode(ddlXRayType.ToString());

            string strWhereTemp = "";
            if (inputDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
            }

            if (ddlXRayType.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
                }
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CPMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUserID", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkStart", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkEnd", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUseTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkStartDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlWorkEndDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlUseTimeDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("Department", Type.GetType("System.String"));
            dtCustom.Columns.Add("wlID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFiledsXRayOnDutyDetail.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wlWorkStartDes":
                            string strwlWorkStartDes = "";
                            strwlWorkStartDes = dt.Rows[i]["wlWorkStart"] == null ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlWorkStartDes;
                            break;
                        case "wlWorkEndDes":
                            string strwlWorkEndDes = "";
                            strwlWorkEndDes = dt.Rows[i]["wlWorkEnd"] == null ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlWorkEndDes;
                            break;
                        case "wlUseTimeDes":
                            string strwlUseTimeDes = "";
                            strwlUseTimeDes = dt.Rows[i]["wlWorkEnd"] == null ? "" : (Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()) - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).ToString("HH:mm");
                            drCustom[strFiledArray[j]] = strwlUseTimeDes;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }

                if (drCustom["wlID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowSubWayBillDetail_DS", dtCustom);

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

            string strOutputFileName = "" + "子运单明细信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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


        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string GetData(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string ddlXRayType)
        {
            StringBuilder sb = new StringBuilder("");
            DataSet ds = null;
            DataTable dt = null;
            string[] arrXRayType = null;
            StringBuilder sbXRayType = new StringBuilder("");
            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            ddlXRayType = Server.UrlDecode(ddlXRayType);

            arrXRayType = ddlXRayType.Split(',');
            for (int i = 0; i < arrXRayType.Length; i++)
            {
                sbXRayType.AppendFormat("'{0}'", arrXRayType[i]);
                if (i != arrXRayType.Length - 1)
                {
                    sbXRayType.Append(",");
                }

            }

            SqlParameter[] parameters = {
                                new SqlParameter("@inputBeginDate",SqlDbType.DateTime),
                                new SqlParameter("@inputEndDate",SqlDbType.DateTime),
                                new SqlParameter("@XRayTypes",SqlDbType.NVarChar)
                                                        };

            parameters[0].Value = Convert.ToDateTime(inputBeginDate);
            parameters[1].Value = Convert.ToDateTime(inputEndDate);
            parameters[2].Value = sbXRayType.ToString();
            try
            {
                ds = SqlServerHelper.RunProcedure("sp_ProduceOnDutyInfoByXRay", parameters, "Default");

                if (ds != null)
                {
                    dt = ds.Tables["Default"];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");

                        int maxCount = -1;

                        if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
                        {
                            maxCount = dt.Rows.Count;
                        }
                        else
                        {
                            maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
                        }

                        for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
                        {
                            sb.Append("{");

                            string[] strFiledArray = strFileds.Split(',');
                            for (int j = 0; j < strFiledArray.Length; j++)
                            {
                                switch (strFiledArray[j])
                                {
                                    case "dDateDes":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["dDate"] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i]["dDate"].ToString()).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["dDate"] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i]["dDate"].ToString()).ToString("yyyy-MM-dd").Replace("\r\n", "")));
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
                    }
                    else
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");
                    }
                }

                if (sb.ToString().EndsWith(","))
                {
                    sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                }
                sb.Append("]");
                sb.Append("}");

            }
            catch (Exception ex)
            {
                sb = new StringBuilder("");
                sb.Append("{");
                sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                sb.Append(",\"rows\":[]}");
            }

            return sb.ToString();
        }

        [HttpGet]
        public ActionResult PrintXRayOnDutyInfoInfo(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string ddlXRayType)
        {
            StringBuilder sb = new StringBuilder("");
            DataSet ds = null;
            DataTable dt = null;
            DataTable dtCustom = new DataTable();
            DataRow drCustom = null;
            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            ddlXRayType = Server.UrlDecode(ddlXRayType);
            //XRayTypeDes,CPMemo,dDate,LoginCount,XRayType
            dtCustom.Columns.Add("XRayTypeDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("CPMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("dDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("dDateDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("LoginCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("XRayType", Type.GetType("System.String"));

            SqlParameter[] parameters = {
                                new SqlParameter("@inputBeginDate",SqlDbType.DateTime),
                                new SqlParameter("@inputEndDate",SqlDbType.DateTime),
                                new SqlParameter("@XRayTypes",SqlDbType.NVarChar)
                                                        };

            parameters[0].Value = Convert.ToDateTime(inputBeginDate);
            parameters[1].Value = Convert.ToDateTime(inputEndDate);
            parameters[2].Value = ddlXRayType;

            ds = SqlServerHelper.RunProcedure("sp_ProduceOnDutyInfoByXRay", parameters, "Default");

            if (ds != null)
            {
                dt = ds.Tables["Default"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    int maxCount = -1;

                    if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
                    {
                        maxCount = dt.Rows.Count;
                    }
                    else
                    {
                        maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
                    }

                    for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
                    {
                        drCustom = dtCustom.NewRow();

                        string[] strFiledArray = strFileds.Split(',');
                        for (int j = 0; j < strFiledArray.Length; j++)
                        {
                            switch (strFiledArray[j])
                            {
                                case "dDateDes":
                                    drCustom[strFiledArray[j]] = dt.Rows[i]["dDate"] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i]["dDate"].ToString()).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                                    break;
                                default:
                                    drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                                    break;
                            }
                        }
                        if (drCustom["XRayType"].ToString() != "")
                        {
                            dtCustom.Rows.Add(drCustom);
                        }
                    }
                    dt = null;
                }
            }


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_WAYBIILL_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowWayBillInfo_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>12in</PageWidth>" +
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
        public ActionResult ExcelXRayOnDutyInfoInfo(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string ddlXRayType, string browserType)
        {
            StringBuilder sb = new StringBuilder("");
            DataSet ds = null;
            DataTable dt = null;
            DataTable dtCustom = new DataTable();
            DataRow drCustom = null;
            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            ddlXRayType = Server.UrlDecode(ddlXRayType);
            //XRayTypeDes,CPMemo,dDate,LoginCount,XRayType
            dtCustom.Columns.Add("XRayTypeDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("CPMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("dDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("dDateDes", Type.GetType("System.String"));
            dtCustom.Columns.Add("LoginCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("XRayType", Type.GetType("System.String"));

            SqlParameter[] parameters = {
                                new SqlParameter("@inputBeginDate",SqlDbType.DateTime),
                                new SqlParameter("@inputEndDate",SqlDbType.DateTime),
                                new SqlParameter("@XRayTypes",SqlDbType.NVarChar)
                                                        };

            parameters[0].Value = Convert.ToDateTime(inputBeginDate);
            parameters[1].Value = Convert.ToDateTime(inputEndDate);
            parameters[2].Value = ddlXRayType;

            ds = SqlServerHelper.RunProcedure("sp_ProduceOnDutyInfoByXRay", parameters, "Default");

            if (ds != null)
            {
                dt = ds.Tables["Default"];
                if (dt != null && dt.Rows.Count > 0)
                {

                    int maxCount = -1;

                    if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
                    {
                        maxCount = dt.Rows.Count;
                    }
                    else
                    {
                        maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
                    }

                    for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
                    {
                        drCustom = dtCustom.NewRow();

                        string[] strFiledArray = strFileds.Split(',');
                        for (int j = 0; j < strFiledArray.Length; j++)
                        {
                            switch (strFiledArray[j])
                            {
                                case "dDateDes":
                                    drCustom[strFiledArray[j]] = dt.Rows[i]["dDate"] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i]["dDate"].ToString()).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                                    break;
                                default:
                                    drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                                    break;
                            }
                        }
                        if (drCustom["XRayType"].ToString() != "")
                        {
                            dtCustom.Rows.Add(drCustom);
                        }
                    }
                    dt = null;
                }
            }
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_WAYBIILL_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowWayBillInfo_DS", dtCustom);

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

            string strOutputFileName = "总运单信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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


        public string GetXRayOnDutyDetail(string order, string page, string rows, string sort, string inputDate, string ddlXRayType)
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

            inputDate = Server.UrlDecode(inputDate.ToString());
            ddlXRayType = Server.UrlDecode(ddlXRayType.ToString());

            string strWhereTemp = "";
            if (inputDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CONVERT(NVARCHAR(10),wlWorkStart,120)='" + Convert.ToDateTime(inputDate.ToString()).ToString("yyyy-MM-dd") + "') ";
                }
            }

            if (ddlXRayType.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (CPMemo like '%" + ddlXRayType.ToString() + ";%')";
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

                string[] strFiledArray = strFiledsXRayOnDutyDetail.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wlWorkStartDes":
                            string strwlWorkStartDes = "";
                            strwlWorkStartDes = dt.Rows[i]["wlWorkStart"] == DBNull.Value ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString()).ToString("HH:mm");
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strwlWorkStartDes);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strwlWorkStartDes);
                            }
                            break;
                        case "wlWorkEndDes":
                            string strwlWorkEndDes = "";
                            strwlWorkEndDes = dt.Rows[i]["wlWorkEnd"] == DBNull.Value ? "" : Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()).ToString("HH:mm");
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strwlWorkEndDes);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strwlWorkEndDes);
                            }
                            break;
                        case "wlUseTimeDes":
                            string strwlUseTimeDes = "";
                            strwlUseTimeDes = dt.Rows[i]["wlWorkEnd"] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i]["wlWorkEnd"].ToString()) - Convert.ToDateTime(dt.Rows[i]["wlWorkStart"].ToString())).TotalMinutes.ToString("0.00");
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strwlUseTimeDes);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strwlUseTimeDes);
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

    }
}
