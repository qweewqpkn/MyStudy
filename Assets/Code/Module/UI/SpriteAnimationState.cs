using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationState{
    private SpriteAnimationAction mAction;
    private float mLength;
    private string mName;
    private float mSpeed = 1;

    private float mNormalizedTime;
    public float NormalizedTime
    {
        get
        {
            mNormalizedTime = CurTime % mLength;
            return mNormalizedTime;
        }

        set
        {
            mNormalizedTime = value;
        }
    }

    public float CurTime
    {
        get;
        set;
    }

    public void Init(SpriteAnimationAction action)
    {
        mAction = action;
    }

    public void Update()
    {
        CurTime = CurTime + Time.deltaTime;
    }

    public void Sample()
    {
        if(mAction != null)
        {
            mAction.Trigger(NormalizedTime);
        }
    }
}
