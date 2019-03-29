using System.Collections.Generic;
namespace BinaryConfig
{
    class AchieveConfig : IBinaryData
    {
        /// <summary>
        /// 成就ID
        /// <summary>
        private int mid;
        public int id
        {
            get{ return mid;}
        }
        /// <summary>
        /// 章节ID
        /// <summary>
        private int mchapter;
        public int chapter
        {
            get{ return mchapter;}
        }
        /// <summary>
        /// 成就名称
        /// <summary>
        private string machieve_name;
        public string achieve_name
        {
            get{ return machieve_name;}
        }
        /// <summary>
        /// 成就描述
        /// <summary>
        private string machieve_detail;
        public string achieve_detail
        {
            get{ return machieve_detail;}
        }
        /// <summary>
        /// 收集成就图
        /// <summary>
        private string mpic;
        public string pic
        {
            get{ return mpic;}
        }
        /// <summary>
        /// 目标
        /// <summary>
        private int mfinish_num;
        public int finish_num
        {
            get{ return mfinish_num;}
        }
        /// <summary>
        /// 伙伴ID列表
        /// <summary>
        private int[] mpartner_list;
        public int[] partner_list
        {
            get{ return mpartner_list;}
        }
        /// <summary>
        /// 奖励ID
        /// <summary>
        private int mreward;
        public int reward
        {
            get{ return mreward;}
        }
        /// <summary>
        /// 排序ID
        /// <summary>
        private int msort_id;
        public int sort_id
        {
            get{ return msort_id;}
        }
        /// <summary>
        /// as11
        /// <summary>
        private Dictionary<string,string> mtest;
        public Dictionary<string,string> test
        {
            get{ return mtest;}
        }
        /// <summary>
        /// as11
        /// <summary>
        private string[] mtest1;
        public string[] test1
        {
            get{ return mtest1;}
        }
        public void Init(BinaryConfigRow row)
        {
            mid = row.GetFieldInfo(0).GetInt();
            mchapter = row.GetFieldInfo(1).GetInt();
            machieve_name = row.GetFieldInfo(2).GetString();
            machieve_detail = row.GetFieldInfo(3).GetString();
            mpic = row.GetFieldInfo(4).GetString();
            mfinish_num = row.GetFieldInfo(5).GetInt();
            mpartner_list = row.GetFieldInfo(6).GetIntList();
            mreward = row.GetFieldInfo(7).GetInt();
            msort_id = row.GetFieldInfo(8).GetInt();
            mtest = row.GetFieldInfo(9).GetStringStringDic();
            mtest1 = row.GetFieldInfo(10).GetStringList();
        }
    }
}

