using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidPlugin : MonoBehaviour {

    public Button mButton;

	// Use this for initialization
	void Start () {
        mButton.onClick.AddListener(() =>
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.test.androidplugin.ToolUtility");
            int value = jc.CallStatic<int>("Add", 1, 22);
            Debug.Log("### Add return value is : " + value);

            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("Instance");
            if(jo != null)
            {
                string value1 = jo.Call<string>("Test");
                Debug.Log("### Test return value is : " + value1);
            }
            else
            {
                Debug.Log("### mInstance jo is null");
            }
        });
    }

}
