using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

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

    public class AssetLoadedInfo
    {
        public AssetBundle mAB;
        public Texture mTexture;
        public byte[] mBytes;
        public int mRefCount;

        public AssetLoadedInfo(AssetBundle ab, int refCount)
        {
            mAB = ab;
            mRefCount = refCount;
        }

        public AssetLoadedInfo(Texture texture, int refCount)
        {
            mTexture = texture;
            mRefCount = refCount;
        }

        public AssetLoadedInfo(byte[] bytes, int refCount)
        {
            mBytes = bytes;
            mRefCount = refCount;
        }
    }

    public class AssetLoadingInfo
    {
        public List<ABLoadRequest> mRequestList = new List<ABLoadRequest>();

        public AssetLoadingInfo()
        {
        }

        public void AddLoadRequest(ABLoadRequest request)
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

    public class ResourceManager : MonoBehaviour
    {
        private AssetBundleManifest mAssestBundleManifest;
        public Dictionary<string, AssetLoadedInfo> mABLoadedMap = new Dictionary<string, AssetLoadedInfo>();
        private Dictionary<string, AssetLoadingInfo> mABLoadingMap = new Dictionary<string, AssetLoadingInfo>();
        private Dictionary<string, List<string>> mABDependencies = new Dictionary<string, List<string>>();
        private Dictionary<string, Shader> mShaderMap = new Dictionary<string, Shader>();
        private Dictionary<Type, Type> mTypeMap = new Dictionary<Type, Type>();
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
            mTypeMap.Add(typeof(GameObject), typeof(PrefabLoadRequest));
            mTypeMap.Add(typeof(Material), typeof(AssetLoadRequest));
            mTypeMap.Add(typeof(AudioClip), typeof(AssetLoadRequest));
            mTypeMap.Add(typeof(Shader), typeof(ShaderLoadRequest));
            mTypeMap.Add(typeof(AssetBundleManifest), typeof(ManifestLoadRequest));
            mTypeMap.Add(typeof(byte[]), typeof(TextLoadRequest));
        }

        public string URL(string abName, AssetType type)
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
                        result.Append("file://" + Application.dataPath + "/../ClientRes/Android/");
                    }
                    break;
                default:
                    {
                        result.Append("file://" + Application.dataPath + "/../ClientRes/");
                    }
                    break;
            }

            switch(type)
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

        public void SetABManifest(AssetBundleManifest manifest)
        {
            mAssestBundleManifest = manifest;
        }

        //加载AB中的资源
        public void LoadABAsset<T>(string abName, string assetName, Action<T> success, Action error = null) where T : UnityEngine.Object
        {
            StartCoroutine(LoadABAssetRequest<T>(abName, assetName, success, error));
        }

        //加载AB中的资源，AB和其中资源同名
        public void LoadABAsset<T>(string name, Action<T> success, Action error = null) where T : UnityEngine.Object
        {
            StartCoroutine(LoadABAssetRequest<T>(name, name, success, error));
        }

        //加载非AB中的资源，如：text
        public void LoadAsset<T>(string assetName, Action<T> success, Action error = null)
        {
            StartCoroutine(LoadAssetRequest<T>(assetName, success, error));

        }

        IEnumerator LoadABAssetRequest<T>(string abName, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            AssetLoadRequest request = CreateLoadRequest(abName, assetName, typeof(T));
            yield return request;
            request.ProcessABAsset<T>(success, error);
        }

        IEnumerator LoadAssetRequest<T>(string assetName, Action<T> success, Action error)
        {
            AssetLoadRequest request = CreateLoadRequest(assetName, typeof(T));
            yield return request;
            request.ProcessAsset<T>(success, error);
        }


        private AssetLoadRequest CreateLoadRequest(string abName, string assetName, Type t)
        {
            return (AssetLoadRequest)Activator.CreateInstance(mTypeMap[t], new object[] { abName, assetName });
        }

        private AssetLoadRequest CreateLoadRequest(string assetName, Type t)
        {
            return (AssetLoadRequest)Activator.CreateInstance(mTypeMap[t], new object[] {assetName });
        }


        public List<string> GetABDependency(string abName)
        {
            if(mAssestBundleManifest == null)
            {
                return null;
            }

            List<string> dependencyList = null;
            if (!mABDependencies.TryGetValue(abName, out dependencyList))
            {
                dependencyList = new List<string>();
                string[] dependencies = mAssestBundleManifest.GetAllDependencies(abName);
                dependencyList.AddRange(dependencies);

                mABDependencies[abName] = dependencyList;
            }

            return dependencyList;
        }

        public void AddLoadedAsset(string name, AssetBundle ab, int refNum)
        {
            AssetLoadedInfo info;
            if(!mABLoadedMap.TryGetValue(name, out info))
            {
                info = new AssetLoadedInfo(ab, refNum);
                mABLoadedMap[name] = info;
            }
        }

        public void AddLoadingAsset(string name)
        {
            AssetLoadingInfo info;
            if (!mABLoadingMap.TryGetValue(name, out info))
            {
                info = new AssetLoadingInfo();
                mABLoadingMap[name] = info;
            }
        }

        public void RemoveLoadingAsset(string name)
        {
            mABLoadingMap.Remove(name);
        }

        public AssetLoadedInfo GetLoadedAsset(string name)
        {
            AssetLoadedInfo info;
            if(mABLoadedMap.TryGetValue(name, out info))
            {
                return info;
            }

            return null;
        }

        public AssetLoadingInfo GetLoadingAsset(string name)
        {
            AssetLoadingInfo info;
            if (mABLoadingMap.TryGetValue(name, out info))
            {
                return info;
            }

            return null;
        }

        public void AddRef(string name)
        {
            AssetLoadedInfo info;
            if (mABLoadedMap.TryGetValue(name, out info))
            {
                info.mRefCount++;
            }
        }

        public void CacheAllShader(UnityEngine.Object[] shaders)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                mShaderMap[shaders[i].name] = shaders[i] as Shader;
            }
        }

        public Shader GetShader(string name)
        {
            Shader shader = null;
            mShaderMap.TryGetValue(name, out shader);
            return shader;
        }

        public void ReleaseAB(string abName)
        {
            AssetLoadedInfo info;
            if (mABLoadedMap.TryGetValue(abName, out info))
            {
                info.mRefCount--;
                if (info.mRefCount == 0)
                {
                    info.mAB.Unload(true);
                    mABLoadedMap.Remove(abName);
                }
            }

            //卸载它依赖的AB
            List<string> dependenciesList;
            if(mABDependencies.TryGetValue(abName, out dependenciesList))
            {
                for (int i = 0; i < dependenciesList.Count; i++)
                {
                    if (mABLoadedMap.TryGetValue(dependenciesList[i], out info))
                    {
                        info.mRefCount--;
                        if (info.mRefCount == 0)
                        {
                            info.mAB.Unload(true);
                            mABLoadedMap.Remove(abName);
                        }
                    }
                }

            }
        }
    }
}
