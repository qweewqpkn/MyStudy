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
            private Action<AssetBundleManifest> mSuccess;
            private Action mError;

            public HManifest(string abName, string assetName, Action<AssetBundleManifest> success, Action error)
            {
                mABName = abName;
                mAssetName = assetName;
                mSuccess = success;
                mError = error;
                ResourceManager.Instance.StartCoroutine(Load(new ABAssetLoadRequest(this)));
            }

            IEnumerator Load(ABAssetLoadRequest request)
            {
                yield return request;
                AssetBundleManifest mainfest = request.GetAssets<AssetBundleManifest>(mAssetName);
                if (mainfest != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mainfest);
                    }
                }
                else
                {
                    if (mError != null)
                    {
                        mError();
                    }
                }
            }
        }
    }
}
