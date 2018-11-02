using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager {

    public static AvatarManager Instance = new AvatarManager();
    private AvatarManager() { }

	public GameObject LoadAvatar(int id)
    {
        //加载包装prefab
        GameObject wrapObj = Resources.Load<GameObject>("AvatarWrap");
        wrapObj = GameObject.Instantiate(wrapObj);
        Avatar avatar = wrapObj.GetComponent<Avatar>();
        if(avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }

        avatar.ChangePart(Avatar.PartType.eSkeleton, "Female/female_skeleton");
        avatar.ChangePart(Avatar.PartType.eHair, "Female/female_hair-1_red");
        avatar.ChangePart(Avatar.PartType.eFace, "Female/female_face-1");
        avatar.ChangePart(Avatar.PartType.eBody, "Female/female_top-1_blue");
        avatar.ChangePart(Avatar.PartType.eWeapon, "");

        return wrapObj;
    }

    public void ChangePart(GameObject target, Avatar.PartType partType, string partID)
    {
        Avatar avatar = target.GetComponent<Avatar>();
        if(avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }
        else
        {
            avatar.ChangePart(partType, partID);
        }
    }

    public void ChangeMaterial(GameObject target, Avatar.PartType partType, string partID)
    {
        Avatar avatar = target.GetComponent<Avatar>();
        if (avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }
        else
        {
            avatar.ChangeMaterial(partType, partID);
        }
    }
}
