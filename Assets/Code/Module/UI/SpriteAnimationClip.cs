using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationClip {

    private int mFrameRate;
    private float mLength;
    private Dictionary<string, List<Sprite>> mDirSpriteMap = new Dictionary<string, List<Sprite>>();

    public static void Parse(Dictionary<string, SpriteAnimationClip> actionClipMap, string atlasName)
    {
        List<Sprite> spriteList = new List<Sprite>();
        for(int i = 0; i < spriteList.Count; i++)
        {
            string[] names = spriteList[i].name.Split('_');
            if(names.Length >= 3)
            {
                string action = names[0];
                string dir = names[1];
                string index = names[2];
                SpriteAnimationClip clip = null;
                if (!actionClipMap.TryGetValue(action, out clip))
                {
                    clip = new SpriteAnimationClip();
                    actionClipMap.Add(action, clip);
                }
                clip.AddSprite(dir, spriteList[i]);
            }
        }
    }

    public static void Parse(Dictionary<string, SpriteAnimationClip> actionClipMap, List<string> atlasNameList)
    {
        for(int i = 0; i < atlasNameList.Count; i++)
        {
            Parse(actionClipMap, atlasNameList[i]);
        }
    }

    private void AddSprite(string dir, Sprite sprite)
    {
        List<Sprite> spriteList = null;
        if(!mDirSpriteMap.TryGetValue(dir, out spriteList))
        {
            spriteList = new List<Sprite>();
            mDirSpriteMap.Add(dir, spriteList);
        }
        spriteList.Add(sprite);
    }

    public Sprite GetSprite(string dir, float normalizeTime)
    {
        if (mDirSpriteMap.ContainsKey(dir))
        {
            int index = Mathf.FloorToInt(Mathf.Lerp(0, mDirSpriteMap[dir].Count, normalizeTime));
            return mDirSpriteMap[dir][index];
        }

        return null;
    }
}
