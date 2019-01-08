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
            public List<ABAssetLoadRequest> mRequestList = new List<ABAssetLoadRequest>();

            public AssetLoadingInfo()
            {
            }

            public void AddLoadRequest(ABAssetLoadRequest request)
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
        public void LoadAB(string abName, Action success, Action error = null)
        {
            HRes res = null;
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAssetBundle(abName, success, error);
                mResMap.Add(abName, res);
            }
        }

        //加载text
        public void LoadText(string assetName, Action<byte[]> success, Action error = null)
        {
            HRes res = new HText(assetName, success, error);
            mResMap.Add(assetName, res);
        }

        public void LoadAsset<T>(string name, Action<T> success, Action error = null) where T : UnityEngine.Object
        {
            LoadAsset<T>(name, name, success, error);
        }

        //通用接口，加载matrial,texture,audioclip,sprite,prefab
        public void LoadAsset<T>(string abName, string assetName, Action<T> success, Action error = null) where T : UnityEngine.Object
        {
            HRes res = null;
            if (mResMap.TryGetValue(name, out res))
            {
                res.Load(success, error);
            }
            else
            {
                res = new HAsset<T>(abName, assetName, success, error);
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

        private string URL(string abName, AssetType type)
        {
            StringBuilder result = new StringBuilder();
            switch (Application.platform)
            {
                //这里除了android 其余的平台都要加file://才能使用www进行加载
                case RuntimePlatform.Android:
                    {
                        result.Append(Application.streamingAssetsPath);
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    {
                        result.Append("file://" + Application.streamingAssetsPath);
                    }
                    break;
                case RuntimePlatform.OSXEditor:
                    {
                        result.Append("file://" + Application.dataPath + "/../ClientRes/IOS/");
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                    {
                        result.Append("file://" + Application.dataPath + "/../ClientRes/Windows/");
                    }
                    break;
                default:
                    {
                        result.Append("file://" + Application.dataPath + "/../ClientRes/");
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
            LoadAsset<AssetBundleManifest>("Assetbundle", "AssetBundleManifest", (manifest) =>
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
            });
        }

        private void LoadShader(string abName, Action<Shader[]> success, Action error = null)
        {
            HRes res = new HShader(abName, success, error);
            mResMap.Add(abName, res);
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
