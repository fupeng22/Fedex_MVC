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
using SQLDAL;
using System.Configuration;
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class OpenCheckVideoQueryController : Controller
    {
        SQLDAL.T_XRaySanLog tXRaySanLog = new T_XRaySanLog();
        SQLDAL.T_OpenCheckLog tOpenCheckLog = new T_OpenCheckLog();
        public const string strFileds = "CargoBC,CargoName,CheckCP,UserName,CheckResults,CheckResultsDescription,CheckDescription,CheckBeginTime,CheckEndTime,CheckByMD_SR,CheckByMD_Chn,CargoID,VideoRePlay,CKID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/OpenCheckVideoQuery.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            ViewData["hidCameraInfo"] = ConfigurationManager.AppSettings["CameraInfo"];
            return View();
        }

        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string ddlCheckResult, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_OpenCheckLog_UserInfo";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CKID";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlCheckResult = Server.UrlDecode(ddlCheckResult == null ? "" : ddlCheckResult.ToString());
            txtCP = Server.UrlDecode(txtCP == null ? "" : txtCP.ToString());

            string strWhereTemp = "";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                    }

                    if (txtCP.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                    }

                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                    }

                    if (ddlCheckResult.ToString() != "" && ddlCheckResult.ToString() != "-99")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and CheckResults in (" + ddlCheckResult.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  CheckResults in (" + ddlCheckResult.ToString() + ")";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataSet dsCame = null;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckCP", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResults", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResultsDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckBeginTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckEndTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckByMD_SR", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckByMD_Chn", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoID", Type.GetType("System.String"));
            dtCustom.Columns.Add("VideoRePlay", Type.GetType("System.String"));
            dtCustom.Columns.Add("CKID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "VideoRePlay":
                            string VideoRePlay = "";
                            dsCame = tOpenCheckLog.GetCameroInfoByDeviceIDChannel(int.Parse(dt.Rows[i]["CheckByMD_SR"].ToString()), int.Parse(dt.Rows[i]["CheckByMD_Chn"].ToString()));
                            if (dsCame != null)
                            {
                                if (dsCame.Tables[0] != null && dsCame.Tables[0].Rows.Count > 0)
                                {
                                    string[] date = dt.Rows[i]["CheckBeginTime"].ToString().Split(' ');
                                    string[] dataArr = date[0].Split('-');
                                    string[] timeArr = date[1].Split(':');
                                    if (int.Parse(dataArr[1]) < 10)
                                    {
                                        dataArr[1] = "0" + dataArr[1];
                                    }
                                    if (int.Parse(dataArr[2]) < 10)
                                    {
                                        dataArr[2] = "0" + dataArr[2];
                                    }
                                    string dateTime = dataArr[0] + dataArr[1] + dataArr[2] + timeArr[0] + timeArr[1] + timeArr[2];
                                    VideoRePlay = "&DeviceID=" + dt.Rows[i]["CheckByMD_SR"] + "&CameraID=" + dsCame.Tables[0].Rows[0]["camera_id"] + "&CameraName=" + dsCame.Tables[0].Rows[0]["name"] + "&RecTime=" + dateTime + "&TagName=" + dt.Rows[i]["CargoBC"] + "&RecLocation=" + dsCame.Tables[0].Rows[0]["record_location_set"];
                                }
                            }
                            drCustom[strFiledArray[j]] = VideoRePlay;
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch = int.Parse(dt.Rows[i]["CheckResults"].ToString());
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
                        case "CheckBeginTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "CheckEndTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["CKID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("OpenCheckVideoQuery_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string ddlCheckResult, string hidSearchType, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_OpenCheckLog_UserInfo";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CKID";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlCheckResult = Server.UrlDecode(ddlCheckResult == null ? "" : ddlCheckResult.ToString());
            txtCP = Server.UrlDecode(txtCP == null ? "" : txtCP.ToString());

            string strWhereTemp = "";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                    }

                    if (txtCP.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                    }

                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                    }

                    if (ddlCheckResult.ToString() != "" && ddlCheckResult.ToString() != "-99")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and CheckResults in (" + ddlCheckResult.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  CheckResults in (" + ddlCheckResult.ToString() + ")";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataSet dsCame = null;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckCP", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResults", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckResultsDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckBeginTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckEndTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckByMD_SR", Type.GetType("System.String"));
            dtCustom.Columns.Add("CheckByMD_Chn", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoID", Type.GetType("System.String"));
            dtCustom.Columns.Add("VideoRePlay", Type.GetType("System.String"));
            dtCustom.Columns.Add("CKID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "VideoRePlay":
                            string VideoRePlay = "";
                            dsCame = tOpenCheckLog.GetCameroInfoByDeviceIDChannel(int.Parse(dt.Rows[i]["CheckByMD_SR"].ToString()), int.Parse(dt.Rows[i]["CheckByMD_Chn"].ToString()));
                            if (dsCame != null)
                            {
                                if (dsCame.Tables[0] != null && dsCame.Tables[0].Rows.Count > 0)
                                {
                                    string[] date = dt.Rows[i]["CheckBeginTime"].ToString().Split(' ');
                                    string[] dataArr = date[0].Split('-');
                                    string[] timeArr = date[1].Split(':');
                                    if (int.Parse(dataArr[1]) < 10)
                                    {
                                        dataArr[1] = "0" + dataArr[1];
                                    }
                                    if (int.Parse(dataArr[2]) < 10)
                                    {
                                        dataArr[2] = "0" + dataArr[2];
                                    }
                                    string dateTime = dataArr[0] + dataArr[1] + dataArr[2] + timeArr[0] + timeArr[1] + timeArr[2];
                                    VideoRePlay = "&DeviceID=" + dt.Rows[i]["CheckByMD_SR"] + "&CameraID=" + dsCame.Tables[0].Rows[0]["camera_id"] + "&CameraName=" + dsCame.Tables[0].Rows[0]["name"] + "&RecTime=" + dateTime + "&TagName=" + dt.Rows[i]["CargoBC"] + "&RecLocation=" + dsCame.Tables[0].Rows[0]["record_location_set"];
                                }
                            }
                            drCustom[strFiledArray[j]] = VideoRePlay;
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch = int.Parse(dt.Rows[i]["CheckResults"].ToString());
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
                        case "CheckBeginTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "CheckEndTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["CKID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("OpenCheckVideoQuery_DS", dtCustom);

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

            string strOutputFileName = "开箱查验扫描结果_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string ddlCheckResult, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_OpenCheckLog_UserInfo";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "CKID";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            ddlCheckResult = Server.UrlDecode(ddlCheckResult == null ? "" : ddlCheckResult.ToString());
            txtCP = Server.UrlDecode(txtCP == null ? "" : txtCP.ToString());

            string strWhereTemp = "";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CargoBC like '%" + txtCode.ToString() + "%') ";
                        }
                    }

                    if (txtCP.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (CheckCP like '%" + txtCP.ToString() + "%') ";
                        }
                    }

                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (CargoBC like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(19),CheckBeginTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(nvarchar(19),CheckEndTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'   ";
                        }
                    }

                    if (ddlCheckResult.ToString() != "" && ddlCheckResult.ToString() != "-99")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and CheckResults in (" + ddlCheckResult.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  CheckResults in (" + ddlCheckResult.ToString() + ")";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataSet dsCame = null;

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
                        case "VideoRePlay":
                            string VideoRePlay = "";
                            dsCame = tOpenCheckLog.GetCameroInfoByDeviceIDChannel(int.Parse(dt.Rows[i]["CheckByMD_SR"].ToString()), int.Parse(dt.Rows[i]["CheckByMD_Chn"].ToString()));
                            if (dsCame!=null )
                            {
                                if (dsCame.Tables[0]!=null && dsCame.Tables[0].Rows.Count>0)
                                {
                                    string[] date = dt.Rows[i]["CheckBeginTime"].ToString().Split(' ');
                                    string[] dataArr = date[0].Split('-');
                                    string[] timeArr = date[1].Split(':');
                                    if (int.Parse(dataArr[1]) < 10)
                                    {
                                        dataArr[1] = "0" + dataArr[1];
                                    }
                                    if (int.Parse(dataArr[2]) < 10)
                                    {
                                        dataArr[2] = "0" + dataArr[2];
                                    }
                                    string dateTime = dataArr[0] + dataArr[1] + dataArr[2] + timeArr[0] + timeArr[1] + timeArr[2];
                                    VideoRePlay = "&DeviceID=" + dt.Rows[i]["CheckByMD_SR"] + "&CameraID=" + dsCame.Tables[0].Rows[0]["camera_id"] + "&CameraName=" + dsCame.Tables[0].Rows[0]["name"] + "&RecTime=" + dateTime + "&TagName=" + dt.Rows[i]["CargoBC"] + "&RecLocation=" + dsCame.Tables[0].Rows[0]["record_location_set"];
                                }
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], VideoRePlay);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], VideoRePlay);
                            }
                            break;
                        case "CheckResultsDescription":
                            string CheckResultsDescription = "";
                            int caseSwitch = int.Parse(dt.Rows[i]["CheckResults"].ToString());
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
                        case "CheckBeginTime":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            break;
                        case "CheckEndTime":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == null ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
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
