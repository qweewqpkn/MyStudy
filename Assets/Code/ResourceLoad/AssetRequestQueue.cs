using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class AssetRequestQueue
    {
        public class RequestData
        {
            public HRes mRes;
            public string mABName;
            public string mAssetName;
            public Action<UnityEngine.Object> mSuccess;
            public Action mError;

            public void Load()
            {
                mRes.Load(mABName, mAssetName, mSuccess, mError);
            }
        }

        private int MAX_LOAD_REQUEST = 5;
        private int mCurLoadCount = 0;
        private Queue<RequestData> mRequestQueue = new Queue<RequestData>();

        public void AddReuqest<T>(HRes res, string abName, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            RequestData request = new RequestData();
            mRequestQueue.Enqueue(request);
            request.mRes = res;
            request.mABName = abName;
            request.mAssetName = assetName;
            request.mError = error;
            request.mSuccess = (obj)=>
            {
                success((T)obj);
                LoadComplete();
            };
        }

        public void LoadComplete() 
        {
            mCurLoadCount--;
        }

        public void Update()
        {
            if(mRequestQueue.Count > 0)
            {
                if(mCurLoadCount < MAX_LOAD_REQUEST)
                {
                    RequestData request = mRequestQueue.Dequeue();
                    mCurLoadCount++;
                    request.Load();
                }
            }
        }

        //todo 删除单个，队列不好处理
        public void Release(string abName, string assetName)
        {
            if(mRequestQueue.Count > 0)
            {
                RequestData[] requestList = mRequestQueue.ToArray();
                for (int i = 0; i < requestList.Length; i++)
                {

                }
            }
        }

        public void ReleaseAll()
        {
            if(mRequestQueue != null)
            {
                mRequestQueue.Clear();
            }
        }
    }
}
