using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarShadow : MonoBehaviour {

    public Material mPlanarMaterial;
    public Transform mLightTrans;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mPlanarMaterial.SetVector("_LightPos", mLightTrans.position);

    }
}
