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


            public HAssetBundle(string abName) : base(abName, "")
            {
            }

            public override void Load<T>(Action<T> success, Action error)
            {
                Action<AssetBundle> complete = (ab) =>
                {
                    success(ab as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, complete, error));
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

            public override void Release()
            {
                //base.Release();
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
