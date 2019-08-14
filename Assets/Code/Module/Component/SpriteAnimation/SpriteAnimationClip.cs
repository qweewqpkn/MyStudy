using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationClip {
    public string Name
    {
        get;
        set;
    }

    public float Length
    {
        get;
        set;
    }

    public SpriteAnimation.AnimationWrapMode WrapMode
    {
        set;
        get;
    }
        

    public List<Sprite> SpriteList
    {
        get;
        set;
    }

    public int SpriteCount
    {
        get
        {
            return SpriteList.Count;
        }
    }

    public Sprite GetSprite(int index)
    {
        return SpriteList[index];
    }


}
