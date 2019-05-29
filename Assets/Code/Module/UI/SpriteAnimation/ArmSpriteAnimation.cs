using AssetLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSpriteAnimation : MonoBehaviour {
    private string mAtlasName;
    private string mAction;
    private int mDir;
    private int mFPS;
    private SpriteAnimation.AnimationWrapMode mWrapMode;
    public SpriteAnimation Animation
    {
        get;
        private set;
    }

    public string ClipName
    {
        get
        {
            if(string.IsNullOrEmpty(mAction))
            {
                return "";
            }

            if(mDir == 0)
            {
                return "";
            }

            return GetClipName(mAction, mDir.ToString());
        }
    }

    private bool mFlipX;

	public void Init(string atlasName, int FPS, SpriteAnimation.AnimationWrapMode wrapMode)
    {
        mAtlasName = atlasName;
        mFPS = FPS;
        mWrapMode = wrapMode;
        Parse(atlasName, FPS, mWrapMode);
    }

    private void Parse(string atlasName, int FPS, SpriteAnimation.AnimationWrapMode wrapMode)
    {
        List<Sprite> spriteList = ResourceManager.Instance.LoadSpriteAtlas(atlasName);
        Dictionary<string, SpriteAnimationClip> clipMap = new Dictionary<string, SpriteAnimationClip>();
        for (int i = 0; i < spriteList.Count; i++)
        {
            if(spriteList[i] == null)
            {
                continue;
            }

            string[] names = spriteList[i].name.Split('_');
            string action = names[0];
            string dir = names[1];
            string index = names[2];
            string clipName = GetClipName(action, dir);
            SpriteAnimationClip clip = null;
            if (!clipMap.TryGetValue(clipName, out clip))
            {
                clip = new SpriteAnimationClip();
                clipMap[clipName] = clip;
                clip.Name = clipName;
                clip.FPS = FPS;
                clip.WrapMode = wrapMode;
                clip.SpriteList = new List<Sprite>();
            }

            clip.SpriteList.Add(spriteList[i]);
        }

        List<SpriteAnimationClip> clipList = new List<SpriteAnimationClip>();
        foreach (var item in clipMap)
        {
            clipList.Add(item.Value);
        }

        Animation =  gameObject.AddComponent<SpriteAnimation>();
        Animation.Init(clipList);
    }

    private string GetClipName(string action, string dir)
    {
        return string.Format("{0}_{1}", action, dir);
    }

    public void SetDir(int dir)
    {
        if(dir != mDir)
        {
            float time = 0;
            if(!string.IsNullOrEmpty(ClipName))
            {
                //记录当前动作下的时间
                if (Animation != null && Animation.StateMap[ClipName] != null)
                {
                    time = Animation.StateMap[ClipName].CurTime;
                }
            }

            if (dir == 7)
            {
                mDir = 4;
                mFlipX = false;
            }
            else if (dir > 3)
            {
                mDir = 6 - dir;
                mFlipX = true;
            }
            else
            {
                mDir = dir;
                mFlipX = false;
            }

            if (!string.IsNullOrEmpty(ClipName))
            {
                Play(mAction);
                if (Animation != null && Animation.StateMap[ClipName] != null)
                {
                    //由于只是切换方向，保持之前的时间继续播放
                    Animation.StateMap[ClipName].CurTime = time;
                }
            }
        }
    }

    public void Play(string action, bool isBack = false)
    {
        mAction = action;
        string name = GetClipName(action, mDir.ToString());
        if(Animation != null)
        {
            Animation.Play(name, mFlipX, isBack);
        }
    }

    public void Stop(string action)
    {
        if(action == mAction)
        {
            mAction = "";
        }
        string name = GetClipName(action, mDir.ToString());
        if (Animation != null)
        {
            Animation.Stop(name);
        }
    }

    private void OnDestroy()
    {
        if(!string.IsNullOrEmpty(mAtlasName))
        {
            ResourceManager.Instance.Release(mAtlasName, "*");
        }
    }
}
