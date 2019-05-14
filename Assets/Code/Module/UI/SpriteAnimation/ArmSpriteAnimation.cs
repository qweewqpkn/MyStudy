using AssetLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSpriteAnimation : MonoBehaviour {
    private string mAtlasName;
    private int mDir;
    private int mFPS;
    private SpriteAnimation.AnimationWrapMode mWrapMode;
    private SpriteAnimation mAnimation;

	public void Init(string atlasName, int FPS, SpriteAnimation.AnimationWrapMode wrapMode, Action callback)
    {
        mAtlasName = atlasName;
        mFPS = FPS;
        mWrapMode = wrapMode;
        Parse(atlasName, FPS, mWrapMode, callback);
    }

    private void Parse(string atlasName, int FPS, SpriteAnimation.AnimationWrapMode wrapMode, Action callback)
    {
        ResourceManager.Instance.LoadSpriteAtlasAsync(atlasName, (spriteList) =>
        {
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

            mAnimation =  gameObject.AddComponent<SpriteAnimation>();
            mAnimation.Init(clipList);

            if(callback != null)
            {
                callback();
            }
        });
    }

    private string GetClipName(string action, string dir)
    {
        return string.Format("{0}_{1}", action, dir);
    }

    public void SetDir(int dir)
    {
        mDir = dir;
    }

    public void Play(string action, bool isBack = false)
    {
        string name = GetClipName(action, mDir.ToString());
        if(mAnimation != null)
        {
            mAnimation.Play(name, isBack);
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
