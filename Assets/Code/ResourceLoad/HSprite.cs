using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HSprite : HRes
        {
            private Dictionary<string, Sprite> mSpriteDict = new Dictionary<string, Sprite>();

            public HSprite(string abName) : base(abName, "", AssetType.eSprite)
            {
            }

            public override void Load<T>(string assetName, Action<T> success, Action error)
            {
                base.Load(assetName, success, error);
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, success, error));
            }

            private IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return abRequest;

                Sprite sprite = null;
                if(mSpriteDict.Count == 0)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, "", true);
                    yield return assetRequest;
                    UnityEngine.Object[] objs = assetRequest.GetAssets();
                    for(int i = 0; i < objs.Length; i++)
                    {
                        if(!mSpriteDict.ContainsKey(objs[i].name))
                        {
                            mSpriteDict.Add(objs[i].name, objs[i] as Sprite);
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
}
