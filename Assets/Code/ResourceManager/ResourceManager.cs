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
                    {
                        return string.Format("{0}/{1}", abName, "*");
                    }
                default:
                    {
                        return assetName == "" ? abName : string.Format("{0}/{1}", abName, assetName);
                    }
            }
        }

        //单独加载AB(比如:Loading界面做预加载)
        public void LoadAB(string abName, Action<AssetBundle> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, "", AssetType.eAB);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAssetBundle(abName);
                res.Load(success, error);
            }
        }

        //加载text
        public void LoadText(string assetName, Action<byte[]> success, Action error = null)
        {
            HRes res = null;
            if (mResMap.TryGetValue(assetName, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HText(assetName);
                res.Load(success, error);
            }
        }

        //加载prefab
        public void LoadPrefab(string abName, string assetName, Action<GameObject> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.ePrefab);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HPrefab(abName, assetName);
                res.Load(success, error);
            }
        }

        //加载图集
        public void LoadSprite(string abName, string assetName, Action<Sprite> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eSprite);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(assetName, success, error);
            }
            else
            {
                res = new HSprite(abName);
                res.Load(assetName, success, error);
            }
        }

        public void LoadTexture(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eTexture);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HTexture(abName, assetName);
                res.Load(success, error);
            }
        }

        //加载音频
        public void LoadAudioClip(string abName, string assetName, Action<AudioClip> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eAudioClip);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAudioCilp(abName, assetName);
                res.Load(success, error);
            }
        }

        //加载材质
        public void LoadMaterial(string abName, string assetName, Action<Material> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eMaterial);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HMaterial(abName, assetName);
                res.Load(success, error);
            }
        }

        public TextAsset LoadLua(string abName, string assetName)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eLua);
            if (mResMap.TryGetValue(name, out res))
            {
                return res.LoadSync<TextAsset>(assetName);
            }
            else
            {
                res = new HLua(abName);
                return res.LoadSync<TextAsset>(assetName);
            }
        }

        public void LoadManifest(string abName, string assetName, Action<AssetBundleManifest> success, Action error)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eManifest);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HManifest(abName, assetName);
                res.Load(success, error);
            }
        }

        public void LoadShader(string abName, string assetName, Action<Shader> success, Action error = null)
        {
            HRes res = null;
            string name = GetResName(abName, assetName, AssetType.eShader);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HShader(abName, assetName);
                res.Load(success, error);
            }
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
                if(res.LoadCount == 0)
                {
                    mResMap.Remove(name);
                }
            }
        }

        public void ReleaseAll()
        {
            foreach(var item in mResMap)
            {
                item.Value.ReleaseAll();
            }

            mResMap.Clear();
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
    }
}
