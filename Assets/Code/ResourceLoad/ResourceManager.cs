using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public enum AssetType
    {
        eNone,
        eAB,
        ePrefab,
        eTexture,
        eAudioClip,
        eText,
        eShader,
        eSprite,
        eLua,
        eManifest,
        eMaterial,
    }

    public class ResourceManager : MonoBehaviour
    {
        private AssetRequestQueue mAssetRequestQueue = new AssetRequestQueue();
        public Action mInitComplete = null;

        private static ResourceManager mInstance;
        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "ResourceManager";
                    mInstance = obj.AddComponent<ResourceManager>();
                    mInstance.Init();
                    DontDestroyOnLoad(obj);
                }

                return mInstance;
            }
        }

        private void Init()
        {
        }

        //单独加载AB(比如:Loading界面做预加载)
        public void LoadAB(string abName, Action<AssetBundle> success, Action error = null)
        {
            HRes res = HRes.GetRes<HAssetBundle>(abName, "", AssetType.eAB);
            mAssetRequestQueue.AddReuqest(res, "", success, error);
        }

        //加载text
        public void LoadText(string abName, string assetName, Action<TextAsset> success, Action error = null)
        {
            HRes res = HRes.GetRes<HText>(abName, "", AssetType.eText);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        //加载prefab
        //todo 连续加载A资源两次，要处理第二次就不要再次调用www了。
        public void LoadPrefab(string abName, string assetName, Action<GameObject> success, Action error = null)
        {
            HRes res = HRes.GetRes<HPrefab>(abName, assetName, AssetType.ePrefab);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        //加载图集
        public void LoadSprite(string abName, string assetName, Action<Sprite> success, Action error = null)
        {
            HRes res = HRes.GetRes<HSprite>(abName, assetName, AssetType.eSprite);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        //加载贴图
        public void LoadTexture(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            HRes res = HRes.GetRes<HTexture>(abName, assetName, AssetType.eTexture);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        //加载音频
        public void LoadAudioClip(string abName, string assetName, Action<AudioClip> success, Action error = null)
        {
            HRes res = HRes.GetRes<HAudioCilp>(abName, assetName, AssetType.eAudioClip);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        //加载材质
        public void LoadMaterial(string abName, string assetName, Action<Material> success, Action error = null)
        {
            HRes res = HRes.GetRes<HMaterial>(abName, assetName, AssetType.eMaterial);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        public void LoadShader(string abName, string assetName, Action<Shader> success, Action error = null)
        {
            HRes res = HRes.GetRes<HShader>(abName, assetName, AssetType.eShader);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        public void LoadLua(string abName, string assetName, Action<TextAsset> success, Action error = null)
        {
            HRes res = HRes.GetRes<HLua>(abName, assetName, AssetType.eLua);
            mAssetRequestQueue.AddReuqest(res, assetName, success, error);
        }

        public void Release(string name, AssetType type)
        {
            name = name.ToLower();
            Release(name, name, type);
        }

        public void Release(string abName, string assetName, AssetType type)
        {
            HRes res;
            string name = GetResName(abName.ToLower(), assetName, type);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Release();
            }
        }

        public void ReleaseAll()
        {
            List<HRes> resList = new List<HRes>();
            foreach (var item in mResMap)
            {
                resList.Add(item.Value);
            }

            for(int i = 0; i < resList.Count; i++)
            {
                resList[i].ReleaseAll();
            }

            //清空正在请求的队列
            mAssetRequestQueue.ReleaseAll();
        }

        void Update()
        {
            mAssetRequestQueue.Update();
        }
    }
}
