using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
 public class T_XRaySanLog
    {
     /// <summary>
     /// 获取所有模型
     /// </summary>
     /// <returns></returns>
     public DataSet GetXRayScanLogModel()
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("select XRayScanLog.CargoBC,XRayScanLog.CargoName,XRayScanLog.ScanCP,UserInfo.UserName,XRayScanLog.ScanTime,XRayScanLog.ScanByMD_SR,XRayScanLog.ScanByMD_Chn,UserInfo.UserNumber,UserInfo.UserPSW,XRayScanLog.ScreenShotOnDevice  ");
         strSql.Append(" from XRayScanLog INNER JOIN UserInfo ON XRayScanLog.ScanUserID = UserInfo.UserID");
         Model.M_Users model = new Model.M_Users();
         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds; 
     }
     /// <summary>
     /// 简单查询
     /// </summary>
     /// <param name="strCode"></param>
     /// <returns></returns>
     public DataSet GetModelByCodeCP(string strCode,string strCP)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("select XRayScanLog.CargoBC,XRayScanLog.CargoName,XRayScanLog.ScanCP,UserInfo.UserName,XRayScanLog.ScanTime,XRayScanLog.ScanByMD_SR,XRayScanLog.ScanByMD_Chn,UserInfo.UserNumber,UserInfo.UserPSW,XRayScanLog.ScreenShotOnDevice  ");
         strSql.Append(" from XRayScanLog INNER JOIN UserInfo ON XRayScanLog.ScanUserID = UserInfo.UserID where");
         if (strCode!="")
         {
             strSql.Append("  XRayScanLog.CargoBC like '%" + strCode + "%'");
         }
         if(strCP!="") 
         {
             if (strCode == "")
             {
                 strSql.Append("  XRayScanLog.ScanCP like '%" + strCP + "%'");
             }
             else
             {
                 strSql.Append(" and XRayScanLog.ScanCP like '%" + strCP + "%'");
             }
         }
        

         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds;
     }


     public DataSet GetDeviceID( string strSR)
     {
         
          StringBuilder strSql = new StringBuilder();
          strSql.Append("select device_id from Device");
          strSql.Append(" where index_code='" + strSR + "'");
          DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
          if (ds.Tables[0].Rows.Count == 0)
          {
              return null;
          }
          else { return ds; }

     }
        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

     public DataSet GetModelByCodeNameTime(string code, string name, string starTime, string endTime)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("select XRayScanLog.CargoBC,XRayScanLog.CargoName,XRayScanLog.ScanCP,UserInfo.UserName,XRayScanLog.ScanTime,XRayScanLog.ScanByMD_SR,XRayScanLog.ScanByMD_Chn,UserInfo.UserNumber,UserInfo.UserPSW,XRayScanLog.ScreenShotOnDevice  ");
         strSql.Append(" from XRayScanLog INNER JOIN UserInfo ON XRayScanLog.ScanUserID = UserInfo.UserID where");
         if (code.Trim() != "")
         {
             strSql.Append(" XRayScanLog.CargoBC='" + code + "'");
         }
         if (name.Trim() != "")
         {
             if (code.Trim() == "")
             {
                 strSql.Append(" UserInfo.UserName='" + name + "'");
             }
             else
             {
                 strSql.Append(" and UserInfo.UserName='" + name + "'");
             }
            
         }
         
         if (starTime != "")
         {
             if (code.Trim() == "" && name.Trim() == "")
             {
                 strSql.Append(" ScanTime>='" + starTime + "'");
             }
             else
             {
                 strSql.Append(" and ScanTime>='" + starTime + "'");
             }
         }
         if (endTime != "")
         {
             if (code.Trim() == "" && name.Trim() == "" && starTime.Trim() == "")
             {
                 strSql.Append(" ScanTime<='" + endTime + "'");
             }
             else
             {
                 strSql.Append("  and ScanTime<='" + endTime + "'");
             }
         }
         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds;
     }


     //未扫描到货物查询
     public DataSet GetQueryNoReader(string strPost,string starTime, string endTime)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("SELECT ScanCP, ScanTime, CargoName, FlightNumber, CargoQuantity, CargoWeight,CargoProperty ");
         strSql.Append(" FROM XRayScanLog");
         strSql.Append(" where");
         if (strPost.Trim() != "")
         {
             strSql.Append(" XRayScanLog.ScanCP LIKE '"+strPost+";%'");
             strSql.Append(" AND (XRayScanLog.ScanTime >= '"+starTime+"' AND XRayScanLog.ScanTime <= '"+endTime+"')");
         }
         else
             {
                 strSql.Append(" (XRayScanLog.ScanTime >= '" + starTime + "' AND XRayScanLog.ScanTime <= '" + endTime + "')");
             }
         strSql.Append(" and  XRayScanLog.CargoBC='NoReader'" );
         
         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds;
     }
     //光电计算器数据
     public int GetQueryTotalSum(string starTime, string endTime,string strPost)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("SELECT SUM(CargoQuantity)");
         strSql.Append(" FROM XRayScanLog");
         if(strPost=="")
         {
         strSql.Append(" where CargoProperty=200 and ScanTime>='"+starTime+"' and ScanTime<='"+endTime+"'");
         }else
         {
             strSql.Append(" where CargoProperty=200 and ScanTime>='" + starTime + "' and ScanTime<='" + endTime + "' and ScanCP LIKE '"+strPost+"%'"); 
         }
      
         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
       
         if (ds.Tables[0].Rows[0][0] != DBNull.Value)
         {
             return int.Parse(ds.Tables[0].Rows[0][0].ToString());
         }
         else
         {
             return 0;
         }
     }

     //扫描器数据
     public int GetReaderSum(string starTime, string endTime, string strPost)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("SELECT count(CargoID)");
         strSql.Append(" FROM XRayScanLog");
         if (strPost == "")
         {
             strSql.Append(" where CargoProperty<>200 and ScanTime>='" + starTime + "' and ScanTime<='" + endTime + "'");
         }
         else
         {
             strSql.Append(" where CargoProperty<>200 and ScanTime>='" + starTime + "' and ScanTime<='" + endTime + "' and ScanCP LIKE '" + strPost + "%'");
         }

         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

         if (ds.Tables[0].Rows[0][0] != DBNull.Value)
         {
             return int.Parse(ds.Tables[0].Rows[0][0].ToString());
         }
         else
         {
             return 0;
         }
     }

    

     //当班日志查询
     public DataSet GetQueryDetail(string strPost, string starTime, string endTime)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("SELECT XRayScanLog.CLID, XRayScanLog.CargoID, XRayScanLog.ScanCP, XRayScanLog.ScanTime, XRayScanLog.FlightNumber, XRayScanLog.CargoBC, XRayScanLog.CargoName, XRayScanLog.CargoQuantity, XRayScanLog.CargoIDCN, XRayScanLog.CargoProperty, XUserInfo.UserNumber, OpenCheckLog.CheckResults, CUserInfo.UserNumber, OpenCheckLog.CKID, OpenCheckLog.CheckCP FROM (((XRayScanLog");
         strSql.Append(" LEFT JOIN UserInfo AS XUserInfo ON XRayScanLog.ScanUserID = XUserInfo.UserID)");
         strSql.Append(" LEFT JOIN OpenCheckLog ON XRayScanLog.CargoID = OpenCheckLog.CargoID)");
         strSql.Append(" LEFT JOIN UserInfo AS CUserInfo ON OpenCheckLog.CheckUserID = CUserInfo.UserID)");
         if (strPost != "")
         {
             strSql.Append(" WHERE (XRayScanLog.CargoProperty = 0 OR 1=2) AND (XRayScanLog.ScanCP LIKE '" + strPost + ";%' OR 1=2) AND (XRayScanLog.ScanTime >= '" + starTime + "' AND XRayScanLog.ScanTime <= '" + endTime + "' OR 1=2) AND XRayScanLog.CLID IN (");
         }
         else
         {
             strSql.Append(" WHERE (XRayScanLog.CargoProperty = 0 OR 1=2) AND  (XRayScanLog.ScanTime >= '" + starTime + "' AND XRayScanLog.ScanTime <= '" + endTime + "' OR 1=2) AND XRayScanLog.CLID IN (");
         }
            
         
         strSql.Append(" SELECT MAX(CLID) AS CLID FROM XRayScanLog WHERE CargoID <> 0 GROUP BY CargoID) AND ( OpenCheckLog.CKID IN (");
         strSql.Append(" SELECT MAX(CKID) AS CKID FROM OpenCheckLog WHERE CargoID <> 0 GROUP BY CargoID) OR OpenCheckLog.CKID IS NULL)");
         strSql.Append(" ORDER BY XRayScanLog.CargoID ASC");


         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds;
     }


     ///<summary>
     ///根据deviceID channel 获取监控点信息
     ///</summary>
     ///

     public DataSet GetCameroInfoByDeviceIDChannel(int deviceID,int channel)
     {
         StringBuilder strSql = new StringBuilder();
         strSql.Append("select camera_id,name,record_location_set from camera_info");
         strSql.Append(" where device_id='" + deviceID + "' and channel_no='"+channel+"'");

         DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
         return ds;
     }





    }
}
