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

    private SpriteRenderer mSpriteRenderer;
    private SpriteAnimationState mCurState;
    private Dictionary<string, SpriteAnimationState> mStateMap = new Dictionary<string, SpriteAnimationState>();

    public void Init(List<SpriteAnimationClip> clipList)
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0;  i < clipList.Count; i++)
        {
            SpriteAnimationState state = new SpriteAnimationState();
            state.Clip = clipList[i];
            mStateMap.Add(clipList[i].Name, state);
        }
    }

    public void Play(string name)
    {
        if(mStateMap.ContainsKey(name))
        {
            if(mCurState != null)
            {
                if (mCurState != mStateMap[name])
                {
                    //切换动画
                    mCurState.Stop();
                    mCurState = mStateMap[name];
                    mCurState.Start();
                }
                else
                {
                    if(mCurState.IsOver)
                    {
                        //播放完了,重新播放
                        mCurState.Start();
                    }
                }
            }
            else if(mCurState == null)
            {
                //第一次播放
                mCurState = mStateMap[name];
                mCurState.Start();
            }
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
