using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBMain : MonoBehaviour {

    public GameObject mObj1;
    public GameObject mObj2;
    private AABB mAABB1;
    private AABB mAABB2;

	// Use this for initialization
	void Start () {
        mAABB1 = AABB.CreateAABB(mObj1);
        mAABB2 = AABB.CreateAABB(mObj2);
    }
	
	// Update is called once per frame
	void Update () {
        mAABB1.UpdateAABB();
        mAABB2.UpdateAABB();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (mAABB1 != null)
        {
            Gizmos.DrawWireCube(mAABB1.Center, mAABB1.Size);
        }

        if (mAABB2 != null)
        {
            Gizmos.DrawWireCube(mAABB2.Center, mAABB2.Size);
        }

        if(mAABB1.CollisionAABB(mAABB2))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(mAABB1.Center, mAABB1.Size);
            Gizmos.DrawWireCube(mAABB2.Center, mAABB2.Size);
        }

    }
}
