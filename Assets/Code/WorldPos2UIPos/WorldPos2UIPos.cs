using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPos2UIPos : MonoBehaviour {

    public Text text;
    public Transform obj;
    public Camera uiCamera;
    public Camera sceneCamera;
    public Vector3 mUIWorldPos;

	// Use this for initialization
	void Start () {
        //通过屏幕坐标作为中介，来从一个摄像机转换到另外一个摄像机的空间，在各自摄像机下该对象与摄像机的位置保持相对不变
        //screenPos的z值是 obj距离sceneCamera摄像机位置的距离（不是欧式距离）
        Vector3 screenPos = sceneCamera.WorldToScreenPoint(obj.position);
        //screenPos.z = 0;
        //uiWorldPos的z值是：保持obj与uicamera距离为screenPos.z的情况下，此时obj所处的世界坐标下的z值
        Vector3 uiWorldPos = uiCamera.ScreenToWorldPoint(screenPos);
        mUIWorldPos = uiWorldPos;
        //注意：上面两个距离摄像机的距离是相等的

        Vector3 temp1 = uiCamera.WorldToViewportPoint(obj.position);
        Vector3 temp2 = uiCamera.WorldToScreenPoint(obj.position);

        //temp3是处于从世界坐标转换到摄像机空间下（右手坐标系）
        //temp4是处于从世界坐标转换到摄像机对象的局部坐标（左手坐标系）
        Vector3 temp3 = sceneCamera.worldToCameraMatrix.MultiplyPoint(obj.position);
        Vector3 temp4 = sceneCamera.transform.InverseTransformPoint(obj.position);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
