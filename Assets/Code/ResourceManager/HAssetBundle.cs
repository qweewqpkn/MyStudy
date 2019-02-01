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

            private List<ABRequest> mRequestList = new List<ABRequest>();
            public AssetBundle AB
            {
                get;
                set;
            }

            public ABLoadStatus LoadStatus
            {
                get;
                set;
            }

            public int RefCount
            {
                get;
                set;
            }

            public HAssetBundle(string abName) : base(abName, "")
            {
            }

            public override void Load(Action<AssetBundle> success, Action error)
            {
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<AssetBundle> success, Action error)
            {
                yield return abRequest;
                if (AB != null)
                {
                    if (success != null)
                    {
                        success(AB);
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
                    mRequestList[i].AddLoadNum();
                }
            }
        }
    }
}
