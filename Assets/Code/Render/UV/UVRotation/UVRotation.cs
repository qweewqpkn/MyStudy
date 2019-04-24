using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVRotation : MonoBehaviour {


    public Material material;

	// Use this for initialization
	void Start () {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height);
        Graphics.Blit(source, destination, material, 0);
    }
}
