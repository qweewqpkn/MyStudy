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
    private Dictionary<string, AudioTypeData> mAudioTypeDataDict = new Dictionary<string, AudioTypeData>();

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

    public enum AudioSourceState
    {
        eIdel,
        eReady,
        ePlay,
    }

    public class AudioData
    {
        public AudioData()
        {
            mAudioNameList = new List<string>();
        }

        //唯一id
        public int mID;
        //播放器状态
        public AudioSourceState mAudioState; 
        //播放器本身
        public AudioSource mAudioSource;
        //类型
        public string mType;
        //空间类型
        public AudioSpaceType mSpaceType;
        //创建类型
        public AudioCreateType mCreateType;
        //该播放跟随的对象(3D播放器才会有跟随对象)
        public GameObject mFollowObj;
        //计时器id
        public long mTimerID;
        //循环与否
        public bool mLoop;
        //最大距离
        public float mMaxRange;
        //播放过的音频列表(主要用于释放这些播放过的音频资源)
        public List<string> mAudioNameList;
        //当前播放器播放的音频名字
        public string PlayClipName
        {
            get
            {
                if(mAudioSource != null && mAudioSource.clip != null)
                {
                    return mAudioSource.clip.name;
                }

                return "";
            }
        }
    }

    //类型数据，包含多个播放器
    public class AudioTypeData
    {
        public AudioTypeData()
        {
            mAudioDataList = new List<AudioData>();
            mIsMute = false;
        }
        //包含具体播放器的信息
        public List<AudioData> mAudioDataList;

        //该类型是否静音
        private bool mIsMute;
        public bool IsMute
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
        public bool IsStop
        {
            get
            {
                return mIsStop;
            }

            set
            {
                mIsStop = value;
                if(mIsStop)
                {
                    for (int i = 0; i < mAudioDataList.Count; i++)
                    {
                        //延时的播放要停止掉
                        if (mAudioDataList[i].mTimerID != 0)
                        {
                            TimeInfoManager.instance.StopTimer(mAudioDataList[i].mTimerID);
                            mAudioDataList[i].mTimerID = 0;
                        }

                        mAudioDataList[i].mAudioSource.Stop();
                    }
                }
            }
        }

        //该类型的音量
        private float mVolume;
        public float Volume
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
    private AudioData CreateAudioData(string type, AudioSpaceType spaceType, AudioCreateType createType, AudioTypeData data, GameObject targetObj, float maxRange, bool isLoop)
    {
        AudioData audioData = new AudioData();
        audioData.mID = mID++;
        audioData.mFollowObj = targetObj;
        audioData.mTimerID = 0;
        audioData.mAudioState = AudioSourceState.eReady;
        audioData.mType = type;
        audioData.mSpaceType = spaceType;
        audioData.mCreateType = createType;
        audioData.mLoop = isLoop;
        audioData.mMaxRange = maxRange;

        GameObject obj = new GameObject();
        obj.transform.SetParent(transform, false);
        obj.name = type.ToString() + audioData.mID;
        audioData.mAudioSource = obj.AddComponent<AudioSource>();
        audioData.mAudioSource.mute = data.IsMute;
        audioData.mAudioSource.volume = data.Volume;

        if(spaceType == AudioSpaceType.e2D)
        {
            audioData.mAudioSource.spatialBlend = 0.0f;
            audioData.mAudioSource.loop = isLoop;
        }
        else if(spaceType == AudioSpaceType.e3D)
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

    //为了将lua的音频类型数据初始化到c#这边
    public void InitAudioTypeData(string type)
    {
        AudioTypeData data = GetAudioTypeData(type);
    }

    private AudioTypeData GetAudioTypeData(string type)
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
        foreach(var item in mAudioTypeDataDict)
        {
            AudioTypeData data = GetAudioTypeData(item.Key);
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

    //获取播放器
    private AudioData GetAudioSource(string type, AudioSpaceType spaceType, AudioCreateType createType, GameObject targetObj, float maxRange, bool isLoop)
    {
        //获取指定type的数据
        AudioTypeData data = GetAudioTypeData(type);
        //获取播放器
        AudioData audioData = null;

        //寻找空闲播放器
        bool bFree = false;
        for (int i = 0; i < data.mAudioDataList.Count; i++)
        {
            if(createType == AudioCreateType.eOnly)
            {
                if(data.mAudioDataList[i].mCreateType == createType)
                {
                    bFree = true;
                    audioData = data.mAudioDataList[i];
                    break;
                }
            }
            else if(createType == AudioCreateType.eNew)
            {
                //没有播放并且不是延时使用且不是同一帧(因为同一帧isPlaying判断不了)
                //并且各个参数相同，表明可以复用
                if (!data.mAudioDataList[i].mAudioSource.isPlaying &&
                    data.mAudioDataList[i].mAudioState == AudioSourceState.eIdel &&
                    data.mAudioDataList[i].mSpaceType == spaceType &&
                    data.mAudioDataList[i].mCreateType == createType)
                {
                    bFree = true;
                    data.mAudioDataList[i].mAudioState = AudioSourceState.eReady;
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

        ResourceManager.instance.ReqResource(name.ToLower(), ResType.SOUND,
        (res) =>
        {
            HSound sound = res as HSound;
            onComplete(sound.m_AudioClip);
        },
        (res) =>
        {
            onComplete(null);
        });
    }

    //释放指定名字的音频资源
    private void DisposeRes(string name)
    {
        //if (mAudioClipDict.ContainsKey(name))
        {
            ResourceManager.instance.DestoryRes(name.ToLower(), ResType.SOUND);
            //mAudioClipDict.Remove(name);
        }
    }

    //释放特定类型的所有音频资源
    public void Dispose(string type)
    {
        AudioTypeData data = GetAudioTypeData(type);
        //遍历指定类型的所有播放器
        for (int i = 0; i < data.mAudioDataList.Count;i++)
        {
            //遍历指定播放器的所有播放记录
            for(int j = 0; j < data.mAudioDataList[i].mAudioNameList.Count; j++)
            {
                string name = data.mAudioDataList[i].mAudioNameList[j];
                DisposeRes(name);
            }
            data.mAudioDataList[i].mAudioNameList.Clear();
        }
    }

    //释放指定播放器中指定name的资源,如果name为空,那么释放该播放器的所有播放记录
    public void Dispose(int id, string name)
    {
        AudioData data = GetAudioData(id);
        if(data != null)
        {
            if (string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < data.mAudioNameList.Count; i++)
                {
                    DisposeRes(data.mAudioNameList[i]);
                }
                data.mAudioNameList.Clear();
            }
            else
            {
                for (int i = 0; i < data.mAudioNameList.Count;)
                {
                    if (data.mAudioNameList[i] == name)
                    {
                        DisposeRes(name);
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
    public void StopAndDisposeAudio(string type)
    {
        StopAudio(type);
        Dispose(type);
    }

    public void StopAndDisposeAudio(int id, string name)
    {
        StopAudio(id, name);
        Dispose(id, name);
    }

    //停止并且释放所有音频
    public void StopAndDisposeAll()
    {
        foreach(var item in mAudioTypeDataDict)
        {
            StopAndDisposeAudio(item.Key);
        }
    }

    //停止指定类型的播放器
    public void StopAudio(string type)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data.IsStop = true;
    }

    //停止指定id的播放器的指定名字音乐
    public void StopAudio(int id, string name)
    {
        AudioData data = GetAudioData(id);
        if(data != null)
        {
            if(string.IsNullOrEmpty(name))
            {
                data.mAudioSource.Stop();
            }
            else
            {
                if(data.PlayClipName == name)
                {
                    data.mAudioSource.Stop();
                }
            }
        }
    }

    //静音
    public void MuteAudio(string type, bool bMute)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data.IsMute = bMute;
    }

    //静音所有
    public void MuteAudioAll(bool bMute)
    {
        foreach (var item in mAudioTypeDataDict)
        {
            MuteAudio(item.Key, bMute);
        }
    }

    //设置指定类型音频的音量
    public void SetAudioVolume(string type, float volume)
    {
        AudioTypeData data = GetAudioTypeData(type);
        data.Volume = volume;
    }

    //获取音频的长度
    public void GetAudioClipLength(string name, Action<float> onComplete)
    {
        GetAudioClip(name, (audioClip) =>
        {
            ResourceManager.instance.DestoryRes(name.ToLower(), ResType.SOUND);
            float length = 0;
            if (audioClip != null)
            {
                length = audioClip.length;
            }
            onComplete(length);
        });
    }

    //会过滤掉同一帧重复播放的音乐
    private void PlayAudioFilter(string type, string name, bool isLoop = false, float delayTime = 0.0f,
        AudioCreateType createType = AudioCreateType.eOnly,
        AudioSpaceType spaceType = AudioSpaceType.e2D, 
        GameObject obj = null, float maxRange = 0.0f)
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

        PlayAudio(type, name, isLoop, delayTime, createType, spaceType, obj, maxRange);
    }

    //播放音乐(使用唯一播放器，如背景)
    public int PlayAudioOnly(string type, string name, bool isLoop = false, float delayTime = 0.0f)
    {
        return PlayAudio(type, name, isLoop, delayTime, AudioCreateType.eOnly, AudioSpaceType.e2D);
    }

    //播放2d音乐
    public int PlayAudio2D(string type, string name, bool isLoop = false, float delayTime = 0.0f)
    {
        return PlayAudio(type, name, isLoop, delayTime, AudioCreateType.eNew, AudioSpaceType.e2D);
    }

    //播放3d音乐
    public int PlayAudio3D(string type, string name, GameObject obj, float maxRange, bool isLoop = false, float delayTime = 0.0f)
    {
        return PlayAudio(type, name, isLoop, delayTime, AudioCreateType.eNew, AudioSpaceType.e3D, obj, maxRange);
    }

    //播放音效
    //type 音频类型
    //spaceType 空间类型(2D或者3D)
    //createType 创建类型(eOnly：唯一播放器  eNew : 多个播放器)
    //name 音频名字
    //delayTime 延时时间
    //obj 3d音效需要指定obj对象
    //maxRange 3D音效的最大范围
    //isLoop 是否循环
    private int PlayAudio(string type, string name, bool isLoop = false, float delayTime = 0.0f,
        AudioCreateType createType = AudioCreateType.eOnly,
        AudioSpaceType spaceType = AudioSpaceType.e2D,
        GameObject obj = null, float maxRange = 0.0f)
    {
        if (string.IsNullOrEmpty(name))
        {
            return -1;
        }

        //重置状态和缓存名字
        AudioTypeData audioTypeData = GetAudioTypeData(type);
        audioTypeData.IsStop = false;
        //获取播放器
        AudioData audioData = GetAudioSource(type, spaceType, createType, obj, maxRange, isLoop);
        audioData.mAudioNameList.Add(name);
        //加载音频,然后播放音频
        GetAudioClip(name, (audioClip) =>
        {
            if(audioClip == null)
            {
                return;
            }

            //判断播放前,是否外部已经停止该类型音乐
            if(audioTypeData.IsStop)
            {
                return;
            }

            audioData.mAudioSource.clip = audioClip;
            audioData.mAudioSource.playOnAwake = false;

            if (Mathf.Abs(delayTime) < Mathf.Epsilon)
            {
                audioData.mAudioSource.Play();
                audioData.mAudioState = AudioSourceState.ePlay;
            }
            else
            {
                audioData.mTimerID = TimeInfoManager.instance.AddTimer(delayTime, () =>
                {
                    if (audioTypeData != null && audioTypeData.IsStop)
                    {
                        return;
                    }

                    audioData.mAudioSource.Play();
                    audioData.mAudioState = AudioSourceState.ePlay;
                    audioData.mTimerID = 0;
                }, 1);
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
        foreach(var item in mAudioTypeDataDict)
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

                if(audioDataList[i].mAudioState == AudioSourceState.ePlay)
                {
                    if (!audioDataList[i].mAudioSource.isPlaying)
                    {
                        audioDataList[i].mAudioState = AudioSourceState.eIdel;
                    }
                }
            }
        }
    }

    private void OnApplicationPause(bool state)
    {
        mAudioListenerObj.gameObject.SetActive(!state);
    }
}

