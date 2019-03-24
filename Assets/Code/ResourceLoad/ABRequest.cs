using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABRequest
{
    static private Dictionary<string, HAssetBundle> mLoadingMap = new Dictionary<string, HAssetBundle>();
    List<HAssetBundle> mABLoadList = new List<HAssetBundle>();

    public ABRequest()
    { 
    }

    public IEnumerator Load(HAssetBundle ab)
    {
        if (ab == null)
        {
            Debug.LogError("ABRequest HAssetbundle is Null");
            yield break;
        }

        mABLoadList.Add(ab);
        ResourceManager.Instance.StartCoroutine(CoLoad(ab));
        for (int i = 0; i < ab.DepList.Count; i++)
        {
            HAssetBundle depAB = HRes.Get<HAssetBundle>(ab.DepList[i], "", null);
            mABLoadList.Add(depAB);
            ResourceManager.Instance.StartCoroutine(CoLoad(depAB));
        }

        for (int i = 0; i < mABLoadList.Count; i++)
        {
            while (mABLoadList[i].Status != ABLoadStatus.eLoaded)
            {
                yield return null;
            }
        }
    }

    private IEnumerator CoLoad(HAssetBundle ab)
    {
        if (ab.Status == ABLoadStatus.eNone)
        {
            ab.Status = ABLoadStatus.eLoading;
            while(mLoadingMap.ContainsKey(ab.ABName))
            {
                //这里检测目的：当加载A后，WWW还没返回A的AB，但是外部却卸载了A，此时如果再次加载A，那么会走到这里等待之前的A被加载后然后释放掉，Unity不允许同时加载同一个资源的AB
                yield return null;
            }
            if (!mLoadingMap.ContainsKey(ab.ABName))
            {
                mLoadingMap.Add(ab.ABName, ab);
            }
            string url = PathManager.URL(ab.ABName, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;
            mLoadingMap.Remove(ab.ABName);
            ab.AB = www.assetBundle;
            ab.Status = ABLoadStatus.eLoaded;
        }
        else if (ab.Status == ABLoadStatus.eLoading)
        {
            while (ab.Status != ABLoadStatus.eLoaded)
            {
                yield return null;
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