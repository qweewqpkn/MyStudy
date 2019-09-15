using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTransform : MonoBehaviour {

    public Transform parent;
    public Transform child;

	// Use this for initialization
	void Start () {
        child.SetParent(parent, false);
    }
}
