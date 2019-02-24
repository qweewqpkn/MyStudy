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
            //ab被引用的次数
            public int RefCount
            {
                get;
                set;
            }

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


            public HAssetBundle(string abName) : base(abName, "", AssetType.eAB)
            {
            }

            public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
            {
                base.Load(abName, assetName, success, error);
                ABRequest abRequest = new ABRequest();
                abRequest.Load(abName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load<T>(ABRequest abRequest, Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return abRequest;
                if (AB != null)
                {
                    if (success != null)
                    {
                        success(AB as T);
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
                base.Release();
                AB = null;
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
