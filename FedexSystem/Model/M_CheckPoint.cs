using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
 public   class M_CheckPoint
    {
        public M_CheckPoint()
        {
        }

        #region  属性

        private int _CPID;

        public int CPID
        {
            get { return _CPID; }
            set { _CPID = value; }
        }
        private string _CPMemo;

        public string CPMemo
        {
            get { return _CPMemo; }
            set { _CPMemo = value; }
        }
        private long _RightValue;

        public long RightValue
        {
            get { return _RightValue; }
            set { _RightValue = value; }
        }
        private int _CurUserID;

        public int CurUserID
        {
            get { return _CurUserID; }
            set { _CurUserID = value; }
        }
        private int _DelFlag;

        public int DelFlag
        {
            get { return _DelFlag; }
            set { _DelFlag = value; }
        }


       
        #endregion
    }
}
