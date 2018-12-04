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
            private string mABName;
            private string mAssetName;
            private Action<AssetBundleManifest> mSuccess;
            private Action mError;
            private ABLoadRequest mRequest;

            public HManifest(string abName, string assetName, Action<AssetBundleManifest> success, Action error)
            {
                mABName = abName;
                mAssetName = assetName;
                mSuccess = success;
                mError = error;
                mRequest = new ABLoadRequest(abName, assetName);
                ResourceManager.Instance.StartCoroutine(Load());
            }

            IEnumerator Load()
            {
                yield return mRequest;
                AssetBundleManifest mainfest = mRequest.GetAssets<AssetBundleManifest>(mAssetName);
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
