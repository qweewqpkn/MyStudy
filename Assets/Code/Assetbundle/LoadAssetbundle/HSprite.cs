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
            private ABRequest mABRequest;
            private AssetRequest mAssetRequest;
            private Action<Sprite> mSuccess;
            private Action mError;
            private Dictionary<string, Sprite> mSpriteDict = new Dictionary<string, Sprite>();       

            public HSprite(string abName, string assestName) : base(abName, assestName)
            {

            }

            public override void Load(Action<Sprite> success, Action error)
            {
                mABRequest = new ABRequest(mAllABList);
                mSuccess += success; 
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;

                Sprite sprite = null;
                if (mSpriteDict == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.GetAB(), mAssetName, true);
                    yield return mAssetRequest;
                    UnityEngine.Object[] sprites = mAssetRequest.GetAssets();
                    if (sprites != null)
                    {
                        for (int i = 0; i < sprites.Length; i++)
                        {
                            mSpriteDict[sprites[i].name] = sprites[i] as Sprite;
                        }
                    }
                    else
                    {
                        Debug.LogError("HSptire sprites is null");
                    }
                }

                if(mSpriteDict.ContainsKey(mAssetName))
                {
                    sprite = mSpriteDict[mAssetName];
                }

                if (sprite != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(sprite);
                    }
                }
                else
                {
                    if (mError != null)
                    {
                        mError();
                    }
                }

                mSuccess = null;
                mError = null;
            }
        }
    }
}
