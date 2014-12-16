using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// 实体类Users 。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
   public class M_Users
    {
       public M_Users()
       {


       }
       #region Model
       private int _userID;
       private string _userNumber;
       private string _userName;
       private string _userPSW;
       private int _sex;


       private long _rightClass;


       private int _userLocked;

  
       private int _delFlag;

      


       /// <summary>
       /// 用户索引
       /// </summary>
       public int UserID
       {
           get { return _userID; }
           set { _userID = value; }
       }
       /// <summary>
       /// 用户号
       /// </summary>
       public string UserNumber
       {
           get { return _userNumber; }
           set { _userNumber = value; }
       }

       /// <summary>
       /// 用户名
       /// </summary>
        public string UserName
       {
           get { return _userName; }
           set { _userName = value; }
       }
       /// <summary>
       /// 密码
       /// </summary>
        public string UserPSW
        {
            get { return _userPSW; }
            set { _userPSW = value; }
        }

       /// <summary>
       /// 性别
       /// </summary>
        public int Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }
       /// <summary>
       /// 权限
       /// </summary>

        public long RightClass
        {
            get { return _rightClass; }
            set { _rightClass = value; }
        }
        /// <summary>
        /// 用户锁定
        /// </summary>
        /// 

        public string Department
        {
            get;
            set;
        }

        public int UserLocked
        {
            get { return _userLocked; }
            set { _userLocked = value; }
        }
       ///<summary>
       ///用户删除标志
       ///</summary>
        public int DelFlag
        {
            get { return _delFlag; }
            set { _delFlag = value; }
        }
       #endregion Model
    }
}
