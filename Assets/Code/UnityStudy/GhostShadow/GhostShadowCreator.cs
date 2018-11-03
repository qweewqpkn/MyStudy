using System.Collections.Generic;
using UnityEngine;

public class GhostShadowCreator : MonoBehaviour
{ 

    [System.Serializable]
    public class GhostShadowData
    {
        public string mAnimationName;
        public float mStartTime;
        public float mEndTime;
        public float mHoldTime;
        public float mFreneselIntensity;
        public int mGhostShadowNum;
        public Color mBeginColor;
        public Color mEndColor;
        [System.NonSerialized]
        public float mTiming;

        public float createDelta
        {
            get
            {
                float differTime = mEndTime - mStartTime;
                if(differTime > 0)
                {
                    return differTime / mGhostShadowNum;
                }

                return 0;
            }
        }
    }

    public List<GhostShadowData> mGhostShadowAnimationList;
    private Animation mAnimation;

    void Start()
    {
        mAnimation = GetComponent<Animation>();
        for (int i = 0; i < mGhostShadowAnimationList.Count; i++)
        {
            GhostShadowData data = mGhostShadowAnimationList[i];
            AnimationEvent[] animationEvents = new AnimationEvent[data.mGhostShadowNum];
            for(int j = 0; j < animationEvents.Length; j++)
            {
                animationEvents[j] = new AnimationEvent();
                animationEvents[j].functionName = "CreateGhostShadow";
                animationEvents[j].time = data.mStartTime + j * data.createDelta;
                animationEvents[j].stringParameter = string.Format("chixushijian(s):{0};bianyuankuandu:{1};kaishiyanse(RGBA):{2},{3},{4},{5};jieshuyanse(RGBA):{6},{7},{8},{9}", 
                    data.mHoldTime, data.mFreneselIntensity, 
                    data.mBeginColor.r, data.mBeginColor.g, data.mBeginColor.b, data.mBeginColor.a, 
                    data.mEndColor.r, data.mEndColor.g, data.mEndColor.b, data.mEndColor.a);
            }

            mAnimation[data.mAnimationName].clip.events = animationEvents;
        }
    }

    public void CreateGhostShadow(string s)
    {
        //解析数据
        float holdTime = 0;
        float freneselIntensity = 0;
        Vector4 beginColor = Vector4.zero;
        Vector4 endColor = Vector4.zero;

        string[] paramsList = s.Split(';');
        for (int i = 0; i < paramsList.Length; i++)
        {
            string[] nameValueList = paramsList[i].Split(':');
            switch (nameValueList[0])
            {
                case "chixushijian(s)":
                    {
                        holdTime = float.Parse(nameValueList[1]);
                    }
                    break;
                case "bianyuankuandu":
                    {
                        freneselIntensity = float.Parse(nameValueList[1]);
                    }
                    break;
                case "kaishiyanse(RGBA)":
                    {
                        beginColor = ParaseVector4(nameValueList[1]);
                    }
                    break;
                case "jieshuyanse(RGBA)":
                    {
                        endColor = ParaseVector4(nameValueList[1]);
                    }
                    break;
            }
        }

        GhostShadow.CreateGhostShadow(gameObject, holdTime, freneselIntensity, beginColor, endColor);
    }

    private static Vector4 ParaseVector4(string s)
    {
        //s格式:x,x,x,x
        float x = 0;
        float y = 0;
        float z = 0;
        float w = 0;

        string[] valueList = s.Split(',');
        if (valueList.Length == 4)
        {
            x = float.Parse(valueList[0]);
            y = float.Parse(valueList[1]);
            z = float.Parse(valueList[2]);
            w = float.Parse(valueList[3]);
        }

        return new Vector4(x, y, z, w);
    }
}