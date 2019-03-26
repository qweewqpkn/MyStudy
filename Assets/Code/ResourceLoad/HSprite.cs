using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HSprite : HRes
    {
        private Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

        public HSprite()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Sprite> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Sprite);
                };
            }

            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(assetName, false, tCallBack);
        }

        public static Sprite Load(string abName, string assetName)
        {
            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(assetName, true, null);
            return res.AssetObj as Sprite;
        }

        protected override IEnumerator CoLoad(string assetName, bool isSync, Action<UnityEngine.Object> callback)
        {
            ABDep = Get<HAssetBundle>(ABName, "", AssetType.eAB);

            ABRequest abRequest = new ABRequest();
            abRequest.Load(ABDep, isSync);
            while(!abRequest.IsComplete)
            {
                yield return null;
            }

            if(spriteMap.Count == 0)
            {
                AssetRequest assetRequest = new AssetRequest();
                assetRequest.Load(ABDep.AB, assetName, isSync, true);
                while (!assetRequest.IsComplete)
                {
                    yield return null;
                }

                if (assetRequest.AssetsList != null)
                {
                    for (int i = 0; i < assetRequest.AssetsList.Length; i++)
                    {
                        spriteMap[assetRequest.AssetsList[i].name.ToLower()] = assetRequest.AssetsList[i] as Sprite;
                    }
                }
            }

            if(spriteMap.ContainsKey(assetName))
            {
                OnCompleted(spriteMap[assetName], callback);
            }
            else
            {
                OnCompleted(null, callback);
            }
        }
    }
}
