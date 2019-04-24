using UnityEngine;
using System.Collections;

public class uv_start_at_top : MonoBehaviour {

    public Material material;
	// Use this for initialization
	void Start () {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnRenderImage(RenderTexture src, RenderTexture des)
    {
        Graphics.Blit(src, des, material);
    }
}
