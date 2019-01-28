using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAssetBundle : HRes
        {
            private ABRequest mRequest;
            private Action<AssetBundle> mSuccess;
            private Action mError;

            public HAssetBundle(string abName) : base(abName, "")
            {
            }

            public override void Load(Action<AssetBundle> success, Action error)
            {
                mRequest = new ABRequest(mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mRequest;
                AssetBundle ab = mRequest.GetAB();
                if (ab != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(ab);
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
