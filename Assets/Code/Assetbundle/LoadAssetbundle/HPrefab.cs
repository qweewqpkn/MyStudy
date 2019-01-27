
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HPrefab : HRes
        {
            private ABRequest mABRequest;
            private AssetRequest mAssetRequest;
            private Action<GameObject> mSuccess;
            private Action mError;
            private GameObject mPrefab;

            public HPrefab(string abName, string assestName) : base(abName, assestName)
            {
            }

            public override void Load(Action<GameObject> success, Action error)
            {
                mABRequest = new ABRequest(mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;

                if(mPrefab == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.GetAB(), mAssetName, true);
                    yield return mAssetRequest;
                    mPrefab = mAssetRequest.GetAssets<GameObject>(mAssetName);
                }


                if(mPrefab != null)
                {
                    GameObject newObj = GameObject.Instantiate(mPrefab);
                    newObj.AddComponent<PrefabAutoDestory>();
                    if(mSuccess != null)
                    {
                        mSuccess(newObj);
                    }
                }
                else
                {
                    if(mError != null)
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
