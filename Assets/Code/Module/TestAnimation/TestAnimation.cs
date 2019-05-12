using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimation : MonoBehaviour {

    public Button mButton1;
    public Button mButton2;
    private Animation mAnimation;
    public bool mStart;

    // Use this for initialization
    void Start () {
        mAnimation = GetComponent<Animation>();
        mAnimation.wrapMode = WrapMode.Loop;

        mButton1.onClick.AddListener(() =>
        {
            Debug.LogError("Scale");
            mAnimation.Play("Scale");
        });

        mButton2.onClick.AddListener(() =>
        {
            Debug.LogError("Move");
            mStart = true;
            mAnimation.wrapMode = WrapMode.Once;
            mAnimation["Move"].speed = 0.1f;
            mAnimation.Play("Move");
        });
    }
	
	// Update is called once per frame
	void Update () {
        //if(mAnimation.IsPlaying("Move"))
        //{
        if(mStart)
        {
            //Debug.Log("mAnimation Time delta : " + Time.deltaTime);
            Debug.Log("mAnimation Move.time : " + mAnimation["Move"].time);
            Debug.Log("mAnimation Move.normalizedTime : " + mAnimation["Move"].normalizedTime);
            Debug.Log("mAnimation Cur state : " + mAnimation.IsPlaying("Move").ToString());
        }
        //}
    }
}
