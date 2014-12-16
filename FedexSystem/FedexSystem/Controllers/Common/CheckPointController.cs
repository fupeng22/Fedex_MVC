using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SQLDAL;
using System.Text;

namespace FedexSystem.Controllers.Common
{
    public class CheckPointController : Controller
    {
        //
        // GET: /CheckPoint/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string CreateCheckPointJSON()
        {
            StringBuilder sbRet = new StringBuilder();
            // return "[{\"id\":\"-99\",\"text\":\"--请选择--\",\"selected\":true},{\"id\":\"0\",\"text\":\"放行\"},{\"id\":\"1\",\"text\":\"等待预检\"},{\"id\":\"2\",\"text\":\"查验放行\"},{\"id\":\"3\",\"text\":\"查验扣留\"},{\"id\":\"4\",\"text\":\"查验待处理\"}]";
            DataSet ds = null;
            DataTable dt = null;

            T_CheckPoint t_CheckPoint = new T_CheckPoint();
            try
            {
                ds = t_CheckPoint.GetCheckPoint();
                if (ds!=null)
                {
                    dt = ds.Tables[0];
                    if (dt!=null && dt.Rows.Count>0)
                    {
                        sbRet.Append("[");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sbRet.Append("{");
                            sbRet.AppendFormat("\"id\":\"{0}\",\"text\":\"{1}\"", dt.Rows[i]["RightValue"].ToString(), dt.Rows[i]["CPMemo"].ToString());
                            sbRet.Append("}");
                            if (i!=dt.Rows.Count-1)
                            {
                                sbRet.Append(",");
                            }
                        }
                        sbRet.Append("]");
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return sbRet.ToString();
        }
    }
}
