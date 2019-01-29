using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        public class HAssetBundle : HRes
        {
            public enum ABLoadStatus
            {
                eNone,
                eLoading,
                eLoaded,
                eLoadError,
            }

            private ABRequest mRequest;
            private Action<AssetBundle> mSuccess;
            private Action mError;
            private List<ABRequest> mRequestList = new List<ABRequest>();
            private AssetBundle mAB;
            public AssetBundle AB
            {
                get
                {
                    return mAB;
                }
            }

            private ABLoadStatus mLoadStatus = ABLoadStatus.eNone;
            public ABLoadStatus LoadStatus
            {
                get
                {
                    return mLoadStatus;
                }

                set
                {
                    mLoadStatus = value;
                }
            }

            private int mRefCount = 0;
            public int RefCount
            {
                get
                {
                    return mRefCount;
                }

                set
                {
                    mRefCount = value;
                }
            }

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
                mAB = mRequest.GetAB();
                if (mAB != null)
                {
                    mLoadStatus = ABLoadStatus.eLoaded;
                    if (mSuccess != null)
                    {
                        mSuccess(mAB);
                    }
                }
                else
                {
                    mLoadStatus = ABLoadStatus.eLoadError;
                    if (mError != null)
                    {
                        mError();
                    }
                }

                mSuccess = null;
                mError = null;
            }

            public void AddRequest(ABRequest request)
            {
                mRequestList.Add(request);
            }

            public void CompleteRequest()
            {
                RefCount += mRequestList.Count;
                for (int i = 0; i < mRequestList.Count; i++)
                {
                    mRequestList[i].AddLoadABNum();
                }
            }
        }
    }
}
