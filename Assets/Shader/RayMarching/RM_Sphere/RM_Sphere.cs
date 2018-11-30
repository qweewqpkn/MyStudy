using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM_Sphere : MonoBehaviour {

    public Material mMaterial;

	// Use this for initialization
	void Start () {
		
	}


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mMaterial);
    }
}
