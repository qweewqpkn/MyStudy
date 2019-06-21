using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestLoopRect : MonoBehaviour {

    public LoopScrollRect mLoopRect;
    public Button mAddBtn;
    public Button mReduceBtn;
    public int mNum;
    private List<int> mTypeList = new List<int>();

	// Use this for initialization
	void Start () {
        mAddBtn.onClick.AddListener(() =>
        {
            mNum = mNum + 5;
            for(int i = 0; i < mNum; i++)
            {
                mTypeList.Add(Random.Range(0, 2));
            }
            mLoopRect.Init(mNum, (t, t1)=>
            {
                t.Get<TextMeshProUGUI>("b_text").text = t1.ToString();
            }, mTypeList);
        });

        mReduceBtn.onClick.AddListener(() =>
        {
            mNum = mNum - 5;
            mLoopRect.Init(mNum, (t, t1) =>
            {
                t.Get<TextMeshProUGUI>("b_text").text = t1.ToString();
                //Debug.Log(t1);
            });
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
