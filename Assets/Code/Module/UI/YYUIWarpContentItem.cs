using UnityEngine;
using System.Collections;
/***
 *@des:warp下Element对应标记
 */
[DisallowMultipleComponent]
public class YYUIWarpContentItem : MonoBehaviour {

	private int index;
	private YYUIWarpContent warpContent;
	void OnDestroy(){
		warpContent = null;
    }

	public YYUIWarpContent WarpContent{
		set{ 
			warpContent = value;
		}
	}

	public int Index {
		set{
			index = value;
			if(warpContent){
				transform.localPosition = warpContent.getLocalPositionByIndex (index);
                //DebugLog.Log(DebugLog.OutputModule.eOther,"### " + transform.localPosition + " / " + index);
				//gameObject.name = (index<10)?("0"+index):(""+index);
				if (warpContent.onInitializeItem != null && index>=0) {
					warpContent.onInitializeItem (gameObject,index);
				}
			}
		}
		get{ 
			return index;
		}
	}

}
