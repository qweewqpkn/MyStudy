using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 服务器版本信息
    /// </summary>
    public class RemoteVersionData : BaseVersionData
    {
        public ResponseProjectManifestData mainifestData;

        public void Init(string jsonStr)
        {
            mainifestData = JsonUtility.FromJson<ResponseProjectManifestData>(jsonStr);
            Version = mainifestData.version;
        }

        public override void Clear()
        {
            base.Clear();
            mainifestData = null;
        }
    }
}
