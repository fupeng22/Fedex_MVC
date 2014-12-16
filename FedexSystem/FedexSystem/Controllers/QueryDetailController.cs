using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Text;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class QueryDetailController : Controller
    {
        //SQLDAL.T_WayBill tWayBill = new T_WayBill();
        //SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "ScanTime,FlightNumber,CargoBC,CargoName,CargoQuantity,CargoIDCN,ScanUserNumber,CheckResults,OpenOrNot,CheckResultsDescription,CheckUserNumber,CKID,CLID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/QueryDetail.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string ddlPostion, string inputBeginDate, string inputEndDate)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_OpenCheckLog_UserInfo_ALL";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CLID";

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

            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());

            string strWhereTemp = " (CargoProperty = 0 OR 1=2) ";
            if (ddlPostion.ToString() != "" && ddlPostion.ToString() != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
            }

            if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
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
            dtCustom.Columns.Add("ScanTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("FlightNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoQuantity", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoIDCN", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanUserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResults", Type.GetType("System.String"));
            dtCustom.Columns.Add("OpenOrNot", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResultsDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckUserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CKID", Type.GetType("System.String"));
            dtCustom.Columns.Add("CLID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "ScanTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'");
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch = dt.Rows[i]["CheckResults"].ToString() == "" ? -1 : int.Parse(dt.Rows[i]["CheckResults"].ToString());
                            switch (caseSwitch)
                            {
                                case 0:
                                    CheckResultsDescription = "忽略";
                                    break;
                                case 1:
                                    CheckResultsDescription = "放行";
                                    break;
                                case 2:
                                    CheckResultsDescription = "扣留";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = CheckResultsDescription;
                            break;
                        case "OpenOrNot":
                            string OpenOrNot = "不开箱";
                            if (dt.Rows[i]["CheckResults"].ToString() != "")
                            {
                                OpenOrNot = "已开箱";
                            }
                            drCustom[strFiledArray[j]] = OpenOrNot;
                            break;
                        case "CargoIDCN":
                            string CargoIDCN = "无";
                            if (dt.Rows[i]["CargoIDCN"].ToString() != "")
                            {
                                OpenOrNot = dt.Rows[i]["CargoIDCN"].ToString();
                            }
                            drCustom[strFiledArray[j]] = CargoIDCN;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["CLID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("QueryDetail_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string ddlPostion, string inputBeginDate, string inputEndDate, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_OpenCheckLog_UserInfo_ALL";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CLID";

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

            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());

            string strWhereTemp = " (CargoProperty = 0 OR 1=2) ";
            if (ddlPostion.ToString() != "" && ddlPostion.ToString() != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
            }

            if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
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
            dtCustom.Columns.Add("ScanTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("FlightNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoQuantity", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoIDCN", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanUserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResults", Type.GetType("System.String"));
            dtCustom.Columns.Add("OpenOrNot", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResultsDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckUserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("CKID", Type.GetType("System.String"));
            dtCustom.Columns.Add("CLID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "ScanTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'");
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch = dt.Rows[i]["CheckResults"].ToString() == "" ? -1 : int.Parse(dt.Rows[i]["CheckResults"].ToString());
                            switch (caseSwitch)
                            {
                                case 0:
                                    CheckResultsDescription = "忽略";
                                    break;
                                case 1:
                                    CheckResultsDescription = "放行";
                                    break;
                                case 2:
                                    CheckResultsDescription = "扣留";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = CheckResultsDescription;
                            break;
                        case "OpenOrNot":
                            string OpenOrNot = "不开箱";
                            if (dt.Rows[i]["CheckResults"].ToString() != "")
                            {
                                OpenOrNot = "已开箱";
                            }
                            drCustom[strFiledArray[j]] = OpenOrNot;
                            break;
                        case "CargoIDCN":
                            string CargoIDCN = "无";
                            if (dt.Rows[i]["CargoIDCN"].ToString() != "")
                            {
                                OpenOrNot = dt.Rows[i]["CargoIDCN"].ToString();
                            }
                            drCustom[strFiledArray[j]] = CargoIDCN;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["CLID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("QueryDetail_DS", dtCustom);

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

            string strOutputFileName = "当班日志查询信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string ddlPostion,  string inputBeginDate, string inputEndDate)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_OpenCheckLog_UserInfo_ALL";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CLID";

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

            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());
           
            string strWhereTemp = " (CargoProperty = 0 OR 1=2) ";
            if (ddlPostion.ToString() != "" && ddlPostion.ToString()!="-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (ScanCP like '" + ddlPostion.ToString() + ";%' or 1=2) ";
                }
            }

            if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  convert(nvarchar(19),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
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
                        case "ScanTime":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'"));
                            }
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch =dt.Rows[i]["CheckResults"].ToString()==""?-1: int.Parse(dt.Rows[i]["CheckResults"].ToString());
                            switch (caseSwitch)
                            {
                                case 0:
                                    CheckResultsDescription = "忽略";
                                    break;
                                case 1:
                                    CheckResultsDescription = "放行";
                                    break;
                                case 2:
                                    CheckResultsDescription = "扣留";
                                    break;
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], CheckResultsDescription);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], CheckResultsDescription);
                            }
                            break;
                        case "OpenOrNot":
                            string OpenOrNot = "不开箱";
                            if (dt.Rows[i]["CheckResults"].ToString()!="")
                            {
                                OpenOrNot = "已开箱";
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], OpenOrNot);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], OpenOrNot);
                            }
                            break;
                        case "CargoIDCN":
                            string CargoIDCN = "无";
                            if (dt.Rows[i]["CargoIDCN"].ToString() != "")
                            {
                                OpenOrNot = dt.Rows[i]["CargoIDCN"].ToString();
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], CargoIDCN);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], CargoIDCN);
                            }
                            break;
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "&quot;").Replace("'", "&apos;"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "&quot;").Replace("'", "&apos;"));
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

            sb = new StringBuilder(sb.ToString().Replace("\\", "/"));

            return sb.ToString();
        }
    }
}
