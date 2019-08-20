using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDOTween : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        obj.transform.DOMove(new Vector3(10, 0, 0), 5).SetEase(Ease.Linear);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
