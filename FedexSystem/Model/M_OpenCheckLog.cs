using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
   public class M_OpenCheckLog
    {
       public M_OpenCheckLog()
       {
       }

        #region Model 属性
       private int _ckid;
       private int _cargoID;
       private string _cargoBC;
       private string _cargoName;
       private int _checkUserID;
       private string _checkCP;
       private int _checkResult;
       private DateTime _checkBeginTime;
       private DateTime _checkEndTime;

      

       /// <summary>
       /// 开箱录像ID
       /// </summary>
       public int Ckid
       {
           get { return _ckid; }
           set { _ckid = value; }
       }
       /// <summary>
       /// 货物ID
       /// </summary>
       public int CargoID
       {
           get { return _cargoID; }
           set { _cargoID = value; }
       }
       /// <summary>
       /// 货物条码
       /// </summary>
       public string CargoBC
       {
           get { return _cargoBC; }
           set { _cargoBC = value; }
       }
       /// <summary>
       /// 货物名称
       /// </summary>
       public string CargoName
       {
           get { return _cargoName; }
           set { _cargoName = value; }
       }
       /// <summary>
       /// 开箱岗位识别号
       /// </summary>
       public string CheckCP
       {
           get { return _checkCP; }
           set { _checkCP = value; }
       }
       /// <summary>
       /// 开箱操作员ID
       /// </summary>
       public int CheckUserID
       {
           get { return _checkUserID; }
           set { _checkUserID = value; }
       }
       /// <summary>
       /// 开箱扫描结果
       /// </summary>
       public int CheckResult
       {
           get { return _checkResult; }
           set { _checkResult = value; }
       }
       /// <summary>
       /// 开箱检查开始时间
       /// </summary>
       public DateTime CheckBeginTime
       {
           get { return _checkBeginTime; }
           set { _checkBeginTime = value; }
       }
       /// <summary>
       /// 开箱检查结束时间
       /// </summary>
       public DateTime CheckEndTime
       {
           get { return _checkEndTime; }
           set { _checkEndTime = value; }
       }

        #endregion
    }
}
