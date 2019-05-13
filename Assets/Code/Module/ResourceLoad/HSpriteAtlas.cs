using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HSpriteAtlas : HRes
    {
        public HSpriteAtlas()
        {
        }

        public static void LoadAsync(string abName, Action<List<Sprite>> callback)
        {
            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null);
                }
                return;
            }

            Action<AssetLoadData> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (data) =>
                {
                    if(data != null && data.mAssets != null)
                    {
                        List<Sprite> spriteList = new List<Sprite>();
                        spriteList = data.mAssets.ConvertAll((item) => { return item as Sprite; });
                        callback(spriteList);
                    }
                    else
                    {
                        callback(null);
                    }
                };
            }

            HSpriteAtlas res = Get<HSpriteAtlas>(abName, "*", AssetType.eSpriteAtlas);
            res.StartLoad(false, true, false, tCallBack);
        }

        public static AsyncRequest LoadCoRequest(string abName)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, (obj) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Assets = obj.ConvertAll((item)=> { return item as UnityEngine.Object; });
            });
        
            return request;
        }

        public static List<Sprite> Load(string abName)
        {
            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HSpriteAtlas res = Get<HSpriteAtlas>(abName, "*", AssetType.eSpriteAtlas);
            res.StartLoad(true, true, false, null);
            List<Sprite> spriteList = new List<Sprite>();
            for (int i = 0; i < res.AssetData.mAssets.Count; i++)
            {
                spriteList.Add(res.AssetData.mAssets[i] as Sprite);
            }

            return spriteList;
        }
    }
}
