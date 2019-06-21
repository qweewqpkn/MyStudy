using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTransform : MonoBehaviour {

    public Button mAddBtn;

	// Use this for initialization
	void Start () {
        mAddBtn.onClick.AddListener(() =>
        {
            transform.position = Quaternion.Euler(0, 45, 0) * transform.position;
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {

    }
}
