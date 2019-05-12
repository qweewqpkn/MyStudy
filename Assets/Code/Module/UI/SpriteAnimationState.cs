using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationState{
    public SpriteAnimationClip Clip
    {
        get;
        set;
    }

    public SpriteAnimation.AnimationWrapMode WrapMode
    {
        get;
        set;
    }

    public float Length
    {
        get
        {
            if(Clip != null)
            {
                return Clip.SpriteCount * 1.0f / Clip.FPS;
            }

            return 0;
        }
    }

    private float mSpeed = 1;
    public float Speed
    {
        get
        {
            return mSpeed;
        }
        set
        {
            mSpeed = value;
        }
    }

    private float mNormalizedTime = 0;
    public float NormalizedTime
    {
        get
        {
            mNormalizedTime = Mathf.Floor(CurTime / Length) + CurTime % Length / Length;
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

    public bool IsOver
    {
        get;
        set;
    }

    public void Update()
    {
        if(IsOver)
        {
            return;
        }

        switch(WrapMode)
        {
            case SpriteAnimation.AnimationWrapMode.eLoop:
                {
                    CurTime = CurTime + Time.deltaTime * Speed;
                }
                break;
            case SpriteAnimation.AnimationWrapMode.eOnce:
                {
                    if (CurTime < Length)
                    {
                        CurTime = CurTime + Time.deltaTime * Speed;
                        CurTime = Mathf.Clamp(CurTime, CurTime, Length);
                    }    
                    else
                    {
                        Stop();
                    }
                }
                break;
        }
    }

    public void Start()
    {
        IsOver = false;
        CurTime = 0;
    }

    public void Stop()
    {
        IsOver = true;
        CurTime = 0;
    }

    public Sprite Sample()
    {
        if(Clip != null)
        {
            int index = (int)(CurTime * Clip.FPS) % Clip.SpriteCount;
            return Clip.GetSprite(index);
        }

        return null;
    }
}
