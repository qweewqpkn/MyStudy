using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectTransform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        RectTransform rt = transform as RectTransform;
        GUILayout.Label("rect : " + rt.rect);
        GUILayout.Label(string.Format("min : {0}, max : {1}, center : {2}", rt.rect.min, rt.rect.max, rt.rect.center));
    }
}
