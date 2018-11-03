using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAndroid : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(100, 100, 100, 100),"点击"))
        {
#if UNITY_ANDROID
            //拿到对应的java类
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //拿到这个java类中的静态对象
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //调用这个java对象中的函数，这个java对象的类型应该就是我们定义的MainActivity
            activity.Call("ShowMessage", "hello world");
#endif
        }
    }
}
