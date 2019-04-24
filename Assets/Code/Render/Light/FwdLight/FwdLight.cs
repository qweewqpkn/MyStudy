using UnityEngine;
using System.Collections;

public class FwdLight : MonoBehaviour {

    public Light m_pointLight;
    public Material m_material;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        m_material.SetFloat("pointRange", m_pointLight.range);
    }
}
