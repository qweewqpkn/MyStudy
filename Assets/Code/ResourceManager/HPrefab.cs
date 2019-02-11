
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

            public HPrefab(string abName, string assestName) : base(abName, assestName, AssetType.ePrefab)
            {
            }

            public override void Load<T>(Action<T> success, Action error)
            {
                base.Load(success, error);
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load<T>(ABRequest abRequest, Action<T> success, Action error) where T : UnityEngine.Object
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
                    PrefabAutoDestory audoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                    audoDestroy.mABName = mABName;
                    if (success != null)
                    {
                        success(newObj as T);
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

            public override void Release()
            {
                base.Release();
                mPrefab = null;
            }
        }
    }
}
