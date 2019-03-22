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

        public static void Load(string abName, string assetName, Action<Shader> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Shader);
            };
            LoadRes<HShader>(abName, assetName, tCallBack);
        }

        protected override void StartLoad(params object[] datas)
        {
            HAB = HAssetBundle.Load(ABName, (ab) =>
            {
                ResourceManager.Instance.StartCoroutine(CoLoad(ab));
            }, false);
        }

        IEnumerator CoLoad(AssetBundle ab)
        {
            AssetRequest assetRequest = new AssetRequest();
            yield return assetRequest.Load(ab, AssetName);
            OnCompleted(assetRequest.AssetObj);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }
    }
}
