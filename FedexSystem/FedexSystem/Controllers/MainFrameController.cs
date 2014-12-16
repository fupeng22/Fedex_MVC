using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FedexSystem.Filter;
using System.Data;
using SQLDAL;
using System.Text;

namespace FedexSystem.Controllers
{
    [ErrorAttribute]
    public class MainFrameController : Controller
    {
        //
        // GET: /MainFrame/

        [RequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetAllXRayTypeInfo()
        {
            DataSet ds = null;
            DataTable dt = null;
            StringBuilder sb = new StringBuilder("");

            sb.Append("[{\"id\":\"-99\",\"text\":\"--请选择--\"}");
            ds = new T_AllXRayType().getAllXRayTypeInfo();
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append(",");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"id\":\"{0}\",\"text\":\"{1}\"", dt.Rows[i]["XRayType"].ToString(), dt.Rows[i]["XRayTypeDes"].ToString());
                        sb.Append("}");
                        if (i!=dt.Rows.Count-1)
                        {
                            sb.Append(",");
                        }
                    }
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
