using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FedexSystem.Filter;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.IO;
using Microsoft.Reporting.WebForms;
using DBUtility;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class NewQueryInterfaceController : Controller
    {
        //SQLDAL.T_WayBill tWayBill = new T_WayBill();
        //SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "ScanTime,FlightNumber,CargoBC,CargoName,CargoQuantity,CargoIDCN,ScanUserNumber,CheckResults,OpenOrNot,CheckResultsDescription,CheckUserNumber,CKID,CLID,cId";
        public const string strFileds_UserInfo = "UserNumber,UserName,UserID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/QueryInterface.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            string isEmployee = "0";
            if (new T_Users().IsEmployee(Session["Global_UserName"] == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : Session["Global_UserName"].ToString()))
            {
                isEmployee = "1";
            }
            ViewData["IsEmployee"] = isEmployee;
            return View();
        }

        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string ddlPostion, string ddlCargoProperty, string inputDateRange, string rdViewType, string userIds, string hid_IsEmployee)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputDateRange;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "cId";

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

            inputDateRange = Server.UrlDecode(inputDateRange == null ? "" : inputDateRange.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());
            ddlCargoProperty = Server.UrlDecode(ddlCargoProperty == null ? "" : ddlCargoProperty.ToString());
            rdViewType = Server.UrlDecode(rdViewType == null ? "" : rdViewType.ToString());
            userIds = Server.UrlDecode(userIds == null ? "" : userIds.ToString());
            hid_IsEmployee = Server.UrlDecode(hid_IsEmployee == null ? "" : hid_IsEmployee.ToString());

            string strWhereTemp = "";
            StringBuilder sbPosition = new StringBuilder("");

            if (ddlCargoProperty.ToString() != "" && ddlCargoProperty.ToString() != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
            }

            if (userIds != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ScanUserID in (" + userIds + ") ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  ScanUserID in (" + userIds + ") ";
                }
            }

            if (ddlPostion.ToString() != "" && ddlPostion.ToString() != "'-99'")
            {
                string[] strPostionArr = ddlPostion.Split(',');
                for (int i = 0; i < strPostionArr.Length; i++)
                {
                    sbPosition.AppendFormat(" ScanCP like {0} ", strPostionArr[i]);
                    if (i != strPostionArr.Length - 1)
                    {
                        sbPosition.Append(" OR ");
                    }
                }
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (" + sbPosition.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (" + sbPosition.ToString() + ")";
                }
            }

            //如果当前用户为员工，则只查询有登陆过的记录
            //if (new T_Users().IsEmployee(Session["Global_UserName"] == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : Session["Global_UserName"].ToString()))
            //{
            //    if (strWhereTemp != "")
            //    {
            //        strWhereTemp = strWhereTemp + " and  (ScanUserID<>0) ";
            //    }
            //    else
            //    {
            //        strWhereTemp = strWhereTemp + "    (ScanUserID<>0) ";
            //    }
            //}

            switch (rdViewType)
            {
                case "1":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看所有，但是必须登录的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                case "2":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看导入过的所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看导入过的并且登录过的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
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
            dtCustom.Columns.Add("cId", Type.GetType("System.String"));
            dtCustom.Columns.Add("TestImg", Type.GetType("System.Byte[]"));

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
                                default:
                                    CheckResultsDescription = "";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = CheckResultsDescription;
                            break;
                        case "OpenOrNot":
                            string OpenOrNot = "0";
                            if (dt.Rows[i]["CheckResults"].ToString() != "")
                            {
                                OpenOrNot = "1";
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
                if (drCustom["cId"].ToString() != "")
                {
                    BinaryReader br = null;
                    if (dt.Rows[i]["CheckResults"].ToString() == "1")
                    {
                        using (FileStream fs = new FileStream(Server.MapPath("~/images/") + "SolidCircle.png", FileMode.Open))
                        {
                            br = new BinaryReader(fs);
                            drCustom["TestImg"] = br.ReadBytes((int)br.BaseStream.Length);
                        }
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(Server.MapPath("~/images/") + "EmptyCircle.png", FileMode.Open))
                        {
                            br = new BinaryReader(fs);
                            drCustom["TestImg"] = br.ReadBytes((int)br.BaseStream.Length);
                        }
                    }

                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("QueryInterface_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>11in</PageWidth>" +
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
        public ActionResult Excel(string order, string page, string rows, string sort, string ddlPostion, string ddlCargoProperty, string inputDateRange, string rdViewType, string userIds, string browserType, string hid_IsEmployee)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputDateRange;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "cId";

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

            inputDateRange = Server.UrlDecode(inputDateRange == null ? "" : inputDateRange.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());
            ddlCargoProperty = Server.UrlDecode(ddlCargoProperty == null ? "" : ddlCargoProperty.ToString());
            rdViewType = Server.UrlDecode(rdViewType == null ? "" : rdViewType.ToString());
            userIds = Server.UrlDecode(userIds == null ? "" : userIds.ToString());
            hid_IsEmployee = Server.UrlDecode(hid_IsEmployee == null ? "" : hid_IsEmployee.ToString());

            string strWhereTemp = "";
            StringBuilder sbPosition = new StringBuilder("");

            if (ddlCargoProperty.ToString() != "" && ddlCargoProperty.ToString() != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
            }

            if (userIds != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ScanUserID in (" + userIds + ") ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  ScanUserID in (" + userIds + ") ";
                }
            }

            if (ddlPostion.ToString() != "" && ddlPostion.ToString() != "'-99'")
            {
                string[] strPostionArr = ddlPostion.Split(',');
                for (int i = 0; i < strPostionArr.Length; i++)
                {
                    sbPosition.AppendFormat(" ScanCP like {0} ", strPostionArr[i]);
                    if (i != strPostionArr.Length - 1)
                    {
                        sbPosition.Append(" OR ");
                    }
                }
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (" + sbPosition.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (" + sbPosition.ToString() + ")";
                }
            }

            //如果当前用户为员工，则只查询有登陆过的记录
            //if (new T_Users().IsEmployee(Session["Global_UserName"] == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : Session["Global_UserName"].ToString()))
            //{
            //    if (strWhereTemp != "")
            //    {
            //        strWhereTemp = strWhereTemp + " and  (ScanUserID<>0) ";
            //    }
            //    else
            //    {
            //        strWhereTemp = strWhereTemp + "    (ScanUserID<>0) ";
            //    }
            //}

            switch (rdViewType)
            {
                case "1":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看所有，但是必须登录的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                case "2":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看导入过的所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看导入过的并且登录过的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
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
            dtCustom.Columns.Add("cId", Type.GetType("System.String"));
            dtCustom.Columns.Add("TestImg", Type.GetType("System.Byte[]"));

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
                                default:
                                    CheckResultsDescription = "";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = CheckResultsDescription;
                            break;
                        case "OpenOrNot":
                            string OpenOrNot = "0";
                            if (dt.Rows[i]["CheckResults"].ToString() != "")
                            {
                                OpenOrNot = "1";
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
                if (drCustom["cId"].ToString() != "")
                {
                    BinaryReader br = null;
                    if (dt.Rows[i]["CheckResults"].ToString() == "1")
                    {
                        using (FileStream fsImg = new FileStream(Server.MapPath("~/images/") + "SolidCircle.png", FileMode.Open))
                        {
                            br = new BinaryReader(fsImg);
                            drCustom["TestImg"] = br.ReadBytes((int)br.BaseStream.Length);
                        }
                    }
                    else
                    {
                        using (FileStream fsImg = new FileStream(Server.MapPath("~/images/") + "EmptyCircle.png", FileMode.Open))
                        {
                            br = new BinaryReader(fsImg);
                            drCustom["TestImg"] = br.ReadBytes((int)br.BaseStream.Length);
                        }
                    }

                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("QueryInterface_DS", dtCustom);

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

            string strOutputFileName = "X光机综合查询_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string ddlPostion, string ddlCargoProperty, string inputDateRange, string rdViewType, string userIds, string hid_IsEmployee)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputDateRange;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "cId";

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

            inputDateRange = Server.UrlDecode(inputDateRange == null ? "" : inputDateRange.ToString());
            ddlPostion = Server.UrlDecode(ddlPostion == null ? "" : ddlPostion.ToString());
            ddlCargoProperty = Server.UrlDecode(ddlCargoProperty == null ? "" : ddlCargoProperty.ToString());
            rdViewType = Server.UrlDecode(rdViewType == null ? "" : rdViewType.ToString());
            userIds = Server.UrlDecode(userIds == null ? "" : userIds.ToString());
            hid_IsEmployee = Server.UrlDecode(hid_IsEmployee == null ? "" : hid_IsEmployee.ToString());

            string strWhereTemp = "";
            StringBuilder sbPosition = new StringBuilder("");

            if (ddlCargoProperty.ToString() != "" && ddlCargoProperty.ToString() != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  CargoProperty in (" + ddlCargoProperty.ToString() + ")";
                }
            }

            if (userIds != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ScanUserID in (" + userIds + ") ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  ScanUserID in (" + userIds + ") ";
                }
            }

            if (ddlPostion.ToString() != "" && ddlPostion.ToString() != "'-99'")
            {
                string[] strPostionArr = ddlPostion.Split(',');
                for (int i = 0; i < strPostionArr.Length; i++)
                {
                    sbPosition.AppendFormat(" ScanCP like {0} ", strPostionArr[i]);
                    if (i != strPostionArr.Length - 1)
                    {
                        sbPosition.Append(" OR ");
                    }
                }
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (" + sbPosition.ToString() + ")";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   (" + sbPosition.ToString() + ")";
                }
            }

            //如果当前用户为员工，则只查询有登陆过的记录
            //if (new T_Users().IsEmployee(Session["Global_UserName"] == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : Session["Global_UserName"].ToString()))
            //{
            //    if (strWhereTemp != "")
            //    {
            //        strWhereTemp = strWhereTemp + " and  (ScanUserID<>0) ";
            //    }
            //    else
            //    {
            //        strWhereTemp = strWhereTemp + "    (ScanUserID<>0) ";
            //    }
            //}

            switch (rdViewType)
            {
                case "1":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看所有，但是必须登录的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                case "2":
                    switch (hid_IsEmployee)
                    {
                        case "0"://查看导入过的所有
                            param[0].Value = inputDateRange;
                            break;
                        case "1"://查看导入过的并且登录过的
                            param[0].Value = inputDateRange;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
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
                                default:
                                    CheckResultsDescription = "";
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
                            string OpenOrNot = "0";
                            if (dt.Rows[i]["CheckResults"].ToString() != "")
                            {
                                OpenOrNot = "1";
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

        public string GetData_UserInfo(string order, string page, string rows, string sort, string strDeptId)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users_Department_With_0";

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
            rows = "100000";
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

            string strWhereTemp = "";
            string strUserIds = "";
            //根据此部门ID查找其部门以及下级部门的所有员工
            strDeptId = Server.UrlDecode(strDeptId == null ? "" : strDeptId.ToString());

            if (strDeptId.ToString() == "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and   1=2 ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "   1=2 ";
                }
            }
            else
            {
                strUserIds = GetUserIds();
                if (strUserIds == "")
                {
                    strUserIds = "0";
                }
                else
                {
                    strUserIds =strUserIds+ ",0";
                }
                if (strUserIds == "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   1=2 ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   1=2 ";
                    }
                }
                else
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   UserID in ( " + strUserIds + ") ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "    UserID in ( " + strUserIds + ") ";
                    }
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

                string[] strFiledArray = strFileds_UserInfo.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
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

        [HttpPost]
        public string GetUserIds()
        {
            string strResult = "";
            List<String> lstUserIds = new List<string>();
            T_Department t_Department = null;
            DataSet ds = null;
            DataTable dt = null;

            DataSet dsUserInfo = null;
            DataTable dtUserInfo = null;

            string strCurrUserNumber = "";
            string deptId = "";

            string isEmployee = "0";

            t_Department = new T_Department();
            strCurrUserNumber = Session["Global_UserName"] == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : Session["Global_UserName"].ToString();

            if (new T_Users().IsEmployee(strCurrUserNumber))
            {
                isEmployee = "1";
            }

            switch (isEmployee)
            {
                case "0"://不是组员
                    dsUserInfo = new T_Users().getUserInfoByUserNumber(strCurrUserNumber);
                    if (dsUserInfo != null)
                    {
                        dtUserInfo = dsUserInfo.Tables[0];
                        if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                        {
                            deptId = dtUserInfo.Rows[0]["Department"].ToString();
                            ds = t_Department.getDepartByParentId(deptId);

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

                            if (ds != null)
                            {
                                dt = ds.Tables[0];
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref lstUserIds);
                                    }

                                }
                            }
                            for (int i = 0; i < lstUserIds.Count; i++)
                            {
                                strResult = strResult + lstUserIds[i] + ",";
                            }
                            if (strResult.EndsWith(","))
                            {
                                strResult = strResult.Substring(0, strResult.Length - 1);
                            }
                        }
                    }
                    break;
                case "1"://是组员
                    dsUserInfo = new T_Users().getUserInfoByUserNumber(strCurrUserNumber);
                    if (dsUserInfo != null)
                    {
                        dtUserInfo = dsUserInfo.Tables[0];
                        if (dtUserInfo != null && dtUserInfo.Rows.Count > 0)
                        {
                            if (!lstUserIds.Contains(dtUserInfo.Rows[0]["UserID"].ToString()))
                            {
                                lstUserIds.Add(dtUserInfo.Rows[0]["UserID"].ToString());
                            }

                            if (ds != null)
                            {
                                dt = ds.Tables[0];
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        CreateDepartJSONWithUsers(dt.Rows[i]["DepId"].ToString(), ref lstUserIds);
                                    }
                                }
                            }
                            for (int i = 0; i < lstUserIds.Count; i++)
                            {
                                strResult = strResult + lstUserIds[i] + ",";
                            }
                            if (strResult.EndsWith(","))
                            {
                                strResult = strResult.Substring(0, strResult.Length - 1);
                            }
                        }
                    }
                    break;
                default:
                    break;
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

            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
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

        [HttpPost]
        public string getAllProduceScanLogHeaderInfo()
        {
            DataSet dsAllProduceScanLogHeader = null;
            DataTable dtAllProduceScanLogHeader = null;
            string strRet = "";
            StringBuilder sbRet = new StringBuilder("");

            sbRet .Append("[{\"id\":\"-99\",\"text\":\"--请选择--\"}");
            dsAllProduceScanLogHeader = new T_ProduceScanLogHeader().getAllProduceScanLogHeaderInfo();
            if (dsAllProduceScanLogHeader!=null)
            {
                dtAllProduceScanLogHeader=dsAllProduceScanLogHeader.Tables[0];
                if (dtAllProduceScanLogHeader!=null && dtAllProduceScanLogHeader.Rows.Count>0)
                {
                    sbRet.Append(",");
                    for (int i = 0; i < dtAllProduceScanLogHeader.Rows.Count; i++)
                    {
                        sbRet.Append("{");
                        sbRet.AppendFormat("\"id\":\"{0}\",\"text\":\"{1}\"", dtAllProduceScanLogHeader.Rows[i]["ScanLogViewName"].ToString(), dtAllProduceScanLogHeader.Rows[i]["ScanLogRange"].ToString());
                        sbRet.Append("}");
                        if (i!=dtAllProduceScanLogHeader.Rows.Count-1)
                        {
                            sbRet.Append(",");
                        }
                    }
                }
            }
            sbRet.Append("]");
            strRet = sbRet.ToString();
            return strRet;
        }
    }
}
