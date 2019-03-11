using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
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
            eRelease,
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
    
    
        public HAssetBundle()
        {
        }
    
        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
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
