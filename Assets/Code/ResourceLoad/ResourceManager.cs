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

    public partial class ResourceManager : MonoBehaviour
    {
        private AssetRequestQueue mAssetRequestQueue = new AssetRequestQueue();
        public AssetBundleManifest mAssestBundleManifest;
        public Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
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

        public static string GetResName(string abName, string assetName, AssetType type)
        {
            switch(type)
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

        public HRes CreateRes(string abName, string assetName, AssetType type)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, type);
            if (!mResMap.TryGetValue(name, out res))
            {
                switch (type)
                {
                    case AssetType.eAB:
                        {
                            res = new HAssetBundle();
                        }
                        break;
                    case AssetType.eAudioClip:
                        {
                            res = new HAudioCilp();
                        }
                        break;
                    case AssetType.eLua:
                        {
                            res = new HLua();
                        }
                        break;
                    case AssetType.eManifest:
                        {
                            res = new HManifest();
                        }
                        break;
                    case AssetType.eMaterial:
                        {
                            res = new HMaterial();
                        }
                        break;
                    case AssetType.ePrefab:
                        {
                            res = new HPrefab();
                        }
                        break;
                    case AssetType.eShader:
                        {
                            res = new HShader();
                        }
                        break;
                    case AssetType.eSprite:
                        {
                            res = new HSprite();
                        }
                        break;
                    case AssetType.eText:
                        {
                            res = new HText();
                        }
                        break;
                    case AssetType.eTexture:
                        {
                            res = new HTexture();
                        }
                        break;
                }
                res.Init(abName, assetName, type);
                if(type != AssetType.eManifest)
                {
                    mResMap.Add(name, res);
                }
            }

            return res;
        }

        public void LoadRes<T>(string abName, string assetName, AssetType type, Action<T> success, Action error) where T : UnityEngine.Object
        {
            HRes res = CreateRes(abName, assetName, type);
            mAssetRequestQueue.AddReuqest(res, abName, assetName, success, error);
        }

        //单独加载AB(比如:Loading界面做预加载)
        public void LoadAB(string abName, Action<AssetBundle> success, Action error = null)
        {
            LoadRes(abName, "", AssetType.eAB, success, error);
        }

        //加载text
        public void LoadText(string abName, string assetName, Action<TextAsset> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eText, success, error);
        }

        //加载prefab
        public void LoadPrefab(string abName, string assetName, Action<GameObject> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.ePrefab, success, error);
        }

        //加载图集
        public void LoadSprite(string abName, string assetName, Action<Sprite> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eSprite, success, error);
        }

        //加载贴图
        public void LoadTexture(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eTexture, success, error);
        }

        //加载音频
        public void LoadAudioClip(string abName, string assetName, Action<AudioClip> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eAudioClip, success, error);
        }

        //加载材质
        public void LoadMaterial(string abName, string assetName, Action<Material> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eMaterial, success, error);
        }

        public void LoadManifest(string abName, string assetName, Action<AssetBundleManifest> success, Action error)
        {
            LoadRes(abName, assetName, AssetType.eManifest, success, error);
        }

        public void LoadShader(string abName, string assetName, Action<Shader> success, Action error = null)
        {
            LoadRes(abName, assetName, AssetType.eShader, success, error);
        }

        public TextAsset LoadLua(string abName, string assetName)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eLua);
            if (!mResMap.TryGetValue(name, out res))
            {
                res = new HLua();
                res.Init(abName, assetName, AssetType.eLua);
            }
            return res.LoadSync<TextAsset>(abName, assetName);
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

        private void Init()
        {
            LoadManifest("Assetbundle", "AssetBundleManifest", (manifest) =>
            {
                mAssestBundleManifest = manifest;
                if (mInitComplete != null)
                {
                    mInitComplete();
                }
            }, null);
        }

        void Update()
        {
            mAssetRequestQueue.Update();
        }
    }
}
