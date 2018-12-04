using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAsset<T> : HRes where T : UnityEngine.Object
        {
            private string mABName;
            private string mAssetName;
            private Action<T> mSuccess;
            private Action mError;
            private ABLoadRequest mRequest;

            public HAsset(string abName, string assetName, Action<T> success, Action error)
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
                T asset = mRequest.GetAssets<T>(mAssetName);
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
