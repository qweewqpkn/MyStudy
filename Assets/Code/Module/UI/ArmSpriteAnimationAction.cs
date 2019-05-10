using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimation))]
public class ArmSpriteAnimationAction : SpriteAnimationAction {
    private SpriteRenderer mSpriteRenderer;
    private float mLength;
    private string mAtlasName;
    private int mDir;
    private string mAction;
    private Dictionary<string, Dictionary<int, List<Sprite>>> spriteMap = new Dictionary<string, Dictionary<int, List<Sprite>>>();

	public void Init(string atlasName, float length)
    {
        mAtlasName = atlasName;
        mLength = length;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        Parse(atlasName);
    }

    public void Parse(string atlasName)
    {

    }

    public void SetDir(int dir)
    {
        mDir = dir;
    }

    public void SetAction(string name)
    {
        mAction = name;
    }

    public override void Trigger(float normalizeTime)
    {
        if(mSpriteRenderer == null)
        {
            return;
        }

        if(spriteMap.ContainsKey(mAction))
        {
            if(spriteMap[mAction].ContainsKey(mDir))
            {
                int count = spriteMap[mAction][mDir].Count;
                int index = Mathf.FloorToInt(Mathf.Lerp(0, count, normalizeTime));
                Sprite sprite = spriteMap[mAction][mDir][index];
                mSpriteRenderer.sprite = sprite;
            }
        }
    }
}
