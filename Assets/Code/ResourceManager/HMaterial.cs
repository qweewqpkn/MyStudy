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
        class HMaterial : HRes
        {
            private ABRequest mABRequest;
            private AssetRequest mAssetRequest;
            private Action<Material> mSuccess;
            private Action mError;
            private Material mMaterial;

            public HMaterial(string abName, string assestName) : base(abName, assestName)
            {

            }

            //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
            public override void Load(Action<Material> success, Action error)
            {
                mABRequest = new ABRequest(mAllABList);
                mSuccess += success; //同时加载该资源多次，那么回调也要累加
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if (mMaterial == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.GetAB(), mAssetName);
                    yield return mAssetRequest;
                    mMaterial = mAssetRequest.GetAssets<Material>(mAssetName);
                }

                if (mMaterial != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mMaterial);
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
