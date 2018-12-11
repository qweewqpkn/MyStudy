using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard2 : MonoBehaviour {

    public Camera mCamera;
    public Material mMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mMaterial.SetVector("_ViewRightDir", mCamera.transform.right);
        mMaterial.SetVector("_ViewUpDir", mCamera.transform.up);
    }
}
