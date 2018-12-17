using UnityEngine;
using System.Collections;

public class RadialBlur : MonoBehaviour {

    public Material mMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnRenderImage(RenderTexture src, RenderTexture des)
    {
        Graphics.Blit(src, des, mMaterial);
    }
}
