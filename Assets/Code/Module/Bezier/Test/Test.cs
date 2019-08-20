using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject startObj;
    public GameObject targetObj;
    private MotionTrail mTrial;

	// Use this for initialization
	void Start () {
        mTrial = new MotionTrail();
        mTrial.Create(startObj, targetObj);
    }
	
	// Update is called once per frame
	void Update () {
        mTrial.Update();
    }
}
