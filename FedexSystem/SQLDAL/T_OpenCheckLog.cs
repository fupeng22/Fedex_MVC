using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace SQLDAL
{
   public class T_OpenCheckLog
    {
       /// <summary>
       /// 获取开箱模型
       /// </summary>
       /// <returns></returns>
        public DataSet GetOpenCheckLogModel()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select CKID,CargoID,CargoBC,CargoName,ScanUserID,ScanCP,CheckResults，CheckBeginTime，CheckEndTime from OpenCheckLog ");
            Model.M_Users model = new Model.M_Users();
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
        /// <summary>
        /// 简单查询
        /// </summary>
        /// <returns></returns>
        public DataSet GetModelByCodeCP(string strCode, string strCP)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select OpenCheckLog.CargoBC,OpenCheckLog.CargoName,OpenCheckLog.CheckCP,UserInfo.UserName,OpenCheckLog.CheckResults,OpenCheckLog.CheckDescription,OpenCheckLog.CheckBeginTime,OpenCheckLog.CheckEndTime,OpenCheckLog.CheckByMD_SR,OpenCheckLog.CheckByMD_Chn,OpenCheckLog.CargoID");
            strSql.Append(" from OpenCheckLog INNER JOIN UserInfo ON OpenCheckLog.CheckUserID = UserInfo.UserID where");
            if (strCode.Trim()!= "")
            {
                strSql.Append("  OpenCheckLog.CargoBC='" + strCode + "'");
            }
            if (strCP != "")
            {
                if (strCode.Trim() == "")
                {
                    strSql.Append(" OpenCheckLog.CheckCP like '%" + strCP + "%'");
                }
                else
                {
                    strSql.Append(" and OpenCheckLog.CheckCP like '%" + strCP + "%'");
                }
            }

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public DataSet GetModelByCodeNameTime(string code, string name, string starTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select OpenCheckLog.CargoBC,OpenCheckLog.CargoName,OpenCheckLog.CheckCP,UserInfo.UserName,OpenCheckLog.CheckResults，OpenCheckLog.CheckDescription,OpenCheckLog.CheckBeginTime,OpenCheckLog.CheckEndTime");
            strSql.Append(" from OpenCheckLog INNER JOIN UserInfo ON OpenCheckLog.ScanUserID = UserInfo.UserID where");
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


        ///<summary>
        ///根据deviceID channel 获取监控点信息
        ///</summary>
        ///

        public DataSet GetCameroInfoByDeviceIDChannel(int deviceID, int channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select camera_id,name,record_location_set from camera_info");
            strSql.Append(" where device_id='" + deviceID + "' and channel_no='" + channel + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
    }
}
