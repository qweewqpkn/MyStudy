using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour {

    private SpriteAnimationState mCurState;
    private Dictionary<string, SpriteAnimationState> mStateMap = new Dictionary<string, SpriteAnimationState>();

    public void Init(List<SpriteAnimationAction> actionList)
    {
        for(int i = 0;  i < actionList.Count; i++)
        {
            SpriteAnimationState state = new SpriteAnimationState();
            state.Init(actionList[i]);
        }
    }

    public void Play(string name)
    {
        if(mStateMap.ContainsKey(name))
        {
            mCurState = mStateMap[name];
        }
    }

    public void Update()
    {
        if(mCurState != null)
        {
            mCurState.Update();
            mCurState.Sample();
        }
    }

    public void OnDestroy()
    {
        
    }
}
