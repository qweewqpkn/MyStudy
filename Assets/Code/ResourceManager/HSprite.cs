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

            public HSprite(string abName) : base(abName, "*", AssetType.eSprite)
            {
            }

            public override void Load<T>(string assetName, Action<T> success, Action error)
            {
                base.Load(assetName, success, error);
                Action<Sprite> complete = (sprite) =>
                {
                    success(sprite as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, complete, error));
            }

            private IEnumerator Load(ABRequest abRequest, string assetName, Action<Sprite> success, Action error)
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
                        success(sprite);
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
                mSpriteDict = null;
            }
        }
    }
}
