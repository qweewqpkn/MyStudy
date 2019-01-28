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
    public partial class ResourceManager
    {
        public class AssetLoadedInfo
        {
            private AssetBundle mAB;
            private int mRef;

            public int Ref
            {
                get
                {
                    return mRef;
                }
                
                set { mRef = value; }
            }

            public AssetBundle AB
            {
                get
                {
                    return mAB;
                }

                set
                {
                    mAB = value;
                }
            }
            
            public AssetLoadedInfo(AssetBundle ab, int refCount)
            {
                mAB = ab;
                mRef = refCount;
            }
        }

        public class AssetLoadingInfo
        {
            public List<ABRequest> mRequestList = new List<ABRequest>();

            public AssetLoadingInfo()
            {
            }

            public void AddLoadRequest(ABRequest request)
            {
                mRequestList.Add(request);
            }

            public void Completed()
            {
                for (int i = 0; i < mRequestList.Count; i++)
                {
                    mRequestList[i].AddLoadABNum();
                }
            }
        }
    }

    public partial class ResourceManager : MonoBehaviour
    {
        public AssetBundleManifest mAssestBundleManifest;
        public Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
        public Dictionary<string, AssetLoadedInfo> mABLoadedMap = new Dictionary<string, AssetLoadedInfo>();
        private Dictionary<string, AssetLoadingInfo> mABLoadingMap = new Dictionary<string, AssetLoadingInfo>();
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
            string name = string.Format("{AB}/{1}", "TEXT", abName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAssetBundle(abName);
                res.Load(success, error);
                mResMap.Add(name, res);
            }
        }

        //加载text
        public void LoadText(string assetName, Action<byte[]> success, Action error = null)
        {
            HRes res = null;
            string name = string.Format("{0}/{1}", "TEXT", assetName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HText(assetName);
                res.Load(success, error);
                mResMap.Add(name, res);
            }
        }

        //加载prefab
        public void LoadPrefab(string abName, string assetName, Action<GameObject> success, Action error = null)
        {
            HRes res = null;
            string name = string.Format("{0}/{1}", abName, assetName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HPrefab(abName, assetName);
                res.Load(success, error);
                mResMap.Add(name, res);
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
                mResMap.Add(abName, res);
            }
        }

        //记载texture
        //todo
        //1.连续调用两次该函数该如何解决？想清楚
        //2.如何贴图已经加载出来了，是否就不用走加载ab和ab其中贴图的流程了，直接拿缓存的texture就可以了？
        public void LoadTexture(string abName, string assetName, Action<Texture> success, Action error = null)
        {
            HRes res = null;
            string name = string.Format("{0}/{1}", abName, assetName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HTexture(abName, assetName);
                res.Load(success, error);
                mResMap.Add(name, res);
            }
        }

        //加载音频
        public void LoadAudioClip(string abName, string assetName, Action<AudioClip> success, Action error = null)
        {
            HRes res = null;
            string name = string.Format("{0}/{1}", abName, assetName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAudioCilp(abName, assetName);
                res.Load(success, error);
                mResMap.Add(name, res);
            }
        }

        //加载材质
        public void LoadMaterial(string abName, string assetName, Action<Material> success, Action error = null)
        {
            HRes res = null;
            string name = string.Format("{0}/{1}", abName, assetName);
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAudioCilp(abName, assetName);
                res.Load(success, error);
                mResMap.Add(name, res);
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
                mResMap.Add(abName, res);
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
                mResMap.Add(abName, res);
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

        public string URL(string abName, AssetType type)
        {
            StringBuilder result = new StringBuilder();
            switch (Application.platform)
            {
                //这里除了android 其余的平台都要加file://才能使用www进行加载
                case RuntimePlatform.Android:
                    {
                        result.Append(PathManager.RES_PATH_ANDROID_PHONE);
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    {
                        result.Append("file://" + PathManager.RES_PATH_IOS_PHONE);
                    }
                    break;
                case RuntimePlatform.OSXEditor:
                    {
                        result.Append("file://" + PathManager.RES_PATH_IOS);
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                    {
                        result.Append("file://" + PathManager.RES_PATH_WINDOWS);
                    }
                    break;
                default:
                    {
                        result.Append("file://" + PathManager.RES_PATH_WINDOWS);
                    }
                    break;
            }

            switch (type)
            {
                case AssetType.eAB:
                case AssetType.eAudioClip:
                case AssetType.eManifest:
                case AssetType.ePrefab:
                case AssetType.eShader:
                case AssetType.eSprite:
                case AssetType.eTexture:
                    {
                        result.Append("/Assetbundle/");
                    }
                    break;
                case AssetType.eText:
                    {
                        result.Append("/Config/");
                    }
                    break;
            }

            result.Append(abName);
            return result.ToString();
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

        private void AddLoadingAsset(string name)
        {
            AssetLoadingInfo info;
            if (!mABLoadingMap.TryGetValue(name, out info))
            {
                info = new AssetLoadingInfo();
                mABLoadingMap[name] = info;
            }
        }

        private void RemoveLoadingAsset(string name)
        {
            mABLoadingMap.Remove(name);
        }

        private AssetLoadingInfo GetLoadingAsset(string name)
        {
            AssetLoadingInfo info;
            if (mABLoadingMap.TryGetValue(name, out info))
            {
                return info;
            }

            return null;
        }
    }
}
