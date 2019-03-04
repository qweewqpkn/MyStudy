using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HText : HRes
    {
        private Dictionary<string, TextAsset> mTextAssetDict = new Dictionary<string, TextAsset>();
    
        public HText()
        {
        }
    
        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
        {
            yield return abRequest;

            TextAsset textAsset = null;
            if (mTextAssetDict.Count == 0)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, "", true);
                yield return assetRequest;
                UnityEngine.Object[] objs = assetRequest.GetAssets();
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!mTextAssetDict.ContainsKey(objs[i].name))
                    {
                        mTextAssetDict.Add(objs[i].name, objs[i] as TextAsset);
                    }
                }
            }

            if (mTextAssetDict.ContainsKey(assetName))
            {
                textAsset = mTextAssetDict[assetName];
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
            mTextAssetDict.Clear();
            mTextAssetDict = null;
        }
    }
}
