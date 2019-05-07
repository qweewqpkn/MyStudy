﻿using UnityEngine;
using System.Collections;

public class DepthToWorldPos : MonoBehaviour {

    public Material material;
    private Camera camera;

	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnRenderImage(RenderTexture source, RenderTexture target)
    {
        material.SetMatrix("ProjectInverseMatrix", (camera.projectionMatrix).inverse);
        material.SetMatrix("WroldToViewInverseMatrix", (camera.worldToCameraMatrix).inverse);
        Graphics.Blit(source, target, material);
    }
}