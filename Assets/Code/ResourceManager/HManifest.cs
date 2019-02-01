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
            private AssetBundleManifest mManifest;

            public HManifest(string abName, string assetName) : base(abName, assetName)
            {
            }

            //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
            public override void Load(Action<AssetBundleManifest> success, Action error)
            {
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<AssetBundleManifest> success, Action error)
            {
                yield return abRequest;
                if (mManifest == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mManifest = assetRequest.GetAssets<AssetBundleManifest>(mAssetName);
                }

                if (mManifest != null)
                {
                    if (success != null)
                    {
                        success(mManifest);
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
        }
    }
}
