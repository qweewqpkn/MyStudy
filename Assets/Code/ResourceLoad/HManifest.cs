using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HManifest : HRes
    {
        private AssetBundleManifest mManifest;

        public HManifest()
        {
        }

        //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
        public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
        {
            base.Load(abName, assetName, success, error);
            ABRequest abRequest = new ABRequest();
            abRequest.Load(abName, mAllABList);
            ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, success, error));
        }

        private IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            yield return abRequest;
            if (mManifest == null)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, assetName);
                yield return assetRequest;
                mManifest = assetRequest.GetAssets<AssetBundleManifest>(assetName);
            }

            if (mManifest != null)
            {
                if (success != null)
                {
                    success(mManifest as T);
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
            mManifest = null;
        }
    }
}
