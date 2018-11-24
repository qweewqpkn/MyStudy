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
        eByte,
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
        private int mRefCount;
        public WWW mWWW;
        public AssetType mAssetType;
        public string mAssetName;

        public int _RefCount
        {
            get
            {
                return mRefCount;
            }

            set
            {
                mRefCount = value;
            }
        }

        public List<AssetLoadRequest> mAssetLoadRequestList = new List<AssetLoadRequest>();

        public AssetLoadingInfo(string url, string assetName, AssetType type)
        {
            //UnityWebRequest request = new UnityWebRequest(URL(abName));
            //DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
            //request.downloadHandler = handler;
            mWWW = new WWW(url);
            mAssetType = type;
            mAssetName = assetName;
        }

        public void AddLoadRequest(AssetLoadRequest request)
        {
            request.AddNeedLoadNum();
            mAssetLoadRequestList.Add(request);
        }

        public void CompleteLoadRequest()
        {
            for (int i = 0; i < mAssetLoadRequestList.Count; i++)
            {
                mAssetLoadRequestList[i].AddCurLoadNum();
            }
        }
    }

    public class ResourceManager : MonoBehaviour
    {

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
                    DontDestroyOnLoad(obj);
                }

                return mInstance;
            }
        }

        private AssetBundleManifest mAssestBundleManifest;
        public Dictionary<string, AssetLoadedInfo> mABLoadedMap = new Dictionary<string, AssetLoadedInfo>();
        private Dictionary<string, AssetLoadingInfo> mABLoadingMap = new Dictionary<string, AssetLoadingInfo>();
        private List<AssetLoadingInfo> mRemoveABLoadingList = new List<AssetLoadingInfo>();
        private Dictionary<string, List<string>> mABDependencies = new Dictionary<string, List<string>>();
        private List<AssetLoadRequest> mAssetRequestList = new List<AssetLoadRequest>();
        private Dictionary<string, Shader> mShaderMap = new Dictionary<string, Shader>();

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
                        result.Append("file://" + Application.dataPath + "/../../ClientRes/IOS/");
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                    {
                        result.Append("file://" + Application.dataPath + "/../../ClientRes/Windows/");
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
                    {
                        result.Append("/Assetbundle/");
                    }
                    break;
                case AssetType.eByte:
                    {
                        result.Append("/Config/");
                    }
                    break;
                case AssetType.eTexture:
                    {
                        result.Append("/Texture/");
                    }
                    break;
            }

            result.Append(abName);

            return result.ToString();
        }

        public void AddRefCount(string abName, bool isDependency)
        {
            AssetLoadedInfo info = null;
            if (mABLoadedMap.TryGetValue(abName, out info))
            {
                info.mRefCount++;
            }

            if(isDependency)
            {
                return;
            }

            List<string> dependenciesList = null;
            if (mABDependencies.TryGetValue(abName, out dependenciesList))
            {
                for (int i = 0; i < dependenciesList.Count; i++)
                {
                    if (mABLoadedMap.TryGetValue(dependenciesList[i], out info))
                    {
                        info.mRefCount++;
                    }
                }
            }
        }

        public void LoadManifest(Action callBack)
        {
            StartCoroutine(LoadMainifestInternel(callBack));
        }

        IEnumerator LoadMainifestInternel(Action callBack)
        {
            WWW www = new WWW(URL("Assetbundle", AssetType.eManifest));
            yield return www;

            mAssestBundleManifest = www.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (callBack != null)
            {
                callBack();
            }
        }

        //1.一个ab加载过一次后，那么他的依赖资源都会被加载进去，所以当再次加载之前依赖资源的AB的时候，他的依赖就不用再次加载了。
        //如：加载A,依赖B,C. 其中B是依赖C的。  那么加载之后A,B,C都会被加载好， 当再次加载B的时候，就不用再加载依赖的C了。
        public void LoadPrefabGO(string abName, string assetName, Action<string, GameObject> callBack)
        {
            StartCoroutine(LoadPrefabGORequest(abName, assetName, callBack));
        }

        IEnumerator LoadPrefabGORequest(string abName, string assetName, Action<string, GameObject> callBack)
        {
            ABAssetLoadRequest request = LoadABAssetRequest(abName,assetName, AssetType.ePrefab);
            yield return request;

            GameObject go = request.GetAsset<GameObject>();
            GameObject newObj = Instantiate(go);
            PrefabAutoDestory prfabDestory = newObj.AddComponent<PrefabAutoDestory>();
            prfabDestory.mAssetBundleName = abName;
            if (callBack != null)
            {
                callBack(assetName, newObj);
            }
        }

        //可以用于预加载ab
        //比如切换场景的时候，我们可以先预加载AB, 然后更新加载进度，当加载完了然后根据逻辑把对应AB中的prefab加载并实例化出来，这样速度更快，比如要释放技能的时候，再加载AB会导致卡顿
        public void LoadAB(string abName, Action<string> callBack)
        {
            StartCoroutine(LoadABRequest(abName, callBack));
        }

        IEnumerator LoadABRequest(string abName, Action<string> callBack)
        {
            ABAssetLoadRequest request = LoadABAssetRequest(abName, "", AssetType.eAB, LoadType.eOnlyAB);
            yield return request;

            if (callBack != null)
            {
                callBack(abName);
            }
        }

        public void LoadSprite(string abName, string assetName, Action<string, Sprite> callBack)
        {
            StartCoroutine(LoadSpriteRequest(abName, assetName, callBack));
        }

        IEnumerator LoadSpriteRequest(string abName, string assetName, Action<string, Sprite> callBack)
        {
            ABAssetLoadRequest request = LoadABAssetRequest(abName, assetName, AssetType.eSprite, LoadType.eAllAsset);
            yield return request;

            Sprite sprite = request.GetAssets<Sprite>(assetName);
            if(sprite != null)
            {
                if(callBack != null)
                {
                    callBack(assetName, sprite);
                }
            }
            else
            {
                
            }
        }

        public void LoadAudioClip(string abName, string assetName, Action<string, AudioClip> callBack)
        {
            StartCoroutine(LoadAudioClipRequest(abName, assetName, callBack));
        }

        IEnumerator LoadAudioClipRequest(string abName, string assetName, Action<string, AudioClip> callBack)
        {
            ABAssetLoadRequest request = LoadABAssetRequest(abName, assetName, AssetType.eAudioClip, LoadType.eOneAsset);
            yield return request;

            AudioClip audioClip = request.GetAssets<AudioClip>(assetName);
            if (audioClip != null)
            {
                if (callBack != null)
                {
                    callBack(assetName, audioClip);
                }
            }
            else
            {

            }
        }

        public void LoadAllShader(string abName, Action<string> callBack)
        {
            StartCoroutine(LoadAllShaderRequest(abName, callBack));
        }

        IEnumerator LoadAllShaderRequest(string abName, Action<string> callBack)
        {
            ABAssetLoadRequest request = LoadABAssetRequest(abName, "", AssetType.eShader, LoadType.eAllAsset);
            yield return request;

            UnityEngine.Object[] shaders = request.GetAssets<Shader>();
            CacheAllShader(shaders);

            if (callBack != null)
            {
                callBack(abName);
            }
        }

        public void LoadTexture(string name, Action<Texture> callBack)
        {
            StartCoroutine(LoadTextureRequest(name, callBack));
        }

        IEnumerator LoadTextureRequest(string name, Action<Texture> callBack)
        {
            NormalAssetLoadRequest request = LoadNormalAssetRequest(name, AssetType.eTexture);
            yield return request;

            if(callBack != null)
            {
                callBack(request.mAssetLoadedInfo.mTexture);
            }
        }

        public void LoadBytes(string name, Action<byte[]> callBack)
        {
            StartCoroutine(LoadBytesRequest(name, callBack));
        }

        IEnumerator LoadBytesRequest(string name, Action<byte[]> callBack)
        {
            NormalAssetLoadRequest request = LoadNormalAssetRequest(name, AssetType.eByte);
            yield return request;

            if (callBack != null)
            {
                callBack(request.mAssetLoadedInfo.mBytes);
            }
        }

        //用来加载没有打包成AB的资源
        NormalAssetLoadRequest LoadNormalAssetRequest(string assetName, AssetType type)
        {
            NormalAssetLoadRequest request = new NormalAssetLoadRequest();
            request.Init(assetName, assetName);
            mAssetRequestList.Add(request);

            LoadAssetInternel(assetName, type, request);
            return request;
        }

        //用来加载打包成AB中的资源
        ABAssetLoadRequest LoadABAssetRequest(string abName, string assetName, AssetType type, LoadType loadType = LoadType.eOneAsset)
        {
            ABAssetLoadRequest request = new ABAssetLoadRequest();
            request.Init(abName, assetName, loadType);
            mAssetRequestList.Add(request);

            bool isHave = LoadAssetInternel(abName, type, request);
            if (!isHave)
            {
                LoadABDependencies(abName, request);
            }

            return request;
        }

        //加isDependency变量的原因：
        //前提：A依赖B，B又依赖C
        //这里有个潜在的问题，当缓存中已经有了B，那么自然也就有C。然后我们开始加载A，会先加载依赖也就是B，此时B已经存在了，就这走到这里
        //AddRefCount会把B依赖的C引用也加1，然后A继续加载依赖的C，也有了，那么C的引用就会加1，这里总共就加了2次。问题就在这，因为
        //A其实已经拿到了所有的依赖，包括直接依赖(这里的B)和间接依赖(这里的C)，所以不需要B再去增加C的引用计数了。
        bool LoadAssetInternel(string abName, AssetType type, AssetLoadRequest request, bool isDependency = false)
        {
            if (mABLoadedMap.ContainsKey(abName))
            {
                AddRefCount(abName, isDependency);
                return true;
            }

            AssetLoadingInfo loadingInfo = null;
            if (mABLoadingMap.TryGetValue(abName, out loadingInfo))
            {
                loadingInfo._RefCount++;
                loadingInfo.AddLoadRequest(request);
                return true;
            }

            AssetLoadingInfo info = new AssetLoadingInfo(URL(abName, type), abName, type);
            mABLoadingMap[abName] = info;
            info.AddLoadRequest(request);
            return false;
        }

        void LoadABDependencies(string abName, AssetLoadRequest request)
        {
            List<string> dependenciesList = null;
            if (!mABDependencies.TryGetValue(abName, out dependenciesList))
            {
                dependenciesList = new List<string>();
                string[] dependencies = mAssestBundleManifest.GetAllDependencies(abName);
                dependenciesList.AddRange(dependencies);

                mABDependencies[abName] = dependenciesList;
            }

            for (int i = 0; i < dependenciesList.Count; i++)
            {
                LoadAssetInternel(dependenciesList[i], AssetType.eAB, request, true);
            }
        }

        void Update()
        {
            //加载AB
            if (mABLoadingMap.Count > 0)
            {
                mRemoveABLoadingList.Clear();
                foreach (var item in mABLoadingMap)
                {
                    AssetLoadingInfo info = item.Value;
                    if (info.mWWW.error != null)
                    {
                        Debug.LogError("Loading AB error : " + item.Value.mWWW.error);
                    }

                    if (info.mWWW.isDone)
                    {
                        switch (info.mAssetType)
                        {
                            case AssetType.eAB:
                            case AssetType.eAudioClip:
                            case AssetType.ePrefab:
                            case AssetType.eShader:
                            case AssetType.eSprite:
                                {
                                    AssetLoadedInfo loadedInfo = new AssetLoadedInfo(info.mWWW.assetBundle, info._RefCount);
                                    mABLoadedMap[item.Key] = loadedInfo;
                                    mRemoveABLoadingList.Add(info);
                                }
                                break;
                            case AssetType.eTexture:
                                {
                                    AssetLoadedInfo loadedInfo = new AssetLoadedInfo(info.mWWW.texture, info._RefCount);
                                    mABLoadedMap[item.Key] = loadedInfo;
                                    mRemoveABLoadingList.Add(info);
                                }
                                break;
                            case AssetType.eByte:
                                {
                                    AssetLoadedInfo loadedInfo = new AssetLoadedInfo(info.mWWW.bytes, info._RefCount);
                                    mABLoadedMap[item.Key] = loadedInfo;
                                    mRemoveABLoadingList.Add(info);
                                }
                                break;
                        }
                    }
                }

                for(int i = 0; i < mRemoveABLoadingList.Count; i++)
                {
                    string assetName = mRemoveABLoadingList[i].mAssetName;
                    mABLoadingMap[assetName].CompleteLoadRequest();
                    mABLoadingMap.Remove(assetName);
                }
            }

            //加载AB里面的资源
            for (int i = 0; i < mAssetRequestList.Count;)
            {
                if (mAssetRequestList[i].Update())
                {
                    mAssetRequestList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private void CacheAllShader(UnityEngine.Object[] shaders)
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
