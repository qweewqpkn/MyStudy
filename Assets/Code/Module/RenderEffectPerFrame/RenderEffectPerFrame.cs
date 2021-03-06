﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEffectPerFrame : MonoBehaviour {

    public Camera camera;
    public int width;
    public int height;
    public int frame;
    private int i = 0;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if(i < frame)
        {
            i++;
            camera.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            camera.Render();
            TextureUtility.SaveRenderTexture(camera.targetTexture, width, height, TextureUtility.TexFormat.ePNG, Application.dataPath + "/TextureSave/" + i + ".png");
            if(i == frame)
            {
                //AssetDatabase.Refresh();
            }
        }
	}
}
