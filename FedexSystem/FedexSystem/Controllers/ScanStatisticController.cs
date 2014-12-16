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
using FedexSystem.Filter;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class ScanStatisticController : Controller
    {
        //SQLDAL.T_WayBill tWayBill = new T_WayBill();
        //SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "indexSort,sumCount,scanCount,scanRate,scanPostion";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/ScanStatistic.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate)
        {
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : Convert.ToDateTime(inputBeginDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : Convert.ToDateTime(inputEndDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.DateTime;
            param[0].ParameterName = "@scanBeginDT";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputBeginDate;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@scanEndDT";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = inputEndDate;

            DataSet ds = SqlServerHelper.RunProcedure("sp_ScanStatistic", param, "result");
            DataTable dt = ds.Tables["result"];

            if (sort == "scanPostion")
            {
                sort = " indexSort ";
            }
            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("sumCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanRate", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanPostion", Type.GetType("System.String"));
            dtCustom.Columns.Add("indexSort", Type.GetType("System.String"));
            
            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["scanPostion"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ScanStatistic_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string browserType)
        {
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : Convert.ToDateTime(inputBeginDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : Convert.ToDateTime(inputEndDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.DateTime;
            param[0].ParameterName = "@scanBeginDT";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputBeginDate;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@scanEndDT";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = inputEndDate;

            DataSet ds = SqlServerHelper.RunProcedure("sp_ScanStatistic", param, "result");
            DataTable dt = ds.Tables["result"];

            if (sort == "scanPostion")
            {
                sort = " indexSort ";
            }
            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("sumCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanRate", Type.GetType("System.String"));
            dtCustom.Columns.Add("scanPostion", Type.GetType("System.String"));
            dtCustom.Columns.Add("indexSort", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\n", "&nbsp;").Replace("\r\n", "&nbsp;").Replace("\"", "'"));
                            break;
                    }

                }
                if (drCustom["scanPostion"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ScanStatistic_DS", dtCustom);

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

            string strOutputFileName = "扫描统计信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate)
        {
            inputBeginDate = Server.UrlDecode(inputBeginDate == null ? "" : Convert.ToDateTime(inputBeginDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
            inputEndDate = Server.UrlDecode(inputEndDate == null ? "" : Convert.ToDateTime(inputEndDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.DateTime;
            param[0].ParameterName = "@scanBeginDT";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = inputBeginDate;

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@scanEndDT";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = inputEndDate;

            DataSet ds = SqlServerHelper.RunProcedure("sp_ScanStatistic", param, "result");
            DataTable dt = ds.Tables["result"];

            if (sort == "scanPostion")
            {
                sort = " indexSort ";
            }
            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
            sb.Append(",\"rows\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
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
    }
}
