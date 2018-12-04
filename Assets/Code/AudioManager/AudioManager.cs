using AssetLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private int mID = 0;
    private float mUpdateDelta = 0;

    private GameObject mAudioListenerObj;
    private AudioListener mAudioListener;
    //AudioListener跟随对象
    private GameObject mAudioListenerFollowObj;
    //缓存当前帧数
    private int mCurFrameCount;
    //当前帧播放列表
    private List<string> mCurFramePlayList = new List<string>();


    //指定音频类型的音频数据
    private Dictionary<AudioType, AudioTypeData> mAudioTypeDataDict = new Dictionary<AudioType, AudioTypeData>();
    //缓存音频资源
    //public Dictionary<string, AudioClip> mAudioClipDict = new Dictionary<string, AudioClip>();

    private static AudioManager mInstance;
    public static AudioManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AudioManager";
                mInstance = obj.AddComponent<AudioManager>();
                mInstance.Init();
                DontDestroyOnLoad(obj);
            }
            return mInstance;
        }
    }


    //不同需求都要新增一种类型
    public enum AudioType
    {
        eNone,

        eBGMusic, //背景音乐
        eOtherMusic, //其余的音乐
        eTalk, //对白语音
        eBtnClick, //按钮点击
        eSceneObj, //场景对象
        eFight, //战斗 
        eStorySF, //剧情音效 
        eUiSF, //ui音效

        //通用
        eCommon,

        eMax, //最大值
    }

    //空间类型
    public enum AudioSpaceType
    {
        e2D,
        e3D,
    }

    //创建类型
    public enum AudioCreateType
    {
        eOnly, //使用唯一的播放器
        eNew, //创建新的播放器
    }

    public class AudioData
    {
        public AudioData()
        {
            mAudioNameList = new List<string>();
        }

        //唯一id
        public int mID;
        //播放器本身
        public AudioSource mAudioSource;
        //类型
        public AudioType mType;
        //空间类型
        public AudioSpaceType mSpaceType;
        //创建类型
        public AudioCreateType mCreateType;
        //该播放跟随的对象(3D播放器才会有跟随对象)
        public GameObject mFollowObj;
        //计时器id
        public int mTimerID;
        //第几帧使用
        public int mFrameCount;
        //循环与否
        public bool mLoop;
        //最大距离
        public float mMaxRange;
        //播放过的音频列表(主要用于释放这些播放过的音频资源)
        public List<string> mAudioNameList;
        //当前播放器播放的音频名字
        public string _PlayClipName
        {
            get
            {
                if (mAudioSource != null && mAudioSource.clip != null)
                {
                    return mAudioSource.clip.name;
                }

                return "";
            }
        }
    }

    public class AudioTypeData
    {
        public AudioTypeData()
        {
            mAudioDataList = new List<AudioData>();
            mIsMute = false;
        }

        public List<AudioData> mAudioDataList;

        //该类型是否静音
        private bool mIsMute;
        public bool _IsMute
        {
            get
            {
                return mIsMute;
            }

            set
            {
                mIsMute = value;
                for (int i = 0; i < mAudioDataList.Count; i++)
                {
                    mAudioDataList[i].mAudioSource.mute = mIsMute;
                }
            }
        }

        //该类型是否要停止
        private bool mIsStop;
        public bool _IsStop
        {
            get
            {
                return mIsStop;
            }

            set
            {
                mIsStop = value;
                if (mIsStop)
                {
                    for (int i = 0; i < mAudioDataList.Count; i++)
                    {
                        //延时的播放要停止掉
                        if (mAudioDataList[i].mTimerID != 0)
                        {
                            TimerManager.Instance.StopTimer(mAudioDataList[i].mTimerID);
                            mAudioDataList[i].mTimerID = 0;
                        }

                        mAudioDataList[i].mAudioSource.Stop();
                    }
                }
            }
        }

        //该类型的音量
        private float mVolume;
        public float _Volume
        {
            get
            {
                return mVolume;
            }

            set
            {
                mVolume = Mathf.Clamp(value, 0.0f, 1.0f);
                for (int i = 0; i < mAudioDataList.Count; i++)
                {
                    mAudioDataList[i].mAudioSource.volume = Mathf.Clamp(mVolume, 0.0f, 1.0f);
                }
            }
        }
    }

    private void Init()
    {
        mAudioListenerObj = new GameObject();
        mAudioListenerObj.name = "AudioListener";
        mAudioListenerObj.transform.SetParent(transform, false);
        mAudioListener = mAudioListenerObj.AddComponent<AudioListener>();
    }

    //设置listern的跟随的对象
    public void SetListenerFollow(GameObject obj)
    {
        mAudioListenerFollowObj = obj;
    }

    //创建播放器（新增类型要修改这里）
    private AudioData CreateAudioData(AudioType type, AudioSpaceType spaceType, AudioCreateType createType, AudioTypeData data, GameObject targetObj, float maxRange, bool isLoop)
    {
        AudioData audioData = new AudioData();
        audioData.mID = mID++;
        audioData.mFollowObj = targetObj;
        audioData.mTimerID = 0;
        audioData.mFrameCount = Time.frameCount;
        audioData.mType = type;
        audioData.mSpaceType = spaceType;
        audioData.mCreateType = createType;
        audioData.mLoop = isLoop;
        audioData.mMaxRange = maxRange;

        GameObject obj = new GameObject();
        obj.transform.SetParent(transform, false);
        obj.name = type.ToString() + audioData.mID;
        audioData.mAudioSource = obj.AddComponent<AudioSource>();
        audioData.mAudioSource.mute = data._IsMute;
        audioData.mAudioSource.volume = data._Volume;

        if (spaceType == AudioSpaceType.e2D)
        {
            audioData.mAudioSource.spatialBlend = 0.0f;
            audioData.mAudioSource.loop = isLoop;
        }
        else if (spaceType == AudioSpaceType.e3D)
        {
            audioData.mAudioSource.spatialBlend = 1.0f;
            audioData.mAudioSource.maxDistance = maxRange;
            audioData.mAudioSource.minDistance = 0;
            audioData.mAudioSource.loop = isLoop;
            audioData.mAudioSource.rolloffMode = AudioRolloffMode.Linear;
            if (targetObj != null)
                obj.transform.position = targetObj.transform.position;
        }

        return audioData;
    }

    private AudioTypeData GetAudioTypeData(AudioType type)
    {
        AudioTypeData data = null;
        if (mAudioTypeDataDict.ContainsKey(type))
        {
            data = mAudioTypeDataDict[type];
        }
        else
        {
            mAudioTypeDataDict[type] = new AudioTypeData();
            data = mAudioTypeDataDict[type];
        }

        return data;
    }

    private AudioData GetAudioData(int id)
    {
        for (int i = 0; i < (int)AudioType.eMax; i++)
        {
            AudioTypeData data = GetAudioTypeData((AudioType)i);
            for (int j = 0; j < data.mAudioDataList.Count; j++)
            {
                if (data.mAudioDataList[j].mID == id)
                {
                    return data.mAudioDataList[j];
                }
            }
        }

        return null;
    }

    //获取播放器（新增类型要修改这里）
    private AudioData GetAudioSource(AudioType type, AudioSpaceType spaceType, AudioCreateType createType, GameObject targetObj, float maxRange, bool isLoop)
    {
        //获取指定type的数据
        AudioTypeData data = GetAudioTypeData(type);
        //获取播放器
        AudioData audioData = null;

        //寻找空闲播放器
        bool bFree = false;
        for (int i = 0; i < data.mAudioDataList.Count; i++)
        {
            if (createType == AudioCreateType.eOnly)
            {
                if (data.mAudioDataList[i].mCreateType == createType)
                {
                    bFree = true;
                    audioData = data.mAudioDataList[i];
                    break;
                }
            }
            else if (createType == AudioCreateType.eNew)
            {
                //没有播放并且不是延时使用且不是同一帧(因为同一帧isPlaying判断不了)
                //并且各个参数相同，表明可以复用
                if (!data.mAudioDataList[i].mAudioSource.isPlaying &&
                    data.mAudioDataList[i].mTimerID == 0 &&
                    Time.frameCount != data.mAudioDataList[i].mFrameCount &&
                    data.mAudioDataList[i].mSpaceType == spaceType &&
                    data.mAudioDataList[i].mCreateType == createType)
                {
                    bFree = true;
                    audioData = data.mAudioDataList[i];
                    audioData.mFollowObj = targetObj;
                    audioData.mAudioSource.loop = isLoop;
                    audioData.mAudioSource.maxDistance = maxRange;
                    break;
                }
            }
        }

        //没有空闲,创建新的
        if (!bFree)
        {
            audioData = CreateAudioData(type, spaceType, createType, data, targetObj, maxRange, isLoop);
            data.mAudioDataList.Add(audioData);
        }
        return audioData;
    }

    //获取音频资源
    private void GetAudioClip(string name, Action<AudioClip> onComplete)
    {
        if (string.IsNullOrEmpty(name))
        {
            onComplete(null);
            return;
        }

        ResourceManager.Instance.LoadABAsset<AudioClip>(name.ToLower(), (audioClip) =>
        {
            onComplete(audioClip);
        }, () =>
        {
            onComplete(null);
        });
    }

    //释放指定名字的音频资源
    private void Dispose(string name)
    {
        ResourceManager.Instance.ReleaseAB(name.ToLower());
    }

    //释放特定类型的所有音频资源
    public void Dispose(AudioType type)
    {
        AudioTypeData data = GetAudioTypeData(type);
        //遍历指定类型的所有播放器
        for (int i = 0; i < data.mAudioDataList.Count; i++)
        {
            //遍历指定播放器的所有播放记录
            for (int j = 0; j < data.mAudioDataList[i].mAudioNameList.Count; j++)
            {
                string name = data.mAudioDataList[i].mAudioNameList[j];
                Dispose(name);
            }
            data.mAudioDataList[i].mAudioNameList.Clear();
        }
    }

    //释放指定播放器中指定name的资源,如果name为空,那么释放该播放器的所有播放记录
    public void Dispose(int id, string name)
    {
        AudioData data = GetAudioData(id);
        if (data != null)
        {
            if (string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < data.mAudioNameList.Count; i++)
                {
                    Dispose(data.mAudioNameList[i]);
                }
                data.mAudioNameList.Clear();
            }
            else
            {
                for (int i = 0; i < data.mAudioNameList.Count;)
                {
                    if (data.mAudioNameList[i] == name)
                    {
                        Dispose(name);
                        data.mAudioNameList.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
    }

    //停止并且释放指定类型的音频
    public void StopAndDisposeAudio(AudioType type)
    {
        StopAudio(type);
        Dispose(type);
    }

    public void StopAndDisposeAudio(int id, string name)
    {
        StopAudio(id);
        Dispose(id, name);
    }

    //停止并且释放所有音频
    public void StopAndDisposeAll()
    {
        for (int i = 0; i < (int)AudioType.eMax; i++)
        {
            StopAndDisposeAudio((AudioType)i);
        }
    }

    //停止指定类型的播放器
    public void StopAudio(AudioType type)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data._IsStop = true;
    }

    //停止指定id的播放器
    public void StopAudio(int id)
    {
        AudioData data = GetAudioData(id);
        if (data != null)
        {
            data.mAudioSource.Stop();
        }
    }

    //静音
    public void MuteAudio(AudioType type, bool bMute)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data._IsMute = bMute;
    }

    //静音所有
    public void MuteAudioAll(bool bMute)
    {
        for (int i = 0; i < (int)AudioType.eMax; i++)
        {
            MuteAudio((AudioType)i, bMute);
        }
    }

    //设置指定类型音频的音量
    public void SetAudioVolume(AudioType type, float volume)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data._Volume = volume;
    }

    //获取音频的长度
    public void GetAudioClipLength(string name, Action<float> onComplete)
    {
        GetAudioClip(name, (audioClip) =>
        {
            ResourceManager.Instance.ReleaseAB(name.ToLower());
            float length = 0;
            if (audioClip != null)
            {
                length = audioClip.length;
            }
            onComplete(length);
        });
    }

    //会过滤掉同一帧重复播放的音乐
    public void PlayAudioFilter(AudioType type, string name, AudioSpaceType spaceType = AudioSpaceType.e2D,
        AudioCreateType createType = AudioCreateType.eOnly,
        bool isLoop = false,
        float delayTime = 0.0f, GameObject obj = null, float maxRange = 0.0f)
    {
        if (mCurFrameCount != Time.frameCount)
        {
            mCurFrameCount = Time.frameCount;
            mCurFramePlayList.Clear();
        }
        else
        {
            //阻止同一帧多次播放相同的音频
            int index = mCurFramePlayList.FindIndex((item) => { return item == name; });
            if (index != -1)
            {
                return;
            }
        }

        PlayAudio(type, name, spaceType, createType, isLoop, delayTime, obj, maxRange);
    }

    public int PlayAudioOnly(AudioType type, string name, bool isLoop = false, float delayTime = 0.0f)
    {
        return PlayAudio(type, name, AudioSpaceType.e2D, AudioCreateType.eOnly, isLoop, delayTime);
    }

    public int PlayAudio2D(AudioType type, string name, bool isLoop = false, float delayTime = 0.0f)
    {
        return PlayAudio(type, name, AudioSpaceType.e2D, AudioCreateType.eNew, isLoop, delayTime);
    }

    //播放3d音效
    public int PlayerAudio3D(AudioType type, string name, bool isLoop = false, float delayTime = 0.0f, GameObject obj = null, float maxRange = 0.0f)
    {
        return PlayAudio(type, name, AudioSpaceType.e3D, AudioCreateType.eNew, isLoop, delayTime, obj, maxRange);
    }

    //播放音效
    //type 类型
    //spaceType 空间类型(2D或者3D)
    //createType 创建类型(eOnly：播放音乐都会使用同一个播放器  eNew : 每次播放可能使用不同的播放器)
    //name 音频名字
    //delayTime 延时时间
    //obj 3d音效需要指定obj对象
    //maxRange 3D音效的最大范围
    //isLoop 是否循环
    public int PlayAudio(AudioType type, string name,
        AudioSpaceType spaceType = AudioSpaceType.e2D,
        AudioCreateType createType = AudioCreateType.eOnly,
        bool isLoop = false,
        float delayTime = 0.0f, GameObject obj = null, float maxRange = 0.0f)
    {
        if (string.IsNullOrEmpty(name))
        {
            return -1;
        }

        //重置状态和缓存名字
        AudioTypeData audioTypeData = GetAudioTypeData(type);
        audioTypeData._IsStop = false;
        //获取播放器
        AudioData audioData = GetAudioSource(type, spaceType, createType, obj, maxRange, isLoop);
        audioData.mAudioNameList.Add(name);
        //加载音频,然后播放音频
        GetAudioClip(name, (audioClip) =>
        {
            if (audioClip == null)
            {
                return;
            }

            //判断播放前,是否外部已经停止该类型音乐
            if (audioTypeData._IsStop)
            {
                return;
            }

            audioData.mAudioSource.clip = audioClip;
            audioData.mAudioSource.playOnAwake = false;

            if (Mathf.Abs(delayTime) < Mathf.Epsilon)
            {
                audioData.mAudioSource.Play();
            }
            else
            {
                audioData.mTimerID = TimerManager.Instance.AddTimer(delayTime, 1, () =>
                {
                    if (audioTypeData != null && audioTypeData._IsStop)
                    {
                        return;
                    }

                    audioData.mAudioSource.Play();
                    audioData.mTimerID = 0;
                });
            }
        });

        return audioData.mID;
    }

    //更新
    private void Update()
    {
        mUpdateDelta += Time.deltaTime;
        if (mUpdateDelta <= 0.5f)
        {
            return;
        }
        else
        {
            mUpdateDelta = 0.0f;
        }

        if (mAudioListenerFollowObj != null)
        {
            mAudioListener.transform.position = mAudioListenerFollowObj.transform.position;
        }

        //进行同步坐标
        foreach (var item in mAudioTypeDataDict)
        {
            List<AudioData> audioDataList = item.Value.mAudioDataList;
            int count = audioDataList.Count;
            for (int i = 0; i < count; i++)
            {
                //跟随对象由于是外部传入，可能已经被销毁了,所以要判空
                if (audioDataList[i].mFollowObj != null)
                {
                    audioDataList[i].mAudioSource.transform.position = audioDataList[i].mFollowObj.transform.position;
                }
            }
        }
    }

    private void OnApplicationPause(bool state)
    {
        mAudioListenerObj.gameObject.SetActive(!state);
    }
}

