using System;
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

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            TextAsset textAsset = null;
            if (mLuaDict.Count == 0)
            {
                AssetRequest assetRequest = new AssetRequest(ab, "", true);
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
