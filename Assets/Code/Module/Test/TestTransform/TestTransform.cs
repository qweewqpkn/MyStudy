using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTransform : MonoBehaviour {

    public Button mAddBtn;
    public bool mState;

	// Use this for initialization
	void Start () {
        mAddBtn.onClick.AddListener(() =>
        {
            transform.position = Quaternion.Euler(0, 45, 0) * transform.position;
        });

        StartCoroutine(CoTest());
    }

    IEnumerator CoTest()
    {
        Debug.Log("hhhh : " + Time.frameCount);
        yield return CoTest1();
        Debug.Log("hhhh1 : " + Time.frameCount);
        yield return null;
    }

    IEnumerator CoTest1()
    {
        Debug.Log("1231!!!!! : " +  Time.frameCount);
        if(false)
        {
            Debug.Log("1231sdafadsf : " + Time.frameCount);
            yield return null;
        }
        mState = true;
        yield return new WaitForSeconds(5);
        Debug.Log("1231!!!!!1 : " + Time.frameCount);
    }
}
