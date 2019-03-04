﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HLua : HRes
    {
        private Dictionary<string, TextAsset> mLuaDict = new Dictionary<string, TextAsset>();

        public HLua() 
        {
        }

        //同步加载
        public override T LoadSync<T>(string abName, string assetName)
        {
            base.LoadSync<T>(abName, assetName);
            assetName = assetName.ToLower();
            ABRequestSync abRequestSync = new ABRequestSync();
            AssetBundle ab = abRequestSync.Load(mABName, mAllABList, AssetType.eLua);

            if (mLuaDict.Count == 0)
            {
                AssetRequestSync assetRequestSync = new AssetRequestSync();
                UnityEngine.Object[] objs = assetRequestSync.LoadAll(ab);
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!mLuaDict.ContainsKey(objs[i].name))
                    {
                        mLuaDict.Add(objs[i].name.ToLower(), objs[i] as TextAsset);
                    }
                }
            }

            TextAsset textAsset = null;
            if (mLuaDict.ContainsKey(assetName))
            {
                textAsset = mLuaDict[assetName];
            }

            return textAsset as T;
        }


        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
        {
            yield return abRequest;

            TextAsset textAsset = null;
            if (mLuaDict.Count == 0)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, "", true);
                yield return assetRequest;
                UnityEngine.Object[] objs = assetRequest.GetAssets();
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!mLuaDict.ContainsKey(objs[i].name))
                    {
                        mLuaDict.Add(objs[i].name, objs[i] as TextAsset);
                    }
                }
            }

            if (mLuaDict.ContainsKey(assetName))
            {
                textAsset = mLuaDict[assetName];
            }

            if (textAsset != null)
            {
                if (success != null)
                {
                    success(textAsset as T);
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
            mLuaDict.Clear();
            mLuaDict = null;
        }
    }
}
