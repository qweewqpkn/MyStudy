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

            private ABRequest mABRequest = new ABRequest();
            private Action<AssetBundle> mSuccess;
            private Action mError;
            private List<ABRequest> mRequestList = new List<ABRequest>();
            public AssetBundle AB { get; set; }
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
                mABRequest.Load(mABName, mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if (AB != null)
                {
                    mLoadStatus = ABLoadStatus.eLoaded;
                    if (mSuccess != null)
                    {
                        mSuccess(AB);
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

            public void CompleteRequest(AssetBundle ab)
            {
                AB = ab;
                RefCount += mRequestList.Count;
                for (int i = 0; i < mRequestList.Count; i++)
                {
                    mRequestList[i].AddLoadABNum();
                }
            }
        }
    }
}
