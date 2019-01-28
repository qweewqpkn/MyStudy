
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HTexture : HRes
        {
            private ABRequest mABRequest;
            private AssetRequest mAssetRequest;
            private Action<Texture> mSuccess;
            private Action mError;
            private Texture mTexture;

            public HTexture(string abName, string assestName) : base(abName, assestName)
            {
            }

            //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
            public override void Load(Action<Texture> success, Action error)
            {
                mABRequest = new ABRequest(mAllABList);
                mSuccess += success; //同时加载该资源多次，那么回调也要累加
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if(mTexture == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.GetAB(), mAssetName);
                    yield return mAssetRequest;
                    mTexture = mAssetRequest.GetAssets<Texture>(mAssetName);
                }

                if (mTexture != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mTexture);
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
