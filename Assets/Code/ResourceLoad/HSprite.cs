﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HSprite : HRes
    {
        private Dictionary<string, Sprite> mSpriteDict = new Dictionary<string, Sprite>();

        public HSprite()
        {
        }

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            Sprite sprite = null;
            if(mSpriteDict.Count == 0)
            {
                AssetRequest assetRequest = new AssetRequest(ab, "", true);
                yield return assetRequest;
                UnityEngine.Object[] objs = assetRequest.GetAssets();
                if(objs != null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (!mSpriteDict.ContainsKey(objs[i].name))
                        {
                            mSpriteDict.Add(objs[i].name, objs[i] as Sprite);
                        }
                    }
                }
            }

            if(mSpriteDict.ContainsKey(assetName))
            {
                sprite = mSpriteDict[assetName];
            }

            if (sprite != null)
            {
                if (success != null)
                {
                    success(sprite as T);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }

        public override void Release()
        {
            base.Release();
            mSpriteDict.Clear();
        }
    }
}
