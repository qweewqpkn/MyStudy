using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HShader : HRes
    {
        public HShader()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Shader> callback)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null);
                }
                return;
            }

            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Shader);
                };
            }

            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        public static Shader Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as Shader;
        }
    }
}
