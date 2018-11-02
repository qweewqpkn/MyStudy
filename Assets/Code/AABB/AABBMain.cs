using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBMain : MonoBehaviour {

    public GameObject mObj;
    private AABB mAABB;
    private MeshRenderer mMeshRenderer;

	// Use this for initialization
	void Start () {
        mMeshRenderer = mObj.GetComponent<MeshRenderer>();
        mAABB = AABB.CreateAABB(mObj);
	}
	
	// Update is called once per frame
	void Update () {
        mAABB.UpdateAABB();
    }

    private void OnDrawGizmos()
    {
        if(mAABB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(mAABB.Center, mAABB.Size);
        }

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(mMeshRenderer.bounds.center, mMeshRenderer.bounds.size);
    }
}
