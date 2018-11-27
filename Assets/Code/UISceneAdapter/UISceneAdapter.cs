using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneAdapter : MonoBehaviour {

    public CanvasScaler mCanvasScaler;

    // Use this for initialization
    void Start () {
        mCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        mCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //大于使用高度匹配，小于使用宽度匹配
        mCanvasScaler.matchWidthOrHeight = Screen.width / Screen.height > mCanvasScaler.referenceResolution.x / mCanvasScaler.referenceResolution.y ? 1.0f : 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

	}
}
