
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HTexture : HRes
        {
            private Texture mTexture;

            public HTexture(string abName, string assetName) : base(abName, assetName, AssetType.eTexture)
            {
            }

            public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
            {
                base.Load(abName, assetName, success, error);
                ABRequest abRequest = new ABRequest();
                abRequest.Load(abName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load<T>(ABRequest abRequest, Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return abRequest;
                if(mTexture == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mTexture = assetRequest.GetAssets<Texture>(mAssetName);
                }

                if (mTexture != null)
                {
                    if (success != null)
                    {
                        success(mTexture as T);
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
                mTexture = null;
            }
        }
    }
}
