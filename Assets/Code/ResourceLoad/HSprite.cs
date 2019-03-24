using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HSprite : HRes
    {
        public HSprite()
        {
        }

        public static void Load(string abName, string assetName, Action<Sprite> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Sprite);
            };
            HSprite res = Get<HSprite>(abName, assetName, tCallBack);
            res.StartLoad();
        }

        protected override IEnumerator CoLoad()
        {
            ABDep = Get<HAssetBundle>(ABName, "", null);

            ABRequest abRequest = new ABRequest();
            yield return abRequest.Load(ABDep);

            AssetRequest assetRequest = new AssetRequest();
            yield return assetRequest.Load(ABDep.AB, AssetName, true);

            OnCompleted(assetRequest.AssetObj);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }
    }
}
