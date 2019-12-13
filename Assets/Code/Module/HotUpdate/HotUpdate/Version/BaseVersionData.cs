using UnityEngine;
using System.Collections.Generic;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 版本号定义
    /// 格式为：[Major].[Minor].[Revision]
    ///     Major: 主版本号, 定义为包的版本号, 变化则需要更新整包
    ///     Minor: 子版本号, 定义为兼容性版本号
    ///     Revision: 修正版本号, 定义为热更新版本号
    /// 与公司统一的热更新业务支撑对接，参考：
    /// http://wiki.info/pages/viewpage.action?pageId=4128794
    /// </summary>
    public class BaseVersionData
    {
        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                if (_version != null)
                {
                    string[] arr = _version.Split('.');
                    _major = int.Parse(arr[0]);
                    _minor = int.Parse(arr[1]);
                    _revision = int.Parse(arr[2]);
                }
                else
                {
                    _major = 0;
                    _minor = 0;
                    _revision = 0;
                }
            }
        }

        private int _major;
        public int Major { get { return _major; } }

        // Unity项目暂时不处理兼容性更新
        private int _minor;
        public int Minor { get { return _minor; } }

        private int _revision;
        public int Revision { get { return _revision; } }

        public BaseVersionData()
        {
            this.Version = "0.0.0";
        }

        public virtual void Clear()
        {
            Version = null;
        }

        public bool Equals(BaseVersionData other)
        {
            return Version == other.Version;
        }
    }
}
