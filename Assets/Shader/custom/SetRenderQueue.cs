using UnityEngine;
using System.Collections;

public class SetRenderQueue : MonoBehaviour 
{
    public int mRendererQueue;
	Renderer[] mRd;
	void Update()
	{
		mRd = GetComponentsInChildren<Renderer>();

		
		if(mRd != null && mRd.Length>0)
		{
			foreach(Renderer tmpR in mRd)
			{
				if(tmpR!=null && tmpR.sharedMaterials!=null && tmpR.sharedMaterials.Length>0)
				{
					foreach(Material tmpM in tmpR.sharedMaterials)
					{
						tmpM.renderQueue =tmpM.shader.renderQueue+mRendererQueue;
						//if(LOG.Log)Debug.Log("Material : "tmpM.name + "renderQueue=" +tmpM.renderQueue);
					}
				}
			}
		}
	
	}
}