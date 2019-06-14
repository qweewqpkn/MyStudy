using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoopRect : MonoBehaviour {

    public LoopScrollRect mLoopRect;
    public Button mAddBtn;
    public Button mReduceBtn;
    public int mNum;

	// Use this for initialization
	void Start () {
        mAddBtn.onClick.AddListener(() =>
        {
            mNum = mNum + 5;
            mLoopRect.Init(mNum, (t, t1)=>
            {
                Debug.Log(t1);
            });
        });

        mReduceBtn.onClick.AddListener(() =>
        {
            mNum = mNum - 5;
            mLoopRect.Init(mNum, (t, t1) =>
            {
                Debug.Log(t1);
            });
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
