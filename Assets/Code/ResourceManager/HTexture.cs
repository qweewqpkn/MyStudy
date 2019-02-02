
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

            public override void Load<T>(Action<T> success, Action error)
            {
                base.Load(success, error);
                Action<Texture> complete = (ab) =>
                {
                    success(ab as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, complete, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<Texture> success, Action error)
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
                        success(mTexture);
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
