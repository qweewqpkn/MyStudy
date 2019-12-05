using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstances : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        GameObject newObj = GameObject.Instantiate(obj);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
