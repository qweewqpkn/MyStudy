
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    class HTexture : HRes
    {
        public HTexture() 
        {
        }

        public static void Load(string abName, string assetName, Action<Texture> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Texture);
            };
            LoadRes<HTexture>(abName, assetName, tCallBack);
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
