using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_WorkingLog
    {
        public DataSet getXRayCurWorkingInfo(string CPs)
        {
            StringBuilder sbConversCPS = new StringBuilder("");
            StringBuilder sb = new StringBuilder();
            StringBuilder strSql = new StringBuilder();
            string[] arrCPs = null;

            arrCPs = CPs.Split(',');
            for (int i = 0; i < arrCPs.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrCPs[i]))
                {
                    if (i != arrCPs.Length - 1)
                    {
                        sbConversCPS.AppendFormat(" ( CPMemo like '%{0}%') or ","X"+arrCPs[i]+";");
                    }
                    else
                    {
                        sbConversCPS.AppendFormat(" ( CPMemo like '%{0}%')  ", "X" + arrCPs[i] + ";");
                    }
                }
            }

            if (string.IsNullOrEmpty(sbConversCPS.ToString()))
            {
                return null;
            }
            else
            {
                strSql.Append("SELECT * from V_XRay_Cur_WorkingInfo where " + sbConversCPS.ToString());
                DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
                if (ds.Tables[0].Rows.Count != 0)
                {
                    return ds;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
