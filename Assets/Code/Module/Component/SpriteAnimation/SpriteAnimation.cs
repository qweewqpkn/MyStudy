using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour {
    public enum AnimationWrapMode
    {
        eOnce,
        eLoop,
    }

    private AnimationWrapMode mWrapMode = AnimationWrapMode.eOnce;
    public AnimationWrapMode WrapMode
    {
        get
        {
            return mWrapMode;
        }

        set
        {
            if(mStateMap != null)
            {
                foreach (var item in mStateMap)
                {
                    item.Value.WrapMode = value;
                }
            }
        }
    }

    public bool IsPlaying
    {
        get
        {
            if(mCurState == null)
            {
                return false;
            }
            else
            {
                return !mCurState.IsOver;
            }
        }
    }

    private Dictionary<string, SpriteAnimationState> mStateMap = new Dictionary<string, SpriteAnimationState>();
    public Dictionary<string, SpriteAnimationState> StateMap
    {
        get
        {
            return mStateMap;
        }
    }

    private SpriteRenderer mSpriteRenderer;
    private SpriteAnimationState mCurState;
    public SpriteAnimationState CurState
    {
        get
        {
            return mCurState;
        }
    }

    public void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(List<SpriteAnimationClip> clipList)
    {
        for (int i = 0;  i < clipList.Count; i++)
        {
            SpriteAnimationState state = new SpriteAnimationState();
            state.Clip = clipList[i];
            state.WrapMode = clipList[i].WrapMode;
            mStateMap.Add(clipList[i].Name, state);
        }
    }

    public void Play(string name, bool isFlipX, bool isBack = false)
    {
        mSpriteRenderer.flipX = isFlipX;
        if(mStateMap.ContainsKey(name))
        {
            if(mCurState != null)
            {
                if (mCurState != mStateMap[name])
                {
                    //切换动画
                    mCurState.Stop();
                    mCurState = mStateMap[name];
                    mCurState.IsBack = isBack;
                    mCurState.Start();
                }
                else
                {
                    if(mCurState.IsOver)
                    {
                        //播放完了,重新播放
                        mCurState.IsBack = isBack;
                        mCurState.Start();
                    }
                }
            }
            else if(mCurState == null)
            {
                //第一次播放
                mCurState = mStateMap[name];
                mCurState.IsBack = isBack;
                mCurState.Start();
            }
        }
    }

    public void Stop(string name)
    {
        if(mStateMap.ContainsKey(name))
        {
            mStateMap[name].Stop();
        }
    }

    public void Update()
    {
        if(mCurState != null && !mCurState.IsOver)
        {
            mCurState.Update();
            mSpriteRenderer.sprite = mCurState.Sample();
        }
    }
}
