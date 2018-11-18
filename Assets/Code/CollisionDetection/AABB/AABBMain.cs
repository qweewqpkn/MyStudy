using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBMain : MonoBehaviour{
    public GameObject mObj;

	// Use this for initialization
	void Start () {
        for(int i = 0; i < 1; i++)
        {
            World.Instance.CreateObj(mObj);
        }
    }
	
	// Update is called once per frame
	void Update () {

    }
}
