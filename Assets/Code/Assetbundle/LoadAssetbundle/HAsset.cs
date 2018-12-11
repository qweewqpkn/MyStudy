using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAsset<T> : HRes where T : UnityEngine.Object
        {
            private ABAssetLoadRequest mRequest;

            public HAsset(string abName, string assetName, Action<T> success, Action error) : base(abName, assetName)
            {
                mRequest = new ABAssetLoadRequest(abName, mAssetName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load<T>(success, error));
            }

            public override IEnumerator Load<T1>(Action<T1> success, Action error)
            {
                yield return mRequest;
                T1 asset = mRequest.GetAssets<T1>(mAssetName);
                if (typeof(T1) == typeof(GameObject))
                {
                    asset = GameObject.Instantiate(asset);
                }

                if (asset != null)
                {
                    if (success != null)
                    {
                        success(asset);
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
