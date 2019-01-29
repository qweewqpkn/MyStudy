using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public enum AssetType
    {
        eAB,
        ePrefab,
        eTexture,
        eAudioClip,
        eText,
        eShader,
        eSprite,
        eManifest,
    }

    public partial class ResourceManager : MonoBehaviour
    {
        public AssetBundleManifest mAssestBundleManifest;
        public Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
        private Dictionary<string, Shader> mShaderMap = new Dictionary<string, Shader>();
        private Dictionary<string, List<byte[]>> mLuaBytesMap = new Dictionary<string, List<byte[]>>();
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

        //单独加载AB(比如:Loading界面做预加载)
        public void LoadAB(string abName, Action<AssetBundle> success, Action error = null)
        {
            HRes res = null;
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
            if (mResMap.TryGetValue(name, out res))
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
        public void LoadSprite(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            HRes res = null;
            if (mResMap.TryGetValue(abName, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HSprite(abName, assetName);
                res.Load(success, error);
            }
        }

        public void LoadTexture(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            HRes res = null;
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

        public void LoadLua()
        {

        }

        public void LoadManifest(string abName, string assetName, Action<AssetBundleManifest> success, Action error)
        {
            HRes res = null;
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

        private void LoadShader(string abName, Action<Shader[]> success, Action error = null)
        {
            HRes res = null;
            if (mResMap.TryGetValue(abName, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HShader(abName);
                res.Load(success, error);
            }
        }

        public Shader GetShader(string name)
        {
            Shader shader = null;
            mShaderMap.TryGetValue(name, out shader);
            return shader;
        }

        public void Release(string name)
        {
            HRes res;
            if (mResMap.TryGetValue(name, out res))
            {
                res.Release();
                mResMap.Remove(name);
            }
        }

        public void ReleaseAll()
        {
            foreach(var item in mResMap)
            {
                item.Value.Release();
            }

            mResMap.Clear();
        }

        private void Init()
        {
            LoadManifest("Assetbundle", "AssetBundleManifest", (manifest) =>
            {
                mAssestBundleManifest = manifest;
                LoadShader("allshader", (shaders) =>
                {
                    for (int i = 0; i < shaders.Length; i++)
                    {
                        mShaderMap[shaders[i].name] = shaders[i];
                    }

                    if (mInitComplete != null)
                    {
                        mInitComplete();
                    }
                });
            }, null);
        }
    }
}
