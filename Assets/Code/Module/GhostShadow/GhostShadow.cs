using System.Collections.Generic;
using UnityEngine;

public class GhostShadow : MonoBehaviour {

    private List<Material> mMaterialList = new List<Material>();
    private List<GameObject> mGhostShadowRemoveList = new List<GameObject>();
    private float mHoldTime = 0.0f; //残影持续时间
    private float mTotalTime = 0.0f;
    private Vector4 mBeginColor = Vector4.zero;
    private Vector4 mEndColor = Vector4.zero;
    private float mFreneselIntensity = 0.0f;

    private static AnimationState GetCurPlayAnimation(Animation animation)
    {
        foreach (AnimationState state in animation)
        {
            if (animation.IsPlaying(state.name))
            {
                return state;
            }
        }

        return null;
    }

    public static void CreateGhostShadow(GameObject obj, float holdTime, float freneselIntensity, Vector4 beginColor, Vector4 endColor)
    {
        Animation animation = obj.GetComponentInChildren<Animation>();
        if(animation == null)
        {
            return;
        }

        AnimationState curState = GetCurPlayAnimation(animation);
        if (curState != null)
        {
            //clone对象
            GameObject gsObj = Instantiate(obj);
            gsObj.transform.parent = null;
            gsObj.transform.name = obj.transform.name + "(GhostShadow)";
            gsObj.transform.position = obj.transform.position;
            gsObj.transform.rotation = obj.transform.rotation;
            GhostShadowCreator gsc = gsObj.GetComponentInChildren<GhostShadowCreator>();
            if(gsc != null)
            {
                gsc.enabled = false;
            }

            //初始化残影数据
            GhostShadow gsScrpit = gsObj.AddComponent<GhostShadow>();
            gsScrpit.SetHoldTime(holdTime);
            gsScrpit.SetFreneselIntensity(freneselIntensity);
            gsScrpit.SetBeginColor(beginColor);
            gsScrpit.SetEndColor(endColor);

            //定格动画
            Animation gsAnimation = gsObj.GetComponentInChildren<Animation>();
            if (gsAnimation != null)
            {
                gsAnimation[curState.name].time = curState.time;
                gsAnimation[curState.name].speed = 0;
                gsAnimation.Play(curState.name);
            }
        }
    }

    // Use this for initialization
    void Start () {
        //改变残影对象的shader
        Renderer[] gsRenderList = GetComponentsInChildren<Renderer>();
        if(gsRenderList != null)
        {
            for (int i = 0; i < gsRenderList.Length; i++)
            {
                Material material = new Material(Shader.Find("Custom/GhostShadow"));
                gsRenderList[i].material = material;
                material.SetFloat("_freneselIntensity", mFreneselIntensity);
                mMaterialList.Add(material);
            }
        }
    }

    //设置残影持续时间
    void SetHoldTime(float holdTime)
    {
        mHoldTime = holdTime;
    }

    //设置起始颜色
    void SetBeginColor(Vector4 color)
    {
        mBeginColor = color;
    }

    //设置结束颜色
    void SetEndColor(Vector4 color)
    {
        mEndColor = color;
    }

    //设置菲尼尔的强度
    void SetFreneselIntensity(float intensity)
    {
        mFreneselIntensity = intensity;
    }

    // Update is called once per frame
    void Update () {

        mTotalTime += Time.deltaTime;
        for (int i = 0; i < mMaterialList.Count; i++)
        {
            Vector4 color = Vector4.Lerp(mBeginColor, mEndColor, Mathf.Min(mTotalTime / mHoldTime, 1.0f));
            mMaterialList[i].SetVector("_color", color);
            if(mTotalTime >= mHoldTime)
            {
                mGhostShadowRemoveList.Add(gameObject);
            }
        }

        int removeNum = mGhostShadowRemoveList.Count;
        if (removeNum > 0)
        {
            for (int i = 0; i < removeNum; i++)
            {
                GameObject.DestroyImmediate(mGhostShadowRemoveList[i]);
            }

            mGhostShadowRemoveList.Clear();
        }
	}
}
