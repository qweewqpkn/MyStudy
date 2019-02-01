
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HPrefab : HRes
        {
            private GameObject mPrefab;

            public HPrefab(string abName, string assestName) : base(abName, assestName)
            {
            }

            public override void Load(Action<GameObject> success, Action error)
            {
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<GameObject> success, Action error)
            {
                yield return abRequest;

                if(mPrefab == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName, false);
                    yield return assetRequest;
                    mPrefab = assetRequest.GetAssets<GameObject>(mAssetName);
                }

                if(mPrefab != null)
                {
                    GameObject newObj = GameObject.Instantiate(mPrefab);
                    newObj.AddComponent<PrefabAutoDestory>();
                    if(success != null)
                    {
                        success(newObj);
                    }
                }
                else
                {
                    if(error != null)
                    {
                        error();
                    }
                }
            }
        }
    }
}
