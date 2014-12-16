using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
   public class M_XRaySanLog
    {
       public M_XRaySanLog()
       {
       }
       #region Model

       private int _clid;
       private int _cargoID;
       private string _cargoBC;
       private string _cargoName;
       private int _scanUserID;
       private int _scanCP;
       private DateTime _scanTime;

       

       /// <summary>
       /// 货物索引
       /// </summary>
       public int Clid
       {
           get { return _clid; }
           set { _clid = value; }
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
       /// 操作员ID
       /// </summary>
       public int ScanUserID
       {
           get { return _scanUserID; }
           set { _scanUserID = value; }
       }
       /// <summary>
       /// 岗位识别号
       /// </summary>
       public int ScanCP
       {
           get { return _scanCP; }
           set { _scanCP = value; }
       }
       /// <summary>
       /// 扫描时间
       /// </summary>
       public DateTime ScanTime
       {
           get { return _scanTime; }
           set { _scanTime = value; }
       }
       #endregion

    }
}
