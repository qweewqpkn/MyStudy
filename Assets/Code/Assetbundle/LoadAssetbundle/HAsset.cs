using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAsset<T> : HRes where T : UnityEngine.Object
        {
            private Action<T> mSuccess;
            private Action mError;

            public HAsset(string abName, string assetName, Action<T> success, Action error)
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
                T asset = request.GetAssets<T>(mAssetName);
                if (typeof(T) == typeof(GameObject))
                {
                    asset = GameObject.Instantiate(asset);
                }

                if (asset != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(asset);
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
