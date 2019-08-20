using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetPixel : MonoBehaviour {
    //主要测试：当贴图贴到mesh上的时候，如果mesh的大小变大和变小，我们设置的单独的像素块也会进行拉伸，动态匹配
    public Texture2D texture;
    public Material material;

	// Use this for initialization
	void Start () {
        texture = new Texture2D(2, 2);
        texture.filterMode = FilterMode.Point;
        for(int i = 0; i < texture.width; i++)
        {
            for(int j = 0; j < texture.height; j++)
            {
                if(i == 0 && j == 0)
                {
                    texture.SetPixel(i, j, new Color(1, 1, 1, 1));
                }
                else
                {
                    texture.SetPixel(i, j, new Color(0, 0, 0, 1));
                }
            }
        }

        material.mainTexture = texture;
        texture.Apply();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
