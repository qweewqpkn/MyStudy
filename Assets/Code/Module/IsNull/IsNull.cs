using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsNull : MonoBehaviour {
    public GameObject obj = null;

	// Use this for initialization
	void Start () {
        if(obj)
        {
            Debug.Log("it is not null");
        }
        else
        {
            Debug.Log("it is null");
        }

        Debug.Log("obj == null : " + (obj == null));
        Debug.Log("(object)obj == null : " + ((object)obj == null));
        Debug.Log("object.ReferenceEquals(obj, null) : " + object.ReferenceEquals(obj, null));

        obj = null;

        Debug.Log("obj == null : " + (obj == null));
        Debug.Log("(object)obj == null : " + ((object)obj == null));
        Debug.Log("object.ReferenceEquals(obj, null) : " + object.ReferenceEquals(obj, null));
    }
}
