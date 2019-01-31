using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HManifest : HRes
        {
            private ABRequest mABRequest = new ABRequest();
            private AssetRequest mAssetRequest;
            private Action<AssetBundleManifest> mSuccess;
            private Action mError;
            private AssetBundleManifest mManifest;

            public HManifest(string abName, string assetName) : base(abName, assetName)
            {
            }

            //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
            public override void Load(Action<AssetBundleManifest> success, Action error)
            {
                mABRequest.Load(mABName, mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if (mManifest == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.mainAB, mAssetName);
                    yield return mAssetRequest;
                    mManifest = mAssetRequest.GetAssets<AssetBundleManifest>(mAssetName);
                }

                if (mManifest != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mManifest);
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
