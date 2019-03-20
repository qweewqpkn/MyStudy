using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public class HRes
    {
        private static Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();

        public string ABName
        {
            get;
            set;
        }

        public string AssetName
        {
            get;
            set;
        }

        public string ResName
        {
            get;
            set;
        }

        //每个类型的资源都关联一个AB资源
        public HAssetBundle ABRes
        {
            get;
            set;
        }

        //该资源加载次数
        public int LoadCount
        {
            get;
            set;
        }

        public HRes(){}

        public static string GetResName(string abName, string assetName, AssetType type)
        {
            switch (type)
            {
                case AssetType.eSprite:
                case AssetType.eLua:
                case AssetType.eText:
                    {
                        return string.Format("{0}/{1}", abName, "*");
                    }
                default:
                    {
                        return assetName == "" ? abName : string.Format("{0}/{1}", abName, assetName);
                    }
            }
        }

        public static T GetRes<T>(string abName, string assetName, AssetType assetType) where T : HRes, new()
        {
            HRes res = null;
            string resName = GetResName(abName, assetName, assetType);
            if (!mResMap.TryGetValue(resName, out res))
            {
                res = new T();
                res.Init(abName, assetName, resName);
                mResMap.Add(resName, res);
            }

            res.LoadCount++;
            return res as T;
        }

        public void Init(string abName, string assetName, string resName)
        {
            ABName = abName;
            AssetName = assetName;
            ResName = resName;
        }

        public void Load<T>(string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(assetName, success, error));
        }

        private IEnumerator CoLoad<T>(string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            ABRes = GetRes<HAssetBundle>(ABName, "", AssetType.eAB);
            yield return ABRes.LoadAB(false); 
            yield return LoadAsset(ABRes.AB, assetName, success, error);
        }

        protected virtual IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            yield return null;
        }

        public void ReleaseAll()
        {
            int count = LoadCount;
            for (int i = 0; i < count; i++)
            {
                Release();
            }
        }

        public virtual void Release()
        {
            if(LoadCount <= 0)
            {
                return;
            }

            LoadCount--;
            if(LoadCount == 0)
            {
                //todo，bug:要修复这里移除了AB，但是没有释放AB，ab.AB.Unload(true);考虑一下如何修复
                if (ResourceManager.Instance.mResMap.ContainsKey(ResName))
                {
                    ResourceManager.Instance.mResMap.Remove(ResName);
                }
            }

            //该资源每次引用对应的ab依赖都要进行释放
            for (int j = 0; j < mAllABList.Count; j++)
            {
                if (ResourceManager.Instance.mResMap.ContainsKey(mAllABList[j]))
                {
                    HAssetBundle ab = ResourceManager.Instance.mResMap[mAllABList[j]] as HAssetBundle;
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eRelease; //标记AB为待释放,因为可能释放资源的时候，它引用的ab还在加载中..
                    if (ab != null)
                    {
                        if(ab.RefCount > 0)
                        {
                            ab.RefCount--;
                        }
                        
                        if (ab.RefCount == 0)
                        {
                            if (ResourceManager.Instance.mResMap.ContainsKey(ab.ResName))
                            {
                                ResourceManager.Instance.mResMap.Remove(ab.ResName);
                            }

                            if (ab.AB != null)
                            {
                                ab.AB.Unload(true);
                            }
                            else
                            {
                                Debug.LogError("HRes Release ab is null, please check!");
                            }
                        }
                    }
                }
            }
        }
    }
}
