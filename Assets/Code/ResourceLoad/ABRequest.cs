using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABRequest
{
    List<HAssetBundle> mABLoadList = new List<HAssetBundle>();
    static Dictionary<string, AssetBundleCreateRequest> mRequestMap = new Dictionary<string, AssetBundleCreateRequest>();

    private bool mIsComplete = false;
    public bool IsComplete
    {
        get
        {
            return mIsComplete;
        }

        private set
        {
            mIsComplete = value;
        }
    }

    public ABRequest()
    { 
    }

    public void Load(HAssetBundle ab, bool isSync)
    {
        ResourceManager.Instance.StartCoroutine(CoLoad(ab, isSync));
    }

    public IEnumerator CoLoad(HAssetBundle ab, bool isSync = false)
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
            ResourceManager.Instance.StartCoroutine(OnLoad(mABLoadList[i], isSync));
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

    private IEnumerator OnLoad(HAssetBundle ab, bool isSync)
    {
        if (ab.Status == LoadStatus.eNone)
        {
            ab.Status = LoadStatus.eLoading;
            if(isSync)
            {
                if (mRequestMap.ContainsKey(ab.ABName))
                {
                    //打断，解释如下面的
                    if (mRequestMap[ab.ABName].assetBundle != null)
                    {
                        if (mRequestMap.ContainsKey(ab.ABName))
                        {
                            mRequestMap[ab.ABName].assetBundle.Unload(true);
                        }
                    }
                }

                if (ab.AB == null)
                {
                    string url = PathManager.URL(ab.ABName, AssetType.eAB, false);
                    Debug.Log("url" + url);
                    ab.AB = AssetBundle.LoadFromFile(url);
                    ab.Status = LoadStatus.eLoaded;
                }
            }
            else
            {
                if (mRequestMap.ContainsKey(ab.ABName))
                {
                    //打断异步加载，解释:这句会让之前的异步加载立马完成，像Goto一样跳转到yield return request后面的
                    //逻辑执行，执行完之后协程的内容后，再回到这里继续执行。很难理解，但是为了在同一帧支持
                    //同步加载和异步加载同一资源！！！
                    if (mRequestMap[ab.ABName].assetBundle != null)
                    {
                        if (mRequestMap.ContainsKey(ab.ABName))
                        {
                            mRequestMap[ab.ABName].assetBundle.Unload(true);
                        }
                    }
                }
                string url = PathManager.URL(ab.ABName, AssetType.eAB, false);
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
            if(isSync)
            {
                if (mRequestMap.ContainsKey(ab.ABName))
                {
                    //打断，解释如上面的
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
                    string url = PathManager.URL(ab.ABName, AssetType.eAB, false);
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

        //当加载完成时外部已标记为删除，这里卸载AB
        if (HRes.mRemoveMap.ContainsKey(ab))
        {
            if (ab.AB != null)
            {
                ab.AB.Unload(true);
                ab.AB = null;
            }
            HRes.mRemoveMap.Remove(ab);
        }
    }
}