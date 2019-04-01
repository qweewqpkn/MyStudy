using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABRequest
{
    List<HAssetBundle> mABLoadList = new List<HAssetBundle>();
    static Dictionary<string, AssetBundleCreateRequest> mRequestMap = new Dictionary<string, AssetBundleCreateRequest>();
    private AssetType mAssetType; //目标资源类型
    private bool mIsSync; //是否是同步请求

    public bool IsComplete
    {
        get;
        private set;
    }

    public ABRequest()
    {
        IsComplete = false;
    }

    public void Load(HAssetBundle ab, bool isSync, AssetType assetType)
    {
        mAssetType = assetType;
        mIsSync = isSync;
        ResourceManager.Instance.StartCoroutine(CoLoad(ab));
    }

    public IEnumerator CoLoad(HAssetBundle ab)
    {
        if (ab == null)
        {
            Debug.LogError("ABRequest HAssetbundle is Null");
            yield break;
        }

        //引用计数
        mABLoadList.Clear();
        mABLoadList.Add(ab);
        for (int i = 0; i < ab.DepList.Count; i++)
        {
            HAssetBundle depAB = HRes.Get<HAssetBundle>(ab.DepList[i], "", AssetType.eAB);
            mABLoadList.Add(depAB);
        }

        //开启所有加载
        for (int i = 0; i < mABLoadList.Count; i++)
        {
            ResourceManager.Instance.StartCoroutine(OnLoad(mABLoadList[i]));
        }

        //等待加载完成
        for (int i = 0; i < mABLoadList.Count; i++)
        {
            while (mABLoadList[i].Status != LoadStatus.eLoaded)
            {
                yield return null;
            }
        }

        IsComplete = true;
    }

    private IEnumerator OnLoad(HAssetBundle ab)
    {
        if (ab.Status == LoadStatus.eNone)
        {
            ab.Status = LoadStatus.eLoading;

            if (mIsSync)
            {
                if (ab.AB == null)
                {
                    string url = PathManager.URL(ab.ABName, mAssetType, false);
                    ab.AB = AssetBundle.LoadFromFile(url);
                    ab.Status = LoadStatus.eLoaded;
                }
            }
            else
            {
                string url = PathManager.URL(ab.ABName, mAssetType, false);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(url);
                mRequestMap.Add(ab.ABName, request);
                yield return request;
                mRequestMap.Remove(ab.ABName);
                ab.AB = request.assetBundle;
                ab.Status = LoadStatus.eLoaded;
            }
        }
        else if (ab.Status == LoadStatus.eLoading)
        {
            if(mIsSync)
            {
                if (mRequestMap.ContainsKey(ab.ABName))
                {
                    Debug.Log("提示:资源异步加载还在进行中,但是又调用了同步接口加载该资源!!!");
                    if (mRequestMap[ab.ABName].assetBundle != null)
                    {
                        if (mRequestMap.ContainsKey(ab.ABName))
                        {
                            mRequestMap[ab.ABName].assetBundle.Unload(true);
                        }
                    }
                }

                if(ab.AB == null)
                {
                    string url = PathManager.URL(ab.ABName, mAssetType, false);
                    ab.AB = AssetBundle.LoadFromFile(url);
                    ab.Status = LoadStatus.eLoaded;
                }
            }
            else
            {
                while (ab.Status != LoadStatus.eLoaded)
                {
                    yield return null;
                }
            }
        }
    }

    public static void StopAllRequest()
    {
        List<AssetBundleCreateRequest> requestList = new List<AssetBundleCreateRequest>();
        foreach (var item in mRequestMap)
        {
            requestList.Add(item.Value);
        }

        //访问AssetBundleCreateRequest异步请求的assetbundle会导致该ab的异步加载立马返回，相当于变为同步加载
        //这句会让之前的异步加载立马完成，像Goto一样跳转到yield return request后面的逻辑执行，执行完后再回到这里执行
        for (int i = 0; i < requestList.Count; i++)
        {
            AssetBundle ab = requestList[i].assetBundle;
        }

        mRequestMap.Clear();
    }
}