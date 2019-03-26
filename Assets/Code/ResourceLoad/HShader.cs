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
            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Shader);
                };
            }

            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
            res.StartLoad(assetName, false, tCallBack);
        }

        public static Shader Load(string abName, string assetName)
        {
            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
            res.StartLoad(assetName, true, null);
            return res.AssetObj as Shader;
        }
    }
}
