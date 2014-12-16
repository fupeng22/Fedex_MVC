using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_Department
    {
        public Int32 d_ID
        {
            get;
            set;
        }

        public string DepId
        {
            get;
            set;
        }

        public string ParentDepId
        {
            get;
            set;
        }

        public Int32 DepOrder
        {
            get;
            set;
        }

        public string DepName
        {
            get;
            set;
        }

        public string DepFullName
        {
            get;
            set;
        }

        public Int32  DelFlag
        {
            get;
            set;
        }

        public string mMemo
        {
            get;
            set;
        }
    }
}
