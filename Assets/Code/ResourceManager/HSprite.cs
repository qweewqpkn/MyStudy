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
            private Sprite mSprite;

            public HSprite(string abName, string assestName) : base(abName, assestName)
            {
            }

            public override void Load(Action<Sprite> success, Action error)
            {
                mABRequest.Load(mABName, mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if(mSprite == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.mainAB, mAssetName, false);
                    yield return mAssetRequest;
                    mSprite = mAssetRequest.GetAssets<Sprite>(mAssetName);
                }
                if (mSprite != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mSprite);
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
