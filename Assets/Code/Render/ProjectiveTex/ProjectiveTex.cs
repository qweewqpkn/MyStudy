using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectiveTex : MonoBehaviour {

    public Camera mProjectiveCamera;
    public Material mMaterial;
    private Matrix4x4 mScaleMat;

    // Use this for initialization
    void Start () {
        Vector4 v1 = new Vector4(0.5f, 0, 0, 0);
        Vector4 v2 = new Vector4(0, 0.5f, 0, 0);
        Vector4 v3 = new Vector4(0, 0, 0.5f, 0);
        Vector4 v4 = new Vector4(0.5f, 0.5f, 0.5f, 1);
        mScaleMat = new Matrix4x4(v1, v2, v3, v4);
	}
	
	// Update is called once per frame
	void Update () {

        mMaterial.SetMatrix("_ProjectMatrix", mScaleMat * GL.GetGPUProjectionMatrix(mProjectiveCamera.projectionMatrix, false) * mProjectiveCamera.worldToCameraMatrix);
    }
}
