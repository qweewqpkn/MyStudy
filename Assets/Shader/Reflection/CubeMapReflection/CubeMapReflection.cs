using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMapReflection : MonoBehaviour {

    public Transform mViewTrans;
    public Material mMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mMaterial.SetVector("_ViewDir", new Vector4(mViewTrans.position.x, mViewTrans.position.y, mViewTrans.position.z, 0.0f));

    }
}
