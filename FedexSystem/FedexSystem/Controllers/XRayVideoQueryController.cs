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
using System.Configuration;
using FedexSystem.Filter;
//using System.Security.Cryptography;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class XRayVideoQueryController : Controller
    {
        SQLDAL.T_XRaySanLog tXRaySanLog = new T_XRaySanLog();
        SQLDAL.T_OpenCheckLog tOpenCheckLog = new T_OpenCheckLog();

        public const string strFileds = "CargoBC,CargoName,ScanCP,UserName,ScanTime,ScanByMD_SR,ScanByMD_Chn,UserNumber,UserPSW,ScreenShotOnDevice,VideoRePlay,ShowPic,CLID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/XRayVideoQuery.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            ViewData["hidCameraInfo"] = ConfigurationManager.AppSettings["CameraInfo"];
            return View();
        }

        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string txtGOperator, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_UserInfo";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            txtGOperator = Server.UrlDecode(txtGOperator == null ? "" : txtGOperator.ToString());
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
                            strWhereTemp = strWhereTemp + " and   (ScanCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (ScanCP like '%" + txtCP.ToString() + "%') ";
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
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                    }

                    if (txtGOperator.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and UserName like '%" + txtGOperator.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  UserName like '%" + txtGOperator.ToString() + "%'  ";
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

            DataSet dsDeviceID = null;
            DataSet dsCame = null;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanCP", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanByMD_SR", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanByMD_Chn", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserPSW", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScreenShotOnDevice", Type.GetType("System.String"));
            dtCustom.Columns.Add("VideoRePlay", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShowPic", Type.GetType("System.String"));
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
                        case "VideoRePlay":
                            break;
                        case "ShowPic":
                            string VideoRePlay = "";
                            string ShowPic = "";
                            dsDeviceID = tXRaySanLog.GetDeviceID(dt.Rows[i]["ScanByMD_SR"].ToString());
                            if (dsDeviceID != null)
                            {
                                if (dsDeviceID.Tables[0] != null && dsDeviceID.Tables[0].Rows.Count > 0)
                                {
                                    dsCame = tXRaySanLog.GetCameroInfoByDeviceIDChannel(int.Parse(dsDeviceID.Tables[0].Rows[0][0].ToString()), int.Parse(dt.Rows[i]["ScanByMD_Chn"].ToString()));
                                    if (dsCame != null)
                                    {
                                        if (dsCame.Tables[0] != null && dsCame.Tables[0].Rows.Count > 0)
                                        {
                                            string imgPaFu = "";
                                            string imgUrl = "";
                                            if (dt.Rows[i]["ScreenShotOnDevice"] != DBNull.Value)
                                            {
                                                imgPaFu = dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                                imgUrl = dt.Rows[i]["ScanCP"].ToString().Split(';')[2];
                                            }

                                            string date = ((DateTime)dt.Rows[i]["ScanTime"]).ToString("yyyyMMddHHmmss");


                                            VideoRePlay = dsDeviceID.Tables[0].Rows[0][0] + ";" + dsCame.Tables[0].Rows[0]["camera_id"] + ";" + dsCame.Tables[0].Rows[0]["name"] + ";" + date + ";" + dt.Rows[i]["CargoBC"] + ";" + dsCame.Tables[0].Rows[0]["record_location_set"];


                                            if (imgPaFu != "")
                                            {
                                                ShowPic = imgUrl + "?usernum=admin&userpsw=21232f297a57a5a743894ae4a801fc3&filepath=" + dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                            }
                                        }
                                    }
                                }
                            }
                            drCustom["VideoRePlay"] = VideoRePlay;
                            drCustom["ShowPic"] = ShowPic;
                            break;
                        case "ScanTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'");
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
            ReportDataSource reportDataSource = new ReportDataSource("XRayVideoQuery_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string txtGOperator, string hidSearchType, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_UserInfo";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            txtGOperator = Server.UrlDecode(txtGOperator == null ? "" : txtGOperator.ToString());
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
                            strWhereTemp = strWhereTemp + " and   (ScanCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (ScanCP like '%" + txtCP.ToString() + "%') ";
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
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                    }

                    if (txtGOperator.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and UserName like '%" + txtGOperator.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  UserName like '%" + txtGOperator.ToString() + "%'  ";
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

            DataSet dsDeviceID = null;
            DataSet dsCame = null;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("CargoBC", Type.GetType("System.String"));
            dtCustom.Columns.Add("CargoName", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanCP", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserName", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanTime", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanByMD_SR", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScanByMD_Chn", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("UserPSW", Type.GetType("System.String"));
            dtCustom.Columns.Add("ScreenShotOnDevice", Type.GetType("System.String"));
            dtCustom.Columns.Add("VideoRePlay", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShowPic", Type.GetType("System.String"));
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
                        case "VideoRePlay":
                            break;
                        case "ShowPic":
                            string VideoRePlay = "";
                            string ShowPic = "";
                            dsDeviceID = tXRaySanLog.GetDeviceID(dt.Rows[i]["ScanByMD_SR"].ToString());
                            if (dsDeviceID != null)
                            {
                                if (dsDeviceID.Tables[0] != null && dsDeviceID.Tables[0].Rows.Count > 0)
                                {
                                    dsCame = tXRaySanLog.GetCameroInfoByDeviceIDChannel(int.Parse(dsDeviceID.Tables[0].Rows[0][0].ToString()), int.Parse(dt.Rows[i]["ScanByMD_Chn"].ToString()));
                                    if (dsCame != null)
                                    {
                                        if (dsCame.Tables[0] != null && dsCame.Tables[0].Rows.Count > 0)
                                        {
                                            string imgPaFu = "";
                                            string imgUrl = "";
                                            if (dt.Rows[i]["ScreenShotOnDevice"] != DBNull.Value)
                                            {
                                                imgPaFu = dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                                imgUrl = dt.Rows[i]["ScanCP"].ToString().Split(';')[2];
                                            }

                                            string date = ((DateTime)dt.Rows[i]["ScanTime"]).ToString("yyyyMMddHHmmss");


                                            VideoRePlay = dsDeviceID.Tables[0].Rows[0][0] + ";" + dsCame.Tables[0].Rows[0]["camera_id"] + ";" + dsCame.Tables[0].Rows[0]["name"] + ";" + date + ";" + dt.Rows[i]["CargoBC"] + ";" + dsCame.Tables[0].Rows[0]["record_location_set"];


                                            if (imgPaFu != "")
                                            {
                                                ShowPic = imgUrl + "?usernum=admin&userpsw=21232f297a57a5a743894ae4a801fc3&filepath=" + dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                            }
                                        }
                                    }
                                }
                            }
                            drCustom["VideoRePlay"] = VideoRePlay;
                            drCustom["ShowPic"] = ShowPic;
                            break;
                        case "ScanTime":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == null ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd HH:mm:ss").Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;")).Replace("\"", "'");
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
            ReportDataSource reportDataSource = new ReportDataSource("XRayVideoQuery_DS", dtCustom);

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

            string strOutputFileName = "X光机扫描结果信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string txtCode, string txtCP, string inputBeginDate, string inputEndDate, string txtGCode, string txtGOperator, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_XRayScanLog_UserInfo";

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

            txtCode = Server.UrlDecode(txtCode == null ? "" : txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode == null ? "" : txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : inputEndDate.ToString());
            txtGOperator = Server.UrlDecode(txtGOperator == null ? "" : txtGOperator.ToString());
            txtCP = Server.UrlDecode(txtCP == null ? "" : txtCP.ToString());
            //"CargoBC,CargoName,ScanCP,UserName,ScanTime,ScanByMD_SR,ScanByMD_Chn,UserNumber,UserPSW,ScreenShotOnDevice,CLID";

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
                            strWhereTemp = strWhereTemp + " and   (ScanCP like '%" + txtCP.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "    (ScanCP like '%" + txtCP.ToString() + "%') ";
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
                            strWhereTemp = strWhereTemp + "  and convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  convert(nvarchar(10),ScanTime,120)>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyy-MM-dd") + "' and convert(nvarchar(10),ScanTime,120)<='" + Convert.ToDateTime(inputEndDate).ToString("yyyy-MM-dd") + "'   ";
                        }
                    }

                    if (txtGOperator.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and UserName like '%" + txtGOperator.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  UserName like '%" + txtGOperator.ToString() + "%'  ";
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

            DataSet dsDeviceID = null;
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
                            break;
                        case "ShowPic":
                            string VideoRePlay = "";
                            string ShowPic = "";
                            dsDeviceID = tXRaySanLog.GetDeviceID(dt.Rows[i]["ScanByMD_SR"].ToString());
                            if (dsDeviceID != null)
                            {
                                if (dsDeviceID.Tables[0] != null && dsDeviceID.Tables[0].Rows.Count > 0)
                                {
                                    dsCame = tXRaySanLog.GetCameroInfoByDeviceIDChannel(int.Parse(dsDeviceID.Tables[0].Rows[0][0].ToString()), int.Parse(dt.Rows[i]["ScanByMD_Chn"].ToString()));
                                    if (dsCame != null)
                                    {
                                        if (dsCame.Tables[0] != null && dsCame.Tables[0].Rows.Count > 0)
                                        {
                                            string imgPaFu = "";
                                            string imgUrl = "";
                                            if (dt.Rows[i]["ScreenShotOnDevice"] != DBNull.Value)
                                            {
                                                imgPaFu = dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                                imgUrl = dt.Rows[i]["ScanCP"].ToString().Split(';')[2];
                                            }

                                            string date = ((DateTime)dt.Rows[i]["ScanTime"]).ToString("yyyyMMddHHmmss");


                                            VideoRePlay = dsDeviceID.Tables[0].Rows[0][0] + ";" + dsCame.Tables[0].Rows[0]["camera_id"] + ";" + dsCame.Tables[0].Rows[0]["name"] + ";" + date + ";" + dt.Rows[i]["CargoBC"] + ";" + dsCame.Tables[0].Rows[0]["record_location_set"];


                                            if (imgPaFu != "")
                                            {
                                                ShowPic = imgUrl + "?usernum=admin&userpsw=21232f297a57a5a743894ae4a801fc3&filepath=" + dt.Rows[i]["ScreenShotOnDevice"].ToString();
                                            }
                                        }
                                    }
                                }
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", "VideoRePlay", VideoRePlay);
                                sb.AppendFormat("\"{0}\":\"{1}\",", "ShowPic", ShowPic);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", "VideoRePlay", VideoRePlay);
                                sb.AppendFormat("\"{0}\":\"{1}\",", "ShowPic", ShowPic);
                            }
                            break;
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

        //[HttpGet]
        //public string TestEncode(string str)
        //{
        //    string strRet = "";

        //    strRet = Encrypt(str, "HuayuTAT");

        //    return strRet;
        //}

        //[HttpGet]
        //public string TestDecode(string str)
        //{
        //    string strRet = "";

        //    strRet = Decrypt(str, "HuayuTAT");

        //    return strRet;
        //}

        ///// <summary>
        ///// 加密
        ///// </summary>
        ///// <param name="pToEncrypt"></param>
        ///// <param name="sKey">注意，这个sKey长度必须为8，可以为任意字母或数字，但是解密和加密时sKey必须一致</param>
        ///// <returns></returns>
        //public string Encrypt(string pToEncrypt, string sKey)
        //{
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

        //    //建立加密对象的密钥和偏移量 
        //    //原文使用ASCIIEncoding.ASCII方法的GetBytes方法 
        //    //使得输入密码必须输入英文文本 
        //    des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        //    des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();

        //        StringBuilder ret = new StringBuilder();
        //        foreach (byte b in ms.ToArray())
        //        {
        //            ret.AppendFormat("{0:X2}", b);
        //        }
        //        ret.ToString();
        //        return ret.ToString();
        //    }
        //}

        ///// <summary>
        ///// 解密
        ///// </summary>
        ///// <param name="pToDecrypt"></param>
        ///// <param name="sKey"></param>
        ///// <returns></returns>
        //public string Decrypt(string pToDecrypt, string sKey)
        //{
        //    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        //    {

        //        byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
        //        for (int x = 0; x < pToDecrypt.Length / 2; x++)
        //        {
        //            int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
        //            inputByteArray[x] = (byte)i;
        //        }

        //        des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        //        des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        //            cs.Write(inputByteArray, 0, inputByteArray.Length);
        //            cs.FlushFinalBlock();

        //            StringBuilder ret = new StringBuilder();

        //            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        //        }
        //    }
        //}

        //[HttpGet]
        //public string TestEncode(string str)
        //{
        //    string strRet = "";

        //    strRet = DESEncrypt.DesEncrypt(str);

        //    return strRet;
        //}

        //[HttpGet]
        //public string TestDecode(string str)
        //{
        //    string strRet = "";

        //    strRet = DESEncrypt.DesDecrypt(str);

        //    return strRet;
        //}
    }

    //public sealed class DESEncrypt
    //{
    //    private DESEncrypt()
    //    {
    //        //
    //        // TODO: 在此处添加构造函数逻辑
    //        //
    //    }

    //    private static string key = "TATfff";

    //    /**/
    //    /// <summary>
    //    /// 对称加密解密的密钥
    //    /// </summary>
    //    public static string Key
    //    {
    //        get
    //        {
    //            return key;
    //        }
    //        set
    //        {
    //            key = value;
    //        }
    //    }

    //    /**/
    //    /// <summary>
    //    /// DES加密
    //    /// </summary>
    //    /// <param name="encryptString"></param>
    //    /// <returns></returns>
    //    public static string DesEncrypt(string encryptString)
    //    {
    //        byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
    //        byte[] keyIV = keyBytes;
    //        byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
    //        DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
    //        MemoryStream mStream = new MemoryStream();
    //        CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
    //        cStream.Write(inputByteArray, 0, inputByteArray.Length);
    //        cStream.FlushFinalBlock();
    //        return Convert.ToBase64String(mStream.ToArray());
    //    }

    //    /**/
    //    /// <summary>
    //    /// DES解密
    //    /// </summary>
    //    /// <param name="decryptString"></param>
    //    /// <returns></returns>
    //    public static string DesDecrypt(string decryptString)
    //    {
    //        byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
    //        byte[] keyIV = keyBytes;
    //        byte[] inputByteArray = Convert.FromBase64String(decryptString);
    //        DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
    //        MemoryStream mStream = new MemoryStream();
    //        CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
    //        cStream.Write(inputByteArray, 0, inputByteArray.Length);
    //        cStream.FlushFinalBlock();
    //        return Encoding.UTF8.GetString(mStream.ToArray());
    //    }
    //}
}
