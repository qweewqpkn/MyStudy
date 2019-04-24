using System.Collections.Generic;
namespace BinaryConfig
{
    class TestConfig : IBinaryData
    {
        /// <summary>
        /// 测试1
        /// <summary>
        private int mtest1;
        public int test1
        {
            get{ return mtest1;}
        }
        /// <summary>
        /// 测试2
        /// <summary>
        private float mtest2;
        public float test2
        {
            get{ return mtest2;}
        }
        /// <summary>
        /// 测试3
        /// <summary>
        private string mtest3;
        public string test3
        {
            get{ return mtest3;}
        }
        /// <summary>
        /// 测试4
        /// <summary>
        private bool mtest4;
        public bool test4
        {
            get{ return mtest4;}
        }
        /// <summary>
        /// 测试5
        /// <summary>
        private int[] mtest5;
        public int[] test5
        {
            get{ return mtest5;}
        }
        /// <summary>
        /// 测试6
        /// <summary>
        private float[] mtest6;
        public float[] test6
        {
            get{ return mtest6;}
        }
        /// <summary>
        /// 测试7
        /// <summary>
        private string[] mtest7;
        public string[] test7
        {
            get{ return mtest7;}
        }
        /// <summary>
        /// 测试8
        /// <summary>
        private bool[] mtest8;
        public bool[] test8
        {
            get{ return mtest8;}
        }
        /// <summary>
        /// 测试9
        /// <summary>
        private Dictionary<int,int> mtest9;
        public Dictionary<int,int> test9
        {
            get{ return mtest9;}
        }
        /// <summary>
        /// 测试10
        /// <summary>
        private Dictionary<int,string> mtest10;
        public Dictionary<int,string> test10
        {
            get{ return mtest10;}
        }
        /// <summary>
        /// 测试11
        /// <summary>
        private Dictionary<string,int> mtest11;
        public Dictionary<string,int> test11
        {
            get{ return mtest11;}
        }
        /// <summary>
        /// 测试12
        /// <summary>
        private Dictionary<string,string> mtest12;
        public Dictionary<string,string> test12
        {
            get{ return mtest12;}
        }
        public void Init(BinaryConfigRow row)
        {
            mtest1 = row.GetFieldInfo(0).GetInt();
            mtest2 = row.GetFieldInfo(1).GetFloat();
            mtest3 = row.GetFieldInfo(2).GetString();
            mtest4 = row.GetFieldInfo(3).GetBool();
            mtest5 = row.GetFieldInfo(4).GetIntList();
            mtest6 = row.GetFieldInfo(5).GetFloatList();
            mtest7 = row.GetFieldInfo(6).GetStringList();
            mtest8 = row.GetFieldInfo(7).GetBoolList();
            mtest9 = row.GetFieldInfo(8).GetIntIntDic();
            mtest10 = row.GetFieldInfo(9).GetIntStringDic();
            mtest11 = row.GetFieldInfo(10).GetStringIntDic();
            mtest12 = row.GetFieldInfo(11).GetStringStringDic();
        }
    }
}

