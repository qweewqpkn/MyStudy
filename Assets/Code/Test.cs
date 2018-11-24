using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A
{
    public virtual T GetObj<T>() where T : new()
    {
        Debug.Log("A CALL");
        return new T();
    }
}

public class B : A
{
    public override T GetObj<T>()
    {
        Debug.Log("B call");
        return new T();
    }
}

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        A a = new B();
        a.GetObj<Texture>();

        A a1 = new A();
        a1.GetObj<AudioClip>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
